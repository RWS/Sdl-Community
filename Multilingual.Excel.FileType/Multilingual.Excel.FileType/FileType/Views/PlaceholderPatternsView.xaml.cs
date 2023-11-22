using System.Windows.Controls;
using Multilingual.Excel.FileType.FileType.Settings;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.Excel.FileType.FileType.Views
{
	/// <summary>
	/// Interaction logic for LanguageMappingView.xaml
	/// </summary>
	public partial class PlaceholderPatternsView : UserControl, IUISettingsControl, IFileTypeSettingsAware<PlaceholderPatternSettings>
	{
		public PlaceholderPatternsView()
		{
			InitializeComponent();
		}

		public PlaceholderPatternSettings Settings { get; set; }

		
		public bool ValidateChildren()
		{
			return true;
		}

		public void Dispose()
		{
		}
	}
}
