namespace IATETerminologyProvider.Model.ResponseModels
{
	public class DefinitionReferencesResponseModel
	{
		public string Code { get; set; }
		public string Text { get; set; }
		public MetadataResponseModel Metadata { get; set; }
	}
}