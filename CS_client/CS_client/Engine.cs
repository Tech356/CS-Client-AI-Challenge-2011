using System;
using System.IO;
using System.Collections.Generic;

namespace Ants
{

    public static class Engine
    {
        private const string READY = "ready";
        private const string GO = "go";
        private const string END = "end";

        private static GameState state;

        public static void PlayGame(Bot bot)
        {
            List<string> input = new List<string>();

            try
            {
                while (true)
                {
                    string line = System.Console.In.ReadLine().Trim().ToLower();

                    if (line.Equals(READY))
                    {
                        ParseSetup(input);
                        //bot.Init();
                        FinishTurn();
                        input.Clear();
                    }
                    else if (line.Equals(GO))
                    {
                        state.StartNewTurn();
                        ParseUpdate(input);
                        bot.DoTurn(state);
                        FinishTurn();
                        input.Clear();
                    }
                    else if (line.Equals(END))
                    {
                        break;
                    }
                    else
                    {
                        input.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                StreamWriter sw = File.CreateText("Exception_Debug_" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".log");
                sw.WriteLine(e);
                sw.Close();
#endif
            }

        }

        // parse initial input and setup starting game state
        private static void ParseSetup(List<string> input)
        {
            int width = 0, height = 0;
            int turntime = 0, loadtime = 0;
            int viewradius2 = 0, attackradius2 = 0, spawnradius2 = 0;

            foreach (string line in input)
            {
                if (line.Length <= 0) continue;

                string[] tokens = line.Split();
                string key = tokens[0];

                if (key.Equals(@"cols"))
                {
                    width = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"rows"))
                {
                    height = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"seed"))
                {
                    ;
                }
                else if (key.Equals(@"turntime"))
                {
                    turntime = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"loadtime"))
                {
                    loadtime = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"viewradius2"))
                {
                    viewradius2 = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"attackradius2"))
                {
                    attackradius2 = int.Parse(tokens[1]);
                }
                else if (key.Equals(@"spawnradius2"))
                {
                    spawnradius2 = int.Parse(tokens[1]);
                }
            }

            state = new GameState(width, height, turntime, loadtime, viewradius2, attackradius2, spawnradius2);
        }

        // parse engine input and update the game state
        private static void ParseUpdate(List<string> input)
        {
            // do some stuff first

            foreach (string line in input)
            {
                if (line.Length <= 0) continue;

                string[] tokens = line.Split();

                if (tokens.Length >= 3)
                {
                    int row = int.Parse(tokens[1]);
                    int col = int.Parse(tokens[2]);

                    if (tokens[0].Equals("a"))
                    {
                        state.AddAnt(row, col, int.Parse(tokens[3]));
                    }
                    else if (tokens[0].Equals("f"))
                    {
                        state.AddFood(row, col);
                    }
                    else if (tokens[0].Equals("r"))
                    {
                        state.RemoveFood(row, col);
                    }
                    else if (tokens[0].Equals("w"))
                    {
                        state.AddWater(row, col);
                    }
                    else if (tokens[0].Equals("d"))
                    {
                        state.DeadAnt(row, col);
                    }
                    else if (tokens[0].Equals("h"))
                    {
                        state.AntHill(row, col, int.Parse(tokens[3]));
                    }
                }
            }
        }

        private static void FinishTurn()
        {
            System.Console.Out.WriteLine(GO);
        }

    }
}