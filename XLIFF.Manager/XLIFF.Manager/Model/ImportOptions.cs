namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ImportOptions : BaseModel
	{
		public ImportOptions()
		{
			BackupFiles = true;
			OverwriteTranslations = true;
			OriginSystem = string.Empty;
			StatusTranslationUpdatedId = "Draft";
			StatusTranslationNotUpdatedId = string.Empty;
			StatusSegmentNotImportedId = string.Empty;
		}

	    public bool BackupFiles { get; set; }
	  
	    public bool OverwriteTranslations { get; set; }

	    public string OriginSystem { get; set; }

	    public string StatusTranslationUpdatedId { get; set; }

	    public string StatusTranslationNotUpdatedId { get; set; }

	    public string StatusSegmentNotImportedId { get; set; }
	}
}
