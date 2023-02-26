using Microsoft.Win32;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rectangle rectSelectArea;
        private Point startPoint;
        bool isMouseDown = false;
        bool startToMove = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image file|*.jpg; *.png; *.bmp";
            fileDialog.Title = "Please select a picture file";

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                BitmapImage ImageSource = new BitmapImage(new Uri(fileDialog.FileName));
                Picture.Source = ImageSource;
            }
            if (Picture.Source == null) 
            {
                String noUpload = "Please upload an image"; 
                tbInfo.Text = noUpload;
            }
        }

        private void DrawRec(object sender, MouseButtonEventArgs e) 
        {
            isMouseDown = true;
            startPoint = e.GetPosition(ImageCanvas);

            rectSelectArea = new Rectangle()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            ImageCanvas.Children.Add(rectSelectArea);
            Canvas.SetLeft(rectSelectArea, startPoint.X);
            Canvas.SetTop(rectSelectArea, startPoint.Y);
            

            rectSelectArea.MouseLeftButtonDown += Rec_MouseLeftButtonDown;
            rectSelectArea.MouseMove += Rec_MouseMove;
            rectSelectArea.MouseLeftButtonUp += Rec_MouseLeftButtonUp;
            //rectSelectArea.MouseRightButtonDown += Rec_MouseRightButtonDown;
        }

        private void Rec_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            //ColorPickerPalette colorPickerPalette = new ColorPickerPalette();

        }

        private void Rec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle) sender).Cursor = Cursors.Hand;
            startToMove = true;
            tbPosition.Text = "pressed the rect!";
        }

        private void Rec_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && startToMove == true) { 
                Point point = e.GetPosition(ImageCanvas);
                Rectangle rec = (Rectangle)sender;
                double margineLeft = point.X - rec.Width / 2;
                double margineTop = point.Y - rec.Height / 2;

                tbPosition.Text = "Canvas left: " + Canvas.GetLeft(Picture);
                if (margineLeft < 0) {
                    margineLeft = 0;
                }
                else if ((margineLeft + rec.Width) > ImageCanvas.Width) {
                    margineLeft = ImageCanvas.Width - rec.Width;
                }
                if(margineTop < 0) { 
                    margineTop= 0;
                }
                else if ((margineTop + rec.Height) > ImageCanvas.Height)
                {
                    margineTop = ImageCanvas.Height - rec.Height;
                }
                Canvas.SetLeft(rec, margineLeft);
                Canvas.SetTop(rec, margineTop);
            }
        }

        private void Rec_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle)sender).ReleaseMouseCapture();
            ((Rectangle)sender).Cursor = Cursors.Arrow;
            startToMove = false;
        }
      
        private void MouseButtonUp(object sender, MouseButtonEventArgs e) 
        {
            isMouseDown = false;
            //rectSelectArea = null;
            tbPosition.Text = "Button up: " + e.GetPosition(ImageCanvas).X + ", " + e.GetPosition(ImageCanvas).Y;
        }

        private void MouseMove(object sender, MouseEventArgs e) 
        {
            var position = e.GetPosition(ImageCanvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var x = Math.Min(position.X, startPoint.X);
                var y = Math.Min(position.Y, startPoint.Y);

                var w = Math.Max(position.X, startPoint.X) - x;
                var h = Math.Max(position.Y, startPoint.Y) - y;

                rectSelectArea.Width = w;
                rectSelectArea.Height = h;

                Canvas.SetLeft(rectSelectArea, x);
                Canvas.SetTop(rectSelectArea, y);

                tbPosition.Text = position.X + ", " + position.Y + "\tMouseDown: " + isMouseDown + (e.LeftButton == MouseButtonState.Pressed);
            }
            else {
                tbPosition.Text = "Stop!\t" + position.X + ", " + position.Y;
            }
        }

    }
}
