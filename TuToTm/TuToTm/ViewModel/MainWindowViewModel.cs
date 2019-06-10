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

		public ICommand RemoveTmCommand => _removeTmCommand ?? (_removeTmCommand = new CommandHandler(RemoveTm, true));

		private void RemoveTm()
		{
			
		}
	}
}
