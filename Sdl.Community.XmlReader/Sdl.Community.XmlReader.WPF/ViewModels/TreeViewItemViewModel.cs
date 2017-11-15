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

        bool _isExpanded;
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
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(body.Member.Name));
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged(() => IsSelected);
                }
            }
        }
    }
}
