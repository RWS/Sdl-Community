namespace Multilingual.Excel.FileType.Models
{
	public class XmlNamespace
	{
		private readonly string _prefix;
		private readonly string _uri;

		public string Prefix
		{
			get { return _prefix; }
		}

		public string Uri
		{
			get { return _uri; }
		}

		public XmlNamespace(string prefix, string uri)
		{
			_prefix = prefix;
			_uri = uri;
		}
	}
}
