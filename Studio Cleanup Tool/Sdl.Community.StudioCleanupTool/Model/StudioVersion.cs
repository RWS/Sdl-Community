using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Annotations;

namespace Sdl.Community.StudioCleanupTool.Model
{
	public class StudioVersion : INotifyPropertyChanged
	{
		private bool _isSelected;
		public string DisplayName { get; set; }
		public string VersionNumber { get; set; }
		public string FullVersionNumber { get; set; }
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged();
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
