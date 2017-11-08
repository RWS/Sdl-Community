using Sdl.Community.XmlReader.Repository;
using Sdl.Community.XmlReader.Helpers;
using Sdl.Community.XmlReader.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sdl.Community.XmlReader
{
    public class XmlFileViewModel
    {
        private ObservableCollection<TargetLanguageCodeViewModel> _xmlFiles;

        public XmlFileViewModel(List<TargetLanguageCode> codes)
        {
            if (codes == null)
            {
                _xmlFiles = new ObservableCollection<TargetLanguageCodeViewModel>();
            } else
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
            } else
            {
                TargetLanguageCodeViewModel targetLanguageCodeViewModel = new TargetLanguageCodeViewModel(targetlanguageCode);
                _xmlFiles.Add(targetLanguageCodeViewModel);
                targetLanguageCodeViewModel.AddChild(analyzeFile);
            }
        }

        public void ResetLists()
        {
            _xmlFiles.Clear();
            XmlFilesRepository.ResetLanguageCodes();
        }
    }
}
