using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using DataFormats = System.Windows.Forms.DataFormats;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel:ViewModelBase
	{
		private bool _selectAll;
		private ICommand _selectFoldersCommand;
		private ICommand _removeCommand;
		private ICommand _selectTmCommand;
		private ICommand _selectAllCommand;
		private ICommand _dragEnterCommand;
		private ObservableCollection<TmFile> _tmsCollection = new ObservableCollection<TmFile>();

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
		public ICommand SelectFoldersCommand => _selectFoldersCommand ??
		                                        (_selectFoldersCommand = new CommandHandler(SelectFolder, true));
		public ICommand RemoveCommand => _removeCommand ??
		                                        (_removeCommand = new CommandHandler(Remove, true));
		public ICommand SelectTmCommand => _selectTmCommand ??
		                                 (_selectTmCommand = new CommandHandler(SelectTm, true));
		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllTms, true));
		public ICommand DragEnterCommand => _dragEnterCommand ??
		                                    (_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		private void HandlePreviewDrop(object dropedFile)
		{
			var file = dropedFile as System.Windows.DataObject;
			if (null == file) return;
			var tmsPath = (string[]) file.GetData(DataFormats.FileDrop);
			if (tmsPath != null)
			{
				foreach (var tm in tmsPath)
				{
					if (!string.IsNullOrEmpty(tm))
					{
						if (Path.GetExtension(tm).Equals(".sdltm"))
						{
							AddTm(tm);
						}
					}
				}
			}
		}
		
		private void SelectTm()
		{
			var fileDialog = new OpenFileDialog();
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				var tmFilePath = fileDialog.FileName;
				if (!string.IsNullOrEmpty(tmFilePath))
				{
					if (Path.GetExtension(tmFilePath).Equals(".sdltm"))
					{
						AddTm(tmFilePath);
					}
				}
			}
		}

		private void Remove()
		{
			var result = MessageBox.Show(@"Do you want to remove selected tms?", @"Confirmation",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
			if (result == DialogResult.OK)
			{
				var selectedTms = TmsCollection.Where(t => t.IsSelected).ToList();
				foreach (var tm in selectedTms)
				{
					TmsCollection.Remove(tm);
				}
			}
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
					AddTm(file);
				}
			}
		}
		private bool TmAlreadyExist(string tmPath)
		{
			return TmsCollection.Any(t => t.Path.Equals(tmPath));
		}
		private void SelectAllTms()
		{
			foreach (var tm in TmsCollection)
			{
				tm.IsSelected = SelectAll;
			}
		}

		private void AddTm(string tmPath)
		{
			if (!TmAlreadyExist(tmPath))
			{
				var tmFileInfo = new FileInfo(tmPath);
				var tmFile = new TmFile
				{
					Name = tmFileInfo.Name,
					Path = tmFileInfo.FullName
				};
				TmsCollection.Add(tmFile);
			}
		}
		
	}
}
