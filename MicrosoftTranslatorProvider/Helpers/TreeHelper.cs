using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MicrosoftTranslatorProvider.Controls
{
	public static class TreeHelper
	{
		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject child)
		{
			var parent = VisualTreeHelper.GetParent(child);
			while (parent is not null)
			{
				yield return parent;
				parent = VisualTreeHelper.GetParent(parent);
			}
		}
	}
}