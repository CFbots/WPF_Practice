using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        /// <summary>
        /// Upload the picture
        /// </summary>
        private void openPic_Click(object sender, RoutedEventArgs e)
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
                tbInfo.Text = "Please upload an image";
            }
        }

        /// <summary>
        /// Save the picture
        /// </summary>
        private void savePic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save current canvas transform
                Transform transform = ImageCanvas.LayoutTransform;
                ImageCanvas.LayoutTransform = null;

                Size size = new Size(ImageCanvas.RenderSize.Width, ImageCanvas.RenderSize.Height);
                
                ImageCanvas.Measure(size);
                ImageCanvas.Arrange(new Rect(size));

                // Create a render bitmap and push the surface to it
                RenderTargetBitmap renderBitmap =
                  new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
                renderBitmap.Render(ImageCanvas);


                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg | All Files | *.*";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == true)
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                    encoder.Save(stream);
                }

                // Restore previously saved layout
                ImageCanvas.LayoutTransform = transform;
                MessageBox.Show("Successful!");
            }
            catch
            {
                MessageBox.Show("Failed!");
            }
        }

        /// <summary>
        /// Draw rectangles when user press left mouse button
        /// </summary>
        private void DrawRec(object sender, MouseButtonEventArgs e) 
        {
            isMouseDown = true;
            startPoint = e.GetPosition(ImageCanvas);

            rectSelectArea = new Rectangle()
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2,
                Fill = Brushes.LightBlue,
            };

            ImageCanvas.Children.Add(rectSelectArea);
            Canvas.SetLeft(rectSelectArea, startPoint.X);
            Canvas.SetTop(rectSelectArea, startPoint.Y);

            tbInfo.Text = "Click the right mouse to delete the rectangle; Click the up right red button to change color";
            rectSelectArea.MouseLeftButtonDown += Rec_MouseLeftButtonDown;
            rectSelectArea.MouseMove += Rec_MouseMove;
            rectSelectArea.MouseLeftButtonUp += Rec_MouseLeftButtonUp;
            rectSelectArea.MouseRightButtonDown += Rec_MouseRightButtonDown;
            // Add Adorner layer
            AdornerLayer.GetAdornerLayer(ImageCanvas).Add(new ResizeAdorner(rectSelectArea));
        }
      
        /// <summary>
        /// Draw the rectangle when user press left mouse button and drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                tbPosition.Text = "Start drawing";
            }
            else
            {
                tbPosition.Text = "Stop drawing";
            }
        }
        /// <summary>
        /// Finish the rectangle drawing when user release left mouse button
        /// </summary>
        private void MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
        }

        /// <summary>
        /// Delete the rectangle
        ///</summary>
        private void Rec_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            ImageCanvas.Children.Remove(rec);
        }

        /// <summary>
        /// Move the rectangle
        /// </summary>
        private void Rec_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Rectangle) sender).Cursor = Cursors.Hand;
            startToMove = true;
        }

        private void Rec_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && startToMove == true) { 

                Point point = e.GetPosition(ImageCanvas);
                Rectangle rec = (Rectangle)sender;
                double margineLeft = point.X - rec.Width / 2;
                double margineTop = point.Y - rec.Height / 2;

                double horizontalMarginSpace = (ImageCanvas.ActualWidth - Picture.ActualWidth) / 2;
                double verticalMarginSpace = (ImageCanvas.ActualHeight - Picture.ActualHeight) / 2;

                // deal the boundary problem
                if (margineLeft < horizontalMarginSpace) {
                    margineLeft = horizontalMarginSpace;
                }
                else if ((margineLeft + rec.Width) > ImageCanvas.ActualWidth - horizontalMarginSpace) {
                    margineLeft = (ImageCanvas.ActualWidth - horizontalMarginSpace) - rec.Width;
                }
                if (margineTop < verticalMarginSpace) { 
                    margineTop= verticalMarginSpace;
                }
                else if ((margineTop + rec.Height) > ImageCanvas.ActualHeight - verticalMarginSpace)
                {
                    margineTop = (ImageCanvas.ActualHeight - verticalMarginSpace) - rec.Height;
                }
                Canvas.SetLeft(rec, margineLeft);
                Canvas.SetTop(rec, margineTop);
            }
        }

        private void Rec_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startToMove = false;
            ((Rectangle)sender).ReleaseMouseCapture();
            ((Rectangle)sender).Cursor = Cursors.Arrow;
        }
    }
}
