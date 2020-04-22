using System.Collections.Generic;
using System.Xml;
using Sdl.Community.SDLBatchAnonymize.Interface;

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

		public void RemoveTemplateId(string projectPath)
		{
			var taskTemplateId = "SDL Batch Anonymizer".ToLower();
			var sdlProj = new XmlDocument();
			sdlProj.Load(projectPath);
			var tasksNodes = sdlProj.GetElementsByTagName("Tasks");
			foreach (XmlNode taskNode in tasksNodes)
			{
				foreach (XmlNode taskChild in taskNode.ChildNodes)
				{
					var templateIdsNode = taskChild.SelectSingleNode("TaskTemplateIds");
					if (templateIdsNode is null) continue;
					foreach (XmlNode templateChild in templateIdsNode.ChildNodes)
					{
						for (var i = 0; i < templateChild.ChildNodes.Count; i++)
						{
							var templateChildNode = templateIdsNode.ChildNodes[i];
							for (var j = 0; j < templateChildNode.ChildNodes.Count; j++)
							{
								var node = templateChildNode.ChildNodes[j];
								if (string.IsNullOrEmpty(node.Value)) continue;
								if (node.Value.ToLower().Equals(taskTemplateId))
								{
									node.ParentNode?.RemoveChild(node);
								}
							}
						}
					}
				}
			}
			sdlProj.Save(projectPath);
		}
	}
}
