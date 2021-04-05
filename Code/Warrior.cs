using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    public abstract class Warrior
    {
        private int health;
        public int Health 
        { 
            get 
            {
                return health; 
            }
            set
            { 
                if (value < 0) 
                    health = 0;
                else
                    health = value;
            }
        }
        public Direction ViewDirection { get; set; }
        public int MovementSpeed /*в пикселях на тик*/ { get; set; }
        public Point Location { get; set; }
        public Image Model { get; set; }
        public Image CorpseModel { get; set; }
        public Gun Weapon { get; set; }
        public int LastShotTick { get; set; }
        public int FireRate { get; set; }
        public bool Alive { get; set; }
        public int RoomBelonging { get; set; }

        public void GetDamage(Projectile projectile, GameField field)
        {
            Health -= projectile.Damage;
            if (Health == 0)
            {
                Alive = false;
                var CorpseModel = this.CorpseModel;
                switch (ViewDirection)
                {
                    case Direction.Down:
                        CorpseModel.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case Direction.Up:
                        break;
                    case Direction.Left:
                        CorpseModel.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case Direction.Right:
                        CorpseModel.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                }
                field.Corpses.Add(new Corpse { Model = CorpseModel, Location = Location });
            }
        }
    }

    public class Corpse
    {
        public Image Model;
        public Point Location;
    }
}
