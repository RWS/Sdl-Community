using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel:ViewModelBase
	{
		private bool _selectAll;
		private ICommand _selectFoldersCommand;
		private ICommand _removeCommand;
		private ICommand _selectTmCommand;
		private ICommand _selectAllCommand;
		private ObservableCollection<TmFile> _tmsCollection = new ObservableCollection<TmFile>();

		public ICommand SelectFoldersCommand => _selectFoldersCommand ??
		                                        (_selectFoldersCommand = new CommandHandler(SelectFolder, true));
		public ICommand RemoveCommand => _removeCommand ??
		                                        (_removeCommand = new CommandHandler(Remove, true));
		public ICommand SelectTmCommand => _selectTmCommand ??
		                                 (_selectTmCommand = new CommandHandler(SelectTm, true));
		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllTms, true));

		private void SelectAllTms()
		{
			foreach (var tm in TmsCollection)
			{
				tm.IsSelected = SelectAll;
			}
		}

		public ObservableCollection<TmFile> TmsCollection
		{
			get => _tmsCollection;

			set
			{
				if (Equals(value, _tmsCollection))
				{
					return;
				}
				_tmsCollection = value;
				OnPropertyChanged(nameof(TmsCollection));
			}
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
		private void SelectTm()
		{
			var fileDialog = new OpenFileDialog();
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				var path = fileDialog.FileName;
			}
		}

		private void Remove()
		{
		}

		private void SelectFolder()
		{
			var folderDialog = new FolderSelectDialog();
			if (folderDialog.ShowDialog())
			{
				var folderPath = folderDialog.FileName;
				var files = Directory.GetFiles(folderPath, "*.sdltm");
				foreach (var file in files)
				{
					var tm = new FileInfo(file);
					var tmFile = new TmFile
					{
						Name = tm.Name,
						Path = tm.FullName
					};
					TmsCollection.Add(tmFile);
				}
			}
		}
	}
}
