using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace SilverlightContrib.Controls
{
    public class StarSelector : Control
    {
        private const int STARSELECTOR_StarCount = 4;

        private readonly List<Star> _stars;
        private int _rating;

        public event RatingChangedEventHandler RatingChanged;

        public StarSelector()
        {
            DefaultStyleKey = typeof(StarSelector);
            _stars = new List<Star>();

            this.MouseEnter += new MouseEventHandler(StarContainer_MouseEnter);
            this.MouseLeave += new MouseEventHandler(StarContainer_MouseLeave);
        }

        protected virtual void OnRatingChanged(RatingChangedEventArgs e)
        {
            if (RatingChanged != null)
            {
                RatingChanged(this, e);
            }
        }

        void StarContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Container Mouse Leave");
            UpdateControlVisualState();
        }

        private void UpdateControlVisualState()
        {
            for (int i = 0; i < _stars.Count; ++i)
            {
                _stars[i].SuspendVisualChanges = true;
                try
                {
                    _stars[i].Highlighted = false;
                    _stars[i].RatingMode = true;
                    _stars[i].RatingSelected = i <= (_rating - 1);
                }
                finally
                {
                    _stars[i].SuspendVisualChanges = false;
                    _stars[i].UpdateVisualState();
                }
            }
        }

        void StarContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Container Mouse Enter");
            for (int i = 0; i < _stars.Count; ++i)
            {
                _stars[i].SuspendVisualChanges = true;
                try
                {
                    _stars[i].RatingMode = false;
                    _stars[i].RatingSelected = false;
                }
                finally
                {
                    _stars[i].SuspendVisualChanges = false;
                    _stars[i].UpdateVisualState();
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootElement = (StackPanel)GetTemplateChild("RootElement");
            for (int i = 0; i < STARSELECTOR_StarCount; ++i)
            {
                Star ratingStar = new Star();
                ratingStar.Template = RootElement.Resources["StarTemplate"] as ControlTemplate;
                ratingStar.RatingMode = true;
                ratingStar.StarIndex = i;
                ratingStar.RatingSelected = i <= (_rating - 1);

                ratingStar.MouseEnter += new MouseEventHandler(ratingStar_MouseEnter);
                ratingStar.MouseLeftButtonUp += new MouseButtonEventHandler(ratingStar_MouseLeftButtonUp);

                RootElement.Children.Add(ratingStar);

                _stars.Add(ratingStar);
            }
        }

        void ratingStar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Star currStar = sender as Star;
              _rating = currStar.StarIndex + 1;
            OnRatingChanged(new RatingChangedEventArgs(currStar.StarIndex + 1));
        }


        void ratingStar_MouseEnter(object sender, MouseEventArgs e)
        {
            Star currStar = sender as Star;
            for (int i = 0; i < _stars.Count; ++i)
            {
                _stars[i].Highlighted = i <= currStar.StarIndex;
                _stars[i].UpdateVisualState();
            }
        }

        private StackPanel RootElement
        {
            get;
            set;
        }

        #region Rating

        public int SelectedRating
        {
            get { return (int)GetValue(SelectedRatingProperty); }
            set { SetValue(SelectedRatingProperty, value); }
        }

        public static readonly DependencyProperty SelectedRatingProperty =
            DependencyProperty.Register(
            "SelectedRating",
            typeof(int),
            typeof(StarSelector),
            new PropertyMetadata(new PropertyChangedCallback(SelectedRatingChanged)));

        private static void SelectedRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StarSelector s = d as StarSelector;
            s._rating = (int)e.NewValue;
            s.UpdateControlVisualState();
        }

        #endregion 
    }
}
