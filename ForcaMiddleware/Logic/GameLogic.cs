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
                "BLUEBERRY", 
                "ARAÇÁ"
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
    }
}
