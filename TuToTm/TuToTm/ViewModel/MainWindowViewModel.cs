using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.TuToTm.Commands;
using Sdl.Community.TuToTm.Helpers;
using Sdl.Community.TuToTm.Model;

namespace Sdl.Community.TuToTm.ViewModel
{
	public class MainWindowViewModel:BaseModel
	{
		private ObservableCollection<TmDetails> _tmCollection;
		private readonly TmHelper _tmHelper;
		private ICommand _removeTmCommand;
		private ICommand _updateCommand;
		private string _sourceText;
		private string _targetText;
		private IList _selectedTms;

		public MainWindowViewModel()
		{
			_tmCollection = new ObservableCollection<TmDetails>();
			_tmHelper = new TmHelper();
			var tmsDetails = _tmHelper.LoadLocalUserTms();
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
		public IList SelectedTms
		{
			get => _selectedTms;
			set
			{
				_selectedTms = value;

				OnPropertyChanged(nameof(SelectedTms));
			}
		}

		public ICommand RemoveTmCommand => _removeTmCommand ?? (_removeTmCommand = new CommandHandler(RemoveTm, true));
		public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new CommandHandler(UpdateTm, true));

		private void UpdateTm()
		{
			var selectedTms = TmsCollection.Where(t => t.IsSelected).ToList();
			if (selectedTms.Any() && AreTextFieldsCompleted())
			{
				foreach (var selectedTm in selectedTms)
				{
					_tmHelper.AddTu(selectedTm,SourceText,TargetText);
				}
			}
		}

		private bool AreTextFieldsCompleted()
		{
			return !string.IsNullOrEmpty(SourceText) && !string.IsNullOrEmpty(TargetText);
		}

		private void RemoveTm()
		{
			if (SelectedTms.Count > 0)
			{
				var selectedTm = SelectedTms[0] as TmDetails;

				var tmToRemove = TmsCollection.FirstOrDefault(t => t.TmPath.Equals(selectedTm?.TmPath));
				if (tmToRemove != null)
				{
					TmsCollection.Remove(tmToRemove);
				}
			}
		}
	}
}
