using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework;

namespace MyGame
{
    [TestFixture]
    class Test
    {
        public GameField InitialiseGameForm()
        {
            var map = new string[]
            {
                "O#POOO",
                "O#OOOO",
                "O#OOOO",
                "OOOOOO",
                "OOOOOO",
            };
            return new GameField(map);
        }
        [Test]
        public void Movement()
        {
            var field = InitialiseGameForm();
            for (var i = 0; i < 100; i++)
                field.Soldier.Move(field, false, true, true, false, 2);
            Assert.AreEqual(new Point(350, 200), field.Soldier.Location);
        }

        [Test]
        public void Movement_ThroughWall()
        {
            var field = InitialiseGameForm();
            for (var i = 0; i < 100; i++)
                field.Soldier.Move(field,true, false, false, false, 2);
            Assert.AreEqual(new Point(136, 0), field.Soldier.Location);
        }

        [Test]
        public void Turning()
        {
            var field = InitialiseGameForm();
            field.Soldier.Turn(new KeyEventArgs(Keys.Right));
            field.Soldier.Turn(new KeyEventArgs(Keys.Down));
            Assert.AreEqual(Direction.Down, field.Soldier.ViewDirection);
        }

        [Test]
        public void Shooting_Projectiles()
        {
            var field = InitialiseGameForm();
            field.Soldier.Turn(new KeyEventArgs(Keys.Right));
            field.Soldier.Turn(new KeyEventArgs(Keys.Down));
            field.Bots.Add(new Bot(new Point(150, 150), 1, field, false,1));
            Shooting.Projectiles = new HashSet<Projectile>();
            Shooting.MakeShot(field.Soldier, 100, field);
            Assert.AreEqual(1, Shooting.Projectiles.Count);
        }

        [Test]
        public void Shooting_Damage()
        {
            var field = InitialiseGameForm();
            field.Soldier.Turn(new KeyEventArgs(Keys.Right));
            field.Soldier.Turn(new KeyEventArgs(Keys.Down));
            field.Bots.Add(new Bot(new Point(150, 200),1, field, false,1));
            Shooting.Projectiles = new HashSet<Projectile>();
            Shooting.MakeShot(field.Soldier, 100, field);
            for (var i = 0; i < 30; i++)
            {
                Shooting.Projectiles.First().FlyTick(field);
                if (!Shooting.Projectiles.First().IsActive)
                    break;
            }
            Assert.AreEqual(55, field.Bots.First().Health);
        }

        [Test]
        public void Shooting_KillingBot()
        {
            var field = InitialiseGameForm();
            field.Soldier.Turn(new KeyEventArgs(Keys.Right));
            field.Soldier.Turn(new KeyEventArgs(Keys.Down));
            field.Bots.Add(new Bot(new Point(150, 200), 1, field, false,1));
            Shooting.Projectiles = new HashSet<Projectile>();
            Shooting.MakeShot(field.Soldier, 100, field);
            Shooting.MakeShot(field.Soldier, 150, field);
            Shooting.MakeShot(field.Soldier, 200, field);
            for (var i = 0; i < 30; i++)
            {
                Shooting.Projectiles = Shooting.Projectiles.Where(p => p.IsActive).ToHashSet();
                foreach (var projectile in Shooting.Projectiles)
                    projectile.FlyTick(field);
            }
            Assert.AreEqual(1, field.Corpses.Count);
        }

        [Test]
        public void Shooting_KillingHostage()
        {
            var field = InitialiseGameForm();
            field.Soldier.Turn(new KeyEventArgs(Keys.Right));
            field.Soldier.Turn(new KeyEventArgs(Keys.Down));
            field.Hostages.Add(new Hostage(new Point(150, 200)));
            Shooting.Projectiles = new HashSet<Projectile>();
            Shooting.MakeShot(field.Soldier, 100, field);
            for (var i = 0; i < 30; i++)
            {
                Shooting.Projectiles.First().FlyTick(field);
                if (!Shooting.Projectiles.First().IsActive)
                    break;
            }
            Assert.AreEqual(1, field.Corpses.Count);
        }
    }
}
