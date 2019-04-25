/********************************************************************
	created:	2014/02/22
	created:	22:2:2014   12:28
	filename: 	R:\CSE483\VCStudio\WindowsProgramming\WPF Samples\BrickBreaker\Brick.cs
	file path:	R:\CSE483\VCStudio\WindowsProgramming\WPF Samples\BrickBreaker
	file base:	Brick
	file ext:	cs
	author:		
	
	purpose:	This file contains the Brick class, which is used in the Brick Breaker
                game. This class simply contains the bound properties used in the
                brick observable collection
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// INotifyPropertyChanged
using System.ComponentModel;

using System.Windows.Media;

// Rectangle
using System.Drawing;


namespace BouncingBall
{
    public class Brick : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private System.Windows.Visibility _brickVisible;
        public System.Windows.Visibility BrickVisible
        {
            get { return _brickVisible; }
            set
            {
                _brickVisible = value;
                OnPropertyChanged("BrickVisible");
            }
        }

        private double _brickCanvasTop;
        public double BrickCanvasTop
        {
            get { return _brickCanvasTop; }
            set
            {
                _brickCanvasTop = value;
                OnPropertyChanged("BrickCanvasTop");
            }
        }

        private double _brickCanvasLeft;
        public double BrickCanvasLeft
        {
            get { return _brickCanvasLeft; }
            set
            {
                _brickCanvasLeft = value;
                OnPropertyChanged("BrickCanvasLeft");
            }
        }

        private double _brickHeight;
        public double BrickHeight
        {
            get { return _brickHeight; }
            set
            {
                _brickHeight = value;
                OnPropertyChanged("BrickHeight");
            }
        }

        private double _brickWidth;
        public double BrickWidth
        {
            get { return _brickWidth; }
            set
            {
                _brickWidth = value;
                OnPropertyChanged("BrickWidth");
            }
        }

        private  System.Windows.Media.Brush _brickFill;
        public System.Windows.Media.Brush BrickFill
        {
            get { return _brickFill; }
            set
            {
                _brickFill = value;
                OnPropertyChanged("BrickFill");
            }
        }

        private String _brickName;
        public String BrickName
        {
            get { return _brickName; }
            set { _brickName = value; }
        }

        private Rectangle _brickRectangle;
        public Rectangle BrickRectangle
        {
            get { return _brickRectangle; }
            set { _brickRectangle = value; }
        }
    }
}
