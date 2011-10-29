using System;
using System.Collections.Generic;

namespace Ants
{
    public static class GameState
    {
        public static int MapWidth { get; private set; }
        public static int MapHeight { get; private set; }

        public static int TotalNumberOfTurns { get; private set; }
        public static int CurrentTurnNumber { get; private set; }
        public static void SetTotalNumberOfTurns(int totalNumberOfTurns) { TotalNumberOfTurns = totalNumberOfTurns; }
        public static void SetCurrentTurnNumber(int currentTurnNumber)
        {
            CurrentTurnNumber = currentTurnNumber;
            StartNewTurn();
        }

        public static int MaxAllowedLoadTime { get; private set; }
        public static int MaxAllowedTurnTime { get; private set; }
        public static void SetMaxAllowedLoadTime(int maxAllowedLoadTime) { MaxAllowedLoadTime = maxAllowedLoadTime; }
        public static void SetMaxAllowedTurnTime(int maxAllowedTurnTime) { MaxAllowedTurnTime = maxAllowedTurnTime; }

        private static DateTime turnStart;
        public static int TimeRemaining
        {
            get
            {
                return MaxAllowedTurnTime - (DateTime.Now - turnStart).Milliseconds;
            }
        }

        public static int ViewRadius2 { get; private set; }
        public static int AttackRadius2 { get; private set; }
        public static int SpawnRadius2 { get; private set; }
        public static void SetViewRadius2(int viewRadius2) { ViewRadius2 = viewRadius2; }
        public static void SetAttackRadius2(int attackRadius2) { AttackRadius2 = attackRadius2; }
        public static void SetSpawnRadius2(int spawnRadius2) { SpawnRadius2 = spawnRadius2; }

        public static int PlayerSeed { get; private set; }
        public static void SetPlayerSeed(int playerSeed) { PlayerSeed = playerSeed; }

        public static List<Ant> MyAnts { get; private set; }
        public static List<AntHill> MyHills { get; private set; }
        public static List<Ant> EnemyAnts { get; private set; }
        public static List<AntHill> EnemyHills { get; private set; }
        public static List<Food> FoodTiles { get; private set; }

        public static Tile[,] map { get; private set; }

        static GameState()
        {
            MyAnts = new List<Ant>();
            MyHills = new List<AntHill>();
            EnemyAnts = new List<Ant>();
            EnemyHills = new List<AntHill>();
            FoodTiles = new List<Food>();

            InitMap();
        }

        public static void SetMapWidth(int mapWidth)
        {
            MapWidth = mapWidth;
            InitMap();
        }

        public static void SetMapHeight(int mapHeight)
        {
            MapHeight = mapHeight;
            InitMap();
        }

        static void InitMap()
        {
            map = new Tile[MapHeight, MapWidth];
            for (int row = 0; row < MapHeight; row++)
                for (int col = 0; col < MapWidth; col++)
                    map[row, col] = new Tile(row, col, TileType.Unseen);
        }

        static void StartNewTurn()
        {
            // start timer
            turnStart = DateTime.Now;

            // dead ants
            for (int i = MyAnts.Count - 1; i >= 0; i--) if (!MyAnts[i].isAlive) MyAnts.RemoveAt(i);

            EnemyAnts.Clear();
            FoodTiles.Clear();
            //MAYBE: Maybe we should set these to Stale so we have an idea of where things are?
        }

        public static void UpdateMap()
        {
            Location[] visible = GetEveryVisibleSquare();
            for (int i = 0; i < visible.Length; i++)
            {
                if (map[visible[i].Row, visible[i].Col].Type == TileType.Unseen)
                    map[visible[i].Row, visible[i].Col] = new Tile(visible[i].Row, visible[i].Col, TileType.Land);
            }
        }

        public static void AddAnt(int row, int col, int team)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile();
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

        public static void AddFood(int row, int col)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile();
            for (int i = 0; i < objects.Length; i++)
            {
                Food f = objects[i] as Food;
                if (f != null)
                    return;
            }

            FoodTiles.Add(new Food(row, col));
        }

        public static void RemoveFood(int row, int col)
        {
            for (int i = 0; i < FoodTiles.Count; i++)
            {
                if (FoodTiles[i].Location.Row == row && FoodTiles[i].Location.Col == col)
                {
                    FoodTiles.RemoveAt(i);
                    break;
                }
            }
        }

        public static void AddWater(int row, int col)
        {
            map[row, col] = new Tile(row, col, TileType.Water);
        }

        public static void AddDeadAnt(int row, int col)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile();
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

