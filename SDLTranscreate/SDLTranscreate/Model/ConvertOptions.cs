namespace Trados.Transcreate.Model
{
	public class ConvertOptions : BaseModel
	{		
		public ConvertOptions()
		{
			MaxAlternativeTranslations = 3;
			CloseProjectOnComplete = true;
		}

		public int MaxAlternativeTranslations { get; set; }	
		
		public bool CloseProjectOnComplete { get; set; }
	}
}
