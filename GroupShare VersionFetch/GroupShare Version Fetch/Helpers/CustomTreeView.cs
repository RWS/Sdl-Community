using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class CustomTreeView : TreeView, INotifyPropertyChanged
	{
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItem", typeof(Object), typeof(CustomTreeView), new PropertyMetadata(null));
		public new Object SelectedItem
		{
			get { return (Object)GetValue(SelectedItemProperty); }
			set
			{
				SetValue(SelectedItemsProperty, value);
				NotifyPropertyChanged("SelectedItem");
			}
		}

		public CustomTreeView()
			: base()
		{
			base.SelectedItemChanged += new RoutedPropertyChangedEventHandler<Object>(MyTreeView_SelectedItemChanged);
		}

		private void MyTreeView_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
		{
			this.SelectedItem = base.SelectedItem;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String aPropertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(aPropertyName));
		}
	}
}
