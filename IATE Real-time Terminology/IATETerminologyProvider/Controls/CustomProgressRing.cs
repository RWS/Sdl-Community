using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sdl.Community.IATETerminologyProvider.Controls
{
    public class CustomProgressRing : Control
    {
        static CustomProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomProgressRing), new FrameworkPropertyMetadata(typeof(CustomProgressRing)));
        }

        public CustomProgressRing()
        {
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            IsVisibleChanged += IsVisibleChangedHandler;
        }

        #region SpinnerColor Property
        public Brush SpinnerColor
        {
            get { return (Brush)GetValue(SpinnerColorProperty); }
            set { SetValue(SpinnerColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpinnerColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpinnerColorProperty =
            DependencyProperty.Register("SpinnerColor", typeof(Brush), typeof(CustomProgressRing), new PropertyMetadata(Brushes.Teal));

        #endregion SpinnerColor Property

        #region SpinnerMargin Property

        public Thickness SpinnerMargin
        {
            get { return (Thickness)GetValue(SpinnerMarginProperty); }
            set { SetValue(SpinnerMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpinnerMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpinnerMarginProperty =
            DependencyProperty.Register("SpinnerMargin", typeof(Thickness), typeof(CustomProgressRing), new PropertyMetadata(new Thickness()));

        #endregion SpinnerMargin Property

        #region SpinnerSize Property

        public double SpinnerSize
        {
            get { return (double)GetValue(SpinnerSizeProperty); }
            set { SetValue(SpinnerSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpinnerSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpinnerSizeProperty =
            DependencyProperty.Register("SpinnerSize", typeof(double), typeof(CustomProgressRing), new PropertyMetadata(30d));

        #endregion SpinnerSize Property

        #region SpinnerThickness Property

        public double SpinnerThickness
        {
            get { return (double)GetValue(SpinnerThicknessProperty); }
            set { SetValue(SpinnerThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpinnerThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpinnerThicknessProperty =
            DependencyProperty.Register("SpinnerThickness", typeof(double), typeof(CustomProgressRing), new PropertyMetadata(1d));

        #endregion SpinnerThickness Property

        #region Text Property
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomProgressRing), new PropertyMetadata("Your text here..."));
        #endregion Text Property

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetActiveState();
        }

        private void SetActiveState()
        {
            _ = Visibility == Visibility.Visible
                            ? VisualStateManager.GoToState(this, "Active", true)
                            : VisualStateManager.GoToState(this, "Inactive", true);
        }

        private void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetActiveState();
        }
    }
}
