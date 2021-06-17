using System.Collections.Generic;
using System.Linq;

namespace Trados.Transcreate.Model
{
	public class ImportOptions : BaseModel
	{
		private List<string> _excludeFilterIds;

		public ImportOptions()
		{
			BackupFiles = true;
			OverwriteTranslations = true;
			OriginSystem = string.Empty;
			StatusTranslationUpdatedId = "Draft";
			StatusTranslationNotUpdatedId = string.Empty;
			StatusSegmentNotImportedId = string.Empty;
			ExcludeFilterIds = new List<string>();
			//ExcludeFilterIds.Add("Locked");
		}

		public bool BackupFiles { get; set; }

		public bool OverwriteTranslations { get; set; }

		public string OriginSystem { get; set; }

		public string StatusTranslationUpdatedId { get; set; }

		public string StatusTranslationNotUpdatedId { get; set; }

		public string StatusSegmentNotImportedId { get; set; }

		public List<string> ExcludeFilterIds
		{
			get => _excludeFilterIds;
			set
			{
				_excludeFilterIds = value?.Distinct().ToList();
				OnPropertyChanged(nameof(ExcludeFilterIds));
			}
		}
	}
}
