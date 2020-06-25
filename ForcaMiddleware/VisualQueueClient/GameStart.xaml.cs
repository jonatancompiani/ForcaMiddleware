using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisualQueueClient
{
    /// <summary>
    /// Interaction logic for GameStart.xaml
    /// </summary>
    public partial class GameStart : Window
    {
        public GameStart()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(txtPlayer.Text).Show();
            this.Close();
        }

        private void txtPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                new MainWindow(txtPlayer.Text).Show();
                this.Close();
            }
        }

        private void txtPlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnPlay.IsEnabled = txtPlayer.Text.Length > 0;
        }
    }
}
