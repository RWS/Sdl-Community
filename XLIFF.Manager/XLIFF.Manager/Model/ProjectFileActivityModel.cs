using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActivityModel: INotifyPropertyChanged, IDisposable
	{
	
		public ProjectFileActivityModel()
		{
			
		}

		public Enumerators.Action Action { get; set; }

		public Enumerators.Status Status { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }

		public string Message { get; set; }

		// Options
		

		// Segments etc...

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
