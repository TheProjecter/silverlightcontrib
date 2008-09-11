using System;

namespace SilverlightContrib.Controls
{
    public delegate void RatingChangedEventHandler(object sender, RatingChangedEventArgs e);

    public class RatingChangedEventArgs : EventArgs
    {
        private readonly int _rating;

        public RatingChangedEventArgs(int rating)
        {
            _rating = rating;
        }

        public int Rating
        {
            get { return _rating; }
        }

    }
}
