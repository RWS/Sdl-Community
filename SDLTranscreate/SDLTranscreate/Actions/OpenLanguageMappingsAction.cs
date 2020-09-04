using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.LanguageMapping;
using Sdl.Community.Transcreate.LanguageMapping.View;
using Sdl.Community.Transcreate.LanguageMapping.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_OpenLanguageMappings_Action",
		Name = "TranscreateManager_LanguageMappings_Name",
		Description = "TranscreateManager_LanguageMappings_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "LanguageMappings"
		)]
	[ActionLayout(typeof(TranscreateManagerSettingsGroup), 6, DisplayType.Large)]
	public class OpenLanguageMappingsAction : AbstractViewControllerAction<TranscreateViewController>
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
