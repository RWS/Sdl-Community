using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.EventArgs;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Community.UsefulTipsService;
using Sdl.Community.UsefulTipsService.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using SettingsService = Sdl.Community.SdlTmAnonymizer.Services.SettingsService;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	[View(
		Id = "SDLTMAnonymizerView",
		Name = "SDLTM Anonymizer",
		Icon = "icon",
		Description = "Anonymize personal information in Translation Memories",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation),
		AllowViewParts = true)]
	public class SDLTMAnonymizerView : AbstractViewController
	{
		private TmAnonymizerViewControl _control;
		private TmAnonymizerExplorerControl _explorerControl;
		private SDLTMAnonymizerLogViewPart _logViewController;

		internal MainViewModel Model;
		internal SettingsService SettingsService;
		internal event EventHandler<SelectedTabIndexArgs> SelectedTabIndexArgs;

		public UserControl ContentControl => _control;

		protected override void Initialize(IViewContext context)
		{
			SettingsService = new SettingsService(new PathInfo());

			Model = new MainViewModel(SettingsService, this);
			Model.PropertyChanged += Model_PropertyChanged;
			Model.SelectedTabIndex = 0;

			_explorerControl = new TmAnonymizerExplorerControl(Model);
			_logViewController = new SDLTMAnonymizerLogViewPart(Model);

			if (SettingsService.GetSettings().Accepted)
			{
				Model.LogViewModel.IsEnabled = true;
				_control = new TmAnonymizerViewControl(Model);

				AddUsefulTips();
			}
			else
			{
				Model.LogViewModel.IsEnabled = false;
			}
		}

		private void AddUsefulTips()
		{
			try
			{				
				var pathService = new PathService("en");				
				var tipsService = new TipsProvider(pathService);

				var tipsImportFile = Path.Combine(SettingsService.PathInfo.TipsFullPath, "TipsImport.xml");
				CreateTipsImportFile(tipsImportFile);

				var tips = tipsService.ReadTipsImportFile(tipsImportFile);

				tipsService.AddTips(tips, StringResources.Application_Name);			
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		private static void CreateTipsImportFile(string fileName)
		{
			var tipsImportFile = "Sdl.Community.SdlTmAnonymizer.UsefulTips.TipsImport.xml";
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tipsImportFile))
			{
				if (stream != null)
				{					
					using (var reader = new StreamReader(stream))
					{
						using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
						{
							writer.Write(reader.ReadToEnd());
							writer.Flush();
						}
					}
				}
			}
		}

		protected override Control GetContentControl()
		{
			return _control;
		}

		protected override Control GetExplorerBarControl()
		{
			return _explorerControl;
		}

		public override void Dispose()
		{
			Model.PropertyChanged -= Model_PropertyChanged;
			base.Dispose();
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.EndsWith(nameof(MainViewModel.SelectedTabIndex)))
			{
				SelectedTabIndexArgs?.Invoke(this, new SelectedTabIndexArgs { SelectedIndex = Model.SelectedTabIndex });
			}
		}
	}
}
