using System;
using System.Collections.Generic;

namespace Ants
{
    public struct Location
    {
        public int Row;
        public int Col;

        public Location(int row, int col)
        {
            Row = row;
            Col = col;
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
    }

    public enum TileType
    {
        Land,
        Water,
        Unseen,
    }

    public class Tile
    {
        /// <summary>
        /// Gets the row of this location.
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the column of this location.
        /// </summary>
        public int Col { get; private set; }

        public TileType Type { get; private set; }



        public Tile(int row, int col, TileType type)
        {
            this.Row = row;
            this.Col = col;
            this.Type = type;
        }

        public GameObject[] GetObjectsOnTile(GameState state)
        {
            List<GameObject> objects = new List<GameObject>();

            for (int i = 0; i < state.FoodTiles.Count; i++)
                if (state.FoodTiles[i].Row == this.Row && state.FoodTiles[i].Col == this.Col)
                    objects.Add(state.FoodTiles[i]);
            for (int i = 0; i < state.EnemyAnts.Count; i++)
                if (state.EnemyAnts[i].Row == this.Row && state.EnemyAnts[i].Col == this.Col)
                    objects.Add(state.EnemyAnts[i]);
            for (int i = 0; i < state.EnemyHills.Count; i++)
                if (state.EnemyHills[i].Row == this.Row && state.EnemyHills[i].Col == this.Col)
                    objects.Add(state.EnemyHills[i]);
            for (int i = 0; i < state.MyAnts.Count; i++)
                if (state.MyAnts[i].Row == this.Row && state.MyAnts[i].Col == this.Col)
                    objects.Add(state.MyAnts[i]);
            for (int i = 0; i < state.MyHills.Count; i++)
                if (state.MyHills[i].Row == this.Row && state.MyHills[i].Col == this.Col)
                    objects.Add(state.MyHills[i]);

            return objects.ToArray();
        }
    }
}

