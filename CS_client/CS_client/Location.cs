using System;

namespace Ants
{
    public class Location
    {
        public int Row;
        public int Col;

        public Location(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void CorrectLocationForMapWrapAround()
        {
            this.Col = (this.Col + GameState.MapWidth) % GameState.MapWidth;
            this.Row = (this.Row + GameState.MapHeight) % GameState.MapHeight;
        }

        public static Location GetDelta(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Location(-1, 0);
                case Direction.South:
                    return new Location(1, 0);
                case Direction.East:
                    return new Location(0, 1);
                case Direction.West:
                    return new Location(0, -1);
            }
            throw new Exception("Unknown Direction Passed to GetDelta: '" + direction + "'");
        }

        public override string ToString()
        {
            return "{" + Row + ", " + Col + "}";
        }
    }
}
