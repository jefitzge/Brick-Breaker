using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// observable collections
using System.Collections.ObjectModel;

// debug output
using System.Diagnostics;

// sleep
//using System.Threading;


// Rectangle
// Must update References manually
using System.Drawing;

// INotifyPropertyChanged
using System.ComponentModel;

// WPF Timer
using System.Threading;


namespace BouncingBall
{
    public partial class Model : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static UInt32 _numBalls = 1;
        private UInt32[] _buttonPresses = new UInt32[_numBalls];
        Random _randomNumber = new Random();

        // .NET Timer
        private Timer _ballHiResTimer;
        private Timer _paddelHiResTimer;

        private double _ballXMove = 1;
        private double _ballYMove = 1;
        System.Drawing.Rectangle _ballRectangle;
        System.Drawing.Rectangle _paddelRectangle;
        bool _movePaddelLeft = false;
        bool _movePaddelRight = false;
        private bool _moveBall = false;
        public bool MoveBall
        {
            get { return _moveBall; }
            set { _moveBall = value; }
        }

        private double _windowHeight = 100;
        public double WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; }
        }

        private double _windowWidth = 100;
        public double WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

        /// <summary>
        /// Model constructor
        /// </summary>
        /// <returns></returns>
        public Model()
        {
        }

        public void InitModel()
        {
            // this delegate is needed for the multi media timer defined 
            // in the TimerQueueTimer class.
            _ballHiResTimer = new Timer(BallTimerCallback,null,2,2);
            _paddelHiResTimer = new Timer(PaddelTimerCallback,null,2,2);

        }

        public void CleanUp()
        {
        }


        public void SetStartPosition()
        {
            
            BallHeight = 25;
            BallWidth = 25;
            PaddelWidth = 120;
            PaddelHeight = 5;

            BallCanvasLeft = _windowWidth/2 - BallWidth/2;
            BallCanvasTop = _windowHeight/3;
           
            _moveBall = false;

            PaddelCanvasLeft = _windowWidth / 2 - PaddelWidth / 2;
            PaddelCanvasTop = _windowHeight - PaddelHeight;
            _paddelRectangle = new System.Drawing.Rectangle((int)PaddelCanvasLeft, (int)PaddelCanvasTop, (int)PaddelWidth, (int)PaddelHeight);
        }

        public void MoveLeft(bool move)
        {
            _movePaddelLeft = move;
        }

        public void MoveRight(bool move)
        {
            _movePaddelRight = move;
        }


        private void BallTimerCallback(object state)
        {

            if (!_moveBall)
                return;

            BallCanvasLeft += _ballXMove;
            BallCanvasTop += _ballYMove;

            // check to see if ball has it the left or right side of the drawing element
            if ((BallCanvasLeft + BallWidth >= _windowWidth) ||
                (BallCanvasLeft <= 0))
                _ballXMove = -_ballXMove;


            // check to see if ball has it the top of the drawing element
            if ( BallCanvasTop <= 0) 
                _ballYMove = -_ballYMove;

            if (BallCanvasTop + BallWidth >= _windowHeight)
            {
                // we hit bottom. stop moving the ball
                _moveBall = false;
            }

            // see if we hit the paddle
            _ballRectangle = new System.Drawing.Rectangle((int)BallCanvasLeft, (int)BallCanvasTop, (int)BallWidth, (int)BallHeight);
            if (_ballRectangle.IntersectsWith(_paddelRectangle))
            {
                // hit paddle. reverse direction in Y direction
                _ballYMove = -_ballYMove;

                // move the ball away from the paddle so we don't intersect next time around and
                // get stick in a loop where the ball is bouncing repeatedly on the paddle
                BallCanvasTop += 2*_ballYMove;

                // move the ball in X some small random value so that ball is not traveling in the same 
                // pattern
                BallCanvasLeft += _randomNumber.Next(5);
            }

        }

        private void PaddelTimerCallback(object state)
        {

            if (_movePaddelLeft && PaddelCanvasLeft > 0)
                PaddelCanvasLeft -= 2;
            else if (_movePaddelRight && PaddelCanvasLeft < _windowWidth - PaddelWidth)
                PaddelCanvasLeft += 2;
            
            _paddelRectangle = new System.Drawing.Rectangle((int)PaddelCanvasLeft, (int)PaddelCanvasTop, (int)PaddelWidth, (int)PaddelHeight);
        }
    }
}
