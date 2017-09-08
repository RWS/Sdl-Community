namespace Sdl.Community.NumberVerifier.Model
{
	public class SourceLanguage
	{
		public string DisplayName { get; set; }
	}

	public class CurrentProjectAdaptee
	{
		public CurrentProjectAdaptee CurrentProject { get; set; }
		public SourceLanguage SourceLanguage { get; set; }
		public string ProjectName { get; set; }

		public CurrentProjectAdaptee GetProjectInformation()
		{
			CurrentProjectAdaptee model = new CurrentProjectAdaptee();
			model.ProjectName = "Test";
			model.SourceLanguage = new SourceLanguage { DisplayName = "Hindi (India)" };
			model.CurrentProject = model;

			return model;
		}
	}
}