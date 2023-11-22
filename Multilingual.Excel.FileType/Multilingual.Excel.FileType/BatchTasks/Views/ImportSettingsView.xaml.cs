using System.Windows.Controls;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Multilingual.Excel.FileType.BatchTasks.Views
{
	/// <summary>
	/// Interaction logic for LanguageMappingView.xaml
	/// </summary>
	public partial class ImportSettingsView : UserControl, IUISettingsControl, ISettingsAware<MultilingualExcelImportSettings>
	{
		public ImportSettingsView()
		{
			InitializeComponent();
		}

		public MultilingualExcelImportSettings Settings { get; set; }

		
		public bool ValidateChildren()
		{
			return true;
		}

		public void Dispose()
		{
		}
	}
}
