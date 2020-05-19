using System;
using System.Windows;
using System.Windows.Controls;

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


		public static  readonly DependencyProperty MaxRating = DependencyProperty.Register("MaxRating",typeof(int),typeof(RatingControl)
			,new FrameworkPropertyMetadata(5,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MaxRatingPropertyChangedCallback));

		public static readonly DependencyProperty Rating = DependencyProperty.Register("Rating", typeof(int), typeof(RatingControl)
			, new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RatingPropertyChangedCallback));

		public static readonly DependencyProperty ImageHeight = DependencyProperty.Register("ImageHeight", typeof(long), typeof(RatingControl)
			, new FrameworkPropertyMetadata((long)32, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ImageHeightPropertyChangedCallback));

		static RatingControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl),new PropertyMetadata(typeof(RatingControl)));
		}

		private Grid RatingControlGrid
		{
			get => _ratingControlGrid;
			set { _ratingControlGrid = value; }
		}

		private ListBox RatingControlItemsControl
		{
			get => _ratingControlItemsControl;
			set { _ratingControlItemsControl = value; } }

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			//Initialize controls
			RatingControlGrid = GetTemplateChild(PART_RatingControl)as Grid;
			if (RatingControlGrid != null)
			{
				RatingControlItemsControl = Template.FindName(PART_ItemsControl,this)as ListBox;
			}
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
		}
	}
}
