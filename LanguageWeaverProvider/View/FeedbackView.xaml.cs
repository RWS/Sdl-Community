using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LanguageWeaverProvider.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace LanguageWeaverProvider.View
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class FeedbackView : UserControl, IUIControl
	{
		private readonly List<string> HexCodes = new() { "#ff4545", "#ffa534", "#ffe234", "#b7dd29", "#57e32c" };
		private int SelectedStar;

        public FeedbackView()
        {
            InitializeComponent();
			SelectedStar = (DataContext as FeedbackViewModel)?.Rating ?? 0;
		}

		private void Star_MouseEnter(object sender, MouseEventArgs e)
		{
			try
			{
				var starNumber = GetStarNumber(sender);
				var starColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HexCodes[starNumber - 1]));
				for (var i = 1; i <= starNumber; i++)
				{
					var button = (Button)FindName($"star_{i}");
					(button.Content as TextBlock).Foreground = starColor;
				}

				for (var i = starNumber + 1; i <= 5; i++)
				{
					var button = (Button)FindName($"star_{i}");
					(button.Content as TextBlock).Foreground = Brushes.LightGray;
				}
			}
			catch { }
		}

		private void Star_MouseLeave(object sender, MouseEventArgs e)
		{
			try
			{
				var colorsIndex = SelectedStar - 1 >= 0 ? SelectedStar - 1 : 0;
				var starColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HexCodes[colorsIndex]));
				for (var i = 1; i <= SelectedStar; i++)
				{
					var button = (Button)FindName($"star_{i}");
					(button.Content as TextBlock).Foreground = starColor;
				}

				for (var i = 5; i > SelectedStar; i--)
				{
					var button = (Button)FindName($"star_{i}");
					(button.Content as TextBlock).Foreground = Brushes.LightGray;
				}
			}
			catch { }
		}

		private void Star_Clicked(object sender, RoutedEventArgs e)
		{
			try
			{
				SelectedStar = GetStarNumber(sender);
				(DataContext as FeedbackViewModel).Rating = SelectedStar;
			}
			catch { }
		}

		private int GetStarNumber(object sender)
		{
			var buttonName = (sender as Button).Name;
			var starNumber = Convert.ToInt32(buttonName.Last().ToString());
			return starNumber;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Window.GetWindow(this) is not Window window)
			{
				return;
			}

			window.DragMove();
		}

		public void Dispose() { }
	}
}
