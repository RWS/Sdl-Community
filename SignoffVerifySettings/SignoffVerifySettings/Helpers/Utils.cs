using System.Collections.Generic;
using System.Xml;
using Sdl.Community.SignoffVerifySettings.Model;
using Sdl.ProjectAutomation.FileBased;

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
		/// <param name="phaseXmlNodeModels">list of the phaseXmlNodeModels used to store the information from .sdlproj document</param>
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
		/// Load the .sdlproj Xml Document from the current project
		/// </summary>
		/// <param name="currentProject">current project</param>
		/// <returns>XmlDocument</returns>
		public static XmlDocument LoadXmlDocument(FileBasedProject currentProject)
		{
			var doc = new XmlDocument();
			doc.Load(currentProject.FilePath);

			return doc;
		}
	}
}