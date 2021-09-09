namespace Sdl.Community.IATETerminologyProvider.Model.ResponseModels
{
	public class IateName
	{
		public string InstitutionCode{ get; set; }
		public string InstitutionName{ get; set; }
		public string ShortName{ get; set; }
		public string Year{ get; set; }
		public string FullName{ get; set; }
		public IateLanguage Language { get; set; }
	}
}