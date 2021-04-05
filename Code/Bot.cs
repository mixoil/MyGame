using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    public class Bot : Warrior
    { 
        public int Level { get; set; }
        public AI Intelligence { get; set; }
        public int ID { get; set; }

        public Bot(Point location, int level, GameField field, bool enableAI, int id)
        {
            ID = id;
            Alive = true;
            Level = level;
            Location = location;
            ViewDirection = Direction.Up;
            if(enableAI)
                Intelligence = new AI(this, field);
            switch (Level)
            {
                case 1: Model = TheLastSavior.Properties.Resources.Terrorist1;
                    CorpseModel = TheLastSavior.Properties.Resources.Terrorist1Corpse;
                    Health = 100;
                    Weapon = Gun.M1911;
                    FireRate = 40;
                    MovementSpeed = 2;
                    break;  
            }
        }

        public void ResetModel()
        {
            switch (Level)
            {
                case 1:
                    Model = TheLastSavior.Properties.Resources.Terrorist1;
                    break;
            }
        }

        public void Move(Direction direction, GameField field)
        {
            var newLocation = new Point();
            switch (direction)
            {
                case Direction.Down:
                    newLocation = new Point(Location.X, Location.Y + MovementSpeed);
                    break;
                case Direction.Up:
                    newLocation = new Point(Location.X, Location.Y - MovementSpeed);
                    break;
                case Direction.Left:
                    newLocation = new Point(Location.X - MovementSpeed, Location.Y);
                    break;
                case Direction.Right:
                    newLocation = new Point(Location.X + MovementSpeed, Location.Y);
                    break;
            }
            if (GameField.IsReachable(field, newLocation.X, newLocation.Y, 44))
                Location = newLocation;
        }

        public void Turn(Direction direction)
        {
            ResetModel();
            switch (direction)
            {
                case Direction.Down:
                    Model.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    ViewDirection = direction;
                    break;
                case Direction.Up:
                    break;
                case Direction.Left:
                    Model.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    ViewDirection = direction;
                    break;
                case Direction.Right:
                    Model.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    ViewDirection = direction;
                    break;
            }
            ViewDirection = direction;
        }
    }
}

