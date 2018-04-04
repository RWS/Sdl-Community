using Sdl.Community.Toolkit.FileType;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class DataAnonymizer 
	{
		private AnonymizerSettings _settings;

		public DataAnonymizer()
		{
		}

		public DataAnonymizer(AnonymizerSettings settings)
		{
			_settings = settings;
		}

		public DataAnonymizer Settings { get; set; }

		public void Process(ISegmentPair segmentPair)
		{
			var test = segmentPair.Source.GetString();

			segmentPair.Target.Replace(0,5,"test");
		}
	}
}
