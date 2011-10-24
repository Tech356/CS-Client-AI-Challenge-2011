using System;
using System.Collections.Generic;

namespace Ants
{
    public class GameState
    {
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public int TotalNumberOfTurns { get; private set; }
        public int CurrentTurnNumber { get; private set; }
        public int MaxAllowedLoadTime { get; private set; }
        public int MaxAllowedTurnTime { get; private set; }

        private DateTime turnStart;
        public int TimeRemaining
        {
            get
            {
                TimeSpan timeSpent = DateTime.Now - turnStart;
                return MaxAllowedTurnTime - timeSpent.Milliseconds;
            }
        }

        public int ViewRadius2 { get; private set; }
        public int AttackRadius2 { get; private set; }
        public int SpawnRadius2 { get; private set; }
        public int PlayerSeed { get; private set; }

        public List<Ant> MyAnts { get; private set; }
        public List<AntHill> MyHills { get; private set; }
        public List<Ant> EnemyAnts { get; private set; }
        public List<AntHill> EnemyHills { get; private set; }
        public List<Food> FoodTiles { get; private set; }

        public Tile[,] map { get; private set; }

        public GameState(int width, int height, int totalNumberOfTurns, int turntime, int loadtime,
                         int viewradius2, int attackradius2, int spawnradius2, int playerSeed)
        {
            this.MapWidth = width;
            this.MapHeight = height;

            this.TotalNumberOfTurns = totalNumberOfTurns;
            this.MaxAllowedLoadTime = loadtime;
            this.MaxAllowedTurnTime = turntime;

            this.ViewRadius2 = viewradius2;
            this.AttackRadius2 = attackradius2;
            this.SpawnRadius2 = spawnradius2;
            this.PlayerSeed = playerSeed;

            this.MyAnts = new List<Ant>();
            this.MyHills = new List<AntHill>();
            this.EnemyAnts = new List<Ant>();
            this.EnemyHills = new List<AntHill>();
            this.FoodTiles = new List<Food>();

            map = new Tile[height, width];
            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    map[row, col] = new Tile(row, col, TileType.Unseen);
        }

        #region State mutators
        public void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.Now;

            // dead ants
            for (int i = MyAnts.Count - 1; i >= 0; i--) if (!MyAnts[i].isAlive) MyAnts.RemoveAt(i);

            EnemyAnts.Clear();
            FoodTiles.Clear();
            //MAYBE: Maybe we should set these to Stale so we have an idea of where things are?
        }

        public void SetTurn(int turnNumber)
        {
            this.CurrentTurnNumber = turnNumber;
        }

        public void AddAnt(int row, int col, int team)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile(this);
            for (int i = 0; i < objects.Length; i++)
            {
                Ant a = objects[i] as Ant;
                if (a != null)
                {
                    a.StaleLength = 0;
                    return;
                }
            }

