using System.Collections.Generic;
using System.ComponentModel;

namespace Sdl.Community.GSVersionFetch.Interface
{
	public interface ITreeViewItemModel : INotifyPropertyChanged
	{
		string SelectedValuePath { get; }

		string DisplayValuePath { get; }

		bool IsExpanded { get; set; }

		bool IsSelected { get; set; }

		IEnumerable<ITreeViewItemModel> GetHierarchy();

		IEnumerable<ITreeViewItemModel> GetChildren();
	}
}
