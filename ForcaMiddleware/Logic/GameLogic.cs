using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public static class GameLogic
    {
        #region Database

        private static List<string> WORDS_DATABASE = new List<string> 
            { 
                "BATATA", 
                "CENOURA", 
                "ALFACE", 
                "TOMATE", 
                "BANANA", 
                "LARANJA", 
                "MELANCIA", 
                "UVA", 
                "BLUEBERRY"
            };

        private static List<int> PRIZES = new List<int>()
        {
            100,
            200,
            300,
            500,
            600,
            700,
            800,
            900,
            1000
        };

        #endregion

        #region Game instance data

        private static List<string> SELECTED_WORDS = new List<string>();

        private static List<char> GUESSES = new List<char>();

        private static int NextGuessMultiplier = 0;

        private static List<Player> PlayersAndScore = new List<Player>();

        #endregion

        // 1.0
        public static void Startup(List<string> players)
        {
            SELECTED_WORDS = new List<string>();
            GUESSES = new List<char>();

            PlayersAndScore = new List<Player>();
            foreach (var player in players)
            {
                PlayersAndScore.Add(new Player(player));
            }

            // Get random words

            var rnd = new Random();
            int index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());

            rnd = new Random();
            index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());

            rnd = new Random();
            index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());
        }

        // 1.0
        public static int MakeGuess(string player, char letter)
        {
            if(GUESSES.Any(x=> char.ToUpper(x) == char.ToUpper(letter)))
            {
                // Letter already revealed, try again
                return -1;
            }
            else
            {
                GUESSES.Add(char.ToUpper(letter));
            }

            // How many letters matching?
            var correctlyGuessedLetters = SELECTED_WORDS.Sum(x => x.Count(y=> y == char.ToUpper(letter)));

            // get the current player
            var currentPlayer = PlayersAndScore.First(x => x.name.Equals(player));

            // set the score
            currentPlayer.score += correctlyGuessedLetters * NextGuessMultiplier;

            // reset the prize multiplier
            NextGuessMultiplier = 0;

            return correctlyGuessedLetters;
        }

        public static List<string> GetWords()
        {
            var revealedWors = new List<string>();
            
            foreach (var word in SELECTED_WORDS)
            {
                string revealedWord = string.Empty;
                foreach (var letter in word)
                {
                    if (GUESSES.Contains(letter))
                    {
                        revealedWord += $"{letter} ";
                    }
                    else
                    {
                        revealedWord += $"_ ";
                    }
                }
                revealedWors.Add(revealedWord);
            }

            return revealedWors;
        }

        // 1.0
        public static int GetPrize()
        {
            var rnd = new Random();
            var index = rnd.Next(PRIZES.Count);
            NextGuessMultiplier = PRIZES[index];
            return NextGuessMultiplier;
        }


        public static List<Tuple<string, int>> GetScores()
        {
            var tuple = new List<Tuple<string, int>>();
            foreach (var player in PlayersAndScore)
            {
                tuple.Add(new Tuple<string, int>(player.name, player.score));
            }

            return tuple;
        }

   


        #region 2.0

        // 2.0
        public static void Startup()
        {
            SELECTED_WORDS = new List<string>();
            GUESSES = new List<char>();

            // Get random words

            var rnd = new Random();
            int index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());

            rnd = new Random();
            index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());

            rnd = new Random();
            index = rnd.Next(WORDS_DATABASE.Count);
            SELECTED_WORDS.Add(WORDS_DATABASE[index].ToUpper());
        }

        // 2.0
        public static void AddPlayer(string player)
        {
            PlayersAndScore.Add(new Player(player) { prize = GameLogic.GetPrize() });
        }

        // 2.0
        public static int GetPrize(string player)
        {
            var rnd = new Random();
            var index = rnd.Next(PRIZES.Count);
            Player playerData = PlayersAndScore.FirstOrDefault(x=> x.name.Equals(player));
            if (playerData != null)
            {
                playerData.prize = PRIZES[index];
                return playerData.prize;
            }
            else
            {
                return 0;
            }
        }

        // 2.0
        public static int MakeGuessForPlayer(string player, char letter)
        {
            if (GUESSES.Any(x => char.ToUpper(x) == char.ToUpper(letter)))
            {
                // Letter already revealed, try again
                return -1;
            }
            else
            {
                GUESSES.Add(char.ToUpper(letter));
            }

            // How many letters matching?
            var correctlyGuessedLetters = SELECTED_WORDS.Sum(x => x.Count(y => y == char.ToUpper(letter)));

            // get the current player
            var currentPlayer = PlayersAndScore.First(x => x.name.Equals(player));

            // if it was an error, reduce the prize from the score
            if (correctlyGuessedLetters == 0)
            {
                currentPlayer.score -=  currentPlayer.prize;
            }
            else
            {
                // set the score
                currentPlayer.score += correctlyGuessedLetters * currentPlayer.prize;
            }

            // reset the prize multiplier
            currentPlayer.prize = 0;

            return correctlyGuessedLetters;
        }

        #endregion


        #region 3.0

        public static List<Player> GetPlayerScores()
        {
            return PlayersAndScore;
        }
        public static List<char> GetInvalidLetters()
        {
            return GameLogic.GUESSES;
        }

        #endregion
    }
}
