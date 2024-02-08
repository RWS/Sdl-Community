namespace GoogleCloudTranslationProvider.Models
{
	public class GoogleApiVersion
	{
		public string Name { get; set; }

		public ApiVersion Version { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}