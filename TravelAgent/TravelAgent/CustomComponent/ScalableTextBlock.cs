using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TravelAgent.CustomComponent
{
    public class ScalableTextBlock : TextBlock
    {
        private double _originalFontSize;

        public ScalableTextBlock()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _originalFontSize = FontSize;
            Window window = Application.Current.MainWindow;
            if (window != null)
            {
                window.SizeChanged += OnWindowSizeChanged;
                UpdateFontSize(window.ActualWidth);
            }
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Window window)
            {
                UpdateFontSize(window.ActualWidth);
            }
        }

        private void UpdateFontSize(double windowWidth)
        {
            double maxWidth = SystemParameters.PrimaryScreenWidth;
            double scaleFactor = windowWidth / maxWidth;
            FontSize = _originalFontSize * scaleFactor;
        }
    }
}
