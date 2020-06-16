using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientSocket
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static string playerName = string.Empty;

        private static List<Tuple<string, string>> score;
        private static List<string> words;

        private static bool isGameRunning = true;

        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoopConnect();
            RegisterPlayer();
            GameLoop();
            Console.ReadLine();
        }

        private static void RegisterPlayer()
        {
            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("RODA A RODA");
                Console.WriteLine();
                Console.ResetColor();
                Console.WriteLine("Qual o seu nome?");
                playerName = Console.ReadLine();

            } while (string.Empty.Equals(playerName));

            byte[] buffer = Encoding.ASCII.GetBytes($"start|{playerName}");
            _clientSocket.Send(buffer);

            byte[] receivedBuff = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuff);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuff, data, rec);
            Console.WriteLine($"Message: {Encoding.ASCII.GetString(data)}");
        }

        private static void GameLoop()
        {
            DrawWords();
            do
            {
                string prize = getPrize();

                Console.Write($"{playerName}, valendo {prize} reais, qual a letra que você escolhe? ");

                var guess = Console.ReadLine();
                bool isGuessValid = false;
                while (!isGuessValid)
                {
                    setWords();
                    if(!words.Any(x => x.Contains("_")))
                    {
                        break;
                    }

                    if (string.IsNullOrEmpty(guess))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{playerName} você precisa escolher uma letra! ");
                        Console.ResetColor();

                        Console.Write($"{playerName}, valendo {prize} reais, qual a letra que você escolhe? ");
                        guess = Console.ReadLine();
                    }
                    else if (guess.Length != 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{playerName}, digite apenas uma letra! ");
                        Console.ResetColor();

                        Console.Write($"{playerName}, valendo {prize} reais, qual a letra que você escolhe? ");
                        guess = Console.ReadLine();
                    }
                    else
                    {
                        string guessCount = makeGuess(guess);

                        if ("-1".Equals(guessCount))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{playerName}, esta letra já foi usada, tente novamente! ");
                            Console.ResetColor();

                            Console.Write($"{playerName}, valendo {prize} reais, qual a letra que você escolhe? ");
                            guess = Console.ReadLine();
                        }
                        else
                        {
                            isGuessValid = true;
                        }
                    }
                }
                DrawWords();
            }
            while (words.Any(x => x.Contains("_")));

            DrawWords();

            var winners = score.Where(x => x.Item2 == score.Max(m => m.Item2));

            if(winners.Any(x=> x.Item1.Equals(playerName)))
            {
                if (winners.Count() == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"PARABÉNS {winners.First().Item1}, VOCÊ VENCEU E LEVA PARA CASA {winners.First().Item2} REAIS!!!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"PARABÉNS {string.Join(" E ", winners.Select(S => S.Item1))}, EMPATARAM EM PRIMEIRO LUGAR E LEVAM PARA CASA {winners.First().Item2} REAIS!!!");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"INFELIZMENTE VOCÊ PERDEU, ATÉ A PRÓXIMA!!!");
            }


            Console.ResetColor();
        }

        private static void setScore()
        {
            _clientSocket.Send(Encoding.ASCII.GetBytes("score"));
            byte[] receivedBuff = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuff);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuff, data, rec);
            score = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(Encoding.ASCII.GetString(data));
        }

        private static void setWords()
        {
            _clientSocket.Send(Encoding.ASCII.GetBytes("words"));
            byte[] receivedBuff = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuff);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuff, data, rec);
            words = JsonConvert.DeserializeObject<List<string>>(Encoding.ASCII.GetString(data));
        }

        private static string getPrize()
        {
            _clientSocket.Send(Encoding.ASCII.GetBytes("prize"));
            byte[] receivedBuff = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuff);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuff, data, rec);
            return Encoding.ASCII.GetString(data);
        }

        private static string makeGuess(string guess)
        {
            _clientSocket.Send(Encoding.ASCII.GetBytes($"guess|{guess[0]}"));
            byte[] receivedBuff = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuff);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuff, data, rec);
            return Encoding.ASCII.GetString(data);
        }

        private static void LoopConnect()
        {
            Console.WriteLine("Starting");
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;

                    _clientSocket.Connect(IPAddress.Loopback, 100);

                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine($"Connection attempts: {attempts.ToString()}");
                }
            }

            Console.Clear();
            Console.WriteLine("Connectred!!");
        }

        private static void DrawWords()
        {
            setScore();

            setWords();

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("RODA A RODA");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("PLACAR:");
            foreach (var player in score)
            {
                if (player.Item1.Equals(playerName)) 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\t{player.Item1} - {player.Item2}");
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else
                {
                    Console.WriteLine($"\t{player.Item1} - {player.Item2}");
                }
            }

            Console.ResetColor();
            Console.WriteLine();

            foreach (var word in words)
            {
                Console.WriteLine(word);
            }

            Console.WriteLine();
        }
    }
}
