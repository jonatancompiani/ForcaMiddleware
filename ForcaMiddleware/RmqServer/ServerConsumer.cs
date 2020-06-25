using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServer
{
    class ServerConsumer : DefaultBasicConsumer
    {
        public ServerConsumer(IModel model) : base(model)
        {
        }
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());
            
            var type = properties.Headers.FirstOrDefault(x => "type".Equals(x.Key));

            switch (Encoding.UTF8.GetString(type.Value as byte[]))
            {
                case "guess":
                    var player = properties.Headers.FirstOrDefault(x => "player".Equals(x.Key));
                    Program.makeGuess(Encoding.UTF8.GetString(player.Value as byte[]), message[0]);
                    break;
                case "newPlayer":
                    Program.addPlayer(message);
                    break;
            }

        }
    }
}
