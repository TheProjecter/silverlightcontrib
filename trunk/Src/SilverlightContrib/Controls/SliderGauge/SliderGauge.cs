using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SilverlightContrib.Controls
{
    public class SliderGauge : Control
    {
        private Path m_gaugePath;
        private bool m_guagePathMouseCaptured;
        private double m_percentage;

        public event GaugePercentageChangedEventHandler PercentChanged;

        public SliderGauge()
        {
            DefaultStyleKey = typeof(SliderGauge);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(SilverlightGauge_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(SilverlightGauge_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(SilverlightGauge_MouseLeftButtonUp);
            this.LayoutUpdated += new EventHandler(SilverlightGauge_LayoutUpdated);
        }

        private void SilverlightGauge_LayoutUpdated(object sender, EventArgs e)
        {
            UpdateVisuals();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_gaugePath = GetTemplateChild("GaugePath") as Path;
            UpdateVisuals();
        }

        private void SilverlightGauge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(this);
            m_percentage = CoercePercentage(location.X / m_gaugePath.ActualWidth);
            UpdateVisuals();  
            m_guagePathMouseCaptured = this.CaptureMouse();
        }

        private void SilverlightGauge_MouseMove(object sender, MouseEventArgs e)
        {
            if(m_guagePathMouseCaptured)
            {
                Point location = e.GetPosition(this);
                m_percentage = CoercePercentage(location.X/m_gaugePath.ActualWidth);
                UpdateVisuals();         
            }
        }

        private void UpdateVisuals()
        {
            if (m_gaugePath == null)
                return;

            if (m_percentage < 0 || m_percentage > 1)
                return;

            double location = m_percentage * m_gaugePath.ActualWidth;
            RectangleGeometry clippingRect = new RectangleGeometry();
            clippingRect.Rect = new Rect(0, 0, location, m_gaugePath.ActualHeight);
            m_gaugePath.Clip = clippingRect;            
        }

        private void SilverlightGauge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_gaugePath.ReleaseMouseCapture();
            m_guagePathMouseCaptured = false;
            if (PercentChanged != null)
            {
                PercentChanged(this, new GaugePercentageChangedEventArgs(m_percentage));
            }
        }

        private double CoercePercentage(double percentage)
        {
            if (percentage < 0)
                percentage = 0;
            if (percentage > 1)
                percentage = 1;
            return percentage;
        }

        public double Percentage
        {
            get { return (int)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(
            "Percentage",
            typeof(double),
            typeof(SliderGauge),
            new PropertyMetadata(new PropertyChangedCallback(PercentageChanged)));

        private static void PercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SliderGauge g = d as SliderGauge;
            g.m_percentage = g.CoercePercentage((double)e.NewValue);
            g.UpdateVisuals();
        }
        
    }
}
