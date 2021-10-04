using System.Windows;

namespace PingGUI.ChildElement
{
    class LimitChild : System.Windows.Controls.Panel
    {
        public LimitChild()
        {
        }

        protected override Size MeasureOverride(System.Windows.Size availableSize)
        {
            System.Diagnostics.Debug.Assert(InternalChildren.Count == 1);
            System.Windows.UIElement child = InternalChildren[0];

            Size panelDesiredSize = new Size();
            // panelDesiredSize.Width = availableSize.Width;
            panelDesiredSize.Width = (double)child.GetValue(FrameworkElement.MinWidthProperty);
            panelDesiredSize.Height = (double)child.GetValue(FrameworkElement.MinHeightProperty);

            child.Measure(panelDesiredSize);

            // IMPORTANT: do not allow PositiveInfinity to be returned, that will raise an exception in the caller! 
            // PositiveInfinity might be an availableSize input; this means that the parent does not care about sizing 
            return panelDesiredSize;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            System.Windows.UIElement child = InternalChildren[0];

            child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            if (finalSize.Width > child.RenderSize.Width)
                finalSize.Width = child.RenderSize.Width;
            if (finalSize.Height > child.RenderSize.Height)
                finalSize.Height = child.RenderSize.Height;

            return finalSize; // Returns the final Arranged size
        }
    }
}