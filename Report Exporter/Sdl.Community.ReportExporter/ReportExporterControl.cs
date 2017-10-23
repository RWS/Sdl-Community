using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.ReportExporter.Helpers;
using Sdl.Community.ReportExporter.Model;
using Sdl.Desktop.IntegrationApi;
using static System.String;
using Help = Sdl.Community.ReportExporter.Helpers.Help;

namespace Sdl.Community.ReportExporter
{
	public partial class ReportExporterControl : UserControl, ISettingsAware<ReportExporterSettings>
	{
		private readonly string _projectXmlPath;

		public ReportExporterControl()
		{
			InitializeComponent();
			_projectXmlPath = Help.GetStudioProjectsPath();
			LoadProjectsList();
		}

		/// <summary>
		/// Reads studio projects from project.xml
		/// Adds projects to listbox
		/// </summary>
		private void LoadProjectsList()
		{
			var projectXmlDocument = new XmlDocument();

			projectXmlDocument.Load(_projectXmlPath);
			
			var projectsNodeList = projectXmlDocument.SelectNodes("//ProjectListItem");
			if (projectsNodeList == null) return;
			foreach (var item in projectsNodeList)
			{
				var projectInfo = ((XmlNode)item).SelectSingleNode("./ProjectInfo");
				if (projectInfo?.Attributes != null && projectInfo.Attributes["IsInPlace"].Value != "true")
				{
					projListbox.Items.Add(CreateProjectDetails((XmlNode)item));
				}
			}
		}

		/// <summary>
		/// Creates project details for given project from xml file
		/// </summary>
		/// <param name="projNode"></param>
		/// <returns></returns>
		private ProjectDetails CreateProjectDetails(XmlNode projNode)
		{
			var projectDetails = new ProjectDetails();
			var projectFolderPath = Empty;

			var selectSingleNode = projNode.SelectSingleNode("ProjectInfo");
			if (selectSingleNode?.Attributes != null)
			{
				projectDetails.ProjectName = selectSingleNode.Attributes["Name"].Value;
			}
			if (projNode.Attributes != null)
			{
				projectFolderPath = projNode.Attributes["ProjectFilePath"].Value;
			}
			if (Path.IsPathRooted(projectFolderPath))
			{
				projectDetails.ProjectPath = projectFolderPath; //location outside standard project place
			}
			else
			{
				var projectsFolderPath = _projectXmlPath.Substring
					(0, _projectXmlPath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
				projectDetails.ProjectPath = projectsFolderPath + projectFolderPath;
			}
			return projectDetails;
		}

		public ReportExporterSettings Settings { get; set; }

		private void browseBtn_Click(object sender, EventArgs e)
		{
			var folderDialog = new FolderSelectDialog();
			if (folderDialog.ShowDialog())
			{
				outputPathField.Text = folderDialog.FileName;
			}
		}

		private void projListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Settings.Test = new List<string>{"From control"};
			if (projListbox.SelectedItem == null) return;

			languagesListBox.Items.Clear();

			var selectedProject = projListbox.SelectedItem as ProjectDetails;
			var doc = new XmlDocument();
			if (selectedProject != null)
			{
				doc.Load(selectedProject.ProjectPath);

				var languages =Help.LoadLanguageDirections(doc);
				Help.LoadReports(doc, selectedProject.ProjectFolderPath, languages);

				//add resutls to the list view
				foreach (var item in languages.Values)
				{
					if (!IsNullOrEmpty(item.PathToReport))
					{
						languagesListBox.Items.Add(item);
					}
					else
					{
						languagesListBox.Items.Add("No report");
					}
				}

				outputPathField.Text = selectedProject.ProjectFolderPath + @"\Reports";
			}
			
		}
	}
}
