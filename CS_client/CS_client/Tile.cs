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

        public GameObject[] GetObjectsOnTile()
        {
            List<GameObject> objects = new List<GameObject>();

            for (int i = 0; i < GameState.FoodTiles.Count; i++)
                if (GameState.FoodTiles[i].Location.Row == this.Row && GameState.FoodTiles[i].Location.Col == this.Col)
                    objects.Add(GameState.FoodTiles[i]);
            for (int i = 0; i < GameState.EnemyAnts.Count; i++)
                if (GameState.EnemyAnts[i].Location.Row == this.Row && GameState.EnemyAnts[i].Location.Col == this.Col)
                    objects.Add(GameState.EnemyAnts[i]);
            for (int i = 0; i < GameState.EnemyHills.Count; i++)
                if (GameState.EnemyHills[i].Location.Row == this.Row && GameState.EnemyHills[i].Location.Col == this.Col)
                    objects.Add(GameState.EnemyHills[i]);
            for (int i = 0; i < GameState.MyAnts.Count; i++)
                if (GameState.MyAnts[i].Location.Row == this.Row && GameState.MyAnts[i].Location.Col == this.Col)
                    objects.Add(GameState.MyAnts[i]);
            for (int i = 0; i < GameState.MyHills.Count; i++)
                if (GameState.MyHills[i].Location.Row == this.Row && GameState.MyHills[i].Location.Col == this.Col)
                    objects.Add(GameState.MyHills[i]);

            return objects.ToArray();
        }
    }
}

