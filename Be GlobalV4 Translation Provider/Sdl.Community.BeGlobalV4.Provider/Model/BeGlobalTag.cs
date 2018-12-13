using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class BeGlobalTag
	{
		public BeGlobalTag(Tag tag)
		{
			SdlTag = tag;
			PadLeft = string.Empty;
			PadRight = string.Empty;
		}

		public  string PadLeft { get; set; }		
		public  string PadRight { get; set; }		
		public Tag SdlTag { get; }
	}
}
