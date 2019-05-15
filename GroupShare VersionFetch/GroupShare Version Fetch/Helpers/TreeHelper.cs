using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public static class TreeHelper
	{
		public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject child)
		{
			var parent = VisualTreeHelper.GetParent(child);
			while (parent != null)
			{
				yield return parent;
				parent = VisualTreeHelper.GetParent(parent);
			}
		}
	}
}
