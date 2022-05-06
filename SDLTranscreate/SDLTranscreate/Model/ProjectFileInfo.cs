using System.Collections.Generic;

namespace Trados.Transcreate.Model
{
	public class ProjectFileInfo
	{
		public ProjectFileInfo()
		{
			Guid = string.Empty;
			SettingsBundleGuid = string.Empty;
			Name = string.Empty;
			Path = string.Empty;
			Role = string.Empty;
			FilterDefinitionId = string.Empty;
			LanguageFileInfos = new List<LanguageFileInfo>();
		}

		public string Guid { get; set; }

		public string SettingsBundleGuid { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public string Role { get; set; }

		public string FilterDefinitionId { get; set; }

		public List<LanguageFileInfo> LanguageFileInfos { get; set; }
	}
}
