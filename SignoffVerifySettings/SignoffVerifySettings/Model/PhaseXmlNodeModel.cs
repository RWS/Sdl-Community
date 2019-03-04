namespace Sdl.Community.SignoffVerifySettings.Model
{
	public class PhaseXmlNodeModel
	{
		public string TargetFileGuid { get; set; }
		public string PhaseName { get; set; }
		public int AssigneesNumber { get; set; }

		// Used to identify which phase is assigned to user(s)
		public string IsCurrentAssignment { get; set; }		
	}
}