using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.StarTransit.Shared.Events;

namespace Sdl.Community.StarTransit.Model
{
	public class TmSummaryOptions : BaseModel
	{
		private TuImportStatistics _importStatistics;
		private int _xliffImportProgress;
		private int _importingTmFileNumber;
		private int _totalTransitTmsFiles;
		private bool _xliffImportStarted;
		private int _projectLangPairProgress;
		private string _filesImportProgress;

		public Image SourceFlag { get; set; }
		public Image TargetFlag { get; set; }
		public CultureInfo TargetLanguage { get; set; }


		public int ProjectLangPairProgress
		{
			get => _projectLangPairProgress;
			set
			{
				if (_projectLangPairProgress == value) return;
				_projectLangPairProgress = value;
				OnPropertyChanged(nameof(ProjectLangPairProgress));
			}
		}

		public TuImportStatistics TuImportStatistics
		{
			get => _importStatistics;
			set
			{
				_importStatistics = value;
				OnPropertyChanged(nameof(TuImportStatistics));
			}
		}

		public int XliffImportProgress
		{
			get => _xliffImportProgress;
			set
			{
				if (_xliffImportProgress == value) return;
				_xliffImportProgress = value;
				OnPropertyChanged(nameof(XliffImportProgress));
			}
		}

		public int ImportingTmFileNumber
		{
			get => _importingTmFileNumber;
			set
			{
				if (_importingTmFileNumber == value) return;
				_importingTmFileNumber = value;
				OnPropertyChanged(nameof(ImportingTmFileNumber));
			}
		}

		public string FilesImportProgress
		{
			get => _filesImportProgress;
			set
			{
				if (_filesImportProgress == value) return;
				_filesImportProgress = value;
				OnPropertyChanged(nameof(FilesImportProgress));
			}
		}

		public bool XliffImportStarted
		{
			get => _xliffImportStarted;
			set
			{
				if (_xliffImportStarted == value) return;
				_xliffImportStarted = value;
				OnPropertyChanged(nameof(XliffImportStarted));
			}
		}

		public List<string> SelectedOption { get; set; }
	}
}
