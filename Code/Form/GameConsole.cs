using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    public partial class GameConsole : Form
    {
        private Label EnteredCommandsLabel;
        private const int CommandLimit = 33;
        private TextBox CommandBox;
        public GameField Field;
        public Queue<string> EnteredCommandsQueue;
        public GameConsole(GameField field)
        {
            Field = field;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Text = "Console";
            BackColor = Color.DarkGray;
            //Hide();
            EnteredCommandsQueue = new Queue<string>();
            for (var i = 0; i < 33; i++)
                EnteredCommandsQueue.Enqueue("");
            SetConsoleAttributes();
            SetEvents();
            Activate();
        }

        public void Process()
        {
            Show();
        }

        private void SetConsoleAttributes()
        {
            EnteredCommandsLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width, ClientSize.Height - 20),
                BackColor = Color.Gray
            };
            CommandBox = new TextBox
            {
                BackColor = Color.White,
                Location = new Point(0, ClientSize.Height - 20),
                Size = new Size(ClientSize.Width, 20)
            };
            InvalidateAttributesOnTextEnter();
            Controls.Add(EnteredCommandsLabel);
            Controls.Add(CommandBox);
        }

        private void InvalidateAttributesOnTextEnter()
        {
            EnteredCommandsLabel.Text = "";
            var commandBoxBuilder = new StringBuilder();
            foreach (var command in EnteredCommandsQueue)
            {
                commandBoxBuilder.Append(command + "\r");
            }
            EnteredCommandsLabel.Text = commandBoxBuilder.ToString();
        }

        private void SetEvents()
        {
            CommandBox.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    EnterCommand(CommandBox.Text);
                    CommandBox.Clear();
                }
            };
        }

        private void EnterCommand(string command)
        {
            foreach (var line in ParseCommand(command))
            {
                EnteredCommandsQueue.Enqueue(line);
                if (EnteredCommandsQueue.Count > CommandLimit)
                    EnteredCommandsQueue.Dequeue();
            }
            InvalidateAttributesOnTextEnter();
        }

        private string[] ParseCommand(string unsplitedCommand)
        {
            var command = unsplitedCommand.Split('.');
            switch (command[0].ToLower())
            {
                case "":
                    return new[] { "" };
                case "help": return AvaliableCommands;
                case "enable": return ExecuteEnableCommand(command[1]);
                case "spawn": return SpawnEntity(command[1]);
                default:
                    return new[] { command[0] + " - Unknown command." };
            }
        }

        private string[] AvaliableCommands = new[]
        {
            "List of available commands(put a dot between parts of complex command):",
            "Simple commands:",
            "help;",
            "Complex commans:",
            "Enable:GodMod;"
        };

        private string[] ExecuteEnableCommand(string command)
        {
            switch (command.ToLower())
            {
                case "godmod":
                    Field.Soldier.Health = 99999999;
                    Field.Hostages = Field.Hostages.Select(h => { h.Immortal = true; return h; }).ToList();
                    return new[] { "GodMod is Enabled." };
                default:
                    return new[] { command + " - Unknown command." };
            }
        }

        private string[] SpawnEntity(string command)
        {
            throw new NotImplementedException();
        }
    }
}
