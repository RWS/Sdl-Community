using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationViewModel : ViewModelBase
	{
		private ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<Rule> _rules;
		private Rule _selectedItem;
		private bool _selectAll;
		private ICommand _selectAllCommand;
		private FlowDocument _document;
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;

		public TranslationViewModel(ObservableCollection<TmFile> tmsCollection)
		{
			_tmsCollection = tmsCollection;
			_rules = Constants.GetDefaultRules();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>
			{
				new SourceSearchResult
				{
					SegmentNumber = "2",
					SourceText = "source from tm",
					TmFilePath = "dasdasdasdasdasdasdasd",
					Document =  Convert("source from tm")

				},
				new SourceSearchResult
				{
					SegmentNumber = "3",
					SourceText = "source from ",
					TmFilePath = "dasdasdasdasdasdasdasd",
					Document = Convert("source from ")

				},
				new SourceSearchResult
				{
					SegmentNumber = "21",
					SourceText = "source",
					TmFilePath = "dasdasdasdasdasdasdasd",
					Document = Convert("source")
				}
			};
		}

		public object Convert(object value)
		{
			FlowDocument doc = new FlowDocument();

			string s = value as string;
			if (s != null)
			{
				using (StringReader reader = new StringReader(s))
				{
					string newLine;
					while ((newLine = reader.ReadLine()) != null)
					{
						Paragraph paragraph = new Paragraph(new Run(newLine));
						doc.Blocks.Add(paragraph);
					}
				}
			}

			return doc;
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllRules, true));

		private void SelectAllRules()
		{
			
		}
		public bool SelectAll
		{
			get => _selectAll;

			set
			{
				if (Equals(value, _selectAll))
				{
					return;
				}
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}
		public ObservableCollection<SourceSearchResult> SourceSearchResults
		{
			get => _sourceSearchResults;

			set
			{
				if (Equals(value, _sourceSearchResults))
				{
					return;
				}
				_sourceSearchResults = value;
				OnPropertyChanged(nameof(SourceSearchResults));
			}
		}

		//public FlowDocument Document
		//{
		//	get => _document;
		//	set
		//	{
		//		_document = value;
		//		OnPropertyChanged(nameof(Document));
		//	}
		//}
		public ObservableCollection<Rule> RulesCollection
		{
			get => _rules;

			set
			{
				if (Equals(value, _rules))
				{
					return;
				}
				_rules = value;
				OnPropertyChanged(nameof(RulesCollection));
			}
		}

		public Rule SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
				if (RulesCollection.Any(r => r.Id == null))
				{
					SetIdForNewRules();
				}
			}
		}

		private void SetIdForNewRules()
		{
			var newRules = RulesCollection.Where(r => r.Id == null).ToList();
			foreach (var rule in newRules)
			{
				rule.Id = Guid.NewGuid().ToString();
			}
		}
	}
}
