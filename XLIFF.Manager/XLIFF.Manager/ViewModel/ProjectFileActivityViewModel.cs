using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XLIFF.Manager.ViewModel
{
	public class ProjectFileActivityViewModel: INotifyPropertyChanged, IDisposable
	{		
		private string _activationAttempt;

		public ProjectFileActivityViewModel()
		{			
		}

		public string ActivationAttempt
		{
			get { return _activationAttempt; }
			set
			{
				_activationAttempt = value;
				OnPropertyChanged(nameof(ActivationAttempt));
			}
		}

		public void Dispose()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}		
	}
}
