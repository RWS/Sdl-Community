using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TMX_UI.ViewModel
{

	public class OptionsViewModel : INotifyPropertyChanged
	{
		private bool _isLoading;
		public bool IsLoading { 
			get => _isLoading;
			set { 
				_isLoading = value;
				OnPropertyChanged(); 
			}
		}

		public enum StateType { SelectDatabases, ImportTmxFile, ExportToTmx };
		private StateType _state = StateType.SelectDatabases;
		public StateType State { 
			get => _state;
			set {
				_state = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StateIndex));
				OnPropertyChanged(nameof(IsSelectDatabases));
				OnPropertyChanged(nameof(IsImportTmxFile));
				OnPropertyChanged(nameof(IsExportToTmx));
			}
		}

		public int StateIndex {
			get => (int)State;
			set => State = (StateType)value;
		}
		public bool IsSelectDatabases => State == StateType.SelectDatabases;
		public bool IsImportTmxFile => State == StateType.ImportTmxFile;
		public bool IsExportToTmx => State == StateType.ExportToTmx;

		private IReadOnlyList<DatabaseItem> _databases = new List<DatabaseItem>();
		public IReadOnlyList<DatabaseItem> Databases { 
			get => _databases;
			set {
				_databases = value;
				OnPropertyChanged();
			} 
		}


		public ExportToTmxViewModel ExportToTmx { get; } = new ExportToTmxViewModel();
		public ImportTmxFileViewModel ImportFromTmxFile { get; } = new ImportTmxFileViewModel();


		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
