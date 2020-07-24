namespace Sdl.Community.Transcreate.Model
{
	public class ConvertOptions : BaseModel
	{		
		public ConvertOptions()
		{
			MaxAlternativeTranslations = 2;
		}

		public int MaxAlternativeTranslations { get; set; }		
	}
}
