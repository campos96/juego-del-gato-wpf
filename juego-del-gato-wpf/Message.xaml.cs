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

namespace juego_del_gato_wpf
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public Message(bool? userWins)
        {
            InitializeComponent();

            if (userWins != null)
            {
                lblMessage.Content = (bool)userWins ? "Tu ganaste!" : "Yo gane!";
                imgWinner.Source = new BitmapImage(new Uri((bool)userWins ? "Resources/close_white.png" : "Resources/circle_white.png", UriKind.Relative));
            }
            else
            {
                lblMessage.Content = "Empate!";
                imgWinner.Source = new BitmapImage(new Uri("Resources/fire_white.png", UriKind.Relative));
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
