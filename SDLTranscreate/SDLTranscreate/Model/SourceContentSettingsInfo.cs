namespace Trados.Transcreate.Model
{
	public class SourceContentSettingsInfo
	{
		public bool AllowSourceEditing { get; set; }
		public bool AllowMergeAcrossParagraphs { get; set; }
		public bool EnableIcuTokenization { get; set; }

		public SourceContentSettingsInfo()
		{
			AllowSourceEditing = false;
			AllowMergeAcrossParagraphs = false;
			EnableIcuTokenization = false;
		}
	}
}
