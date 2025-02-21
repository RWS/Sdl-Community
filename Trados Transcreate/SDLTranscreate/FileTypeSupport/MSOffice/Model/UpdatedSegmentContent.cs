using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	internal class UpdatedSegmentContent
	{

		public UpdatedSegmentContent()
		{
			TranslationTokens = new List<Token>();
			BackTranslationTokens = new List<Token>();
		}

		public List<Token> TranslationTokens
		{
			get;
			set;
		}

		public List<Token> BackTranslationTokens
		{
			get;
			set;
		}

		public bool TranslationHasTrackedChanges
		{
			get;
			set;
		}

		public bool BackTranslationHasTrackedChanges
		{
			get;
			set;
		}

	}
}
