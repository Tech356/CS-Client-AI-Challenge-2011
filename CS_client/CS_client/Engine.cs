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

        public static void PlayGame(Bot bot)
        {
            PlayGame(bot, Console.In);
        }

        public static void PlayGame(Bot bot, TextReader inputTextReader)
        {
            List<string> input = new List<string>();

            try
            {
                while (true)
                {
                    string line = inputTextReader.ReadLine().Trim().ToLower();

                    switch (line)
                    {
                        case READY:
                            ParseSetup(input);
                            bot.Init();
                            input.Clear();
                            FinishTurn();
                            break;
                        case GO:
                            ParseUpdate(input);
                            bot.DoTurn();
                            input.Clear();
                            FinishTurn();
                            break;
                        case END:
                            return;
                        default:
                            input.Add(line);
                            break;
                    }
                }
            }
#if DEBUG
            catch (Exception e)
            {
                StreamWriter sw = File.CreateText("Exception_Debug_" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".log");
                sw.WriteLine("Turn Number: " + GameState.CurrentTurnNumber);
                sw.WriteLine();
                sw.WriteLine(e);
                sw.Close();

            }
#else
            catch (Exception) { }
#endif
        }

        // parse initial input and setup starting game state
        private static void ParseSetup(List<string> input)
        {
            foreach (string line in input)
            {
                if (line.Length <= 0) continue;

                string[] tokens = line.Split();
                string key = tokens[0];

                switch (key)
                {
                    case "cols":
                        GameState.SetMapWidth(int.Parse(tokens[1]));
                        break;
                    case "rows":
                        GameState.SetMapHeight(int.Parse(tokens[1]));
                        break;
                    case "turn":
                        // We don't care about the first turn during setup
                        break;
                    case "turns":
                        GameState.SetTotalNumberOfTurns(int.Parse(tokens[1]));
                        break;
                    case "turntime":
                        GameState.SetMaxAllowedTurnTime(int.Parse(tokens[1]));
                        break;
                    case "loadtime":
                        GameState.SetMaxAllowedLoadTime(int.Parse(tokens[1]));
                        break;
                    case "viewradius2":
                        GameState.SetViewRadius2(int.Parse(tokens[1]));
                        break;
                    case "attackradius2":
                        GameState.SetAttackRadius2(int.Parse(tokens[1]));
                        break;
                    case "spawnradius2":
                        GameState.SetSpawnRadius2(int.Parse(tokens[1]));
                        break;
                    case "player_seed":
                        GameState.SetPlayerSeed(int.Parse(tokens[1]));
                        break;
                    default:
                        throw new Exception("Unknown setup input token: '" + key + "'");
                }
            }
        }

        // parse engine input and update the game state
        private static void ParseUpdate(List<string> input)
        {
            foreach (string line in input)
            {
                if (line.Length <= 0) continue;

                string[] tokens = line.Split();

                if (tokens.Length >= 3)
                {
                    int row = int.Parse(tokens[1]);
                    int col = int.Parse(tokens[2]);

                    switch (tokens[0])
                    {
                        case "a":
                            GameState.AddAnt(row, col, int.Parse(tokens[3]));
                            break;
                        case "f":
                            GameState.AddFood(row, col);
                            break;
                        case "r":
                            GameState.RemoveFood(row, col);
                            break;
                        case "w":
                            GameState.AddWater(row, col);
                            break;
                        case "d":
                            GameState.AddDeadAnt(row, col);
                            break;
                        case "h":
                            GameState.AddAntHill(row, col, int.Parse(tokens[3]));
                            break;
                        default:
                            throw new Exception("Unknown update input token: '" + tokens[0] + "'");
                    }
                }
                else if (tokens[0] == "turn")
                    GameState.SetCurrentTurnNumber(int.Parse(tokens[1]));

                else
                    throw new Exception("Unknown or invalid update input token: '" + line + "'");
            }
        }

        private static void FinishTurn()
        {
            System.Console.Out.WriteLine(GO);
        }
    }
}