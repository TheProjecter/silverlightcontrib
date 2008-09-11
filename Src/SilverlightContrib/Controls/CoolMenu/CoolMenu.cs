using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SilverlightContrib.Controls
{
    public class CoolMenu : Control
    {
        public event MenuIndexChangedHandler MenuIndexChanged;
        public event MenuIndexChangedHandler MenuItemClicked;

        public enum ClickEffect { None, Bounce };

        private const string ROOT_ELEMENT = "RootElement";
        private const double LARGE_ICON_SIZE = 1;
        private const double MEDIUM_ICON_SIZE = 0.75;
        private const double SMALL_ICON_SIZE = 0.65;
        private const double NORMAL_ICON_SIZE = 0.58;

        private Panel m_rootElement;

        public CoolMenu()
        {
            this.DefaultStyleKey = typeof(CoolMenu);
            this.Loaded += new RoutedEventHandler(CoolMenu_Loaded);
        }

        private void CoolMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Items == null)
                this.Items = new CoolMenuItemCollection();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_rootElement = this.GetTemplateChild(ROOT_ELEMENT) as Panel;

            if(m_rootElement == null) 
                return;

            m_rootElement.MouseLeave += new MouseEventHandler(m_rootElement_MouseLeave);
            
            for(int i = 0; i < this.Items.Count; ++i)
            {
                CoolMenuItem cmi = this.Items[i];
                FrameworkElement contentElement = cmi.Content;
                contentElement.Height = this.MaxItemHeight*NORMAL_ICON_SIZE;
                contentElement.Width = this.MaxItemWidth*NORMAL_ICON_SIZE;
                contentElement.Tag = i;
                contentElement.RenderTransform = BuildTransformGroup();
                contentElement.VerticalAlignment = VerticalAlignment.Bottom; 
                contentElement.MouseEnter += new MouseEventHandler(contentElement_MouseEnter);
                contentElement.MouseLeave += new MouseEventHandler(contentElement_MouseLeave);
                contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(contentElement_MouseLeftButtonDown);

                m_rootElement.Children.Add(contentElement);
                
            }
        }

        private TransformGroup BuildTransformGroup()
        {
            TransformGroup tg = new TransformGroup();
            TranslateTransform tt = new TranslateTransform();
            tt.Y = 0;
            tg.Children.Add(tt);
            return tg;
        }

        private void contentElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement selectedItem = (FrameworkElement)sender;
            
            if(this.MenuItemClickEffect == ClickEffect.Bounce)
                ApplyBounceEffect(selectedItem);

            if(MenuItemClicked != null)
            {
                int index = (int)selectedItem.Tag;
                SelectedMenuItemArgs menuArgs = new SelectedMenuItemArgs(this.Items[index], index);
                MenuItemClicked(this, menuArgs);
            }
        }

        private void m_rootElement_MouseLeave(object sender, MouseEventArgs e)
        {
            AdjustSizes(-1);
        }

        private void contentElement_MouseLeave(object sender, MouseEventArgs e)
        {
            // Don't bubble mouseLeave to parent.
            e.Handled = true;
        }

        private void contentElement_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement r = sender as FrameworkElement;
            int index = (int) r.Tag;
            AdjustSizes(index);

            if(MenuIndexChanged != null)
            {
                SelectedMenuItemArgs args = new SelectedMenuItemArgs(this.Items[index], index);
                MenuIndexChanged(this, args);
            }
        }

        private void AdjustSizes(int index)
        {
            for (int i = 0; i < this.Items.Count; ++i )
            {
                if(index == -1)
                {
                    ApplyResizeEffect(this.Items[i].Content, NORMAL_ICON_SIZE, this.MaxItemWidth, this.MaxItemHeight);
                    continue;
                }

                if (i == index)
                    ApplyResizeEffect(this.Items[i].Content, LARGE_ICON_SIZE, this.MaxItemWidth, this.MaxItemHeight);
                else if (i == index - 1 || i == index + 1)
                    ApplyResizeEffect(this.Items[i].Content, MEDIUM_ICON_SIZE, this.MaxItemWidth, this.MaxItemHeight);
                else if (i == index - 2 || i == index + 2)
                    ApplyResizeEffect(this.Items[i].Content, SMALL_ICON_SIZE, this.MaxItemWidth, this.MaxItemHeight);
                else
                    ApplyResizeEffect(this.Items[i].Content, NORMAL_ICON_SIZE, this.MaxItemWidth, this.MaxItemHeight);
            }
        }

        private void ApplyResizeEffect(FrameworkElement element, double factor, double width, double height)
        {
            TimeSpan speed = TimeSpan.FromMilliseconds(100);
            DoubleAnimation daWidth = new DoubleAnimation { To = factor*width, Duration = new Duration(speed) };
            DoubleAnimation daHeight = new DoubleAnimation { To = factor*height, Duration = new Duration(speed) };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(daWidth, element);
            Storyboard.SetTarget(daHeight, element);
            Storyboard.SetTargetProperty(daHeight, new PropertyPath("(UIElement.Height)"));
            Storyboard.SetTargetProperty(daWidth, new PropertyPath("(UIElement.Width)"));
            sb.Children.Add(daWidth);
            sb.Children.Add(daHeight);
            sb.Begin();
        }

        private void ApplyBounceEffect(FrameworkElement e)
        {
            var da = new DoubleAnimationUsingKeyFrames();
            var k1 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)),
                Value = this.MaxItemHeight * 0.30
            };
            var k2 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)),
                Value = 0
            };
            var k3 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)),
                Value = this.MaxItemHeight * 0.10
            };
            var k4 = new SplineDoubleKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(350)),
                Value = 0
            };
            da.KeyFrames.Add(k1);
            da.KeyFrames.Add(k2);
            da.KeyFrames.Add(k3);
            da.KeyFrames.Add(k4);

            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(da, e);
            Storyboard.SetTargetProperty(da,
                new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
            sb.Children.Add(da);
            sb.Begin();
        }

        #region MaxItemHeight Property
        public static readonly DependencyProperty MaxItemHeightProperty = DependencyProperty.Register(
            "MaxItemHeight",
            typeof(double),
            typeof(CoolMenu),
            null);

        public double MaxItemHeight
        {
            get
            {
                return (double)GetValue(MaxItemHeightProperty);
            }
            set { SetValue(MaxItemHeightProperty, value); }
        }
        #endregion

        #region MaxItemWidth Property
        public static readonly DependencyProperty MaxItemWidthProperty = DependencyProperty.Register(
            "MaxItemWidth",
            typeof(double),
            typeof(CoolMenu),
            null);

        public double MaxItemWidth
        {
            get
            {
                return (double)GetValue(MaxItemWidthProperty);
            }
            set { SetValue(MaxItemWidthProperty, value); }
        }
        #endregion

        #region Items Property
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(CoolMenuItemCollection),
            typeof(CoolMenu),
            null);

        public CoolMenuItemCollection Items
        {
            get 
            { 
                return GetValue(ItemsProperty) as CoolMenuItemCollection;  
            }
            set { SetValue(ItemsProperty, value); }
        }
        #endregion

        #region MenuItemClickEffect Property
        public static readonly DependencyProperty MenuItemClickEffectProperty = DependencyProperty.Register(
            "MenuItemClickEffect",
            typeof(CoolMenu.ClickEffect),
            typeof(CoolMenu),
            null);

        public ClickEffect MenuItemClickEffect
        {
            get { return (ClickEffect)GetValue(MenuItemClickEffectProperty); }
            set { SetValue(MenuItemClickEffectProperty, value); }
        }
        #endregion

    }
}
