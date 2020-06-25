using Logic;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace QueueServer
{
    class Program
    {
        private const string HOST_NAME = "localhost";

        static IModel channel;
        static IConnection connection;
        static string consumerTag;

        static void Main(string[] args)
        {

            GameLogic.Startup();

            // active listening
            var factory = new ConnectionFactory() { HostName = HOST_NAME };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("serverQueue", false, false, false, null);
                var consumer = new ServerConsumer(channel);
                consumerTag = channel.BasicConsume("serverQueue", true, consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

                channel.BasicCancel(consumerTag);
            }
        }

        public static void makeGuess(string player, char guess)
        {
            var hitCount = GameLogic.MakeGuessForPlayer(player, guess);
            GameLogic.GetPrize(player);

            sendMessage("score", JsonConvert.SerializeObject(GameLogic.GetPlayerScores()));
            sendMessage("invalidLetters", JsonConvert.SerializeObject(GameLogic.GetInvalidLetters()));

            if (hitCount > 0)
            {
                var words = GameLogic.GetWords();
                sendMessage("word", JsonConvert.SerializeObject(words));
                if (!words.Any(x => x.Contains("_")))
                {
                    sendMessage("gameOver", "");
                }
            }
        }

        public static void addPlayer(string player)
        {
            GameLogic.AddPlayer(player);
            sendMessage("score", JsonConvert.SerializeObject(GameLogic.GetPlayerScores()));
            sendMessage("word", JsonConvert.SerializeObject(GameLogic.GetWords()));
        }

        private static void sendMessage(string type, string content)
        {
            var factory = new ConnectionFactory() { HostName = HOST_NAME };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: "clientExchange", type: ExchangeType.Fanout);

                //channel.QueueDeclare(queue: "clientQueue",
                //                     durable: false,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                var body = Encoding.UTF8.GetBytes(content);

                IBasicProperties props = channel.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("type", type);

                channel.BasicPublish(exchange: "clientExchange",
                                     routingKey: "",
                                     basicProperties: props,
                                     body: body);
            }
        }
    }
}
