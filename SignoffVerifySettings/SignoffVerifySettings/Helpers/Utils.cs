using System.Collections.Generic;
using System.Xml;
using Sdl.Community.SignoffVerifySettings.Model;

namespace Sdl.Community.SignoffVerifySettings.Helpers
{
	public static class Utils
	{
		/// <summary>
		/// Get phase information from .sdlproj doc, for the corresponding target file SettingsBundle, for each SettingsGroup of the phase
		/// </summary>
		/// <param name="phaseType">phase type</param>
		/// <param name="childNode">child node which represents the phase SettingsGroup tag</param>
		/// <param name="targetFile">target file</param>
		/// <param name="phaseXmlNodeModel">list of the phaseXmlNodeModels used to store the information from .sdlproj document</param>
		public static void GetPhaseInformation(
			string phaseType,
			XmlNode childNode,
			LanguageFileXmlNodeModel targetFile,
			List<PhaseXmlNodeModel> phaseXmlNodeModels)
		{
			if (childNode.Attributes["Id"].Value.Equals(phaseType))
			{
				var phaseXmlNodeModel = new PhaseXmlNodeModel();
				foreach (XmlNode node in childNode.ChildNodes)
				{
					if (node.Attributes["Id"].Value.Equals(Constants.IsCurrentAssignment))
					{
						// get info for the current assignement of the phase
						phaseXmlNodeModel.PhaseName = phaseType;
						phaseXmlNodeModel.TargetLanguageCode = targetFile.LanguageCode;
						phaseXmlNodeModel.IsCurrentAssignment = node.InnerText;
						phaseXmlNodeModel.TargetFileGuid = targetFile.LanguageFileGUID;
					}
				}

				// get the number of assignees
				var assigneesNumber = childNode.LastChild != null
					? childNode.LastChild != null
					? childNode.LastChild.LastChild != null
					? childNode.LastChild.LastChild.ChildNodes.Count > 0
					? childNode.LastChild.LastChild.ChildNodes.Count
					: 0 : 0 : 0 : 0;

				phaseXmlNodeModel.AssigneesNumber = assigneesNumber;
				phaseXmlNodeModels.Add(phaseXmlNodeModel);
			}
		}

		/// <summary>
		/// Get the "RunAt" value from the "Verify Files .xml" report or "Verify Files {sourceLanguage name-targetLanguage name}.xml" report
		/// </summary>
		/// <param name="document">report .xml document</param>
		/// <returns>RunAt value</returns>
		public static string GetRunAtValue(XmlDocument document)
		{
			var runAt = string.Empty;
			// the first element is selected, because there is only one 'task' tag defined in the report structure
			var taskInfoNode = document.GetElementsByTagName("taskInfo")[0];
			if (taskInfoNode.Attributes.Count > 0)
			{
				runAt = taskInfoNode.Attributes["runAt"].Value;
			}
			return runAt;
		}

		/// <summary>
		/// Load the .sdlproj Xml Document from the current project
		/// </summary>
		/// <param name="currentProject">current project</param>
		/// <returns>XmlDocument</returns>
		public static XmlDocument LoadXmlDocument(string filePath)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);

			return doc;
		}
	}
}