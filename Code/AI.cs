using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGame
{
    public class AI
    {
        public static readonly int SwingInterwal = 40;
        public static readonly int WalkInterwal = 20;
        public static readonly int WaitInterwal = 100;
        public static readonly int ShootsCount = 3;

        public Point StartPosition;
        public static List<Room> Rooms;
        public GameField Field;
        public Bot Bot;
        public int ActionsCount;
        public int WaitCount;
        public int ShootsMade;
        public bool InBattle;
        public Direction MoveDirection;

        public AI(Bot bot, GameField field)
        {
            Bot = bot;
            Field = field;
            StartPosition = Bot.Location;
            SetRoomBelonging(Bot);
            InBattle = false;
        }

        public void MakeTick(int tick)
        {
            if (InOneRoomWithPlayer())
            {
                if (Bot.Level > 1)
                    InBattle = true;
                Bot.Turn(TurnToSoldier());
                if (ActionsCount == 0)
                {
                    InBattle = true;
                    ActionsCount = SwingInterwal;
                    ShootsMade = 0;
                    GenerateDirection(true, tick);
                }
                DoActions(tick);
                return;
            }
            InBattle = false;
            if (ActionsCount == 0)
            {
                ActionsCount = WalkInterwal;
                WaitCount = WaitInterwal;
                GenerateDirection(false, tick);
            }
            DoActions(tick);
        }

        private void DoActions(int tick)
        {
            if (InBattle)
            {
                if (ShootsMade < ShootsCount)
                {
                    if(Shooting.MakeShot(Bot, tick, Field))
                     ShootsMade++;
                    return;
                }
                if(ActionsCount > 0)
                {
                    Bot.Move(MoveDirection, Field);
                    ActionsCount--;
                }
                return;
            }
            if(WaitCount > 0)
            {
                WaitCount--;
                return;
            }
            if (ActionsCount > 0)
            {
                Bot.Move(MoveDirection, Field);
                ActionsCount--;
            }
        }

        private void GenerateDirection(bool swing, int tick)
        {
            var rnd = new Random(Bot.ID * tick);
            if (swing)
            {
                var num = rnd.Next(1, 3);
                switch (Bot.ViewDirection)
                {
                    case Direction.Down:
                        if (num == 1)
                            MoveDirection = Direction.Left;
                        else MoveDirection = Direction.Right;
                        return;
                    case Direction.Up:
                        if (num == 1)
                            MoveDirection = Direction.Left;
                        else MoveDirection = Direction.Right;
                        return;
                    case Direction.Left:
                        if (num == 1)
                            MoveDirection = Direction.Up;
                        else MoveDirection = Direction.Down;
                        return;
                    case Direction.Right:
                        if (num == 1)
                            MoveDirection = Direction.Up;
                        else MoveDirection = Direction.Down;
                        return;
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                var num = rnd.Next(1, 5);
                switch (num)
                {
                    case 1:
                        MoveDirection = Direction.Up;
                        break;
                    case 2:
                        MoveDirection = Direction.Down;
                        break;
                    case 3:
                        MoveDirection = Direction.Left;
                        break;
                    case 4:
                        MoveDirection = Direction.Right;
                        break;
                    default:
                        throw new ArgumentException();
                }
                Bot.Turn(MoveDirection);
            }
        }

        private bool InOneRoomWithPlayer()
        {
            return Field.Soldier.RoomBelonging == Bot.RoomBelonging;
        }

        public static void SetRoomBelonging(Warrior warrior)
        {
            if (!warrior.Alive)
                warrior.RoomBelonging = -1;
            foreach (var room in AI.Rooms)
            {
                if (room.UpperLeftCorner.X <= warrior.Location.X + 37 && room.LowerRightCorner.X >= warrior.Location.X + 37 &&
                    room.UpperLeftCorner.Y <= warrior.Location.Y + 37 && room.LowerRightCorner.Y >= warrior.Location.Y + 37)
                {
                    warrior.RoomBelonging = room.ID;
                    return;
                }
            }
            return;
        }

        private Direction TurnToSoldier()
        {
            var dx = Bot.Location.X - Field.Soldier.Location.X;
            var dy = Bot.Location.Y - Field.Soldier.Location.Y;
            if (dx < 0)
            {
                if (dy < 0)
                    if (dx < dy)
                        return Direction.Right;
                    else return Direction.Down;
                else if (Math.Abs(dx) > dy)
                    return Direction.Right;
                else return Direction.Up;
            }
            if (dy >= 0)
                if (dx >= dy)
                    return Direction.Left;
                else return Direction.Up;
            else if (Math.Abs(dy) >= dx)
                return Direction.Down;
            else return Direction.Left;
        }
    }

    public class Room
    {
        public int ID;
        public Point UpperLeftCorner;
        public Point LowerRightCorner;
    }
}
