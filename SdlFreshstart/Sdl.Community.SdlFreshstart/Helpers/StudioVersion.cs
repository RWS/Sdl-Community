namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class StudioVersion : IStudioVersion
	{
		public string[] AppDataLocalPaths { get; set; }
		public string[] AppDataRoamingPaths { get; set; }
		public string DocumentsPath { get; set; }
		public string[] ProgramDataPaths { get; set; }
		public string ProgramFilesPath { get; set; }
	}
}