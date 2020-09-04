namespace Sdl.Community.Transcreate.Model
{
	public class ConvertOptions : BaseModel
	{		
		public ConvertOptions()
		{
			MaxAlternativeTranslations = 2;
			CloseProjectOnComplete = true;
		}

		public int MaxAlternativeTranslations { get; set; }	
		
		public bool CloseProjectOnComplete { get; set; }
	}
}
