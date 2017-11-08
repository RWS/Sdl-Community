using Sdl.Community.XmlReader.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.XmlReader
{
    public class XmlFileViewModel
    {
        private List<TargetLanguageCodeViewModel> _xmlFiles;

        public XmlFileViewModel(List<TargetLanguageCode> codes)
        {
            _xmlFiles = new List<TargetLanguageCodeViewModel>(codes.Select(code => new TargetLanguageCodeViewModel(code)));
        }

        public List<TargetLanguageCodeViewModel> XmlFiles {  get { return _xmlFiles; } }
    }
}
