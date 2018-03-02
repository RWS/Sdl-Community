using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.ItemTemplates;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class ImportScriptPageViewModel:ViewModelBase
    {
	    private static  MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
	    private ICommand _dragEnterCommand;
	    private ICommand _removeFileCommand;

		private ObservableCollection<ImportScriptItemTemplate> _filesNameCollection = new ObservableCollection<ImportScriptItemTemplate>();
		public ImportScriptPageViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public ImportScriptPageViewModel()
	    {
		    
	    }
	    public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

	    public ICommand RemoveFileCommand => _removeFileCommand ?? (_removeFileCommand = new RelayCommand(RemoveFile));

	    private void RemoveFile(object file)
	    {
		    if (file != null)
		    {
			    var filePath = (string) file;
			    var fileToRemove = FilesNameCollection.FirstOrDefault(f => f.FilePath.Equals(filePath));
			    if (fileToRemove != null)
			    {
				    FilesNameCollection.Remove(fileToRemove);
			    }
		    }
	    }

		private void HandlePreviewDrop(object dropedFile)
	    {
			var file = dropedFile as IDataObject;
		    if (null == file) return;
		    var documentsPath = (string[])file.GetData(DataFormats.FileDrop);
		    var defaultFormat = DataFormats.Text;

		    if (documentsPath != null)
		    {
				foreach (var path in documentsPath)
				{
					var pathAlreadyAdded = FilesNameCollection.Any(p => p.FilePath.Equals(path));
					if (!pathAlreadyAdded)
					{
						var newFile = new ImportScriptItemTemplate
						{
							Content =Path.GetFileNameWithoutExtension(path),
							RemoveFileCommand = new RelayCommand(RemoveFile),
							FilePath = path
						};
						FilesNameCollection.Add(newFile);
					}
				}
			}
		    // var test = File.ReadAllText(docPath[0]);
		}
		private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
	    public ObservableCollection<ImportScriptItemTemplate> FilesNameCollection
		{
		    get => _filesNameCollection;

		    set
		    {
			    if (Equals(value, _filesNameCollection))
			    {
				    return;
			    }
			    _filesNameCollection = value;
			    OnPropertyChanged(nameof(FilesNameCollection));
		    }
	    }
	}
}