            Ant newAnt = new Ant(row, col, team);
            if (team == 0)
                MyAnts.Add(newAnt);
            else
                EnemyAnts.Add(newAnt);
        }

        public void AddFood(int row, int col)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile(this);
            for (int i = 0; i < objects.Length; i++)
            {
                Food f = objects[i] as Food;
                if (f != null)
                    return;
            }

            FoodTiles.Add(new Food(row, col));
        }

        public void RemoveFood(int row, int col)
        {
            for (int i = 0; i < FoodTiles.Count; i++)
            {
                if (FoodTiles[i].Row == row && FoodTiles[i].Col == col)
                {
                    FoodTiles.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddWater(int row, int col)
        {
            map[row, col] = new Tile(row, col, TileType.Water);
        }

        public void DeadAnt(int row, int col)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile(this);
            for (int i = 0; i < objects.Length; i++)
            {
                Ant a = objects[i] as Ant;
                if (a != null)
                {
                    a.isAlive = false;
                    return;
                }
            }

            // ignore unknown dead ants
        }

        public void AntHill(int row, int col, int team)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile(this);
            for (int i = 0; i < objects.Length; i++)
            {
                AntHill a = objects[i] as AntHill;
                if (a != null)
                {
                    a.StaleLength = 0;
                    return;
                }
            }

            AntHill hill = new AntHill(row, col, team);
            if (team == 0)
                MyHills.Add(hill);
            else
                EnemyHills.Add(hill);
        }
        #endregion

        /// <summary>
        /// Gets whether <paramref name="location"/> is passable or not.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
        /// <seealso cref="GetIsUnoccupied"/>
        public bool GetIsPassable(Location location)
        {
            return map[location.Row, location.Col].Type != TileType.Water;
        }

        /// <summary>
        /// Gets whether <paramref name="location"/> is occupied or not.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
        public bool GetIsUnoccupied(Location location)
        {
            if (!GetIsPassable(location))
                return false;

            GameObject[] objects = map[location.Row, location.Col].GetObjectsOnTile(this);
            for (int i = 0; i < objects.Length; i++)
            {
                Ant a = objects[i] as Ant;
                if (a != null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the destination if an ant at <paramref name="location"/> goes in <paramref name="direction"/>, accounting for wrap around.
        /// </summary>
        /// <param name="location">The starting location.</param>
        /// <param name="direction">The direction to move.</param>
        /// <returns>The new location, accounting for wrap around.</returns>
        public Location GetDestination(Location location, Direction direction)
        {
            Location delta = Location.GetDelta(direction);

            int row = (location.Row + delta.Row) % this.MapHeight;
            if (row < 0) row += this.MapHeight; // because the modulo of a negative number is negative

            int col = (location.Col + delta.Col) % this.MapWidth;
            if (col < 0) col += this.MapWidth;

            return new Location(row, col);
        }

        /// <summary>
        /// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
        /// </summary>
        /// <param name="loc1">The first location to measure with.</param>
        /// <param name="loc2">The second location to measure with.</param>
        /// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
        public int GetDistance(Tile loc1, Tile loc2)
        {
            int d_row = Math.Abs(loc1.Row - loc2.Row);
            d_row = Math.Min(d_row, this.MapHeight - d_row);

            int d_col = Math.Abs(loc1.Col - loc2.Col);
            d_col = Math.Min(d_col, this.MapWidth - d_col);

            return d_row + d_col;
        }

        /// <summary>
        /// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
        /// </summary>
        /// <param name="loc1">The location to start from.</param>
        /// <param name="loc2">The location to determine directions towards.</param>
        /// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
        public ICollection<Direction> GetDirections(Tile loc1, Tile loc2)
        {
            List<Direction> directions = new List<Direction>();

            if (loc1.Row < loc2.Row)
            {
                if (loc2.Row - loc1.Row >= this.MapHeight / 2)
                    directions.Add(Direction.North);
                if (loc2.Row - loc1.Row <= this.MapHeight / 2)
                    directions.Add(Direction.South);
            }
            if (loc2.Row < loc1.Row)
            {
                if (loc1.Row - loc2.Row >= this.MapHeight / 2)
                    directions.Add(Direction.South);
                if (loc1.Row - loc2.Row <= this.MapHeight / 2)
                    directions.Add(Direction.North);
            }

            if (loc1.Col < loc2.Col)
            {
                if (loc2.Col - loc1.Col >= this.MapWidth / 2)
                    directions.Add(Direction.West);
                if (loc2.Col - loc1.Col <= this.MapWidth / 2)
                    directions.Add(Direction.East);
            }
            if (loc2.Col < loc1.Col)
            {
                if (loc1.Col - loc2.Col >= this.MapWidth / 2)
                    directions.Add(Direction.East);
                if (loc1.Col - loc2.Col <= this.MapWidth / 2)
                    directions.Add(Direction.West);
            }

            return directions;
        }

        public bool GetIsVisible(Location loc)
        {
            List<Location> offsets = new List<Location>();
            int squares = (int)Math.Floor(Math.Sqrt(this.ViewRadius2));
            for (int r = -1 * squares; r <= squares; ++r)
            {
                for (int c = -1 * squares; c <= squares; ++c)
                {
                    int square = r * r + c * c;
                    if (square < this.ViewRadius2)
                    {
                        offsets.Add(new Location(r, c));
                    }
                }
            }
            foreach (Ant ant in this.MyAnts)
            {
                foreach (Location offset in offsets)
                {
                    if ((ant.Col + offset.Col) == loc.Col &&
                        (ant.Row + offset.Row) == loc.Row)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}

