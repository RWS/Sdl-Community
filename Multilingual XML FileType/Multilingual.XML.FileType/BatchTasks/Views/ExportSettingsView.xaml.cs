using System.Windows.Controls;
using Multilingual.XML.FileType.BatchTasks.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Multilingual.XML.FileType.BatchTasks.Views
{
	/// <summary>
	/// Interaction logic for LanguageMappingView.xaml
	/// </summary>
	public partial class ExportSettingsView : UserControl, IUISettingsControl, ISettingsAware<MultilingualXmlExportSettings>
	{
		public ExportSettingsView()
		{
			InitializeComponent();
		}

		public MultilingualXmlExportSettings Settings { get; set; }

		
		public bool ValidateChildren()
		{
			return true;
		}

		public void Dispose()
		{
		}
	}
}
