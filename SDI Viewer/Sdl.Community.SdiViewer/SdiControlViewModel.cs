using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.SdiViewer
{
	public class SdiControlViewModel : INotifyPropertyChanged
	{
		private string _test;

		public SdiControlViewModel()
		{
			_test = "and";
		}

		public string Test
		{
			get => _test;
			set
			{
				if (_test == value)
				{
					return;
				}
				_test = value;
				OnPropertyChanged(nameof(Test));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
