using System.Collections.Generic;
using System.ComponentModel;

namespace Sdl.Community.GSVersionFetch.Interface
{
    public interface ITreeViewItemModel : INotifyPropertyChanged
    {
        string DisplayValuePath { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        string SelectedValuePath { get; }

        IEnumerable<ITreeViewItemModel> GetChildren();

        IEnumerable<ITreeViewItemModel> GetHierarchy();
    }
}