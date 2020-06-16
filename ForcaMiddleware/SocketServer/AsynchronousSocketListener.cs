using Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static List<Tuple<string, string>> players = new List<Tuple<string, string>>();

        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer();
            GameLogic.Startup();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting server up...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            Console.WriteLine("Client Connected!!");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] dataBuff = new byte[received];
            Array.Copy(_buffer, dataBuff, received);
            string text = Encoding.ASCII.GetString(dataBuff);
            Console.WriteLine($"Text received: {text}");

            string response = string.Empty;
            var playerDetails = players.FirstOrDefault(x => x.Item2.Equals(socket.RemoteEndPoint.ToString()));

            switch (text.Split("|")[0].ToLower())
            {
                case "start":
                    string player = text.Split("|")[1];
                    players.Add(new Tuple<string, string>(player, socket.RemoteEndPoint.ToString()));
                    GameLogic.AddPlayer(player);
                    response = "Start!";
                    Console.WriteLine($"{player} started playing the game!");
                    break;

                case "score":
                    response = JsonConvert.SerializeObject(GameLogic.GetScores());
                    break;

                case "words":
                    response = JsonConvert.SerializeObject(GameLogic.GetWords());
                    break;

                case "prize":
                    if (playerDetails != null)
                    {
                        response = GameLogic.GetPrize(playerDetails.Item1).ToString();
                    }
                    else
                    {
                        response = "0";
                    }
                    break;

                case "guess":
                    if (playerDetails != null)
                    {
                        response = GameLogic.MakeGuessForPlayer(playerDetails.Item1, text.Split("|")[1][0]).ToString();
                    }
                    else
                    {
                        response = "0";
                    }
                    break;

                default:
                    break;
            }

            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }


        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }


    }
}
