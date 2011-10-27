using System;

namespace Ants
{
    public class Ant : GameObject
    {
        public int Team { get; private set; }
        public bool isAlive { get; set; }
        public bool isStale { get { return (StaleLength > 0); } }
        public int StaleLength { get; set; }

        public Ant(int row, int col, int team)
            : base(row, col)
        {
            this.isAlive = true;
            this.Team = team;
            this.StaleLength = 0;
        }

        public void Move(Direction direction)
        {
            System.Console.Out.WriteLine("o {0} {1} {2}", this.Row, this.Col, (Char)direction);
            this.Col = (this.Col + Location.GetDelta(direction).Col + GameState.MapWidth) % GameState.MapWidth;
            this.Row = (this.Row + Location.GetDelta(direction).Row + GameState.MapHeight) % GameState.MapHeight;
        }

        public override string ToString()
        {
            return "{" + Row + ", " + Col + "|" + Team + "}";
        }
    }
}
