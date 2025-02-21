using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace MicrosoftTranslatorProvider.Helpers
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