namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Options
	{
		public Options()
		{
			// default == true
			ResendDraft = true;
		}

		public bool ResendDraft { get; set; }
	}
}
