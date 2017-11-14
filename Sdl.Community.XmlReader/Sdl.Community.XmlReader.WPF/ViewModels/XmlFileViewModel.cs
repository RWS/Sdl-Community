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
        private ObservableCollection<TargetLanguageCodeViewModel> _xmlFiles;

        public XmlFileViewModel(List<TargetLanguageCode> codes)
        {
            if (codes == null)
            {
                _xmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>();
            }
            else
            {
                _xmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>(codes.Select(code => new TargetLanguageCodeViewModel(code)));
            }
        }

        public ObservableCollection<TargetLanguageCodeViewModel> XmlFiles
        {
            get { return _xmlFiles; }
        }

        public void AddFile(string filePath)
        {
            if (Helper.GetFileName(filePath) == null)
            {
                return;
            }

            var targetlanguageCode = XmlFilesRepository.AddFile(filePath);
            var analyzeFile = targetlanguageCode.AnalyzeFiles.LastOrDefault();
            var existedTargetLanguageCodeViewModel = _xmlFiles.FirstOrDefault(x => x.TargetLanguageCode.Equals(targetlanguageCode.LanguageCode));
            if (existedTargetLanguageCodeViewModel != null)
            {
                existedTargetLanguageCodeViewModel.AddChild(analyzeFile);
            }
            else
            {
                TargetLanguageCodeViewModel targetLanguageCodeViewModel = new TargetLanguageCodeViewModel(targetlanguageCode);
                _xmlFiles.Add(targetLanguageCodeViewModel);
                targetLanguageCodeViewModel.AddChild(analyzeFile);
            }
        }

        public void RemoveParent(TargetLanguageCodeViewModel parent)
        {
            _xmlFiles.Remove(parent);
            XmlFilesRepository.DeleteParent(parent.TargetLanguageCode);
        }

        public void RemoveChildren(string languageCode, List<AnalyzeFileViewModel> analyzeFilesViewModel)
        {
            foreach (var analyzeFile in analyzeFilesViewModel)
            {
                var targetLanguageCode = _xmlFiles.FirstOrDefault(x => x.TargetLanguageCode.Equals(languageCode));
                targetLanguageCode.Children.Remove(analyzeFile);

                XmlFilesRepository.RemoveFile(languageCode, analyzeFile.AnalyzeFileName);
            }
        }

        public void ResetLists()
        {
            _xmlFiles.Clear();
            XmlFilesRepository.ResetLanguageCodes();
        }
    }
}
