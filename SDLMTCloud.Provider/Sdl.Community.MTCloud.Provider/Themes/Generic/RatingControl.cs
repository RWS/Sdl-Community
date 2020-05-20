using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Themes.Generic
{
	[TemplatePart(Name = PART_RatingControl, Type = typeof(Grid))]
	[TemplatePart(Name = PART_ItemsControl, Type = typeof(ListBox))]
	public class RatingControl : Control, IDisposable
	{
		private const string PART_RatingControl = "PART_RatingControl";
		private const string PART_ItemsControl = "PART_ItemsControl";

		static RatingControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl),
				new FrameworkPropertyMetadata(typeof(RatingControl)));
		}
		private Grid _ratingControlGrid;
		private ListBox _ratingControlItemsControl;


		public static readonly DependencyProperty MaxRatingProperty = DependencyProperty.Register("MaxRating", typeof(int), typeof(RatingControl)
			, new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MaxRatingPropertyChangedCallback));

		public static readonly DependencyProperty RatingProperty = DependencyProperty.Register("Rating", typeof(int), typeof(RatingControl)
			, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RatingPropertyChangedCallback));

		public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(long), typeof(RatingControl)
			, new FrameworkPropertyMetadata((long)32, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ImageHeightPropertyChangedCallback));
		public static readonly DependencyProperty SelectedImageProperty =
			DependencyProperty.Register("SelectedImage", typeof(string), typeof(RatingControl),
				new FrameworkPropertyMetadata(@"C:\Repository\Sdl-Community\SDLMTCloud.Provider\Sdl.Community.MTCloud.Provider\Resources\StarYellow.png",
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EnabledImagePropertyChangedCallback));

		public static readonly DependencyProperty DisabledImageProperty =
			DependencyProperty.Register("DisabledImage", typeof(string), typeof(RatingControl),
				new FrameworkPropertyMetadata(@"C:\Repository\Sdl-Community\SDLMTCloud.Provider\Sdl.Community.MTCloud.Provider\Resources\StarGrey.png",
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DisabledImagePropertyChangedCallback));

		public long ImageHeight
		{
			get => (long)GetValue(ImageHeightProperty);
			set => SetValue(ImageHeightProperty, value);
		}

		public int MaxRating
		{
			get => (int)GetValue(MaxRatingProperty);
			set => SetValue(MaxRatingProperty, value);
		}
		public int Rating
		{
			get => (int)GetValue(RatingProperty);
			set => SetValue(RatingProperty, value);
		}
		public string DisabledImage
		{
			get => (string)GetValue(DisabledImageProperty);
			set => SetValue(DisabledImageProperty, value);
		}
		public string SelectedImage
		{
			get => (string)GetValue(SelectedImageProperty);
			set => SetValue(SelectedImageProperty, value);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			//Initialize controls
			RatingControlGrid = GetTemplateChild(PART_RatingControl) as Grid;
			if (RatingControlGrid != null)
			{
				RatingControlItemsControl = Template.FindName(PART_ItemsControl, this) as ListBox;
			}
		}

		private Grid RatingControlGrid
		{
			get => _ratingControlGrid;
			set
			{
				_ratingControlGrid?.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
				_ratingControlGrid = value;
				_ratingControlGrid?.AddHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
			}
		}
		private ListBox RatingControlItemsControl
		{
			get => _ratingControlItemsControl;
			set
			{
				if (_ratingControlItemsControl != null)
				{
					_ratingControlItemsControl.SelectionChanged -= RatingControlItemsControl_SelectionChanged;
				}
				_ratingControlItemsControl = value;
				if (_ratingControlItemsControl != null)
				{
					_ratingControlItemsControl.SelectionChanged += RatingControlItemsControl_SelectionChanged;
					UpdateRatingCollection(Rating);
				}
			}
		}

		private void UpdateRatingCollection(int rating)
		{
			if (RatingControlItemsControl.ItemsSource == null || RatingControlItemsControl.Items.Count != MaxRating)
			{
				RatingControlItemsControl.ItemsSource = GetRateItItems(rating);
			}
			else
			{
				UpdateRatingValues(rating);
			}
		}
		private IEnumerable<IRateItItem> GetRateItItems(int value)
		{
			var items = new List<IRateItItem>();
			for (var i = 0; i < MaxRating; i++)
			{
				items.Add(i < value
					? new RateItItem { Selected = true }
					: new RateItItem { Selected = false });
			}

			return items;
		}
		private void UpdateRatingValues(int rating)
		{
			var index = 0;
			foreach (var item in RatingControlItemsControl.ItemsSource.Cast<IRateItItem>())
			{
				item.Selected = index < rating;
				index++;
			}
		}

		private void RatingControlItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RatingControlItemsControl.SelectedItems.Count > 0)
			{
				Rating = RatingControlItemsControl.SelectedIndex + 1;
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var element = e.OriginalSource as FrameworkElement;
			if (element?.DataContext is IRateItItem rateItItem)
			{
				var index = 0;
				foreach (var item in RatingControlItemsControl.ItemsSource.Cast<IRateItItem>())
				{
					index++;
					if (rateItItem != item)
					{
						continue;
					}

					Rating = index;
					return;
				}
			}
		}

		private static void ImageHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is RatingControl control)) return;
			if (control.RatingControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		private static void RatingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is RatingControl control)) return;
			if (control.RatingControlGrid != null)
			{
				control.UpdateRatingCollection((int)e.NewValue);
			}
		}

		private static void MaxRatingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is RatingControl control)) return;
			if (control.RatingControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}
		private static void DisabledImagePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RatingControl control))
			{
				return;
			}

			if (control.RatingControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}
		private static void EnabledImagePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (!(dependencyObject is RatingControl control)) return;
			if (control.RatingControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		public void Dispose()
		{
			_ratingControlGrid?.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
			if (_ratingControlItemsControl != null)
			{
				_ratingControlItemsControl.SelectionChanged -= RatingControlItemsControl_SelectionChanged;
			}
		}
	}
}

