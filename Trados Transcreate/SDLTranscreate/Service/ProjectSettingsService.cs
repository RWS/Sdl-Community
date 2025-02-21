using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Service
{
	public class ProjectSettingsService
	{
		public List<LanguageDirectionInfo> GetLanguageDirections(string filePath)
		{
			var languageDirections = new List<LanguageDirectionInfo>();

			if (string.IsNullOrEmpty(filePath))
			{
				return languageDirections;
			}

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "LanguageDirections", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						foreach (var languageDirection in element.Elements())
						{
							var languageDirectionInfo = new LanguageDirectionInfo
							{
								Guid = GetSettingsBundleAttributeValue(languageDirection, "Guid"),
								SettingsBundleGuid = GetSettingsBundleAttributeValue(languageDirection, "SettingsBundleGuid"),
								TargetLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "TargetLanguageCode"),
								SourceLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "SourceLanguageCode")
							};
							languageDirections.Add(languageDirectionInfo);
						}
					}
				}
			}

			return languageDirections;
		}

		public string GetProjectId(string filePath)
		{
			var id = string.Empty;

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				foreach (var attribute in project.Attributes())
				{
					if (string.Compare(attribute.Name.LocalName, "Guid", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						id = attribute.Value;
						break;
					}
				}
			}

			return id;
		}

		public List<ProjectFileInfo> GetProjectFiles(string filePath)
		{
			var projectFilesInfo = new List<ProjectFileInfo>();

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				// recover the project files
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "ProjectFiles", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						projectFilesInfo = GetProjectFileInfo(element);
					}
				}

				// associate the file assignment information
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "SettingsBundles", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						foreach (var settingsBundle in element.Elements())
						{
							if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								if (!settingsBundle.HasAttributes)
								{
									continue;
								}

								var settingsBundleGuid = GetSettingsBundleAttributeValue(settingsBundle, "Guid");
								var languageFile = GetLanguageFileInfo(projectFilesInfo, settingsBundleGuid);
								if (languageFile == null)
								{
									continue;
								}

								AddLanguageFileAssignmentInfo(settingsBundle, languageFile);
							}
						}
					}
				}
			}

			return projectFilesInfo;
		}

		public List<ProjectUserInfo> GetProjectUsers(string filePath)
		{
			var projectUsers = new List<ProjectUserInfo>();

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				// recover the project files
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "Users", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						foreach (var projectUser in element.Elements())
						{
							if (string.Compare(projectUser.Name.LocalName, "User", StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								if (!projectUser.HasAttributes)
								{
									continue;
								}

								var projectUserInfo = new ProjectUserInfo();

								SetProjectUserInfoAttributes(projectUser, projectUserInfo);

								projectUsers.Add(projectUserInfo);
							}
						}
					}
				}
			}

			return projectUsers;
		}

		private static void AssignSourceContentSettings(XElement project, string generalSettingsBundleGuid, SourceContentSettingsInfo sourceContentSettings)
		{
			foreach (var element in project.Elements())
			{
				if (string.Compare(element.Name.LocalName, "SettingsBundles", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var settingsBundle in element.Elements())
					{
						if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var bundleGuid = GetSettingsBundleAttributeValue(settingsBundle, "Guid");

							if (string.Compare(bundleGuid, generalSettingsBundleGuid, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								SetSourceContentSettingsInfo(settingsBundle, sourceContentSettings);
							}
						}
					}
				}
			}
		}

		private static void AssignTargetSpecificAnalysisTaskSettings(XElement project, string targetSettingsBundleGuid, AnalysisTaskSettingsInfo analysisSettings)
		{
			foreach (var element in project.Elements())
			{
				if (string.Compare(element.Name.LocalName, "SettingsBundles", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var settingsBundle in element.Elements())
					{
						if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var bundleGuid = GetSettingsBundleAttributeValue(settingsBundle, "Guid");

							if (string.Compare(bundleGuid, targetSettingsBundleGuid, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								SetAnalysisTaskSettingsInfo(settingsBundle, analysisSettings);
							}
						}
					}
				}
			}
		}

		private static void AssignGeneralAnalysisTaskSettings(XElement project, string generalSettingsBundleGuid, AnalysisTaskSettingsInfo analysisSettings)
		{
			foreach (var element in project.Elements())
			{
				if (string.Compare(element.Name.LocalName, "SettingsBundles", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var settingsBundle in element.Elements())
					{
						if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var bundleGuid = GetSettingsBundleAttributeValue(settingsBundle, "Guid");

							if (string.Compare(bundleGuid, generalSettingsBundleGuid, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								SetAnalysisTaskSettingsInfo(settingsBundle, analysisSettings);
							}
						}
					}
				}
			}
		}

		private static void SetSourceContentSettingsInfo(XElement settingsBundleRoot, SourceContentSettingsInfo sourceContentSettings)
		{
			foreach (var settingsBundle in settingsBundleRoot.Elements())
			{
				if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var settingsGroup in settingsBundle.Elements())
					{
						if (string.Compare(settingsGroup.Name.LocalName, "SettingsGroup", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (!settingsGroup.HasAttributes)
							{
								continue;
							}

							var settingsBundleId = GetSettingsBundleAttributeValue(settingsGroup, "Id");
							if (settingsBundleId != "SourceContentSettings")
							{
								continue;
							}

							SetSourceContentSettingsAttributes(settingsGroup, sourceContentSettings);
						}
					}
				}
			}
		}

		private static void SetAnalysisTaskSettingsInfo(XElement settingsBundleRoot, AnalysisTaskSettingsInfo analysisSettings)
		{
			foreach (var settingsBundle in settingsBundleRoot.Elements())
			{
				if (string.Compare(settingsBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var settingsGroup in settingsBundle.Elements())
					{
						if (string.Compare(settingsGroup.Name.LocalName, "SettingsGroup", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (!settingsGroup.HasAttributes)
							{
								continue;
							}

							var settingsBundleId = GetSettingsBundleAttributeValue(settingsGroup, "Id");
							if (settingsBundleId != "AnalysisTaskSettings")
							{
								continue;
							}

							SetAnalysisTaskSettingsAttributes(settingsGroup, analysisSettings);
						}
					}
				}
			}
		}

		private static void AddLanguageFileAssignmentInfo(XElement settingsBundle, LanguageFileInfo languageFile)
		{
			foreach (var settingBundle in settingsBundle.Elements())
			{
				if (string.Compare(settingBundle.Name.LocalName, "SettingsBundle", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var fileStateInfo = GetFileStateInfo(settingBundle);
					if (fileStateInfo == null)
					{
						break;
					}

					foreach (var settingsGroup in settingBundle.Elements())
					{
						if (string.Compare(settingsGroup.Name.LocalName, "SettingsGroup", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (!settingsGroup.HasAttributes)
							{
								continue;
							}

							var fileAssignmentInfo = GetFileAssignmentInfo(settingsGroup, fileStateInfo);
							if (fileAssignmentInfo != null)
							{
								languageFile.FileAssignmentInfos.Add(fileAssignmentInfo);
							}
						}
					}
				}
			}
		}

		private static FileAssignmentInfo GetFileAssignmentInfo(XElement settingsGroup, FileStateInfo fileStateInfo)
		{
			var settingsBundleId = GetSettingsBundleAttributeValue(settingsGroup, "Id");
			if (settingsBundleId == "LanguageFileServerStateSettings")
			{
				return null;
			}

			var fileAssignmentInfo = new FileAssignmentInfo
			{
				FileStateInfo = fileStateInfo,
				Phase = GetFileAssignmentPhase(settingsBundleId)
			};

			if (string.IsNullOrEmpty(fileAssignmentInfo.Phase))
			{
				return fileAssignmentInfo;
			}

			SetFileAssignmentAttributes(settingsGroup, fileAssignmentInfo);
			return fileAssignmentInfo;
		}

		private static string GetSettingsBundleAttributeValue(XElement settingsBundle, string attributeId)
		{
			var settingsBundleGuid = string.Empty;
			foreach (var attribute in settingsBundle.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, attributeId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					settingsBundleGuid = attribute.Value;
				}
			}

			return settingsBundleGuid;
		}

		private static LanguageFileInfo GetLanguageFileInfo(IEnumerable<ProjectFileInfo> projectFilesInfo, string settingsBundleGuid)
		{
			LanguageFileInfo languageFile = null;
			foreach (var projectFileInfo in projectFilesInfo)
			{
				foreach (var langugageFileInfo in projectFileInfo.LanguageFileInfos)
				{
					if (string.Compare(langugageFileInfo.SettingsBundleGuid, settingsBundleGuid, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						languageFile = langugageFileInfo;
						break;
					}
				}

				if (languageFile != null)
				{
					break;
				}
			}

			return languageFile;
		}

		private static FileStateInfo GetFileStateInfo(XElement settingBundle)
		{
			FileStateInfo fileStateInfo = null;
			foreach (var settingsGroup in settingBundle.Elements())
			{
				if (string.Compare(settingsGroup.Name.LocalName, "SettingsGroup", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!settingsGroup.HasAttributes)
					{
						return fileStateInfo;
					}

					var settingsBundleId = GetSettingsBundleAttributeValue(settingsGroup, "Id");
					if (settingsBundleId == "LanguageFileServerStateSettings")
					{
						fileStateInfo = new FileStateInfo();
						SetFileStateInfoAttributes(settingsGroup, fileStateInfo);
					}
				}
			}

			return fileStateInfo;
		}

		private static string GetFileAssignmentPhase(string settingsId)
		{
			var phase = string.Empty;
			switch (settingsId)
			{
				case "LanguageFileServerAssignmentsSettings_Preparation":
					{
						phase = "Preparation";
					}
					break;
				case "LanguageFileServerAssignmentsSettings_Translation":
					{
						phase = "Translation";
					}
					break;
				case "LanguageFileServerAssignmentsSettings_Review":
					{
						phase = "Review";
					}
					break;
				case "LanguageFileServerAssignmentsSettings_Finalisation":
					{
						phase = "Finalisation";
					}
					break;
			}

			return phase;
		}

		private static List<ProjectFileInfo> GetProjectFileInfo(XElement projectFiles)
		{
			var projectFileInfos = new List<ProjectFileInfo>();

			foreach (var projectFile in projectFiles.Elements())
			{
				if (string.Compare(projectFile.Name.LocalName, "ProjectFile", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!projectFile.HasAttributes)
					{
						continue;
					}

					var projectFileInfo = new ProjectFileInfo();

					SetProjectFileAttributes(projectFile, projectFileInfo);

					foreach (var languageFiles in projectFile.Elements())
					{
						if (string.Compare(languageFiles.Name.LocalName, "LanguageFiles", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							foreach (var languageFile in languageFiles.Elements())
							{
								if (string.Compare(languageFile.Name.LocalName, "LanguageFile", StringComparison.InvariantCultureIgnoreCase) == 0)
								{
									if (!languageFile.HasAttributes)
									{
										continue;
									}

									var languageFileInfo = GetLanguageFileInfo(languageFile);

									projectFileInfo.LanguageFileInfos.Add(languageFileInfo);
								}
							}
						}
					}

					projectFileInfos.Add(projectFileInfo);
				}
			}

			return projectFileInfos;
		}

		private static LanguageFileInfo GetLanguageFileInfo(XElement languageFile)
		{
			var languageFiles = new LanguageFileInfo();

			if (languageFile.HasAttributes)
			{
				SetLanguageFileAttributes(languageFile, languageFiles);
			}

			foreach (var fileVersions in languageFile.Elements())
			{
				if (string.Compare(fileVersions.Name.LocalName, "FileVersions", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var fileVersion in fileVersions.Elements())
					{
						if (string.Compare(fileVersion.Name.LocalName, "fileVersion", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							if (!fileVersion.HasAttributes)
							{
								continue;
							}

							var fileVersionInfo = GetFileVersionInfo(fileVersion);

							languageFiles.FileVersionInfos.Add(fileVersionInfo);
						}
					}
				}
			}

			return languageFiles;
		}

		private static FileVersionInfo GetFileVersionInfo(XElement fileVersion)
		{
			var fileVersionInfo = new FileVersionInfo();

			SetFileVersionAttributes(fileVersion, fileVersionInfo);

			return fileVersionInfo;
		}

		private static void SetProjectFileAttributes(XElement projectFile, ProjectFileInfo projectFileInfo)
		{
			foreach (var attribute in projectFile.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, "Guid", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.Guid = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "SettingsBundleGuid", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.SettingsBundleGuid = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "Name", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.Name = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "Path", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.Path = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "Role", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.Role = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "FilterDefinitionId", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectFileInfo.FilterDefinitionId = attribute.Value;
				}
			}
		}

		private static void SetLanguageFileAttributes(XElement languageFile, LanguageFileInfo languageFileInfo)
		{
			foreach (var attribute in languageFile.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, "Guid", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					languageFileInfo.Guid = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "SettingsBundleGuid", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					languageFileInfo.SettingsBundleGuid = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "LanguageCode", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					languageFileInfo.LanguageCode = attribute.Value;
				}
			}
		}

		private static void SetFileVersionAttributes(XElement fileVersion, FileVersionInfo fileVersionInfo)
		{
			foreach (var attribute in fileVersion.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, "Guid", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.Guid = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "VersionNumber", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.VersionNumber = Convert.ToInt32(attribute.Value);
				}

				if (string.Compare(attribute.Name.LocalName, "Size", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.Size = Convert.ToInt32(attribute.Value);
				}

				if (string.Compare(attribute.Name.LocalName, "FileName", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.FileName = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "PhysicalPath", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.PhysicalPath = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "CreatedBy", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					fileVersionInfo.CreatedBy = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "CreatedAt", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					DateTime.TryParse(attribute.Value, new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out var createdAt);
					fileVersionInfo.CreatedAt = createdAt;
				}

				if (string.Compare(attribute.Name.LocalName, "FileTimeStamp", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					DateTime.TryParse(attribute.Value, new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out var fileTimeStamp);
					fileVersionInfo.FileTimeStamp = fileTimeStamp;
				}

				if (string.Compare(attribute.Name.LocalName, "IsAutoUpload", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					bool.TryParse(attribute.Value, out var isAutoUpload);
					fileVersionInfo.IsAutoUpload = isAutoUpload;
				}
			}
		}

		private static void SetFileAssignmentAttributes(XElement settingsGroup, FileAssignmentInfo fileAssignmentInfo)
		{
			foreach (var setting in settingsGroup.Elements())
			{
				if (string.Compare(setting.Name.LocalName, "Setting", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!setting.HasAttributes)
					{
						continue;
					}

					foreach (var attribute in setting.Attributes())
					{
						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "AssignedAt", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							DateTime.TryParse(setting.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeUniversal, out var assignedAt);
							fileAssignmentInfo.AssignedAt = assignedAt;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "DueDate", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							DateTime.TryParse(setting.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeUniversal, out var dueDate);
							fileAssignmentInfo.DueDate = dueDate;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "AssignedBy", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							fileAssignmentInfo.AssignedBy = setting.Value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "IsCurrentAssignment", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var isCurrentAssignment);
							fileAssignmentInfo.IsCurrentAssignment = isCurrentAssignment;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "Assignees", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var regexAssigned = new Regex(@"<string>(?<assigned>.*?)<\/string>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
							var matches = regexAssigned.Matches(setting.ToString());
							foreach (Match match in matches)
							{
								var assigned = match.Groups["assigned"].Value.Trim();
								if (!fileAssignmentInfo.Assignees.Contains(assigned))
								{
									fileAssignmentInfo.Assignees.Add(assigned);
								}
							}
						}
					}
				}
			}
		}

		private static void SetFileStateInfoAttributes(XElement settingsGroup, FileStateInfo fileStateInfo)
		{
			foreach (var setting in settingsGroup.Elements())
			{
				if (string.Compare(setting.Name.LocalName, "Setting", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!setting.HasAttributes)
					{
						continue;
					}

					foreach (var attribute in setting.Attributes())
					{
						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "LatestServerVersionTimestamp",
								StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							fileStateInfo.LatestServerVersionTimestamp = setting.Value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "LatestServerVersionNumber", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							fileStateInfo.LatestServerVersionNumber = Convert.ToInt32(setting.Value);
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "CheckedOutTo", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							fileStateInfo.CheckedOutTo = setting.Value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "IsCheckedOutOnline", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var isCheckedOutOnline);
							fileStateInfo.IsCheckedOutOnline = isCheckedOutOnline;
						}
					}
				}
			}
		}

		private static void SetProjectUserInfoAttributes(XElement projectUser, ProjectUserInfo projectUserInfo)
		{
			foreach (var attribute in projectUser.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, "UserId", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.UserId = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "FullName", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.FullName = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "Description", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.Description = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "Email", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.Email = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "PhoneNumber", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.PhoneNumber = attribute.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "EmailType", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					projectUserInfo.EmailType = attribute.Value;
				}
			}
		}

		private static void SetSourceContentSettingsAttributes(XElement settingsGroup, SourceContentSettingsInfo sourceContentSettings)
		{
			foreach (var setting in settingsGroup.Elements())
			{
				if (string.Compare(setting.Name.LocalName, "Setting", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!setting.HasAttributes)
					{
						continue;
					}

					foreach (var attribute in setting.Attributes())
					{
						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "AllowSourceEditing", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							sourceContentSettings.AllowSourceEditing = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "AllowMergeAcrossParagraphs", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							sourceContentSettings.AllowMergeAcrossParagraphs = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "EnableIcuTokenization", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							sourceContentSettings.EnableIcuTokenization = value;
						}
					}
				}
			}
		}

		private static void SetAnalysisTaskSettingsAttributes(XElement settingsGroup, AnalysisTaskSettingsInfo analysisSettings)
		{
			foreach (var setting in settingsGroup.Elements())
			{
				if (string.Compare(setting.Name.LocalName, "Setting", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					if (!setting.HasAttributes)
					{
						continue;
					}

					foreach (var attribute in setting.Attributes())
					{
						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ReportCrossFileRepetitions", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ReportCrossFileRepetitions = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ReportInternalFuzzyMatchLeverage", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ReportInternalFuzzyMatchLeverage = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ReportLockedSegmentsSeparately", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ReportLockedSegmentsSeparately = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ExcludeLockedSegments", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ExcludeLockedSegments = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ExportFrequentSegments", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ExportFrequentSegments = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "ExportUnknownSegments", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							bool.TryParse(setting.Value, out var value);
							analysisSettings.ExportUnknownSegments = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "UnknownSegmentsMaximumMatchValue", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							int.TryParse(setting.Value, out var value);
							analysisSettings.UnknownSegmentsMaximumMatchValue = value;
						}

						if (string.Compare(attribute.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase) == 0
							&& string.Compare(attribute.Value, "FrequentSegmentsNoOfOccurrences", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							int.TryParse(setting.Value, out var value);
							analysisSettings.FrequentSegmentsNoOfOccurrences = value;
						}
					}
				}
			}
		}
	}
}
