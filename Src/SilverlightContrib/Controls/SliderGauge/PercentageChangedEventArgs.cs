using System;

namespace SilverlightContrib.Controls
{
    public delegate void GaugePercentageChangedEventHandler(object sender, GaugePercentageChangedEventArgs e);

    public class GaugePercentageChangedEventArgs : EventArgs
    {
        private readonly double _percentage;

        public GaugePercentageChangedEventArgs(double percentage)
        {
            _percentage = percentage;
        }

        public double Percentage
        {
            get { return _percentage; }
        }

    }
}
