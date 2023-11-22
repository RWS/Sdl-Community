using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMX_UI.Services;

namespace TMX_UI.ViewModel
{
	public class ImportTmxFileViewModel : INotifyPropertyChanged
	{
		private string _fileName = "";
		public string FileName { 
			get => _fileName;
			set { 
				_fileName = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanImport));
			}
		}

		// note: the first Database item is "Create New Database"
		private IReadOnlyList<DatabaseItem> _databases = new List<DatabaseItem>();
		public IReadOnlyList<DatabaseItem> Databases
		{
			get => _databases;
			set
			{
				_databases = value;
				OnPropertyChanged();
			}
		}

		private int _databaseIdx = 0;
		public int DatabaseIdx { 
			get => _databaseIdx;
			set {
				_databaseIdx = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCreateNewDatabase));
				OnPropertyChanged(nameof(CanImport));
			}
		}

		public bool IsCreateNewDatabase => _databaseIdx == 0;


		private string _newDatabaseName = "";
		public string NewDatabaseName {
			get => _newDatabaseName;
			set {
				_newDatabaseName = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanImport));
			}
		}

		private bool _importComplete = false;
		public bool ImportComplete { 
			get => _importComplete; 
			set {
				_importComplete = value;
				OnPropertyChanged();
			}
		}

		public bool CanImport => FileName != "" && File.Exists(FileName) && DatabaseIdx >= 0 
			&& (DatabaseIdx > 0 || NewDatabaseName != "") && !ImportService.Instance.IsImporting();

		public bool IsImporting => ImportService.Instance.IsImporting();
		public double ImportProgress => ImportService.Instance.ImportProgress();

		public string ImportDatabaseName() => DatabaseIdx == 0 ? NewDatabaseName : Databases[DatabaseIdx].Name;

		public void RefreshProgress()
		{
			OnPropertyChanged(nameof(IsImporting));
			OnPropertyChanged(nameof(ImportProgress));
			OnPropertyChanged(nameof(CanImport));
		}


		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
