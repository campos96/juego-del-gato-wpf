using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace juego_del_gato_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool GameIsRunning = false;
        private Random random = new Random();
        private bool IsUsersTurn = false;
        private BackgroundWorker bwPcProcess = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            bwPcProcess.WorkerSupportsCancellation = true;
            bwPcProcess.WorkerReportsProgress = true;
            bwPcProcess.DoWork += BwPcProcess_DoWork;
            bwPcProcess.RunWorkerCompleted += BwPcProcess_RunWorkerCompleted;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Content = "Reiniciar";
            RestartGame();

            GameIsRunning = true;
            SetTurn();
            lblStatus.Content = IsUsersTurn ? "Es tu turno" : "Es mi turno";
            if (!IsUsersTurn)
            {
                PCMove();
            }
            else
            {
                IsUsersTurn = true;
            }
        }

        private void RestartGame()
        {

            var buttons = ButtonsGrid.Children.OfType<Button>().ToList();
            foreach (var button in buttons)
            {
                button.Tag = "";
                button.Content = null;
                button.Background = Brushes.White;
            }
        }

        private void SetTurn()
        {
            IsUsersTurn = Convert.ToBoolean(random.Next(0, 2));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!GameIsRunning || !IsUsersTurn)
            {
                return;
            }
            var button = (Button)sender;
            if (button.Tag.ToString() == "")
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new Uri("Resources/close.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                button.Tag = "x";
                if (!IsGameOver())
                {
                    PCMove();
                }
            }
        }

        private bool IsGameOver()
        {
            var matrix = GetMatrix();
            if (LineFind(matrix))
            {
                GameIsRunning = false;
                lblStatus.Content = IsUsersTurn ? "Tu ganaste" : "Yo gane";
                Message message = new Message(IsUsersTurn);
                message.ShowDialog();
                return true;
            }
            else
            {
                return false;
            }
        }

        //puede mejorar
        private bool LineFind(string[,] matrix)
        {
            if (LineFindBy(matrix, "o"))
            {
                return true;
            }

            if (LineFindBy(matrix, "x"))
            {
                return true;
            }

            return false;
        }

        private bool LineFindBy(string[,] matrix, string type)
        {
            var buttons = ButtonsGrid.Children.OfType<Button>().ToList();
            var winnerColor = Brushes.Green;

            if (matrix[0, 0] == type && matrix[0, 1] == type && matrix[0, 2] == type)
            {
                buttons[0].Background = winnerColor;
                buttons[1].Background = winnerColor;
                buttons[2].Background = winnerColor;
                return true;
            }
            if (matrix[1, 0] == type && matrix[1, 1] == type && matrix[1, 2] == type)
            {
                buttons[3].Background = winnerColor;
                buttons[4].Background = winnerColor;
                buttons[5].Background = winnerColor;
                return true;
            }
            if (matrix[2, 0] == type && matrix[2, 1] == type && matrix[2, 2] == type)
            {
                buttons[6].Background = winnerColor;
                buttons[7].Background = winnerColor;
                buttons[8].Background = winnerColor;
                return true;
            }

            if (matrix[0, 0] == type && matrix[1, 0] == type && matrix[2, 0] == type)
            {
                buttons[0].Background = winnerColor;
                buttons[3].Background = winnerColor;
                buttons[6].Background = winnerColor;
                return true;
            }
            if (matrix[0, 1] == type && matrix[1, 1] == type && matrix[2, 1] == type)
            {
                buttons[1].Background = winnerColor;
                buttons[4].Background = winnerColor;
                buttons[7].Background = winnerColor;
                return true;
            }
            if (matrix[0, 2] == type && matrix[1, 2] == type && matrix[2, 2] == type)
            {
                buttons[2].Background = winnerColor;
                buttons[5].Background = winnerColor;
                buttons[8].Background = winnerColor;
                return true;
            }

            if (matrix[0, 0] == type && matrix[1, 1] == type && matrix[2, 2] == type)
            {
                buttons[0].Background = winnerColor;
                buttons[4].Background = winnerColor;
                buttons[8].Background = winnerColor;
                return true;
            }
            if (matrix[0, 2] == type && matrix[1, 1] == type && matrix[2, 0] == type)
            {
                buttons[6].Background = winnerColor;
                buttons[4].Background = winnerColor;
                buttons[2].Background = winnerColor;
                return true;
            }
            return false;
        }

        private string[,] GetMatrix()
        {
            var matrix = new string[3, 3];
            var buttons = ButtonsGrid.Children.OfType<Button>().ToList();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var x = (i * 2) + j + i;
                    matrix[i, j] = buttons[x].Tag.ToString();
                }
            }
            return matrix;
        }

        private void PCMove()
        {
            IsUsersTurn = false;
            lblStatus.Content = IsUsersTurn ? "Es tu turno" : "Es mi turno";
            var buttons = ButtonsGrid.Children.OfType<Button>().Where(b => b.Tag.ToString() == "").ToList();
            if (buttons.Count() > 0)
            {
                if (!bwPcProcess.IsBusy)
                {
                    bwPcProcess.RunWorkerAsync();
                }
            }
            else
            {
                ValidateDraw();
            }
        }

        private void ValidateDraw()
        {
            var buttons = ButtonsGrid.Children.OfType<Button>().Where(b => b.Tag.ToString() == "").ToList();
            if (buttons.Count() == 0)
            {
                lblStatus.Content = "Empate";
                Message message = new Message(null);
                message.ShowDialog();
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!GameIsRunning || !IsUsersTurn)
            {
                return;
            }
            var button = (Button)sender;
            if (button.Tag.ToString() == "")
            {
                button.Content = new Image
                {
                    Source = new BitmapImage(new Uri("Resources/close.png", UriKind.Relative)),
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!GameIsRunning || !IsUsersTurn)
            {
                return;
            }
            var button = (Button)sender;
            if (button.Tag.ToString() == "")
            {
                button.Content = null;
            }
        }


        private void BwPcProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(3000);
        }

        private void BwPcProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var button = GetSelection();
            button.Tag = "o";
            button.Content = new Image
            {
                Source = new BitmapImage(new Uri("Resources/circle.png", UriKind.Relative)),
                VerticalAlignment = VerticalAlignment.Center
            };

            if (!IsGameOver())
            {
                IsUsersTurn = true;
                lblStatus.Content = IsUsersTurn ? "Es tu turno" : "Es mi turno";

                ValidateDraw();
            }
        }

        private Button GetSelection()
        {
            var buttons = ButtonsGrid.Children.OfType<Button>().Where(b => b.Tag.ToString() == "").ToList();
            var button = buttons[random.Next(0, buttons.Count())];

            var potential = GetPotentialByType("o");
            if (potential != null)
            {
                button = potential;
            }
            else
            {
                potential = GetPotentialByType("x");
                if (potential != null)
                {
                    button = potential;
                }
            }
            return button;
        }

        private Button GetPotentialByType(string type)
        {
            var button = GetPotentialByRange(new[] { 0, 1, 2 }, type);
            if (button != null)
            {
                return button;
            }
            button = GetPotentialByRange(new[] { 3, 4, 5 }, type);
            if (button != null)
            {
                return button;
            }
            button = GetPotentialByRange(new[] { 6, 7, 8 }, type);
            if (button != null)
            {
                return button;
            }

            button = GetPotentialByRange(new[] { 0, 3, 6 }, type);
            if (button != null)
            {
                return button;
            }
            button = GetPotentialByRange(new[] { 1, 4, 7 }, type);
            if (button != null)
            {
                return button;
            }
            button = GetPotentialByRange(new[] { 2, 5, 8 }, type);
            if (button != null)
            {
                return button;
            }

            button = GetPotentialByRange(new[] { 0, 4, 8 }, type);
            if (button != null)
            {
                return button;
            }

            button = GetPotentialByRange(new[] { 2, 4, 6 }, type);
            if (button != null)
            {
                return button;
            }



            return null;
        }

        private Button GetPotentialByRange(int[] range, string type)
        {
            var buttons = ButtonsGrid.Children.OfType<Button>().ToList();
            var buttonList = new List<Button> {
                buttons[range[0]], buttons[range[1]], buttons[range[2]]
            }.ToList();

            if (buttonList.Where(b => b.Tag.ToString() == type).Count() > 1)
            {
                if (buttonList.Where(b => b.Tag.ToString() == "").Count() == 1)
                {
                    return buttonList.Where(b => b.Tag.ToString() == "").FirstOrDefault();
                }
            }

            return null;
        }
    }
}
