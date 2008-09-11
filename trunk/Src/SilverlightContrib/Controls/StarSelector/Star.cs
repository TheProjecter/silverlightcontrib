using System;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightContrib.Controls
{

    [TemplateVisualState(Name = STAR_stateNormalName, GroupName = "RatingModeStates")]
    [TemplateVisualState(Name = STAR_stateHighlightedName, GroupName = "RatingModeStates")]
    [TemplateVisualState(Name = STAR_stateRatingName, GroupName = "RatingModeStates")]
    public class Star : Control
    {
        private const string STAR_stateNormalName = "Normal";
        private const string STAR_stateHighlightedName = "MouseOver";
        private const string STAR_stateRatingName = "Selected";

        private bool _ratingSelected;
        private bool _highlighted;
        private string _state;
        private bool _suspendVisualChanges;
        private bool _ratingMode;

        public Star()
        {
            DefaultStyleKey = typeof(Star);
            _suspendVisualChanges = false;
        }

        public bool SuspendVisualChanges
        {
            get { return _suspendVisualChanges; }
            set { _suspendVisualChanges = value; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState();
        }

        public void UpdateVisualState()
        {
            if (!_suspendVisualChanges)
            {
                if (_ratingMode && _ratingSelected)
                {
                    VisualStateManager.GoToState(this, STAR_stateRatingName, true);
                    _state = STAR_stateRatingName;
                }
                else if (_highlighted)
                {
                    VisualStateManager.GoToState(this, STAR_stateHighlightedName, true);
                    _state = STAR_stateHighlightedName;
                }
                else
                {

                    VisualStateManager.GoToState(this, STAR_stateNormalName, true);
                    _state = STAR_stateNormalName;
                }

                System.Diagnostics.Debug.WriteLine(this.StarIndex + ": " + _state);
            }
        }

        internal int StarIndex
        {
            get;
            set;
        }

        internal bool RatingMode
        {
            get
            {
                return _ratingMode;
            }
            set
            {
                _ratingMode = value;
                UpdateVisualState();
            }
        }

        internal bool RatingSelected
        {
            get
            {
                return _ratingSelected;
            }
            set
            {
                _ratingSelected = value;
                UpdateVisualState();
            }
        }

        internal bool Highlighted
        {
            get
            {
                return _highlighted;
            }
            set
            {
                _highlighted = value;
                UpdateVisualState();
            }
        }
    }
}
