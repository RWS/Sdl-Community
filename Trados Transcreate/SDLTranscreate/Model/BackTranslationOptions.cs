namespace Trados.Transcreate.Model
{
	public class BackTranslationOptions : BaseModel
	{
		public BackTranslationOptions()
		{
			CopySourceToTargetForEmptyTranslations = true;
			OverwriteExistingBackTranslations = true;
		}

	    public bool CopySourceToTargetForEmptyTranslations { get; set; }
	    
	    public bool OverwriteExistingBackTranslations { get; set; }
	}
}
