using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    public partial class GameForm : Form
    {
        public GameConsole Console;
        public Form MenuForm;
        public GameField Field;
        public Timer Timer;
        public int Time;
        public int EndTime;
        public bool GameLost;
        public bool GameEnd;

        private bool IsPressedW;
        private bool IsPressedD;
        private bool IsPressedS;
        private bool IsPressedA;

        public GameForm(int level, Form menuForm)
        {
            MenuForm = menuForm;
            Load += (sender, args) =>
            {
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                DoubleBuffered = true;
                Text = "The Last Savior. Level " + level.ToString();
                BackgroundImage = TheLastSavior.Properties.Resources.Empty;
                KeyPreview = true;
            };
            Time = 0;
            Timer = new Timer
            {
                Interval = 10
            };
            EndTime = 200;
            GameLost = false;
            GameEnd = false;
            var lvl = Level.LoadLevel(level);
            AI.Rooms = lvl.Rooms;
            Field = new GameField(lvl.Map);
            Console = new GameConsole(Field);
            Field.Console = Console;
            ClientSize = new Size(Field.Width * GameField.CellSize, Field.Height * GameField.CellSize + 150);
            SetEnvironment();
            MakeLowerPanel();
            Shooting.Projectiles = new HashSet<Projectile>();
            var lowerLabel = new Label
            {
                Size = new Size(200, 60),
                Font = new Font("Bold", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                BackgroundImage = TheLastSavior.Properties.Resources.ButtonBack,
                Location = new Point(45, Field.Height * GameField.CellSize + 45),
                BorderStyle = BorderStyle.FixedSingle
            };
            var healthLabel = new Label
            {
                Size = new Size(200, 60),
                Font = new Font("Bold", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(Field.Width * GameField.CellSize - 490, Field.Height * GameField.CellSize + 45),
                BackgroundImage = TheLastSavior.Properties.Resources.ButtonBack,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(healthLabel);
            Controls.Add(lowerLabel);
            lowerLabel.BringToFront();
            healthLabel.BringToFront();
            Timer.Tick += (sender, args) =>
            { 
                Time++;
                if (GameEnd)
                    EndGame();
                if (!Field.Soldier.Alive)
                {
                    GameLost = true;
                    EndGame();
                    GameEnd = true;
                }
                Field.Soldier.Move(Field, IsPressedA, IsPressedD, IsPressedS, IsPressedW, Field.Soldier.MovementSpeed);
                Shooting.Projectiles = Shooting.Projectiles.Where(p => p.IsActive).ToHashSet();
                foreach (var projectile in Shooting.Projectiles)
                    projectile.FlyTick(Field);
                var hostagesCount = Field.Hostages.Count;
                Field.Hostages = Field.Hostages.Where(h => h.Alive).ToList();
                if(Field.Hostages.Count < hostagesCount)
                {
                    GameLost = true;
                    EndGame();
                    GameEnd = true;
                }
                Field.Bots = Field.Bots.Where(b => b.Alive).ToList();
                if(Field.Bots.Count == 0)
                {
                    GameLost = false;
                    EndGame();
                    GameEnd = true;
                }
                AI.SetRoomBelonging(Field.Soldier);
                foreach (var bot in Field.Bots)
                    bot.Intelligence.MakeTick(Time);
                healthLabel.Text = "HP:" + Field.Soldier.Health.ToString();
                lowerLabel.Text = Field.Soldier.RoomBelonging.ToString() +
                " " + Field.Soldier.Location.X.ToString()+ " " + Field.Soldier.Location.Y.ToString();
                Invalidate();
            };
            Paint += (sender, args) =>
            {
                foreach(var corpse in Field.Corpses)
                    args.Graphics.DrawImage(corpse.Model, corpse.Location);
                if (Field.Soldier.Alive)
                    args.Graphics.DrawImage(Field.Soldier.Model, Field.Soldier.Location);
                foreach (var projectile in Shooting.Projectiles)
                    args.Graphics.DrawLine(new Pen(Color.Black, 3), projectile.CurrentLocation, projectile.GetSecondDrawingPoint());
                foreach (var bot in Field.Bots)
                    args.Graphics.DrawImage(bot.Model, bot.Location);
                foreach (var hostage in Field.Hostages)
                    args.Graphics.DrawImage(hostage.Model, hostage.Location);
            };
            SetController();
            Timer.Start();
            FormClosing += (sender, args) => Application.Exit();
        }

        private void SetEnvironment()
        {
            for (var x = 0; x < Field.Width; x++)
                for (var y = 0; y < Field.Height; y++)
                {
                    if (Field[x, y] != CellState.Wall)
                        continue;
                    var picture = new Panel
                    {
                        Location = new Point(x * GameField.CellSize, y * GameField.CellSize),
                        Size = new Size(GameField.CellSize, GameField.CellSize),
                        BackgroundImage = TheLastSavior.Properties.Resources.Wall
                    };
                    picture.SendToBack();
                    Controls.Add(picture);
                }
        }

        private void MakeLowerPanel()
        {
            var lowerPanel = new PictureBox
            {
                Location = new Point(0, Field.Height * GameField.CellSize),
                Size = new Size(Field.Width * GameField.CellSize, 150),
                Image = TheLastSavior.Properties.Resources.LowerPanel,
                BorderStyle = BorderStyle.Fixed3D
            };
            var exitButton = new NotSelectableButton
            {
                Text = "Back",
                Size = new Size(200, 60),
                Font = new Font("Bold", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(Field.Width * GameField.CellSize - 245, lowerPanel.Top + 45),
                BackgroundImage = TheLastSavior.Properties.Resources.ButtonBack
            };
            exitButton.Click += (sender, args) => Application.Restart();
            Controls.Add(lowerPanel);
            Controls.Add(exitButton);
            lowerPanel.SendToBack();
        }

        public void SetController()
        {
            KeyDown += (sender, args) =>
            {
                //Перемещение
                switch (args.KeyCode)
                {
                    case Keys.D:
                        IsPressedD = true;
                        break;
                    case Keys.A:
                        IsPressedA = true;
                        break;
                    case Keys.W:
                        IsPressedW = true;
                        break;
                    case Keys.S:
                        IsPressedS = true;
                        break;
                }
                //Поворот
                if (args.KeyCode == Keys.Up || args.KeyCode == Keys.Down ||
                args.KeyCode == Keys.Right || args.KeyCode == Keys.Left)
                    Field.Soldier.Turn(args);
                if (args.KeyCode == Keys.Space)
                    Shooting.MakeShot(Field.Soldier, Time, Field);
                if (args.KeyCode.ToString() == "P")
                {
                    Console.Process();
                }
            };
            KeyUp += (sender, args) =>
            {
                switch (args.KeyCode)
                {
                    case Keys.D:
                        IsPressedD = false;
                        break;
                    case Keys.A:
                        IsPressedA = false;
                        break;
                    case Keys.W:
                        IsPressedW = false;
                        break;
                    case Keys.S:
                        IsPressedS = false;
                        break;
                }
            };
        }

        public void EndGame()
        {
            if (!GameEnd)
            {
                //var GOLabel = new Label
                //{
                //    Location = new Point(Field.Width * GameField.CellSize / 2 - 500, Field.Height * GameField.CellSize / 2 - 200),
                //    Size = new Size(1000, 150),
                //    Font = new Font("Bold", 85, FontStyle.Bold),
                //    ForeColor = Color.Blue,
                //    TextAlign = ContentAlignment.TopCenter,
                //    BackColor = Color.Transparent
                //};
                //if (GameLost)
                //    GOLabel.Text = "GAME OVER";
                //else
                //    GOLabel.Text = "YOU WON";
                //GOLabel.BringToFront();
                //Controls.Add(GOLabel);
            }
            if (EndTime > 0)
            {
                EndTime--;
                return;
            }
            Application.Restart();
        }
    }
}