        public static void AddAntHill(int row, int col, int team)
        {
            GameObject[] objects = map[row, col].GetObjectsOnTile();
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

        /// <summary>
        /// Gets whether <paramref name="location"/> is passable or not.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location is not water, <c>false</c> otherwise.</returns>
        /// <seealso cref="GetIsUnoccupied"/>
        public static bool GetIsPassable(Location location)
        {
            return map[location.Row, location.Col].Type != TileType.Water;
        }

        /// <summary>
        /// Gets whether <paramref name="location"/> is occupied or not.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>true</c> if the location is passable and does not contain an ant, <c>false</c> otherwise.</returns>
        public static bool GetIsUnoccupied(Location location)
        {
            if (!GetIsPassable(location))
                return false;

            GameObject[] objects = map[location.Row, location.Col].GetObjectsOnTile();
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
        public static Location GetDestination(Location location, Direction direction)
        {
            Location delta = Location.GetDelta(direction);

            int row = (location.Row + delta.Row) % MapHeight;
            if (row < 0) row += MapHeight; // because the modulo of a negative number is negative

            int col = (location.Col + delta.Col) % MapWidth;
            if (col < 0) col += MapWidth;

            return new Location(row, col);
        }

        /// <summary>
        /// Gets the distance between <paramref name="loc1"/> and <paramref name="loc2"/>.
        /// </summary>
        /// <param name="loc1">The first location to measure with.</param>
        /// <param name="loc2">The second location to measure with.</param>
        /// <returns>The distance between <paramref name="loc1"/> and <paramref name="loc2"/></returns>
        public static int GetDistance(Tile loc1, Tile loc2)
        {
            int d_row = Math.Abs(loc1.Row - loc2.Row);
            d_row = Math.Min(d_row, MapHeight - d_row);

            int d_col = Math.Abs(loc1.Col - loc2.Col);
            d_col = Math.Min(d_col, MapWidth - d_col);

            return d_row + d_col;
        }

        /// <summary>
        /// Gets the closest directions to get from <paramref name="loc1"/> to <paramref name="loc2"/>.
        /// </summary>
        /// <param name="loc1">The location to start from.</param>
        /// <param name="loc2">The location to determine directions towards.</param>
        /// <returns>The 1 or 2 closest directions from <paramref name="loc1"/> to <paramref name="loc2"/></returns>
        public static ICollection<Direction> GetDirections(Tile loc1, Tile loc2)
        {
            List<Direction> directions = new List<Direction>();

            if (loc1.Row < loc2.Row)
            {
                if (loc2.Row - loc1.Row >= MapHeight / 2)
                    directions.Add(Direction.North);
                if (loc2.Row - loc1.Row <= MapHeight / 2)
                    directions.Add(Direction.South);
            }
            if (loc2.Row < loc1.Row)
            {
                if (loc1.Row - loc2.Row >= MapHeight / 2)
                    directions.Add(Direction.South);
                if (loc1.Row - loc2.Row <= MapHeight / 2)
                    directions.Add(Direction.North);
            }

            if (loc1.Col < loc2.Col)
            {
                if (loc2.Col - loc1.Col >= MapWidth / 2)
                    directions.Add(Direction.West);
                if (loc2.Col - loc1.Col <= MapWidth / 2)
                    directions.Add(Direction.East);
            }
            if (loc2.Col < loc1.Col)
            {
                if (loc1.Col - loc2.Col >= MapWidth / 2)
                    directions.Add(Direction.East);
                if (loc1.Col - loc2.Col <= MapWidth / 2)
                    directions.Add(Direction.West);
            }

            return directions;
        }

        public static bool GetIsVisible(Location loc)
        {
            Location[] offsets = GetEveryVisibleSquare();

            for (int i = 0; i < offsets.Length; i++)
                if (offsets[i].Row == loc.Row && offsets[i].Col == loc.Col)
                    return true;

            return false;
        }

        public static Location[] GetEveryVisibleSquare()
        {
            List<Location> offsets = new List<Location>();
            List<Location> visibleSquares = new List<Location>();
            int squares = (int)Math.Floor(Math.Sqrt(ViewRadius2));
            for (int r = -1 * squares; r <= squares; ++r)
            {
                for (int c = -1 * squares; c <= squares; ++c)
                {
                    int square = r * r + c * c;
                    if (square < ViewRadius2)
                    {
                        offsets.Add(new Location(r, c));
                    }
                }
            }

            for (int i = 0; i < MyAnts.Count; i++)
                for (int j = 0; j < offsets.Count; j++)
                {
                    Location offset = new Location(MyAnts[i].Location.Row + offsets[j].Row, MyAnts[i].Location.Col + offsets[j].Col);
                    offset.CorrectLocationForMapWrapAround();
                    visibleSquares.Add(offset);
                }

            List<Location> finalList = new List<Location>();
            for (int i = 0; i < visibleSquares.Count; i++)
                if (!finalList.Contains(visibleSquares[i]))
                    finalList.Add(visibleSquares[i]);
            return finalList.ToArray();
        }
    }
}

