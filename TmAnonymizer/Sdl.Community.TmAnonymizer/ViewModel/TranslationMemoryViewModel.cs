using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Studio;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
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
		private ICommand _loadServerTmCommand;
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

		public ICommand LoadServerTmCommand => _loadServerTmCommand ??
		                                       (_loadServerTmCommand = new CommandHandler(ShowLogInWindow, true));

		private void ShowLogInWindow()
		{
			var loginWindow = new LoginWindow();
			var viewModel = new LoginWindowViewModel(loginWindow,TmsCollection);
			loginWindow.DataContext = viewModel;
			//if we don't set element host we are not able to type in text box
			ElementHost.EnableModelessKeyboardInterop(loginWindow);
			loginWindow.Show();
			Dispatcher.Run();
		}

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

			//merge
				//var tm = translationMemories[1];
				//var languageDirections = tm.LanguageDirections;
				//foreach (var languageDirection in languageDirections)
				//{
				//	var tmIterator = new RegularIterator();
				//	var tus = languageDirection.GetTranslationUnits(ref tmIterator);

				//	foreach (var tu in tus)
				//	{
				//		var sourceTranslationUnits = tu.SourceSegment.Elements.ToList();
				//		foreach (var sourceTu in sourceTranslationUnits)
				//		{
				//			var visitor = new SegmentElementVisitor();
				//			sourceTu.AcceptSegmentElementVisitor(visitor);
				//			var segmentColection = visitor.SegmentColection;
				//		}

				//	}
				//}


				//var languageDirections = tm.LanguageDirections;
				//foreach (var languageDirection in languageDirections)
				//{
				//	var tmIterator = new RegularIterator();
				//	var tu = languageDirection.GetTranslationUnits(ref tmIterator);
				//}
				//asa se seteaza pathul tmului
				//	var tmPath = tm.ParentResourceGroupPath == "/" ? "" : tm.ParentResourceGroupPath;
				//	var path = tmPath + "/" + tm.Name;
				//	var test = translationProviderServer.GetTranslationMemory(path, TranslationMemoryProperties.All);

				//}
			}

			private void Remove()
		{
			var result = MessageBox.Show(@"Do you want to remove selected tms?", @"Confirmation",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
			if (result == DialogResult.OK)
			{
				var selectedTms = TmsCollection.Where(t => t.ShouldRemove).ToList();
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
					Path = tmFileInfo.FullName,
					IsSelected = true,
					IsServerTm = false
				};
				TmsCollection.Add(tmFile);
			}
		}
		
	}
}
