using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;

namespace VisualQueueClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IModel channel;
        IConnection connection;
        string consumerTag;

        string player;

        public delegate void updateDelegate(string type, string content);
        public updateDelegate gameUpdateDelegate;

        private ObservableCollection<Player> playerList = new ObservableCollection<Player>();
        private ObservableCollection<string> wordList = new ObservableCollection<string>();
        private List<char> attemptList = new List<char>();

        public MainWindow(string player)
        {
            InitializeComponent();

            this.player = player;
            this.Title = $"Roda a Roda - {player}";

            gameUpdateDelegate = new updateDelegate(gameUpdate);
            dtgPlayers.ItemsSource = playerList;
            lstWords.ItemsSource = wordList;

            // Sytart listening
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //connection = factory.CreateConnection();
            //channel = connection.CreateModel();
            //channel.QueueDeclare("clientQueue", false, false, false, null);
            //var consumer = new MyConsumer(this, channel);
            //consumerTag = channel.BasicConsume("clientQueue", true, consumer);

           var factory = new ConnectionFactory() { HostName = "localhost" };
           connection = factory.CreateConnection();
           channel = connection.CreateModel();


            channel.ExchangeDeclare(exchange: "clientExchange", type: ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "clientExchange",
                              routingKey: "");
            var consumer = new MyConsumer(this, channel);
            channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);




            sendMessage("newPlayer", this.player);
        }

        #region Delegate Methods


        public void gameUpdate(string type, string content)
        {
            switch (type)
            {
                case "score":

                    var players = JsonConvert.DeserializeObject<List<Player>>(content);

                    playerList.Clear();
                    foreach (var player in players)
                    {
                        playerList.Add(player);
                        if (player.name.Equals(this.player))
                        {
                            lblMessage.Content = $"{this.player} valendo {player.prize} reais, qual letra você escolhe?";
                        }
                    }
                    break;
                case "word":

                    var words = JsonConvert.DeserializeObject<List<string>>(content);

                    wordList.Clear();
                    foreach (var word in words)
                    {
                        wordList.Add(word);
                    }
                    break;
                case "invalidLetters":
                    this.attemptList = JsonConvert.DeserializeObject<List<char>>(content);
                    lblAttempts.Content = string.Join(", ", this.attemptList);

                    break;
                case "gameOver":

                    btnSubmitGuess.Visibility = Visibility.Hidden;
                    txtGuess.Visibility = Visibility.Hidden;

                    var myScore = playerList.First(X => X.name.Equals(this.player));
                    if(playerList.OrderByDescending(o=> o.score).First().name.Equals(this.player))
                    {
                        lblMessage.Foreground = new SolidColorBrush(Colors.Green);
                        lblMessage.Content = $"PARABÉNS, VOCÊ GANHOU E LEVA PARA CASA {myScore.score} REAIS!!!";
                    }
                    else
                    {
                        lblMessage.Foreground = new SolidColorBrush(Colors.Red);
                        lblMessage.Content = $"INFELIZMENTE VOCÊ PERDEU, MAIS SORTE NA PRÓXIMA!!!";

                    }
                    lblMessage.FontSize = 25;


                    break;
            }


            
        }

        #endregion


        /// <summary>
        /// Submit guess button click
        /// </summary>
        private void btnSubmitGuess_Click(object sender, RoutedEventArgs e)
        {
            sendGuess();
        }

        /// <summary>
        /// Window closing event
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                channel.BasicCancelNoWait(consumerTag);
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtGuess_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSubmitGuess.IsEnabled = txtGuess.Text.Length > 0;
        }

        private void txtGuess_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                sendGuess();
            }
        }


        private void sendGuess()
        {
            if (attemptList.Any(x => Char.ToUpper(x) == Char.ToUpper(txtGuess.Text[0])))
            {
                MessageBox.Show("Esta letra já foi tentada, por favor escolha outra!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                sendMessage("guess", txtGuess.Text);
            }
            txtGuess.Clear();
            txtGuess.Focus();
        }

        private void sendMessage(string type, string content)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "serverQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(content);

                IBasicProperties props = channel.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("type", type);
                props.Headers.Add("player", this.player ?? "");

                channel.BasicPublish(exchange: "",
                                     routingKey: "serverQueue",
                                     basicProperties: props,
                                     body: body);
            }
        }
    }
}
