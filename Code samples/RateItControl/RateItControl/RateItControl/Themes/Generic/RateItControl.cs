using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Sdl.Community.RateItControl.API;
using Sdl.Community.RateItControl.Implementation;

namespace Sdl.Community.RateItControl.Themes.Generic
{
	[TemplatePart(Name = PART_RateItControl, Type = typeof(Grid))]
	[TemplatePart(Name = PART_ItemsControl, Type = typeof(ListBox))]
	public class RateItControl : Control
	{
		private const string PART_RateItControl = "PART_RateItControl";
		private const string PART_ItemsControl = "PART_ItemsControl";

		static RateItControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RateItControl), new FrameworkPropertyMetadata(typeof(RateItControl)));
		}

		private Grid _rateItControlGrid;

		private Grid RateItControlGrid
		{
			get => _rateItControlGrid;
			set
			{
				if (_rateItControlGrid != null)
				{

				}

				_rateItControlGrid = value;

				if (_rateItControlGrid != null)
				{

				}
			}
		}

		private ListBox _rateItControlItemsControl;

		private ListBox RateItControlItemsControl
		{
			get => _rateItControlItemsControl;
			set
			{
				if (_rateItControlItemsControl != null)
				{
					RateItControlItemsControl.SelectionChanged -= RateItControlItemsControl_SelectionChanged;
				}

				_rateItControlItemsControl = value;

				if (_rateItControlItemsControl != null)
				{
					RateItControlItemsControl.SelectionChanged += RateItControlItemsControl_SelectionChanged;
					
					UpdateRating(Rating);
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RateItControlGrid = GetTemplateChild(PART_RateItControl) as Grid;

			if (RateItControlGrid != null)
			{
				RateItControlItemsControl = Template.FindName(PART_ItemsControl, this) as ListBox;
			}
		}

		public static readonly DependencyProperty MaxRatingProperty =
			DependencyProperty.Register("MaxRating", typeof(int), typeof(RateItControl),
				new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRating(control.Rating);
			}
		}

		public int MaxRating
		{
			get => (int)GetValue(MaxRatingProperty);
			set => SetValue(MaxRatingProperty, value);
		}

		public static readonly DependencyProperty RatingProperty =
			DependencyProperty.Register("Rating", typeof(int), typeof(RateItControl),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					ItemsPropertyChangedCallback));

		public int Rating
		{
			get => (int)GetValue(RatingProperty);
			set => SetValue(RatingProperty, value);
		}

		private static void ItemsPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRating((int)d.NewValue);
			}
		}

		private void UpdateRating(int value)
		{
			var items = new List<IRateItItem>();
			for (var i = 0; i < MaxRating; i++)
			{
				items.Add(i < value
					? new RageItItem { Selected = true }
					: new RageItItem { Selected = false });
			}

			RateItControlItemsControl.ItemsSource = items;
		}

		private void RateItControlItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RateItControlItemsControl.SelectedItems.Count > 0)
			{				
				Rating = RateItControlItemsControl.SelectedIndex + 1;			
			}
		}
	}
}
