using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMX_UI.Properties;
using TMX_UI.Services;

namespace TMX_UI.ViewModel
{
	public class ExportToTmxViewModel : INotifyPropertyChanged
	{
		private string _fileName = "";
		public string FileName
		{
			get => _fileName;
			set
			{
				_fileName = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanExport));
			}
		}

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
		public int DatabaseIdx
		{
			get => _databaseIdx;
			set
			{
				_databaseIdx = value;
				OnPropertyChanged();
			}
		}

		private bool _exportComplete = false;
		public bool ExportComplete
		{
			get => _exportComplete;
			set
			{
				_exportComplete = value;
				OnPropertyChanged();
			}
		}

		// FileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&  -> note: to make this 100% correct, I need to account for "\\" which is valid in the path,
		// and for the second char to be colon (:), in case it's a full name
		public bool CanExport => FileName != "" && DatabaseIdx >= 0 && !IsExporting;

		public bool IsExporting => ExportService.Instance.IsExporting();
		public double ExportProgress => ExportService.Instance.ExportProgress();

		public void RefreshProgress()
		{
			OnPropertyChanged(nameof(IsExporting));
			OnPropertyChanged(nameof(ExportProgress));
			OnPropertyChanged(nameof(CanExport));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
