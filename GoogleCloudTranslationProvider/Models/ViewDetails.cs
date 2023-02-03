namespace GoogleCloudTranslationProvider.Models
{
	public class ViewDetails : BaseModel
	{
		public string Name { get; set; }

		public BaseModel ViewModel { get; set; }
	}
}