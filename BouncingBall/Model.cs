using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// observable collections
using System.Collections.ObjectModel;

// debug output
using System.Diagnostics;

// timer, sleep
using System.Threading;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

// hi res timer
using PrecisionTimers;

// Rectangle
// Must update References manually
using System.Drawing;

// INotifyPropertyChanged
using System.ComponentModel;

using System.Windows.Threading;


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
        //VARIABLES
        private static UInt32 _numBalls = 1;
        private UInt32[] _buttonPresses = new UInt32[_numBalls];
        Random _randomNumber = new Random();
        private TimerQueueTimer.WaitOrTimerDelegate _ballTimerCallbackDelegate;
        private TimerQueueTimer.WaitOrTimerDelegate _paddleTimerCallbackDelegate;
        private TimerQueueTimer _ballHiResTimer;
        private TimerQueueTimer _paddleHiResTimer;
        System.Drawing.Rectangle _paddleRectangle;
        System.Drawing.Rectangle _ballRectangle;
        bool _movepaddleLeft = false;
        bool _movepaddleRight = false;
        uint _pushMove = 20;
        System.Windows.Media.Brush FillColorRed;
        System.Windows.Media.Brush FillColorBlue;
        private DispatcherTimer _mytimer;

        public ObservableCollection<Brick> BrickCollection;
        static int _numBricks = 30;
        Rectangle[] _brickRectangles = new Rectangle[_numBricks];
        // note that the brick hight, number of brick columns and rows
        // must match our window demensions.
        double _brickHeight = 25;
        double _brickWidth = 75;
        int _numBrickRows = 3;
        int _numBricksColumns = 10;

        //This controls the speed of the ball
        private double _ballXMove = .5;
        private double _ballYMove = .5;

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

        //This is for the score canvas stuff
        private int _elapsedTimeBlock;
        public int ElapsedTimeBlock
        {
            get { return _elapsedTimeBlock; }
            set
            {
                _elapsedTimeBlock = value;
                OnPropertyChanged("ElapsedTimeBlock");
            }
        }

        private int _scoreBlock = 0;
        public int ScoreBlock
        {
            get { return _scoreBlock; }
            set
            {
                _scoreBlock = value;
                OnPropertyChanged("ScoreBlock");
            }
        }

        /// <summary>
        /// Model constructor
        /// </summary>
        /// <returns></returns>
        public Model()
        {
            _randomNumber = new Random();
            SolidColorBrush mySolidColorBrushRed = new SolidColorBrush();
            SolidColorBrush mySolidColorBrushBlue = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.

            mySolidColorBrushRed.Color = System.Windows.Media.Color.FromRgb(255, 0, 0);
            FillColorRed = mySolidColorBrushRed;
            mySolidColorBrushBlue.Color = System.Windows.Media.Color.FromRgb(0, 0, 255);
            FillColorBlue = mySolidColorBrushBlue;
        }

        public void checkTime()
        {
            if (_mytimer.IsEnabled)
            {
                _mytimer.Stop();
            }
            else
            {
                _mytimer.Start();
            }
        }

        public void InitModel()
        {
            // create brick collection
            // place them manually at the top of the item collection in the view
            BrickCollection = new ObservableCollection<Brick>();
            for (int i = 0; i < _numBrickRows; i++)
            {
                for (int brick = 0; brick < _numBricksColumns; brick++)
                {
                    BrickCollection.Add(new Brick()
                    {
                        BrickCanvasTop = i * _brickHeight,
                        BrickCanvasLeft = brick * _brickWidth,
                        BrickFill = FillColorRed,
                        BrickHeight = _brickHeight,
                        BrickWidth = _brickWidth,
                        BrickVisible = System.Windows.Visibility.Visible,
                        BrickName = brick.ToString(),
                    });

                    //BrickCollection[brick].BrickCanvasLeft = _windowWidth / 2 - _brickWidth / 2;
                    //BrickCollection[brick].BrickCanvasTop = brick * _brickHeight + 150; // offset the bricks from the top of the screen by a bitg
                }
            }



            //ELAPSED TIMER GOES HERE
         
            _mytimer = new DispatcherTimer();
            _mytimer.Tick += new EventHandler(BallTimerCallback);

            _mytimer.Interval = new TimeSpan(0, 0, 1);
            

            
            // this delegate is needed for the multi media timer defined 
            // in the TimerQueueTimer class.
            _ballTimerCallbackDelegate = new TimerQueueTimer.WaitOrTimerDelegate(BallMMTimerCallback);
            _paddleTimerCallbackDelegate = new TimerQueueTimer.WaitOrTimerDelegate(paddleMMTimerCallback);

            // create our multi-media timers
            _ballHiResTimer = new TimerQueueTimer();
            try
            {
                // create a Multi Media Hi Res timer.
                _ballHiResTimer.Create(1, 1, _ballTimerCallbackDelegate);
            }
            catch (QueueTimerException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Failed to create Ball timer. Error from GetLastError = {0}", ex.Error);
            }

            _paddleHiResTimer = new TimerQueueTimer();
            try
            {
                // create a Multi Media Hi Res timer.
                _paddleHiResTimer.Create(2, 2, _paddleTimerCallbackDelegate);
            }
            catch (QueueTimerException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Failed to create paddle timer. Error from GetLastError = {0}", ex.Error);
            }

        }

        private void BallTimerCallback(object obj, EventArgs ea)
        {
            ElapsedTimeBlock++;
        }

        //This is for the brick canvas stuff

        public void SetStartPosition()
        {
            BallHeight = 35;
            BallWidth = 35;
            paddleWidth = 120;
            paddleHeight = 10;

            ballCanvasLeft = _windowWidth / 2 - BallWidth / 2;
            ballCanvasTop = _windowHeight / 4;

            _moveBall = false;

            paddleCanvasLeft = _windowWidth / 2 - paddleWidth / 2;
            paddleCanvasTop = _windowHeight - paddleHeight;
            _paddleRectangle = new System.Drawing.Rectangle((int)paddleCanvasLeft, (int)paddleCanvasTop, (int)paddleWidth, (int)paddleHeight);

        }


        enum InterectSide { NONE, LEFT, RIGHT, TOP, BOTTOM };
        private InterectSide IntersectsAt(Rectangle brick, Rectangle ball)
        {
            if (brick.IntersectsWith(ball) == false)
                return InterectSide.NONE;

            Rectangle r = Rectangle.Intersect(brick, ball);

            // did we hit the top of the brick
            if (ball.Top + ball.Height - 1 == r.Top &&
                r.Height == 1)
                return InterectSide.TOP;

            if (ball.Top == r.Top &&
                r.Height == 1)
                return InterectSide.BOTTOM;

            if (ball.Left == r.Left &&
                r.Width == 1)
                return InterectSide.RIGHT;

            if (ball.Left + ball.Width - 1 == r.Left &&
                r.Width == 1)
                return InterectSide.LEFT;

            return InterectSide.NONE;
        }

        public void MoveLeft(bool move)
        {
            _movepaddleLeft = move;
        }

        public void MoveRight(bool move)
        {
            _movepaddleRight = move;
        }
        
        public void CleanUp()
        {
            _ballHiResTimer.Delete();
            _paddleHiResTimer.Delete();
        }

        public void ResetAll()
        {
            ScoreBlock = 0;
            ElapsedTimeBlock = 0;
            _mytimer.Stop();
            for (int i = 0; i < _numBricks; i++)
            {
                if (BrickCollection[i].BrickVisible == System.Windows.Visibility.Hidden)
                {
                    BrickCollection[i].BrickVisible = System.Windows.Visibility.Visible;
                }
            }

        }

        public void ResetBall()
        {
            ballCanvasLeft = _windowWidth / 2 - BallWidth / 2;
            ballCanvasTop = _windowHeight / 4;
        }

     

        private void BallMMTimerCallback(IntPtr pWhat, bool success)
        {

            if (!_moveBall)
                return;

            // start executing callback. this ensures we are synched correctly
            // if the form is abruptly closed
            // if this function returns false, we should exit the callback immediately
            // this means we did not get the mutex, and the timer is being deleted.
            if (!_ballHiResTimer.ExecutingCallback())
            {
                Console.WriteLine("Aborting timer callback.");
                return;
            }

            ballCanvasLeft += _ballXMove;
            ballCanvasTop += _ballYMove;

            // check to see if ball has it the left or right side of the drawing element
            if ((ballCanvasLeft + BallWidth >= _windowWidth) ||
                (ballCanvasLeft <= 0))
                _ballXMove = -_ballXMove;


            // check to see if ball has it the top of the drawing element
            if (ballCanvasTop <= 0)
                _ballYMove = -_ballYMove;

            if (ballCanvasTop + BallWidth >= _windowHeight)
            {
                // we hit bottom. stop moving the ball
                _moveBall = false;
                _mytimer.Stop();
            }

            // see if we hit the paddle
            _ballRectangle = new System.Drawing.Rectangle((int)ballCanvasLeft, (int)ballCanvasTop, (int)BallWidth, (int)BallHeight);
            if (_ballRectangle.IntersectsWith(_paddleRectangle))
            {
                // hit paddle. reverse direction in Y direction
                _ballYMove = -_ballYMove;

                // move the ball away from the paddle so we don't intersect next time around and
                // get stick in a loop where the ball is bouncing repeatedly on the paddle
                ballCanvasTop += 2 * _ballYMove;

                // add move the ball in X some small random value so that ball is not traveling in the same 
                // pattern
                ballCanvasLeft += _randomNumber.Next(5);
            }

            // done in callback. OK to delete timer
            _ballHiResTimer.DoneExecutingCallback();


            //UPDATE THE RECTANGLES HERE
            for (int brick = 0; brick < _numBricks; brick++)
            {
                BrickCollection[brick].BrickRectangle = new System.Drawing.Rectangle((int)BrickCollection[brick].BrickCanvasLeft, (int)BrickCollection[brick].BrickCanvasTop, (int)BrickCollection[brick].BrickWidth, (int)BrickCollection[brick].BrickHeight);
                if (BrickCollection[brick].BrickFill != FillColorRed) continue;

                InterectSide whichSide = IntersectsAt(BrickCollection[brick].BrickRectangle, _ballRectangle);
                switch (whichSide)
                {
                    case InterectSide.NONE:
                        break;

                    case InterectSide.TOP:
                        if (BrickCollection[brick].BrickVisible == System.Windows.Visibility.Visible)
                        {
                            _ballYMove = -_ballYMove;
                            BrickCollection[brick].BrickVisible = System.Windows.Visibility.Hidden;
                            ScoreBlock++;
                        }
                        
                        break;

                    case InterectSide.BOTTOM:
                        
                        if (BrickCollection[brick].BrickVisible == System.Windows.Visibility.Visible)
                        {
                            _ballYMove = -_ballYMove;
                            BrickCollection[brick].BrickVisible = System.Windows.Visibility.Hidden;
                            ScoreBlock++;
                        }
                        break;

                    case InterectSide.LEFT:
                       
                        if (BrickCollection[brick].BrickVisible == System.Windows.Visibility.Visible)
                        {
                            _ballXMove = -_ballXMove;
                            BrickCollection[brick].BrickVisible = System.Windows.Visibility.Hidden;
                            ScoreBlock++;
                        }
                        break;

                    case InterectSide.RIGHT:
                        
                        if (BrickCollection[brick].BrickVisible == System.Windows.Visibility.Visible)
                        {
                            _ballXMove = -_ballXMove;
                            BrickCollection[brick].BrickVisible = System.Windows.Visibility.Hidden;
                            ScoreBlock++;
                        }
                        break;
                }
            }
        }

        private void paddleMMTimerCallback(IntPtr pWhat, bool success)
        {

            // start executing callback. this ensures we are synched correctly
            // if the form is abruptly closed
            // if this function returns false, we should exit the callback immediately
            // this means we did not get the mutex, and the timer is being deleted.
            if (!_paddleHiResTimer.ExecutingCallback())
            {
                Console.WriteLine("Aborting timer callback.");
                return;
            }

            if (_movepaddleLeft && paddleCanvasLeft > 0)
                paddleCanvasLeft -= 2;
            else if (_movepaddleRight && paddleCanvasLeft < _windowWidth - paddleWidth)
                paddleCanvasLeft += 2;

            _paddleRectangle = new System.Drawing.Rectangle((int)paddleCanvasLeft, (int)paddleCanvasTop, (int)paddleWidth, (int)paddleHeight);


            // done in callback. OK to delete timer
            _paddleHiResTimer.DoneExecutingCallback();
        }
    }
}
