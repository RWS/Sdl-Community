using System.Collections.Generic;
using System.Windows.Documents;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class ApiUrl
	{
		public static string BaseUrl;
		public  static List<string> Scopes= new List<string> {"ManagementRestApi", "ProjectServerRestApi", "MultiTermRestApi", "TMServerRestApi"};

		public static string Login()
		{
			return $"{BaseUrl}/authentication/api/1.0/login";
		}
	}
}
