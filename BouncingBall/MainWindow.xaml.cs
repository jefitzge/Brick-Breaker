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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BouncingBall
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model _model;


        public MainWindow()
        {
            InitializeComponent();

            // make it so the user cannot resize the window
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // create an instance of our Model
            _model = new Model();
            _model.WindowHeight = BallCanvas.RenderSize.Height;
            _model.WindowWidth = BallCanvas.RenderSize.Width;
            this.DataContext = _model;
            _model.InitModel();
            _model.SetStartPosition();
            BrickItems.ItemsSource = _model.BrickCollection;

        }

        private void KeypadDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                _model.MoveLeft(true);
            else if (e.Key == Key.Right)
                _model.MoveRight(true);
            else if (e.Key == Key.S)
            {
                _model.MoveBall = !_model.MoveBall;
                _model.checkTime();
            }
            else if (e.Key == Key.R)
            {
                _model.SetStartPosition();
                _model.ResetAll();
            }
            else if (e.Key == Key.B)
                _model.ResetBall();
            else if (e.Key == Key.E)
                Close();
        }

        private void KeypadUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                _model.MoveLeft(false);
            else if (e.Key == Key.Right)
                _model.MoveRight(false);
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _model.CleanUp();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // one of the buttons in our collection. need to figure out
            // which one. Since we know the button is part of a collection, we 
            // have a special way that we need to get at its bame

            var selectedButton = e.OriginalSource as FrameworkElement;
            if (selectedButton != null)
            {
                var currentTile = selectedButton.DataContext as Brick;
               // _model.ToggleBrickColor(currentTile.BrickName);
            }
        }

    }
}
