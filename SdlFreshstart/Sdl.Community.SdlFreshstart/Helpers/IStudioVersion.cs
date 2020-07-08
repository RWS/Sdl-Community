namespace Sdl.Community.SdlFreshstart.Helpers
{
	public interface IStudioVersion
	{
		string[] AppDataLocalPaths { get; set; }
		string[] AppDataRoamingPaths { get; set; }
		string DocumentsPath { get; set; }
		string[] ProgramDataPaths { get; set; }
		string ProgramFilesPath { get; set; }
	}
}