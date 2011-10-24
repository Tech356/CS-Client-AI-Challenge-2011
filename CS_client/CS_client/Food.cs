using System;

namespace Ants
{
    public class Food : GameObject
    {
        public bool isStale { get { return (StaleLength > 0); } }
        public int StaleLength { get; set; }

        public Food(int row, int col) : base(row, col) { }
    }
}
