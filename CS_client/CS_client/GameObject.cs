using System;

namespace Ants
{
    public abstract class GameObject
    {
        public int Row { get; protected set; }
        public int Col { get; protected set; }

        public Location Location { get { return new Location(this.Row, this.Col); } }

        public GameObject(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }
    }
}
