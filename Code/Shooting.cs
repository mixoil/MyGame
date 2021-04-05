using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    class Shooting
    {
        public static HashSet<Projectile> Projectiles;
        public static bool MakeShot(Warrior shooter, int currentTime, GameField field)
        {
            if (!shooter.Alive)
                return false;
            if (shooter.LastShotTick + shooter.FireRate > currentTime)
                return false;
            shooter.LastShotTick = currentTime;
            var projectile = new Projectile(shooter);
            if (!projectile.FindedObstacle(projectile.CurrentLocation, field))
                Projectiles.Add(projectile);
            return true;
        }

        //public static int CountDamage(Point Location, Projectile projectile)
        //{
        //    if (projectile.DirectionOfMoving == Direction.Left || projectile.DirectionOfMoving == Direction.Right)
        //        return 45 - Math.Abs(Location.Y + 22 - projectile.CurrentLocation.Y) * 3;
        //    return 45 - Math.Abs(Location.X + 22 - projectile.CurrentLocation.X) * 3;
        //}
    }

    public class Projectile
    {
        public Point CurrentLocation;
        public Direction DirectionOfMoving;
        public bool IsActive;
        public int Damage;
        public int Speed;
        public int TrackLength;

        public Projectile(Warrior shooter)
        {
            IsActive = true;
            switch (shooter.Weapon)
            {
                case Gun.M1911:
                    Damage = 30;
                    Speed = 30;
                    TrackLength = 10;
                    switch (shooter.ViewDirection)
                    {
                        case Direction.Down:
                            CurrentLocation = new Point(shooter.Location.X + 32, shooter.Location.Y + 69 + TrackLength);
                            break;
                        case Direction.Up:
                            CurrentLocation = new Point(shooter.Location.X + 42, shooter.Location.Y + 5 - TrackLength);
                            break;
                        case Direction.Left:
                            CurrentLocation = new Point(shooter.Location.X + 5 - TrackLength, shooter.Location.Y + 33);
                            break;
                        case Direction.Right:
                            CurrentLocation = new Point(shooter.Location.X + 69 + TrackLength, shooter.Location.Y + 41);
                            break;
                    }
                    break;
                case Gun.FiveSeven:
                    Damage = 45;
                    Speed = 30;
                    TrackLength = 10;
                    switch (shooter.ViewDirection)
                    {
                        case Direction.Down:
                            CurrentLocation = new Point(shooter.Location.X + 32, shooter.Location.Y + 69 + TrackLength);
                            break;
                        case Direction.Up:
                            CurrentLocation = new Point(shooter.Location.X + 42, shooter.Location.Y + 5 - TrackLength);
                            break;
                        case Direction.Left:
                            CurrentLocation = new Point(shooter.Location.X + 5 - TrackLength, shooter.Location.Y + 33);
                            break;
                        case Direction.Right:
                            CurrentLocation = new Point(shooter.Location.X + 69 + TrackLength, shooter.Location.Y + 41);
                            break;
                    }
                    break;
            }
            DirectionOfMoving = shooter.ViewDirection;
            }

        public void FlyTick(GameField field)
        {
            var newLocation = PointFinder(Speed);
            if (FindedObstacle(newLocation, field))
                IsActive = false;
            CurrentLocation = newLocation;
        }

        public bool FindedObstacle(Point newLocation, GameField field)
        {
            var cellPoint = GameField.GetCellFromPixelsLocation(newLocation.X, newLocation.Y);
            var inBounds = newLocation.X < field.Width * GameField.CellSize &&
                    newLocation.Y < field.Height * GameField.CellSize &&
                    newLocation.X >= 0 && newLocation.Y >= 0;
            foreach(var bot in field.Bots)
            {
                if (CheckWarriorHitting(bot, newLocation, field))
                    return true;
            }
            foreach (var hostage in field.Hostages)
            {
                if (hostage.Location.X + 15 <= newLocation.X && hostage.Location.X + 59 >= newLocation.X &&
                    hostage.Location.Y + 15 <= newLocation.Y && hostage.Location.Y + 59 >= newLocation.Y)
                {
                    hostage.Kill(field);
                    return true;
                }
            }
            var playerHitting = false;
            if (field.Soldier.Alive)
                playerHitting = CheckWarriorHitting(field.Soldier, newLocation, field);
            return !inBounds || playerHitting ||
                    field[cellPoint.X, cellPoint.Y] == CellState.Wall;
        }

        public bool CheckWarriorHitting(Warrior warrior, Point bullet, GameField field)
        {
            if (!(warrior.Location.X + 15 <= bullet.X && warrior.Location.X + 59 >= bullet.X &&
                    warrior.Location.Y + 15 <= bullet.Y && warrior.Location.Y + 59 >= bullet.Y))
                return false;
            warrior.GetDamage(this, field);
            return true;
        }

        public Point GetSecondDrawingPoint()
        {
            return PointFinder(-TrackLength);
        }

        private Point PointFinder(int argument)
        {
            switch (DirectionOfMoving)
            {
                case Direction.Down:
                    return new Point(CurrentLocation.X, CurrentLocation.Y + argument);
                case Direction.Up:
                    return new Point(CurrentLocation.X, CurrentLocation.Y - argument);
                case Direction.Left:
                    return new Point(CurrentLocation.X - argument, CurrentLocation.Y);
                case Direction.Right:
                    return new Point(CurrentLocation.X + argument, CurrentLocation.Y);
                default:
                    throw new NullReferenceException();
            }
        }
    }

    public enum Gun
    {
        FiveSeven,
        M1911
    }
}
