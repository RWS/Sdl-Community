namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class PhaseGroup
	{
		public PhaseGroup()
		{
			Phase = new Phase();	
		}

		public Phase Phase { get; set; }
	}
}
