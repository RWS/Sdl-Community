namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	public class Header
	{
		public Header()
		{
			Skl = new Skl();
			PhaseGroup = new PhaseGroup();
		}

		public Skl Skl { get; set; }

		public PhaseGroup PhaseGroup { get; set; }
	}
}
