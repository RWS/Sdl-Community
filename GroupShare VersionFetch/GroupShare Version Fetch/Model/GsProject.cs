using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Sdl.Community.GSVersionFetch.Model
{
    public class GsProject : BaseModel
    {
        private bool _isSelected;
        private string _name;
        public string CreatedBy { get; set; }

        public string DueDate { get; set; }

        public Image Image { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string ProjectId { get; set; }
        public string SourceLanguage { get; set; }
        public string Status { get; set; }
        public ObservableCollection<TargetLanguageFlag> TargetLanguageFlags { get; set; }

        public void SetFileProperties(WizardModel wizardModel, List<GsFile> projectFiles, bool selectFile)
        {
            foreach (var gsFile in projectFiles)
            {
                if (gsFile.LanguageCode != SourceLanguage)
                {
                    gsFile.IsSelected = selectFile;
                    gsFile.ProjectId = ProjectId;
                    gsFile.ProjectName = Name;
                    gsFile.LanguageFlagImage = new Language(gsFile.LanguageCode).GetFlagImage();
                    gsFile.LanguageName = new Language(gsFile.LanguageCode).DisplayName;

                    var file = wizardModel.GsFiles.FirstOrDefault(f => f.UniqueId.ToString().Equals(gsFile.UniqueId.ToString()));
                    if (file == null)
                    {
                        wizardModel.GsFiles?.Add(gsFile);
                    }
                }
            }
        }
    }
}