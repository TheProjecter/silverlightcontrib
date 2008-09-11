using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SilverlightContrib.Controls
{
    public class ColorPicker : Control
    {
        public delegate void ColorSelectedHandler(Color c);
        public event ColorSelectedHandler ColorSelected;

        private readonly ColorSpace m_colorSpace;
        private bool m_hueMonitorMouseCaptured;
        private bool m_sampleMouseCaptured;
        private float m_selectedHue;
        //private Color m_selectedColor;
        private int m_sampleX;
        private int m_sampleY;
        
        private Rectangle m_hueMonitor;
        private Canvas m_sampleSelector;
        private Canvas m_hueSelector;

        private Rectangle m_selectedColorView;
        private Rectangle m_colorSample;
        private TextBlock m_hexValue;

        public ColorPicker()
        {
            DefaultStyleKey = typeof(ColorPicker);
            m_selectedHue = 0;
            m_colorSpace = new ColorSpace();
            this.Width = this.Height = 200;
            this.LayoutUpdated += new EventHandler(ColorPicker_LayoutUpdated);
        }

        void ColorPicker_LayoutUpdated(object sender, EventArgs e)
        {
          //  UpdateSliders(this.SelectedColor);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_hueMonitor = GetTemplateChild("HueMonitor") as Rectangle;
            m_sampleSelector = GetTemplateChild("SampleSelector") as Canvas;
            m_hueSelector = GetTemplateChild("HueSelector") as Canvas;
            m_selectedColorView = GetTemplateChild("SelectedColorView") as Rectangle;
            m_colorSample = GetTemplateChild("ColorSample") as Rectangle;
            m_hexValue = GetTemplateChild("HexValue") as TextBlock;


            m_hueMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonDown);
            m_hueMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonUp);
            m_hueMonitor.MouseMove += new MouseEventHandler(rectHueMonitor_MouseMove);

            m_colorSample.MouseLeftButtonDown += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonDown);
            m_colorSample.MouseLeftButtonUp += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonUp);
            m_colorSample.MouseMove += new MouseEventHandler(rectSampleMonitor_MouseMove);

            m_sampleX = (int)m_colorSample.Width;
            m_sampleY = 0;
        
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void rectHueMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            UpdateSelection(yPos);
            m_hueMonitorMouseCaptured = m_hueMonitor.CaptureMouse();
        }

        private void rectHueMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            m_hueMonitor.ReleaseMouseCapture();
            m_hueMonitorMouseCaptured = false;
        }

        private void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if(!m_hueMonitorMouseCaptured) 
                return;

            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            UpdateSelection(yPos);
        }

        private void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            UpdateSample(m_sampleX, m_sampleY);

            m_sampleMouseCaptured = m_colorSample.CaptureMouse();
        }

        private void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_colorSample.ReleaseMouseCapture();
            m_sampleMouseCaptured = false;
        }

        private void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if(!m_sampleMouseCaptured)
                return;

            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void UpdateSample(int xPos, int yPos)
        {
            if (m_colorSample == null)
                return;

            if (xPos < 0 || xPos > m_colorSample.Width)
                return;

            if (yPos < 0 || yPos > m_colorSample.Height)
                return;

            m_sampleSelector.SetValue(Canvas.LeftProperty, xPos - (m_sampleSelector.Height / 2));
            m_sampleSelector.SetValue(Canvas.TopProperty, yPos - (m_sampleSelector.Height / 2));

            float yComponent = 1 - (float)(yPos / m_colorSample.Height);
            float xComponent = (float)(xPos / m_colorSample.Width);

            this.SelectedColor = m_colorSpace.ConvertHsvToRgb(m_selectedHue, xComponent, yComponent);
            m_selectedColorView.Fill = new SolidColorBrush(this.SelectedColor);
            m_hexValue.Text = m_colorSpace.GetHexCode(this.SelectedColor);

            if (ColorSelected != null)
                ColorSelected(this.SelectedColor);
        }

        private void UpdateSelection(int yPos)
        {
            if (m_hueMonitor == null)
                return;

            if (yPos < 0 || yPos > m_hueMonitor.Height)
                return;

            int huePos = (int)(yPos / m_hueMonitor.Height * 255);
            int gradientStops = 6;
            Color c = m_colorSpace.GetColorFromPosition(huePos * gradientStops);
            m_colorSample.Fill = new SolidColorBrush(c);
            m_hueSelector.SetValue(Canvas.TopProperty, yPos - (m_hueSelector.Height / 2));
            m_selectedHue = (float)(yPos / m_hueMonitor.Height) * 360;
            UpdateSample(m_sampleX, m_sampleY);
        }

        #region SelectedColor Dependency Property
        public Color SelectedColor
        {
            get { return (Color) GetValue(SelectedColorProperty);}
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(ColorPicker),
                new PropertyMetadata(new PropertyChangedCallback(SelectedColorChanged)));

        private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker p = d as ColorPicker;
            if (p != null)
            {
               // p.UpdateSliders((Color) e.NewValue);
            }
        }


        #endregion

    }
}
