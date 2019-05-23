using System;
using System.Drawing;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsFileVersion:BaseModel
	{
		private bool _isSelected;
		public string LanguageFileId { get; set; }
		public string FileId { get; set; }
		public string FileName { get; set; }
		public int Version { get; set; }
		public DateTime? LastModified { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedAt { get; set; }
		public string CheckInComment { get; set; }
		public string ProjectId { get; set; }
		public Image LanguageFlagImage { get; set; }
		public string LanguageName { get; set; }
		public string ProjectName { get; set; }
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
	}
}
