﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.RateItControl.API;
using Sdl.Community.RateItControl.Implementation;

namespace Sdl.Community.RateItControl.Themes.Generic
{
	[TemplatePart(Name = PART_RateItControl, Type = typeof(Grid))]
	[TemplatePart(Name = PART_ItemsControl, Type = typeof(ListBox))]
	public class RateItControl : Control, IDisposable
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
					_rateItControlGrid.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
				}

				_rateItControlGrid = value;

				if (_rateItControlGrid != null)
				{
					if (EnableMouseHoverSelection)
					{
						_rateItControlGrid.AddHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove), true);
					}
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
					_rateItControlItemsControl.SelectionChanged -= RateItControlItemsControl_SelectionChanged;
				}

				_rateItControlItemsControl = value;

				if (_rateItControlItemsControl != null)
				{
					_rateItControlItemsControl.SelectionChanged += RateItControlItemsControl_SelectionChanged;

					UpdateRatingCollection(Rating);
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

		public static readonly DependencyProperty ImageHeightProperty =
			DependencyProperty.Register("ImageHeight", typeof(long), typeof(RateItControl),
				new FrameworkPropertyMetadata((long)32, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ImageHeightPropertyChangedCallback));

		private static void ImageHeightPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		public long ImageHeight
		{
			get => (long)GetValue(ImageHeightProperty);
			set => SetValue(ImageHeightProperty, value);
		}

		public static readonly DependencyProperty SelectedImageProperty =
			DependencyProperty.Register("SelectedImage", typeof(Uri), typeof(RateItControl),
				new FrameworkPropertyMetadata(new Uri("../../Resources/StarYellow.png", UriKind.RelativeOrAbsolute),
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EnabledImagePropertyChangedCallback));

		private static void EnabledImagePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		public Uri SelectedImage
		{
			get => (Uri)GetValue(SelectedImageProperty);
			set => SetValue(SelectedImageProperty, value);
		}

		public static readonly DependencyProperty DisabledImageProperty =
			DependencyProperty.Register("DisabledImage", typeof(Uri), typeof(RateItControl),
				new FrameworkPropertyMetadata(new Uri( "../../Resources/StarGrey.png", UriKind.RelativeOrAbsolute),
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DisabledImagePropertyChangedCallback));

		private static void DisabledImagePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		public Uri DisabledImage
		{
			get => (Uri)GetValue(DisabledImageProperty);
			set => SetValue(DisabledImageProperty, value);
		}


		public static readonly DependencyProperty EnableMouseHoverSelectionProperty =
			DependencyProperty.Register("EnableMouseHoverSelection", typeof(bool), typeof(RateItControl),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EnableMouseHoverSelectionPropertyChangedCallback));

		private static void EnableMouseHoverSelectionPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				var oldValue = (bool)d.OldValue;
				var newValue = (bool)d.NewValue;

				if (oldValue || !newValue)
				{
					control.RateItControlGrid.RemoveHandler(MouseMoveEvent, new MouseEventHandler(control.OnMouseMove));
				}

				if (newValue)
				{
					control.RateItControlGrid.AddHandler(MouseMoveEvent, new MouseEventHandler(control.OnMouseMove), true);
				}
			}
		}

		public bool EnableMouseHoverSelection
		{
			get => (bool)GetValue(EnableMouseHoverSelectionProperty);
			set => SetValue(EnableMouseHoverSelectionProperty, value);
		}

		public static readonly DependencyProperty MaxRatingProperty =
			DependencyProperty.Register("MaxRating", typeof(int), typeof(RateItControl),
				new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MaxRatingPropertyChangedCallback));

		private static void MaxRatingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRatingCollection(control.Rating);
			}
		}

		public int MaxRating
		{
			get => (int)GetValue(MaxRatingProperty);
			set => SetValue(MaxRatingProperty, value);
		}

		public static readonly DependencyProperty RatingProperty =
			DependencyProperty.Register("Rating", typeof(int), typeof(RateItControl),
				new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					RatingPropertyChangedCallback));

		public int Rating
		{
			get => (int)GetValue(RatingProperty);
			set => SetValue(RatingProperty, value);
		}

		private static void RatingPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs d)
		{
			if (!(dependencyObject is RateItControl control))
			{
				return;
			}

			if (control.RateItControlGrid != null)
			{
				control.UpdateRatingCollection((int)d.NewValue);
			}
		}

		private void UpdateRatingCollection(int value)
		{
			if (RateItControlItemsControl.ItemsSource == null || RateItControlItemsControl.Items.Count != MaxRating)
			{
				RateItControlItemsControl.ItemsSource = GetRateItItems(value);
			}
			else
			{
				UpdateRatingValues(value);
			}
		}

		private void UpdateRatingValues(int value)
		{
			var index = 0;
			foreach (var item in RateItControlItemsControl.ItemsSource.Cast<IRateItItem>())
			{
				item.Selected = index < value;
				index++;
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

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var element = e.OriginalSource as FrameworkElement;
			if (element?.DataContext is IRateItItem rateItItem)
			{
				var index = 0;
				foreach (var item in RateItControlItemsControl.ItemsSource.Cast<IRateItItem>())
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

		private void RateItControlItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (RateItControlItemsControl.SelectedItems.Count > 0)
			{
				Rating = RateItControlItemsControl.SelectedIndex + 1;
				RateItControlItemsControl.SelectedIndex = -1;
			}
		}

		public void Dispose()
		{
			if (_rateItControlGrid != null)
			{
				_rateItControlGrid.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
			}

			if (_rateItControlItemsControl != null)
			{
				_rateItControlItemsControl.SelectionChanged -= RateItControlItemsControl_SelectionChanged;
			}
		}
	}
}
