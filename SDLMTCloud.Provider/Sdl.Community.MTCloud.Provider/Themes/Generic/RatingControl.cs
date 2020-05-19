using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.MTCloud.Provider.Themes.Generic
{
	[TemplatePart(Name = PART_RatingControl,Type = typeof(Grid))]
	[TemplatePart(Name = PART_ItemsControl,Type = typeof(ListBox))]
	public class RatingControl:Control,IDisposable
	{
		private const string PART_RatingControl = "PART_RatingControl";
		private const string PART_ItemsControl = "PART_ItemsControl";
		private Grid _ratingControlGrid;
		private ListBox _ratingControlItemsControl;


		public static  readonly DependencyProperty MaxRatingProperty = DependencyProperty.Register("MaxRating",typeof(int),typeof(RatingControl)
			,new FrameworkPropertyMetadata(5,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MaxRatingPropertyChangedCallback));

		public static readonly DependencyProperty RatingProperty = DependencyProperty.Register("Rating", typeof(int), typeof(RatingControl)
			, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RatingPropertyChangedCallback));

		public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(long), typeof(RatingControl)
			, new FrameworkPropertyMetadata((long)32, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ImageHeightPropertyChangedCallback));
		static RatingControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), new PropertyMetadata(typeof(RatingControl)));
		}

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
				_ratingControlGrid?.AddHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove),true);
			}
		}
		private ListBox RatingControlItemsControl
		{
			get => _ratingControlItemsControl; 
			set { _ratingControlItemsControl = value; }//TODO: Add Selection changed event
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
		}

		private static void ImageHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		private static void RatingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		private static void MaxRatingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		public void Dispose()
		{
			_ratingControlGrid?.RemoveHandler(MouseMoveEvent, new MouseEventHandler(OnMouseMove));
			//TODO: Add  for ItemsControls
		}
	}
}
