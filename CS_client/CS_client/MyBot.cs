using System;
using System.Collections.Generic;

namespace Ants
{
    class MyBot : Bot
    {
        public override void Init() { }

        // DoTurn is run once per turn
        public override void DoTurn(GameState state)
        {
            // loop through all my ants and try to give them orders
            foreach (Ant ant in state.MyAnts)
            {
                // try all the directions
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    // GetDestination will wrap around the map properly
                    // and give us a new location
                    Location newLoc = state.GetDestination(ant.Location, direction);

                    // GetIsPassable returns true if the location is land
                    if (state.GetIsPassable(newLoc))
                    {
                        ant.Move(direction);
                        // stop now, don't give 1 and multiple orders
                        break;
                    }
                }

                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 10) break;
            }

        }

        public static void Main(string[] args)
        {
            Engine.PlayGame(new MyBot());
        }
    }
}