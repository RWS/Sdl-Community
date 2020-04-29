using System.Collections.Generic;
using System.IO;
using System.Xml;
using SdlCommunityUnitTest.Model;

namespace SdlCommunityUnitTest.Services
{
	public class PublicApiVersion
	{
		public XmlDocument LoadConfigFile(string studioFolderPath)
		{
			var configFilePath = Path.Combine(studioFolderPath, "pluginconfig.xml");
			var pluginConfigFile = new XmlDocument();
			pluginConfigFile.Load(configFilePath);
			return pluginConfigFile;
		}

		public XmlNodeList GetPublicAssembliesNodeList(XmlDocument pluginConfigFile)
		{
			var fileNodes = pluginConfigFile.DocumentElement?.ChildNodes;
			if (fileNodes != null)
			{
				foreach (XmlNode fileNode in fileNodes)
				{
					if (fileNode.Name.Equals("PublicAssemblies"))
					{
						return fileNode.ChildNodes;
					}
				}
			}
			return null;
		}

		public List<ApiDetails> GetPublicApiDetails(XmlNodeList publicAssembliesNodeList)
		{
			var publicAssembliesDetails = new List<ApiDetails>();
			foreach (XmlNode node in publicAssembliesNodeList)
			{
				if (node?.Attributes?.Count > 0)
				{
					var apiDetails = new ApiDetails();
					foreach (XmlAttribute attribute in node.Attributes)
					{
						if (attribute != null)
						{
							if (attribute.Name.ToLower().Equals("name"))
							{
								apiDetails.ApiName = $"{attribute.Value}.dll";
							}
							if (attribute.Name.ToLower().Equals("version"))
							{
								apiDetails.Version = attribute.Value;
							}
						}
					}
					publicAssembliesDetails.Add(apiDetails);
				}
			}
			return publicAssembliesDetails;
		}
	}
}
