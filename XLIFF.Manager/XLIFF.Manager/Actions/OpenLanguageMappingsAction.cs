using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.LanguageMapping;
using Sdl.Community.XLIFF.Manager.LanguageMapping.View;
using Sdl.Community.XLIFF.Manager.LanguageMapping.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.XLIFF.Manager.Actions
{
	[Action("XLIFFManager_OpenLanguageMappings_Action",
		Name = "XLIFFManager_LanguageMappings_Name",
		Description = "XLIFFManager_LanguageMappings_Description",
		ContextByType = typeof(XLIFFManagerViewController),
		Icon = "LanguageMappings"
		)]
	[ActionLayout(typeof(XLIFFManagerSettingsGroup), 6, DisplayType.Large)]
	public class OpenLanguageMappingsAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private PathInfo _pathInfo;
		private LanguageProvider _languageProvider;

		protected override void Execute()
		{			
			var view = new LanguageMappingWindow();
			var viewModel = new LanguageMappingViewModel(view, _languageProvider);
			view.DataContext = viewModel;
			view.ShowDialog();
		}

		public override void Initialize()
		{
			Enabled = true;
			_pathInfo = new PathInfo();
			_languageProvider = new LanguageProvider(_pathInfo);
		}
	}
}
