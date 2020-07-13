namespace Sdl.Community.SdlFreshstart.Model
{
	public interface IStudioVersion
	{
		string PublicVersion { get; set; }
		string AppDataLocalPluginsPath { get; set; }
		string AppDataLocalStudioPath { get; set; }
		string AppDataRoamingPluginsPath { get; set; }
		string AppDataRoamingStudioPath { get; set; }
		string DocumentsPath { get; set; }
		string ProgramDataPluginsPath { get; set; }
		string ProgramDataStudioPath { get; set; }
		string ProgramDataStudioDataSubfolderPath { get; set; }
		string ProjectTemplatesPath { get; set; }
		string ProjectApiPath { get; set; }
	}
}