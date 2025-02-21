namespace Trados.Transcreate.Model
{
	public class FileStateInfo
	{
		public FileStateInfo()
		{
			LatestServerVersionTimestamp = string.Empty;
			LatestServerVersionNumber = 0;
			CheckedOutTo = string.Empty;
			IsCheckedOutOnline = false;
		}

		public string LatestServerVersionTimestamp { get; set; }

		public int LatestServerVersionNumber { get; set; }

		public string CheckedOutTo { get; set; }

		public bool IsCheckedOutOnline { get; set; }
	}
}
