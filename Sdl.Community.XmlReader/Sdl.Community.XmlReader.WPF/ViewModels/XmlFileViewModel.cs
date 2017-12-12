using System;
using Sdl.Community.XmlReader.WPF.Helpers;
using Sdl.Community.XmlReader.WPF.Models;
using Sdl.Community.XmlReader.WPF.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.XmlReader.WPF.Annotations;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
	public class XmlFileViewModel : INotifyPropertyChanged
	{
		private ICommand _clearCommand;
		private ICommand _generateExcelCommand;
		private string _messageVisibility;
		private bool _isGenerateEnabled;
		private bool _isClearEnabled;

		public ObservableCollection<TargetLanguageCodeViewModel> XmlFiles { get; }


		public XmlFileViewModel(List<TargetLanguageCode> codes)
		{
			MessageVisibility = "Hidden";
			IsGenerateEnabled = false;
			IsClearEnabled = false;
			if (codes == null)
			{
				XmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>();
			}
			else
			{
				XmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>(
					codes.Select(code => new TargetLanguageCodeViewModel(code, null)));
			}
		}

		public string MessageVisibility
		{
			get => _messageVisibility;
			set
			{
				if (Equals(value, _messageVisibility))
				{
					return;
					
				}
				_messageVisibility = value;
				OnPropertyChanged();
			}
		}
		public bool IsGenerateEnabled
		{
			get => _isGenerateEnabled;
			set
			{
				if (Equals(value, _isGenerateEnabled))
				{
					return;

				}
				_isGenerateEnabled = value;
				OnPropertyChanged();
			}
		}

		public bool IsClearEnabled
		{
			get => _isClearEnabled;
			set
			{
				if (Equals(value, _isClearEnabled))
				{
					return;

				}
				_isClearEnabled = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(ResetLists, true));

		public ICommand GenerateExcelCommand => _generateExcelCommand ??
		                                        (_generateExcelCommand = new CommandHandler(GenerateExcel, true));

		private async void GenerateExcel()
		{
			var folderPath = new FolderSelectDialog();
			if (folderPath.ShowDialog())
			{
				var selectedItems = XmlFiles.Where(f => f.IsSelected).ToList();
				MessageVisibility = "Visible";
				await Task.Run(()=> Report.GenerateExcelReport(folderPath.FileName, selectedItems));
				MessageVisibility = "Hidden";

			}
			
		}

	public void AddFile(string filePath)
        {
            if (Helper.GetFileName(filePath) == null)
            {
                return;
            }

            var targetlanguageCode = XmlFilesRepository.AddFile(filePath);
            var analyzeFile = targetlanguageCode.AnalyzeFiles.LastOrDefault();
            var existedTargetLanguageCodeViewModel = XmlFiles.FirstOrDefault(x => x.TargetLanguageCode.Equals(targetlanguageCode.LanguageCode));
            if (existedTargetLanguageCodeViewModel != null)
            {
                existedTargetLanguageCodeViewModel.AddChild(analyzeFile);
            }
            else
            {
                var iconUri = Helper.GetImagePathByStudioCode(Helper.GetImageStudioCodeByLanguageCode(targetlanguageCode.LanguageCode));
                var targetLanguageCodeViewModel = new TargetLanguageCodeViewModel(targetlanguageCode, iconUri);
                XmlFiles.Add(targetLanguageCodeViewModel);
                targetLanguageCodeViewModel.AddChild(analyzeFile);
            }
        }

        public void RemoveParent(TargetLanguageCodeViewModel parent)
        {
            XmlFiles.Remove(parent);
            XmlFilesRepository.DeleteParent(parent.TargetLanguageCode);
        }

        public void RemoveChildren(string languageCode, List<AnalyzeFileViewModel> analyzeFilesViewModel)
        {
            foreach (var analyzeFile in analyzeFilesViewModel)
            {
                var targetLanguageCode = XmlFiles.FirstOrDefault(x => x.TargetLanguageCode.Equals(languageCode));
                targetLanguageCode.Children.Remove(analyzeFile);

                XmlFilesRepository.RemoveFile(languageCode, analyzeFile.AnalyzeFileName);
            }
        }

        public void ResetLists()
        {
			XmlFiles.Clear();
	        XmlFilesRepository.ResetLanguageCodes();
	        IsClearEnabled = false;
	        IsGenerateEnabled = false;
        }

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
