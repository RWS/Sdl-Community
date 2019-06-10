using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TuToTm.Commands;
using Sdl.Community.TuToTm.Helpers;
using Sdl.Community.TuToTm.Model;

namespace Sdl.Community.TuToTm.ViewModel
{
	public class MainWindowViewModel:BaseModel
	{
		private ObservableCollection<TmDetails> _tmCollection;
		private ICommand _removeTmCommand;
		private string _sourceText;
		private string _targetText;


		public MainWindowViewModel()
		{
			_tmCollection = new ObservableCollection<TmDetails>();
			var tmHelper = new TmHelper();
			var tmsDetails = tmHelper.LoadLocalUserTms();
			foreach (var tm in tmsDetails)
			{
				_tmCollection.Add(tm);
			}
		}

		public ObservableCollection<TmDetails> TmsCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged(nameof(TmsCollection));
			}
		}

		public string SourceText
		{
			get => _sourceText;
			set
			{
				if (_sourceText == value)
				{
					return;
				}
				_sourceText = value;
				OnPropertyChanged(nameof(SourceText));
			}
		}
		public string TargetText
		{
			get => _targetText;
			set
			{
				if (_targetText == value)
				{
					return;
				}
				_targetText = value;
				OnPropertyChanged(nameof(TargetText));
			}
		}

		public ICommand RemoveTmCommand => _removeTmCommand ?? (_removeTmCommand = new CommandHandler(RemoveTm, true));

		private void RemoveTm()
		{
			
		}
	}
}
