using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LanguageMappingsViewModel : BaseViewModel
	{
		private TranslationModel _selectedModel;
		private bool _reSendChecked;
		private readonly ProjectsController _projectController;
		private LanguageMappingModel _selectedLanguageMapping;

		public LanguageMappingsViewModel(BeGlobalTranslationOptions options)
		{
			Options = options;
			TranslationOptions = new ObservableCollection<TranslationModel>();
			LanguageMappings = new ObservableCollection<LanguageMappingModel>();
			_projectController = GetProjectController();
			LoadProjectLanguagePairs();


			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
				if (options.Model != null)
				{
					var model = TranslationOptions.FirstOrDefault(m => m.Model.Equals(options.Model));
					if (model != null)
					{
						var selectedModelIndex = TranslationOptions.IndexOf(model);
						SelectedModelOption = TranslationOptions[selectedModelIndex];
					}
				}
			}
		}

		public BeGlobalTranslationOptions Options { get; set; }
		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }			
		public ObservableCollection<LanguageMappingModel> LanguageMappings { get; set; }
		public List<string> MTCodeSourceList = new List<string>();
		public List<string> MTCodeTargetList = new List<string>();

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;				
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public LanguageMappingModel SelectedLanguageMapping
		{
			get => _selectedLanguageMapping;
			set
			{
				_selectedLanguageMapping = value;
				OnPropertyChanged(nameof(SelectedLanguageMapping));
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (Options?.Model != null)
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		private ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		private void LoadProjectLanguagePairs()
		{
			var currentProjectInfo = _projectController?.CurrentProject?.GetProjectInfo();
			var sourceLanguage = currentProjectInfo?.SourceLanguage;
			var targetLanguages = currentProjectInfo?.TargetLanguages;
			var mtCodeSource = AppInitializer.MTCodes.FirstOrDefault(s => s.TradosCode.Equals(sourceLanguage?.CultureInfo?.Name)
			|| s.TradosCode.Equals(sourceLanguage?.IsoAbbreviation));
			if(mtCodeSource != null)
			{
				MTCodeSourceList.Add(mtCodeSource.MTCodeMain);
				if (!string.IsNullOrEmpty(mtCodeSource.MTCodeLocale))
				{
					MTCodeSourceList.Add(mtCodeSource.MTCodeLocale);
				}
			}

			foreach (var targetLanguage in targetLanguages)
			{
				var languagePair = $"{sourceLanguage.DisplayName} - {targetLanguage.DisplayName}";

				var mtCodeTarget = AppInitializer.MTCodes.FirstOrDefault(s => s.TradosCode.Equals(targetLanguage?.CultureInfo?.Name)
				|| s.TradosCode.Equals(targetLanguage?.IsoAbbreviation));
				if (mtCodeTarget != null)
				{
					MTCodeTargetList.Add(mtCodeTarget.MTCodeMain);
					if (!string.IsNullOrEmpty(mtCodeTarget.MTCodeLocale))
					{
						MTCodeTargetList.Add(mtCodeTarget.MTCodeLocale);
					}

					var languageMappingModel = new LanguageMappingModel
					{
						ProjectLanguagePair = languagePair,
						MTCodeSource = new ObservableCollection<string>(MTCodeSourceList),
						SelectedMTCodeSource = MTCodeSourceList[0],
						MTCodeTarget = new ObservableCollection<string>(MTCodeTargetList),
						SelectedMTCodeTarget = MTCodeTargetList[0],
						Engines = new ObservableCollection<TranslationModel>()
					};
					LanguageMappings.Add(languageMappingModel);
					MTCodeTargetList.Clear();
				}
			}
		}
	}
}