using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    public class Player : Warrior
    {
        public Player(Point location)
        {
            Alive = true;
            Location = location;
            ViewDirection = Direction.Up;
            Health = 100;
            Model = TheLastSavior.Properties.Resources.SpecOps;
            CorpseModel = TheLastSavior.Properties.Resources.SpecOpsCorpse;
            Weapon = Gun.FiveSeven;
            FireRate = 15;
            MovementSpeed = 2;
        }

        public void Move(GameField field, bool pressedA, bool pressedD, bool pressedS, bool pressedW, int step)
        {
            if (!Alive)
                return;
            var dx = 0;
            var dy = 0;
            if (pressedA)
                dx -= step;
            if (pressedD)
                dx += step;
            if (!GameField.IsReachable(field, Location.X + dx, Location.Y, 44))
                dx = 0;
            if (pressedS)
                dy += step;
            if (pressedW)
                dy -= step;
            if (!GameField.IsReachable(field, Location.X, Location.Y + dy, 44))
                dy = 0;
            Location = new Point(Location.X + dx, Location.Y + dy);
        }

        public void Turn(KeyEventArgs key)
        {
            if (!Alive)
                return;
            switch(ViewDirection)
            {
                case Direction.Down:
                    if (key.KeyCode == Keys.Left)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        ViewDirection = Direction.Left;
                    }
                    if (key.KeyCode == Keys.Right)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        ViewDirection = Direction.Right;
                    }
                    break;
                case Direction.Up:
                    if (key.KeyCode == Keys.Left)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        ViewDirection = Direction.Left;
                    }
                    if (key.KeyCode == Keys.Right)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        ViewDirection = Direction.Right;
                    }
                    break;
                case Direction.Left:
                    if (key.KeyCode == Keys.Up)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        ViewDirection = Direction.Up;
                    }
                    if (key.KeyCode == Keys.Down)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        ViewDirection = Direction.Down;
                    }
                    break;
                case Direction.Right:
                    if (key.KeyCode == Keys.Up)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        ViewDirection = Direction.Up;
                    }
                    if (key.KeyCode == Keys.Down)
                    {
                        Model.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        ViewDirection = Direction.Down;
                    }
                    break;
            }
        }
    }
}
