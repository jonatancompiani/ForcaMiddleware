using Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForcaMonoProcesso
{
    class Program
    {
        private static string Player_1 = string.Empty;
        private static string Player_2 = string.Empty;
        private static string Player_3 = string.Empty;

        private static List<string> words;

        static void Main(string[] args)
        {

            CollectPLayerNames();

            GameLogic.Startup(new List<string> {Player_1, Player_2, Player_3 });

            DrawWords();

            //MAIN LOOP
            while (words.Any(x=> x.Contains("_")))
            {
                PlayerTurn(Player_1);
                PlayerTurn(Player_2);
                PlayerTurn(Player_3);
            }

            var players = GameLogic.GetScores();

            var winners = players.Where(x => x.Item2 == players.Max(m => m.Item2));

            if (winners.Count() == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"PARABÉNS {winners.First().Item1}, VOCÊ VENCEU E LEVA PARA CASA {winners.First().Item2} REAIS!!!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"PARABÉNS {string.Join(" E ",winners.Select(S=> S.Item1))}, EMPATARAM EM PRIMEIRO LUGAR E LEVAM PARA CASA {winners.First().Item2} REAIS!!!");
            }

            Console.ResetColor();
        }

        static void CollectPLayerNames()
        {
            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("RODA A RODA");
                Console.WriteLine();
                Console.ResetColor();
                Console.WriteLine("Qual o nome do primeiro jogador?");
                Player_1 = Console.ReadLine();

            } while (string.Empty.Equals(Player_1));

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("RODA A RODA");
                Console.WriteLine();
                Console.ResetColor();
                Console.WriteLine("Qual o nome do segundo jogador?");
                Player_2 = Console.ReadLine();

            } while (string.Empty.Equals(Player_2));

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("RODA A RODA");
                Console.WriteLine();
                Console.ResetColor();
                Console.WriteLine("Qual o nome do terceiro jogador?");
                Player_3 = Console.ReadLine();

            } while (string.Empty.Equals(Player_3));
        }

        private static void PlayerTurn(string player)
        {
            var guessCount = -1;
            while (guessCount != 0 && words.Any(x => x.Contains("_")))
            {
                var prize = GameLogic.GetPrize();
                Console.Write($"{player}, valendo {prize} reais, qual a letra que você escolhe? ");

                var guess = Console.ReadLine();

                if (string.IsNullOrEmpty(guess))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{player} você precisa escolher uma letra! ");
                    Console.ResetColor();
                }
                else if (guess.Length != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{player}, digite apenas uma letra! ");
                    Console.ResetColor();
                }
                else
                {
                    guessCount = GameLogic.MakeGuess(player, guess[0]);
                    if (guessCount == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{player}, esta letra já foi usada, tente novamente! ");
                        Console.ResetColor();
                    }
                    else
                    {
                        DrawWords();
                    }
                }
            }
        }

        private static void DrawWords()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RODA A RODA");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("PLACAR:");
            var players = GameLogic.GetScores();
            foreach (var player in players)
            {
                Console.WriteLine($"\t{player.Item1} - {player.Item2}");
            }

            Console.ResetColor();
            Console.WriteLine();

            words = GameLogic.GetWords();
            foreach (var word in words)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine();
        }
    }
}
