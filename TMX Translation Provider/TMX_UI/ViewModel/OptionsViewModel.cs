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
		public bool HasAnyDatabase => Databases.Count > 0;
		public bool HasNoDatabase => Databases.Count == 0;

		private IReadOnlyList<DatabaseItem> _databases = new List<DatabaseItem>();
		public IReadOnlyList<DatabaseItem> Databases { 
			get => _databases;
			set {
				_databases = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasAnyDatabase));
				OnPropertyChanged(nameof(HasNoDatabase));
				OnPropertyChanged(nameof(IsMongoDbNotInstalled));
			}
		}

		private bool _careForLocale = false;
		public bool CareForLocale { 
			get => _careForLocale;
			set {
				_careForLocale = value;
				OnPropertyChanged();
			} 
		}

		// very simple way to verify if user has Mongodb Community Server installed locally 
		// obviously, it doesn't always work, but it's a very simple method that works probably 98% of the cases
		private static bool TryDetectLocalMongoDb()
		{
			return false;
			var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			if (programFiles.EndsWith("(x86)"))
				// the idea - we can be run as a 32-bit plugin
				programFiles = programFiles.Substring(0, programFiles.Length - 5).Trim();

			var mongodbServer = $"{programFiles}\\MongoDB\\Server\\6.0\\bin\\mongod.exe";
			var exists = System.IO.File.Exists(mongodbServer);
			return exists;
		}

		public bool IsMongoDbNotInstalled => TryDetectLocalMongoDb() == false && Databases.Count == 0;

		public ExportToTmxViewModel ExportToTmx { get; } = new ExportToTmxViewModel();
		public ImportTmxFileViewModel ImportFromTmxFile { get; } = new ImportTmxFileViewModel();


		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
