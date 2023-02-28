using cf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cf
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb ThumbUpperLeft, ThumbBottomRight, ThumbRightSide, ThumbBottomSide;
        Rectangle RectBorder;
        Button BtnChangeColor;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            ThumbUpperLeft = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            ThumbBottomRight = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            ThumbRightSide = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            ThumbBottomSide = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            RectBorder = new Rectangle() { Stroke = Brushes.Coral, StrokeThickness = 2, StrokeDashArray = { 3, 2 } }; //dotted line border

            //Button for color changing
            StackPanel pnl = new StackPanel();
            pnl.Orientation = Orientation.Horizontal;

            Image colorIcon = new Image();
            colorIcon.Source = new BitmapImage(new Uri(@"Resource\color_palette.png", UriKind.Relative));
            colorIcon.Stretch = Stretch.Uniform;
            colorIcon.Width = 15;
            colorIcon.Height = 15;

            pnl.Children.Add(colorIcon);

            BtnChangeColor = new Button { 
                Height = 20, Width = 20, Background = Brushes.White
            };
            BtnChangeColor.Content = pnl;

            ThumbUpperLeft.DragDelta += ThumbUpperLeft_DragDelta;
            ThumbBottomRight.DragDelta += ThumbBottomRight_DragDelta;
            ThumbRightSide.DragDelta += ThumbRightSide_DragDelta;
            ThumbBottomSide.DragDelta += ThumbBottomSide_DragDelta;
            BtnChangeColor.Click += BtnChangeColor_Click;

            AdornerVisuals.Add(RectBorder);
            AdornerVisuals.Add(ThumbUpperLeft);
            AdornerVisuals.Add(ThumbBottomRight);
            AdornerVisuals.Add(ThumbRightSide);
            AdornerVisuals.Add(ThumbBottomSide);
            AdornerVisuals.Add(BtnChangeColor);
        }

        private void BtnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            var rect = (Rectangle)AdornedElement;
            ColorPicker colorPicker = new ColorPicker();
            if (colorPicker.ShowDialog() == true)
            {
                rect.Fill = colorPicker.SelectColor.Background;
                rect.Stroke = colorPicker.SelectColor.Background;
            }
        }

        private void ThumbUpperLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Control item = (Control)this.DataContext;

            //var elem = (FrameworkElement)AdornedElement;
            //Canvas imageCanvas = (Canvas)Application.Current.MainWindow.FindName("ImageCanvas");
            //var position = Mouse.GetPosition(imageCanvas);

            //double translatedX = position.X + e.HorizontalChange;
            //double translatedY = position.Y + e.VerticalChange;
            //double translatedY = Canvas.GetTop(item) + e.VerticalChange;

            //Canvas.SetLeft(elem, translatedX);
            //Canvas.SetTop(item, translatedY);
            //item.Height = Math.Max(0, item.Height - e.VerticalChange);
            //elem.Width = Math.Max(0, elem.Width - e.HorizontalChange);
        }

        private void ThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = (FrameworkElement)AdornedElement;
            elem.Height = elem.Height + e.VerticalChange < 0 ? 0 : elem.Height + e.VerticalChange;
            elem.Width = elem.Width + e.HorizontalChange < 0 ? 0: elem.Width + e.HorizontalChange;
        }

        private void ThumbRightSide_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = (FrameworkElement)AdornedElement;
            elem.Width = elem.Width + e.HorizontalChange < 0 ? 0 : elem.Width + e.HorizontalChange;
        }

        private void ThumbBottomSide_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = (FrameworkElement)AdornedElement;
            elem.Height = elem.Height + e.VerticalChange < 0 ? 0 : elem.Height + e.VerticalChange;
        }


        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            RectBorder.Arrange(new Rect(-2.5, -2.5, AdornedElement.DesiredSize.Width + 2.5, AdornedElement.DesiredSize.Height + 2.5));
            BtnChangeColor.Arrange(new Rect(AdornedElement.DesiredSize.Width - 20, -20, 20, 20));

            ThumbUpperLeft.Arrange(new Rect(-5, -5, 10, 10));
            ThumbBottomRight.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));
            ThumbRightSide.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, (AdornedElement.DesiredSize.Height - 5) /2, 10, 10));
            ThumbBottomSide.Arrange(new Rect((AdornedElement.DesiredSize.Width - 5)/2, AdornedElement.DesiredSize.Height - 5, 10, 10));
            return base.ArrangeOverride(finalSize);
        }
    }
}
