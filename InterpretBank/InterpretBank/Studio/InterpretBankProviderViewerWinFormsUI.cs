using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using DocumentFormat.OpenXml.Bibliography;
using InterpretBank.Model;
using InterpretBank.TermbaseViewer.UI;
using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
    [TerminologyProviderViewerWinFormsUI]
	internal class InterpretBankProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		private TermbaseViewerControl _termbaseControl;
		public event EventHandler<EntryEventArgs> SelectedTermChanged;

		public event EventHandler TermChanged;

		public Control Control
		{
			get
			{
				if (_termbaseControl is not null) return _termbaseControl;

				//var settingsId = InterpretBankProvider.Uri.AbsolutePath.Split('.')[0].TrimStart('/');
				//var settings = PersistenceService.PersistenceService.GetSettings(settingsId);

				TermbaseViewerViewModel = new TermbaseViewerViewModel(InterpretBankProvider.TermSearchService, ServiceManager.DialogService);
				TermbaseViewerViewModel.LoadTerms(SourceLanguage, TargetLanguage, InterpretBankProvider.Settings.Glossaries);

				var termbaseViewer = new TermbaseViewer.UI.TermbaseViewer { DataContext = TermbaseViewerViewModel };
				_termbaseControl = new TermbaseViewerControl(termbaseViewer);

				return _termbaseControl;
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
			
		}

		public void AddTerm(string source, string target)
		{
			((TermbaseViewerControl)Control).TermbaseViewer.AddNewTermButton_Click(
				new TermModel { SourceTerm = source, TargetTerm = target }, null);
		}

		public void EditTerm(IEntry term)
		{
			TermbaseViewerViewModel.EditTerm(term);
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