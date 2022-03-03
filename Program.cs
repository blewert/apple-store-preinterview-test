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
                "1353551752", "1436151665", "1043639756"
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
                    Console.WriteLine($"[info] Looking up game {gameID}");

                    //Make a game
                    var game = new Game(gameID);

                    //Look it up
                    var gameObject = game.Lookup();
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[error] An error occurred. See the details below.");
                    Console.WriteLine($"[error] {e.Message}");
                    Console.ResetColor();

                    //Is it just an error with parsing? If so.. just move to the next game
                    if (e is HttpParseException || e is FormatException)
                        continue;

                    //Otherwise its probably something more serious, so break
                    else break;
                }
            }
        }
    }
}
