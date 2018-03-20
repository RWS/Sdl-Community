namespace Sdl.Community.Amgen.Core.EventArgs
{
	public class ProgressEventArgs : System.EventArgs
	{
		public int Maximum { get; set; }

		public int Current { get; set; }

		public int Percent { get; set; }

		public string Message { get; set; }
	}
}