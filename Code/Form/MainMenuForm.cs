using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            ClientSize = new Size(1000, 800);
            BackgroundImage = TheLastSavior.Properties.Resources.MainMenu;
            var gameName = new Label
            {
                Text = "The Last Savior",
                Size = new Size(1000, 150),
                Font = new Font("Bold", 85, FontStyle.Bold),
                ForeColor = Color.Blue,
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };
            var playButton = CreateButton("Play");
            var exitButton = CreateButton("Exit");
            
            
            SetStyle(ControlStyles.Selectable, false);
            Controls.Add(gameName);
            Controls.Add(playButton);
            Controls.Add(exitButton);
            SizeChanged += (sender, args) =>
            {
                gameName.Location = new Point((ClientSize.Width - gameName.Size.Width) / 2, (int)(ClientSize.Height * 0.15));
                playButton.Location = new Point((ClientSize.Width - playButton.Size.Width) / 2, gameName.Bottom + 50);
                exitButton.Location = new Point((ClientSize.Width - playButton.Size.Width) / 2, playButton.Bottom + 25);
            };

            Load += (sender, args) =>
            {
                DoubleBuffered = true;
                Text = "The Last Saviour";
                OnSizeChanged(EventArgs.Empty);
            };
            exitButton.Click += (sender, args) => Application.Exit();
            playButton.Click += (sender, args) =>
            {
                Controls.Remove(playButton);
                Controls.Remove(exitButton);
                var lvl1Button = CreateButton("Level 1");
                var lvl2Button = CreateButton("Level 2");
                Controls.Add(lvl1Button);
                Controls.Add(lvl2Button);
                SizeChanged += (sender1, args1) =>
                {
                    gameName.Location = new Point((ClientSize.Width - gameName.Size.Width) / 2, (int)(ClientSize.Height * 0.15));
                    lvl1Button.Location = new Point((ClientSize.Width - lvl1Button.Size.Width) / 2, gameName.Bottom + 50);
                    lvl2Button.Location = new Point((ClientSize.Width - lvl2Button.Size.Width) / 2, lvl1Button.Bottom + 25);
                };
                OnSizeChanged(EventArgs.Empty);
                lvl1Button.Click += (sender1, args1) =>
                {
                    this.Hide();
                    var game = new GameForm(1, this);
                    game.ShowDialog();
                };
                lvl2Button.Click += (sender1, args1) =>
                {
                    this.Hide();
                    var game = new GameForm(2, this);
                    game.ShowDialog();
                };
            };
        }

        private NotSelectableButton CreateButton(string text)
        {
            return new NotSelectableButton
            {
                Text = text,
                Size = new Size(200, 60),
                Font = new Font("Bold", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                BackgroundImage = TheLastSavior.Properties.Resources.ButtonBack
            };
        }
    }

    public class NotSelectableButton : Button
    {
        public NotSelectableButton()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
