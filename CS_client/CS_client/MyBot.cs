using System;
using System.Collections.Generic;

namespace Ants
{
    class MyBot : Bot
    {
        public override void Init() { }

        // DoTurn is run once per turn
        public override void DoTurn()
        {
            // loop through all my ants and try to give them orders
            foreach (Ant ant in GameState.MyAnts)
            {
                // try all the directions
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    // GetDestination will wrap around the map properly
                    // and give us a new location
                    Location newLoc = GameState.GetDestination(ant.Location, direction);

                    // GetIsPassable returns true if the location is land
                    if (GameState.GetIsPassable(newLoc))
                    {
                        ant.Move(direction);
                        // stop now, don't give 1 and multiple orders
                        break;
                    }
                }

                // check if we have time left to calculate more orders
                if (GameState.TimeRemaining < 10) break;
            }

        }

        public static void Main(string[] args)
        {
            Engine.PlayGame(new MyBot());
        }
    }
}