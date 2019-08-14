using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        readonly ObservableCollection<TreeViewItemViewModel> _children;
        readonly TreeViewItemViewModel _parent;

	    bool _isSelected;

        public TreeViewItemViewModel() { }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;
            _children = new ObservableCollection<TreeViewItemViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

	    protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
	    {
		    var body = propertyExpression.Body as MemberExpression;
		    if (PropertyChanged != null)
		    {
			    if (body != null)
			    {
				    PropertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
			    }
		    }
	    }

	    /// <summary>
	    /// Returns the logical child items of this object.
	    /// </summary>
	    public ObservableCollection<TreeViewItemViewModel> Children => _children;
       

        public TreeViewItemViewModel Parent => _parent;

	    /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded { get; set; }

	    /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
		    set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged(() => IsSelected);
                }
            }
        }
    }
}
