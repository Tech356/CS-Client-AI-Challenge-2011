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

        public bool Move(Direction direction)
        {
            // if we move to a location where another ant is, both ants will die
            // we don't want to kill our own ants
            Location MoveToLocation = GameState.GetDestination(this.Location, direction);
            for (int i = 0; i < GameState.MyAnts.Count; i++)
                if (GameState.MyAnts[i].Location == MoveToLocation)
                    return false;

            System.Console.Out.WriteLine("o {0} {1} {2}", this.Location.Row, this.Location.Col, (Char)direction);
            this.Location = new Location(this.Location.Row + Location.GetDelta(direction).Row,
                                         this.Location.Col + Location.GetDelta(direction).Col);
            this.Location.CorrectLocationForMapWrapAround();
            return true;
        }

        public override string ToString()
        {
            return "Ant {" + this.Location.Row + ", " + this.Location.Col + "|" + Team + "}";
        }
    }
}
