using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationViewModel : ViewModelBase
	{
		private ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<Rule> _rules;
		private bool _selectAll;
		private ICommand _selectAllCommand;

		public TranslationViewModel(ObservableCollection<TmFile> tmsCollection)
		{
			_tmsCollection = tmsCollection;
			_rules = new ObservableCollection<Rule>
			{
				new Rule
				{
					Name = "dma"
				},
				new Rule
				{
					Name = "daaama"
				}
			};
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
	}
}
