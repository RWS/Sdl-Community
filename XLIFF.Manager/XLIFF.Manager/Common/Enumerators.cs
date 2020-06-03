namespace Sdl.Community.XLIFF.Manager.Common
{
	public class Enumerators
	{
		public enum Action
		{
			None = 0,
			Export = 1,
			Import = 2
		}

		public enum Status
		{
			None = 0,
			Ready = 1,
			Success = 2,
			Error = 3,
			Warning
		}

		public enum XLIFFSupport
		{
			none = 0,
			xliff12sdl = 1,
			xliff12polyglot =2
			// TODO spport for this format will come later on in the development cycle
			//xliff20sdl = 2
		}
	}
}
