namespace Sdl.Community.Amgen.Core
{
	public class ProcessorOptions
	{
		public SourceToTargetHandler SourceToTargetCopier { get; set; }
	}

	public class SourceToTargetHandler
	{
		public bool CopySourceToTaret { get; set; }
		public bool Preserve { get; set; }
	}
}