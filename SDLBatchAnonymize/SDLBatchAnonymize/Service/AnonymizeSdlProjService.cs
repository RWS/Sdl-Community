using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Sdl.Community.SDLBatchAnonymize.Interface;
using System.Linq;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class AnonymizeSdlProjService: IAnonymizeSdlProj
	{
		public void RemoveFileVersionComment(string projectPath)
		{
			const string comment = "Created by \'SDL Batch Anonymizer\'";
			var nodesToBeRemoved = new List<XmlNode>();
			var sdlProj = new XmlDocument();
			sdlProj.Load(projectPath);
			var projectFilesNode = sdlProj.GetElementsByTagName("ProjectFiles");
			foreach (XmlNode projectFileNode in projectFilesNode)
			{
				var projectFileChilds = projectFileNode.ChildNodes;
				foreach (XmlNode projectFileChild in projectFileChilds)
				{
					var languageFileNodes = projectFileChild.SelectNodes("LanguageFiles");
					if (languageFileNodes is null) continue;
					foreach (XmlNode languageFileNode in languageFileNodes)
					{
						var languageFileNodeChilds = languageFileNode.ChildNodes;
						foreach (XmlNode languageFileChild in languageFileNodeChilds)
						{
							var languageFileVersions = languageFileChild.SelectSingleNode("FileVersions");
							if(languageFileVersions is null)continue;
							foreach (XmlNode fileVersion in languageFileVersions.ChildNodes)
							{
								var fileVersionCommentValue = fileVersion.Attributes?["Comment"]?.Value.ToLower();
								if (string.IsNullOrEmpty(fileVersionCommentValue)) continue;
								if (fileVersionCommentValue.Equals(comment.ToLower()))
								{
									nodesToBeRemoved.Add(fileVersion);
								}
							}
						}
					}
				}
			}
			foreach (var child in nodesToBeRemoved)
			{
				child.ParentNode?.RemoveChild(child);
			}
			sdlProj.Save(projectPath);
		}

		public void RemoveTraces(string projectPath)
		{
			var taskTemplateId = "SDL Batch Anonymizer";
			var rootElement = XElement.Load(projectPath);
			
			rootElement.Element("Tasks")?.Elements()
				.FirstOrDefault(el=>el.Value == taskTemplateId)
				?.Remove();

			rootElement.Element("InitialTaskTemplate")?.Elements().Elements()
				.FirstOrDefault(el => el.Attribute("TaskTemplateId")?.Value == taskTemplateId)
				?.Remove();

			rootElement.Save(projectPath);
		}
	}
}
