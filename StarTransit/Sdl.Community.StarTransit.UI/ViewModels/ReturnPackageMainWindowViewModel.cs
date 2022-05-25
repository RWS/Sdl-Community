using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using Sdl.Community.StarTransit.Shared.Interfaces;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Community.StarTransit.UI.Commands;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class ReturnPackageMainWindowViewModel : BaseViewModel
	{
		private ICommand _createPackageCommand;
		private readonly ReturnFilesViewModel _returnFilesViewModel;
		private ReturnPackage _returnPackage;
		private readonly ReturnPackageService _returnService;
		private readonly CellViewModel _cellViewModel;
        private readonly IMessageBoxService _messageBoxService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private bool _active;
		
		public Action CloseAction { get; set; }

		public ReturnPackageMainWindowViewModel(ReturnFilesViewModel returnFilesViewModel, CellViewModel cellViewModel, IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
			_returnFilesViewModel = returnFilesViewModel;
			_cellViewModel = cellViewModel;
			_returnService = new ReturnPackageService();
		}

		public ICommand CreatePackageCommand => _createPackageCommand ?? (_createPackageCommand = new CommandHandler(CreatePackage, true));

		public bool Active
		{
			get => _active;
			set
			{
				if (Equals(value, _active)) { return; }
				_active = value;
			}
		}

		private async void CreatePackage()
		{
			try
			{
				_returnPackage = _returnFilesViewModel.GetReturnPackage();
				if (_returnPackage?.TargetFiles?.Count == 0)
				{
					var dialog = new MetroDialogSettings
					{
						AffirmativeButtonText = "OK"

					};
                    _messageBoxService.ShowWarningMessage("Please select at least one file.", "No files selected!");
                }
				else
				{
					Active = true;
					if (_returnPackage != null)
					{
						string returnPackageFolderPath;

						if (_returnPackage.FolderLocation == null)
						{
							var projectPath = _returnPackage.ProjectLocation?.Substring(0,
								_returnPackage.ProjectLocation.LastIndexOf(@"\", StringComparison.Ordinal));

							returnPackageFolderPath = CreateReturnPackageFolder(projectPath);
						}
						else
						{
							returnPackageFolderPath = CreateReturnPackageFolder(_returnPackage.FolderLocation);
						}

						//location of return package folder
						_returnPackage.FolderLocation = returnPackageFolderPath;

						await System.Threading.Tasks.Task.Run(() => _returnService.ExportFiles(_returnPackage));
						Active = false;
						_cellViewModel.ClearSelectedProjectsList();

						var result = _messageBoxService.ShowInformationResultMessage("The target file(s) was successfully returned", "Informative message");
						if (result == DialogResult.OK)
						{
							CloseAction();
						}
					}
					CloseAction();
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Create return  package folder in the studio project folder
		/// </summary>
		private string CreateReturnPackageFolder(string projectPath)
		{
			var returnPackageFolderPath = Path.Combine(projectPath, "Return package");
			Directory.CreateDirectory(returnPackageFolderPath);

			return returnPackageFolderPath;
		}
	}
}