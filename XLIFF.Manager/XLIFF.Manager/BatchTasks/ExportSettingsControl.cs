using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.BatchTasks.View;
using Sdl.Community.XLIFF.Manager.BatchTasks.ViewModel;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public partial class ExportSettingsControl : UserControl, ISettingsAware<ExportSettings>
	{
		private ExportSettingsViewModel _exportOptionsViewModel;

		public ExportSettingsControl()
		{
			InitializeComponent();
		}
		
		public ExportSettings Settings { get; set; }

		public void SetDataContext()
		{
			if (elementHost1.Child is ExportSettingsView view && view.DataContext == null)
			{
				ExportOptionsViewModel = new ExportSettingsViewModel(Settings, new DialogService());
				view.DataContext = ExportOptionsViewModel;
			}
		}

		public ExportSettingsViewModel ExportOptionsViewModel
		{
			get => _exportOptionsViewModel;
			set
			{
				if (_exportOptionsViewModel != null)
				{
					_exportOptionsViewModel.PropertyChanged -= ExportOptionsViewModel_PropertyChanged;
				}

				_exportOptionsViewModel = value;

				if (_exportOptionsViewModel != null)
				{
					_exportOptionsViewModel.PropertyChanged += ExportOptionsViewModel_PropertyChanged;
				}
			}
		}

		private void ExportOptionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{			
			UpdateSettings(e.PropertyName);
		}

		private void UpdateSettings(string propertyName)
		{
			if (propertyName == nameof(ExportSettingsViewModel.ProjectFiles))
			{
				Settings.ProjectFiles = _exportOptionsViewModel.ProjectFiles;
			}

			if (propertyName == nameof(ExportSettingsViewModel.CopySourceToTarget) ||
			    propertyName == nameof(ExportSettingsViewModel.IncludeTranslations) ||
			    propertyName == nameof(ExportSettingsViewModel.SelectedXliffSupport))
			{
				var options = new ExportOptions
				{
					CopySourceToTarget = _exportOptionsViewModel.CopySourceToTarget,
					IncludeTranslations = _exportOptionsViewModel.IncludeTranslations,
					XliffSupport = _exportOptionsViewModel.SelectedXliffSupport.SupportType
				};

				Settings.ExportOptions = options;
			}

			if (propertyName == nameof(ExportSettingsViewModel.OutputFolder))
			{
				Settings.TransactionFolder = _exportOptionsViewModel.OutputFolder;
			}

			if (propertyName == nameof(ExportSettingsViewModel.SelectedExcludeFilterItems))
			{
				Settings.SelectedFilterItemIds = _exportOptionsViewModel.SelectedExcludeFilterItems.Select(a => a.Id).ToList();
			}
		}


		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			if (ExportOptionsViewModel != null)
			{
				ExportOptionsViewModel.PropertyChanged -= ExportOptionsViewModel_PropertyChanged;
			}

			base.Dispose(disposing);
		}
	}
}
