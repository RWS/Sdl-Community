using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using DocumentFormat.OpenXml.Bibliography;
using InterpretBank.TermbaseViewer.UI;
using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	[TerminologyProviderViewerWinFormsUI]
	internal class InterpretBankProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		public event EventHandler<EntryEventArgs> SelectedTermChanged;

		public event EventHandler TermChanged;

		public Control Control
		{
			get
			{
				var settingsId = InterpretBankProvider.Uri.AbsolutePath.Split('.')[0].TrimStart('/');
				var settings = PersistenceService.PersistenceService.GetSettings(settingsId);
				TermbaseViewerViewModel = new TermbaseViewerViewModel(InterpretBankProvider.TermSearchService);

				TermbaseViewerViewModel.LoadTerms(SourceLanguage, TargetLanguage, settings.Glossaries);

				var termbaseViewer = new TermbaseViewer.UI.TermbaseViewer { DataContext = TermbaseViewerViewModel };

				var termbaseControl = new TermbaseViewerControl(termbaseViewer);

				return termbaseControl;
			}
		}

		public bool Initialized
		{
			get
			{
				return true;
			}
		}

		public IEntry SelectedTerm { get; set; }
		public InterpretBankProvider InterpretBankProvider { get; private set; }
		public TermbaseViewerViewModel TermbaseViewerViewModel { get; private set; }

		public void AddAndEditTerm(IEntry term, string source, string target)
		{
			throw new NotImplementedException();
		}

		public void AddTerm(string source, string target)
		{
			throw new NotImplementedException();
		}

		public void EditTerm(IEntry term)
		{
			throw new NotImplementedException();
		}

		public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
		{
			if (terminologyProvider is not InterpretBankProvider interpretBankProvider)
				return;

			InterpretBankProvider = interpretBankProvider;

			var currentProject = StudioContext.ProjectsController.CurrentProject;
			var targetLanguages = currentProject.GetTargetLanguageFiles().Select(p => p.Language);

			TargetLanguage = targetLanguages.FirstOrDefault(l => l.CultureInfo.Equals(target));
			SourceLanguage = currentProject.GetProjectInfo().SourceLanguage;

			var y = source.DisplayName.Split(' ')[0];
			var z = target.DisplayName.Split(' ')[0];
		}

		public Language TargetLanguage { get; set; }

		public Language SourceLanguage { get; set; }

		public void JumpToTerm(IEntry entry)
		{
			TermbaseViewerViewModel.JumpToTerm(entry);
		}

		public void Release()
		{

		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
			=> terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);
	}
}