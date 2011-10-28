using System;

namespace Ants
{
    public abstract class GameObject
    {
        public Location Location { get; protected set; }

        public GameObject(Location location)
            : this(location.Row, location.Col)
        { }

        public GameObject(int row, int col)
        {
            // Create a new Location so we aren't just referencing an external Location
            this.Location = new Location(row, col);
        }
    }
}
