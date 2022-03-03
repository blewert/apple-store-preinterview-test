using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleGameInfo
{
    class Program
    {
        /// <summary>
        /// The game ids to look up.
        /// </summary>
        private static string[] gameIDs;

        /// <summary>
        /// Main entry point of the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //The game ids to look up.
            gameIDs = new string[]
            {
                "1353551752", "1436151665", "1043639756"
            };

            //Lookup games
            TestLookupGames();

            Log(ConsoleColor.White, "[info] Done!");
            Console.ReadLine();
        }

        /// <summary>
        /// Logs a message with a colour, then resets the colour.
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="input">The message</param>
        private static void Log(ConsoleColor color, string input)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        /// <summary>
        /// Looks up games by ID, in Program.gameIDs
        /// </summary>
        private static void TestLookupGames()
        {
            //For each one, look it up
            foreach (var gameID in gameIDs)
            {
                try
                {
                    Log(ConsoleColor.White, $"[info] Looking up game '{gameID}'");

                    //Make a game
                    var game = new GameLookup(gameID);

                    //Look it up
                    var gameResponses = game.Lookup();

                    if (gameResponses.resultCount <= 0)
                    {
                        //Skip, there were 0 results!
                        Log(ConsoleColor.Green, "[info] Found no results.");
                        continue;
                    }

                    //Show results
                    Log(ConsoleColor.Green, $"[info] Found {gameResponses.resultCount} result(s). Here are the bundle IDs:");
                    Log(ConsoleColor.Green, "[info] >> " + gameResponses.results.Select(x => x.bundleId).Aggregate((a, b) => $"{a}, {b}"));
                }
                catch(AggregateException e)
                {
                    //Wuh-oh something has gone REALLY wrong
                    //..

                    Log(ConsoleColor.Red, "[error] One or more errors occurred, see below:");

                    foreach (var inner in e.InnerExceptions)
                        Log(ConsoleColor.Red, $"[error]\t-- {inner.Message}");

                    Log(ConsoleColor.Red, "[error] Program exiting.");

                    break;
                }
                catch (Exception e)
                { 
                    Log(ConsoleColor.Red, $"[error] An error occurred. See the details below.");
                    Log(ConsoleColor.Red, $"[error] {e.Message}");

                    //Is it just an error with parsing? If so.. just move to the next game
                    if (e is HttpParseException || e is FormatException)
                        continue;

                    //Is it a JSON parsing error? If so, move to the next game
                    if (e is Newtonsoft.Json.JsonException)
                        continue;

                    //Otherwise its probably something more serious, so break
                    else break;
                }
                finally
                {
                    //Put a new line between blocks of output.
                    Console.WriteLine("");
                }
            }
        }
    }
}
