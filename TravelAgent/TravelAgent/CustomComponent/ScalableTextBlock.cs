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

        public double ScaleRate
        {
            get { return (double)GetValue(ScaleRateProperty); }
            set { SetValue(ScaleRateProperty, value); }
        }

        public static readonly DependencyProperty ScaleRateProperty =
            DependencyProperty.Register("ScaleRate", typeof(double), typeof(ScalableTextBlock), 
                new PropertyMetadata(1.0));

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

            scaleFactor = CalculateScaleFactorWithHardnessFactor(scaleFactor);

            FontSize = _originalFontSize * scaleFactor;
        }

        private double CalculateScaleFactorWithHardnessFactor(double scaleFactor)
        {
            if (ScaleRate > 1.0)
            {
                // first we calculate the difference between 1 and the scale factor
                double d = 1 - scaleFactor;

                // then we divide it with the hardness scale factor
                double d2 = d / ScaleRate;

                /* then we add the difference of these two number to the scale factor,
                   for example, if the hardness factor is 3 then, 2/3 of the differenece between 
                   1 and the scale factor will be added to the scale factor which will decrease the 
                   scaling rate */
                return scaleFactor + (d - d2);
            }
            else
            {
                return scaleFactor;
            }
        }
    }
}
