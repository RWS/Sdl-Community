using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reports.Viewer.Plus.Model
{
	public class GroupType: INotifyPropertyChanged
	{
		public string Name { get; set; }
		public string Type { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
