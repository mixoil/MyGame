using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    public class Hostage
    {
        public bool Immortal;
        public Image Model;
        public Point Location;
        public bool Alive;
        public Hostage(Point location)
        {
            Immortal = false;
            Alive = true;
            Location = location;
            Model = TheLastSavior.Properties.Resources.Hostage;
        }

        public void Kill(GameField field)
        {
            if (Immortal) return;
            Alive = false;
            field.Corpses.Add(new Corpse { Model = TheLastSavior.Properties.Resources.HostageCorpse, Location = Location });
        }
    }
}
