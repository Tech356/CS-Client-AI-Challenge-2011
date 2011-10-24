using System;

namespace Ants
{
    public class AntHill : GameObject
    {
        public int Team { get; private set; }
        public bool isAlive { get; set; }
        public bool isStale { get { return (StaleLength > 0); } }
        public int StaleLength { get; set; }

        public AntHill(int row, int col, int team)
            : base(row, col)
        {
            this.isAlive = true;
            this.Team = team;
            this.StaleLength = 0;
        }
    }
}
