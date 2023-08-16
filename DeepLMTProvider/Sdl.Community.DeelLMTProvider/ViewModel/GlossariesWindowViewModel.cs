using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
	public class GlossariesWindowViewModel : ViewModel
	{
		private ObservableCollection<GlossaryInfo> _glossaries;

		public GlossariesWindowViewModel(DeepLGlossaryClient deepLGlossaryClient, IMessageService messageService, IGlossaryBrowserService glossaryBrowserService, ITsvReader tsvReader)
		{
			DeepLGlossaryClient = deepLGlossaryClient;
			MessageService = messageService;
			GlossaryBrowserService = glossaryBrowserService;
			TsvReader = tsvReader;
			LoadGlossaries();
		}

		public ObservableCollection<GlossaryInfo> Glossaries
		{
			get => _glossaries;
			set => SetField(ref _glossaries, value);
		}

		public ICommand ImportGlossaryCommand => new AsyncParameterlessCommand(ImportGlossary);

		private DeepLGlossaryClient DeepLGlossaryClient { get; set; }
		private IGlossaryBrowserService GlossaryBrowserService { get; }
		private IMessageService MessageService { get; }
		private ITsvReader TsvReader { get; }

		private void HandleError(string message, [CallerMemberName] string failingMethod = null)
		{
			MessageService.ShowWarning(message, failingMethod);
		}

		private async Task ImportGlossary()
		{
			var (success, result, message) = await DeepLGlossaryClient.GetGlossarySupportedLanguagePairs(DeepLTranslationProviderClient.ApiKey);

			if (!success)
			{
				HandleError(message);
				return;
			}

			var glossarySupportedLanguages = result.Select(glp => glp.SourceLanguage).Distinct().ToList();
			if (GlossaryBrowserService.Browse(glossarySupportedLanguages, out var path, out var sourceLanguage, out var targetLanguage))
			{
				var selectedFilePath = path;
				var glossaryFile = TsvReader.ReadTsvGlossary(selectedFilePath);

				glossaryFile.SourceLanguage = sourceLanguage;
				glossaryFile.TargetLanguage = targetLanguage;
				glossaryFile.Name = Path.GetFileNameWithoutExtension(selectedFilePath);

				GlossaryInfo glossary;
				(success, glossary, message) = await DeepLGlossaryClient.ImportGlossary(glossaryFile, DeepLTranslationProviderClient.ApiKey);

				if (!success)
				{
					HandleError(message);
					return;
				}

				Glossaries.Add(glossary);
			}
		}

		private async void LoadGlossaries()
		{
			var (success, result, message) = await DeepLGlossaryClient.GetGlossaries(DeepLTranslationProviderClient.ApiKey);

			if (!success)
			{
				HandleError(message);
				return;
			}

			Glossaries = new ObservableCollection<GlossaryInfo>(result);
		}
	}
}