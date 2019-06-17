using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsFile : BaseModel
	{
		private bool _isSelected;
		public Guid UniqueId { get; set; }
		public string FileName { get; set; }
		public int FileSize { get; set; }
		public string Status { get; set; }
		public string LanguageCode { get; set; }
		public Image LanguageFlagImage { get; set; }
		public string FileType { get; set; }
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string LanguageName { get; set; }
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public void SetFileProperties(WizardModel wizardModel,List<GsFileVersion> fileVersions)
		{
			foreach (var fileVersion in fileVersions)
			{
				fileVersion.ProjectName = ProjectName;
				fileVersion.LanguageFlagImage = LanguageFlagImage;
				fileVersion.LanguageName = LanguageName;
				fileVersion.LanguageCode = LanguageCode;
				fileVersion.ProjectId = ProjectId;
				fileVersion.OriginalFileId = UniqueId;
				wizardModel?.FileVersions?.Add(fileVersion);
			}
		}
	}
}
