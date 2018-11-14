namespace IATETerminologyProvider.Helpers
{
	public class ApiUrls
	{
		public static string BaseUri(string expand, string offset, string limit)
		{
			return @"https://iate.europa.eu/em-api/entries/_search?expand=" + expand + "&offset=" + offset + "&limit=" + limit;
		}
	}
}