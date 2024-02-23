using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace LanguageWeaverProvider.Helpers
{
	public static class AnimationsHelper
    {
		public static void StartOpeningWindowAnimation(Window window)
		{
			var opacityAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.15));
			window.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
		}

		public static void StartClosingWindowAnimation(Window window)
		{
			var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.1));
			fadeOutAnimation.Completed += (s, _) => window.Close();
			window.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
		}
	}
}