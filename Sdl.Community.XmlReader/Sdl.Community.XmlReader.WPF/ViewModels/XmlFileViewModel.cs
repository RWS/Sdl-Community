using Sdl.Community.XmlReader.WPF.Helpers;
using Sdl.Community.XmlReader.WPF.Models;
using Sdl.Community.XmlReader.WPF.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
    public class XmlFileViewModel
    {
	    public XmlFileViewModel(List<TargetLanguageCode> codes)
        {
            if (codes == null)
            {
                XmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>();
            }
            else
            {
                XmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>(codes.Select(code => new TargetLanguageCodeViewModel(code, null)));
            }
        }

        public ObservableCollection<TargetLanguageCodeViewModel> XmlFiles { get; }

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
        }
    }
}
