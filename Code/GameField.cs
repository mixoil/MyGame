using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyGame
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class GameField
    {
        public GameConsole Console;
		public Player Soldier;
		public List<Bot> Bots;
        public List<Hostage> Hostages;
        public List<Corpse> Corpses;
        public CellState[,] Field;
        public readonly int Width;
        public readonly int Height;
        public const int CellSize = 75;
        public GameField(string[] lines)
		{
            Corpses = new List<Corpse>();
            Width = lines[0].Length;
            Height = lines.Length;
            Bots = new List<Bot>();
            Hostages = new List<Hostage>();
			Field = new CellState[lines[0].Length, lines.Length];
            var botID = 1;
			for (var y = 0; y < lines.Length; y++)
			{
				for (var x = 0; x < lines[0].Length; x++)
				{
					switch (lines[y][x])
					{
						case '#':
							Field[x, y] = CellState.Wall;
							break;
						case 'P':
							Field[x, y] = CellState.Empty;
                            if (Soldier != null) throw new ArgumentException();
							Soldier = new Player(new Point(x * CellSize, y * CellSize));
							break;
						case '1':
							Field[x, y] = CellState.Empty;
							Bots.Add(new Bot(new Point(x * CellSize, y * CellSize), 1, this, true, ++botID));
							break;
                        case 'H':
                            Field[x, y] = CellState.Empty;
                            Hostages.Add(new Hostage(new Point(x * CellSize, y * CellSize)));
                            break;
                        default:
							Field[x, y] = CellState.Empty;
							break;
					}
				}
			}
		}

        public CellState this[int x, int y]
        {
            get { return Field[x, y]; }
            set { this[x, y] = value; }
        }

        public static Point GetCellFromPixelsLocation(int x, int y)
        {
            return new Point((x) / GameField.CellSize, (y) / GameField.CellSize);
        }

        public static bool IsReachable(GameField field, int x, int y, int hitBoxLength)
        {
            if (!(x + 37 + hitBoxLength / 2 < field.Width * GameField.CellSize && //37 - половина размера клетки
                    y + 37 + hitBoxLength / 2 < field.Height * GameField.CellSize &&
                    x + 37 - hitBoxLength / 2 >= 0 && y + 37 - hitBoxLength / 2 >= 0)) return false;
            var notCrossingWall = true;
            foreach (var cell in GetLocationCells(x, y, hitBoxLength))
            {
                if (field[cell.X, cell.Y] == CellState.Wall)
                    notCrossingWall = false;
            }
            return notCrossingWall;
        }

        private static IEnumerable<Point> GetLocationCells(int x, int y, int hitBoxLength)
        {
            x += 37 - hitBoxLength / 2;
            y += 37 - hitBoxLength / 2;
            yield return GetCellFromPixelsLocation(x, y);
            yield return GetCellFromPixelsLocation(x + hitBoxLength, y);
            yield return GetCellFromPixelsLocation(x, y + hitBoxLength);
            yield return GetCellFromPixelsLocation(x + hitBoxLength, y + hitBoxLength);
        }
    }

    class Level
    {
        public string[] Map;
        public List<Room> Rooms;
        
        public static Level LoadLevel(int level)
        {
            Level result = new Level();
            switch (level)
            {
                case 1:
                    result.Map = Map1;
                    result.Rooms = Rooms1;
                    break;
                case 2:
                    result.Map = Map2;
                    result.Rooms = Rooms2;
                    break;
            }
            return result;
        }

        private static readonly string[] Map1 = new string[]
        {
            "##########",
            "#OOOOOOOO#",
            "#O1OOOOOO#",
            "#HOOOOOOO#",
            "#1OOOOOOO#",
            "#OOOOOOOO#",
            "########O#",
            "OOOOOOOOOO",
            "OOOOOOOOOO",
            "OOOOPOOOOO"
        };

        private static readonly List<Room> Rooms1 = new List<Room>
        {
            new Room {UpperLeftCorner = new Point(75,75), LowerRightCorner = new Point(674,449), ID = 2},
            new Room {UpperLeftCorner = new Point(600,450), LowerRightCorner = new Point(674,524), ID = 1},
            new Room {UpperLeftCorner = new Point(0,524), LowerRightCorner = new Point(749,749), ID = 0},
        };

        private static readonly string[] Map2 = new string[]
        {
            "##########",
            "#1OOO#O1O#",
            "#OHOOOOOO#",
            "#1OOO#OOO#",
            "########O#",
            "#O1OOOOOO#",
            "#OOOOOO1O#",
            "####O#####",
            "OOOOOOOOOO",
            "OOOOPOOOOO"
        };

        private static readonly List<Room> Rooms2 = new List<Room>
        {
            new Room {UpperLeftCorner = new Point(75,75), LowerRightCorner = new Point(374,299), ID = 6},
            new Room {UpperLeftCorner = new Point(375,150), LowerRightCorner = new Point(449,224), ID = 5},
            new Room {UpperLeftCorner = new Point(450,75), LowerRightCorner = new Point(674,299), ID = 4},
            new Room {UpperLeftCorner = new Point(600,300), LowerRightCorner = new Point(674,374), ID = 3},
            new Room {UpperLeftCorner = new Point(75,375), LowerRightCorner = new Point(674,524), ID = 2},
            new Room {UpperLeftCorner = new Point(300,525), LowerRightCorner = new Point(374,599), ID = 1},
            new Room {UpperLeftCorner = new Point(0,600), LowerRightCorner = new Point(749,749), ID = 0},
        };
    }
}
