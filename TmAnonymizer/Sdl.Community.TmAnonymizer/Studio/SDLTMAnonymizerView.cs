using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.EventArgs;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Community.UsefulTipsService;
using Sdl.Community.UsefulTipsService.Model;
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
				var languageId = "en";
				var pathService = new PathService(languageId);
				var tipsService = new TipsProvider(pathService);

				var tipsLanguageFullPath = GetTipsLanguagePath(languageId);
				
				CreateTipsImportContent(tipsLanguageFullPath, languageId);

				var tipsImportFile = Path.Combine(tipsLanguageFullPath, "TipsImport.xml");
				var tips = GetTips(tipsService, tipsImportFile, tipsLanguageFullPath);

				tipsService.AddTips(tips, StringResources.Application_Name);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		private static List<Tip> GetTips(TipsProvider tipsService, string tipsImportFile, string tipsLanguageFullPath)
		{
			var tips = tipsService.ReadTipsImportFile(tipsImportFile);

			// update the relative path infor for each of the Tips
			foreach (var tip in tips)
			{
				tip.Icon = GetFullPath(tip.Icon, tipsLanguageFullPath);
				tip.Content = GetFullPath(tip.Content, tipsLanguageFullPath);
				tip.DescriptionImage = GetFullPath(tip.DescriptionImage, tipsLanguageFullPath);
			}

			// save the updated tips with the relative path info
			tipsService.CreateTipsImportFile(tipsImportFile, tips);

			return tips;
		}

		private string GetTipsLanguagePath(string languageId)
		{
			var tipsLanguageFullPath = Path.Combine(SettingsService.PathInfo.TipsFullPath, languageId);
			if (!Directory.Exists(tipsLanguageFullPath))
			{
				Directory.CreateDirectory(tipsLanguageFullPath);
			}

			return tipsLanguageFullPath;
		}

		private static string GetFullPath(string fileName, string tipsLanguageFullPath)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				var name = Path.GetFileName(fileName);
				return Path.Combine(tipsLanguageFullPath, name);
			}

			return fileName;
		}

		private static void CreateTipsImportContent(string tipsLanguageFullPath, string languageId)
		{
			RemoveAllFiles(tipsLanguageFullPath);

			var tipsContentFileInput = "Sdl.Community.SdlTmAnonymizer.UsefulTips." + languageId + "." + languageId + ".zip";
			var tipsContentFileOutput = Path.Combine(tipsLanguageFullPath, languageId + ".zip");			

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tipsContentFileInput))
			{
				if (stream != null)
				{
					using (var reader = new BinaryReader(stream))
					{
						using (Stream writer = File.Create(tipsContentFileOutput))
						{
							var buffer = new byte[2048];
							while (true)
							{								
								var current = reader.Read(buffer, 0, buffer.Length);
								if (current == 0)
								{
									break;
								}

								writer.Write(buffer, 0, current);
							}
						}
					}
				}
			}

			ZipFile.ExtractToDirectory(tipsContentFileOutput, tipsLanguageFullPath);
		}

		private static void RemoveAllFiles(string tipsLanguageFullPath)
		{
			try
			{
				var files = Directory.GetFiles(tipsLanguageFullPath, "*.*");
				foreach (var file in files)
				{
					File.Delete(file);
				}
			}
			catch
			{
				//ignore
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
