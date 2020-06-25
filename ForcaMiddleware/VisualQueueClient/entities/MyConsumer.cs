using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualQueueClient
{
    class MyConsumer : DefaultBasicConsumer
    {
        MainWindow window;

        public MyConsumer(MainWindow instance, IModel model) : base(model)
        {
            this.window = instance;
        }
        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var message = Encoding.UTF8.GetString(body.ToArray());

            var type = properties.Headers.FirstOrDefault(x => "type".Equals(x.Key));
            this.window.Dispatcher.BeginInvoke(this.window.gameUpdateDelegate, System.Windows.Threading.DispatcherPriority.Normal, Encoding.UTF8.GetString(type.Value as byte[]), message);
        }
    }
}
