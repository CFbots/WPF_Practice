using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class ResizeAdorner : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb ThumbUpperLeft, ThumbBottomRight;
        Rectangle RectBorder;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisuals = new VisualCollection(this);
            ThumbUpperLeft = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            ThumbBottomRight = new Thumb() { Background = Brushes.Coral, Height = 10, Width = 10 };
            RectBorder = new Rectangle() { Stroke = Brushes.Coral, StrokeThickness = 2, StrokeDashArray = { 3, 2 } };

            ThumbUpperLeft.DragDelta += ThumbUpperLeft_DragDelta;
            ThumbBottomRight.DragDelta += ThumbBottomRight_DragDelta;

            AdornerVisuals.Add(RectBorder);
            AdornerVisuals.Add(ThumbUpperLeft);
            AdornerVisuals.Add(ThumbBottomRight);
        }
        private void ThumbUpperLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = (FrameworkElement)AdornedElement;
            elem.Height = Math.Max(0, elem.Height - e.VerticalChange);
            elem.Width = Math.Max(0, elem.Width - e.HorizontalChange);
        }

        private void ThumbBottomRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var elem = (FrameworkElement)AdornedElement;
            elem.Height = Math.Max(0, elem.Height + e.VerticalChange);
            elem.Width = Math.Max(0, elem.Width + e.HorizontalChange);
        }


        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            RectBorder.Arrange(new Rect(-2.5, -2.5, AdornedElement.DesiredSize.Width + 5, AdornedElement.DesiredSize.Height + 5));
            ThumbUpperLeft.Arrange(new Rect(-5, -5, 10, 10));
            ThumbBottomRight.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));
            return base.ArrangeOverride(finalSize);
        }
    }
}
