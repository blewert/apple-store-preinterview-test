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

        static void Main(string[] args)
        {
            //The game ids to look up.
            gameIDs = new string[]
            {
                "1353551752", "1436151665", "4", "-5", null, "5.1", "e", "definitely not a number", "1043639756"
            };

            //Lookup games
            TestLookupGames();

            Console.WriteLine("[info] Done!");
            Console.ReadLine();
            
            
        }

        private static void TestLookupGames()
        {
            //For each one, look it up
            foreach (var gameID in gameIDs)
            {
                try
                {
                    Console.WriteLine($"[info] Looking up game '{gameID}'");

                    //Make a game
                    var game = new Game(gameID);

                    //Look it up
                    var gameResponses = game.Lookup();

                    if (gameResponses.resultCount <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[info] Found no results.");
                        Console.ResetColor();

                        //Skip!
                        continue;
                    }

                    //Show results
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[info] Found {gameResponses.resultCount} result(s). Here are the bundle IDs:");
                    Console.WriteLine("[info] >> " + gameResponses.results.Select(x => x.bundleId).Aggregate((a, b) => $"{a}, {b}"));
                    Console.ResetColor();
                }
                catch(AggregateException e)
                {
                    //Wuh-oh something has gone REALLY wrong
                    //..

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[error] One or more errors occurred, see below:");

                    foreach (var inner in e.InnerExceptions)
                        Console.WriteLine($"[error]\t-- {inner.Message}");

                    Console.WriteLine("[error] Program exiting.");
                    Console.ResetColor();

                    break;
                }
                catch (Exception e)
                { 
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[error] An error occurred. See the details below.");
                    Console.WriteLine($"[error] {e.Message}");
                    Console.ResetColor();

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
