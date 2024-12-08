using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ProjectFileService
	{
		public ISegmentPair GetSegmentPair(IStudioDocument document, string paragraphUnitId, string segmentId)
		{
			foreach (var segmentPair in document.SegmentPairs)
			{
				if (paragraphUnitId == segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id
					&& segmentId == segmentPair.Properties.Id.Id)
				{
					return segmentPair;
				}
			}

			return null;
		}

		public IParagraphUnit GetParagraphUnit(IStudioDocument document, string paragraphUnitId)
		{
			foreach (var segmentPair in document.SegmentPairs)
			{
				if (paragraphUnitId == segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id)
				{
					return document.GetParentParagraphUnit(segmentPair);
				}
			}

			return null;
		}

		public IParagraphUnit GetParagraphUnit(ISegmentPair segmentPair)
		{
			var type = segmentPair.GetType();
			var memberInfo = type.GetMember("_segmentPair", BindingFlags.NonPublic | BindingFlags.Instance);
			if (memberInfo.Length > 0)
			{
				var fieldInfo = memberInfo[0] as FieldInfo;
				if (fieldInfo != null)
				{
					var iSegmentPair = fieldInfo.GetValue(segmentPair) as ISegmentPair;
					return iSegmentPair?.Source.ParentParagraphUnit;
				}
			}

			return null;
		}

		public List<ISegmentPair> GetSegmentPairs(IStudioDocument document, bool exportSelectedSegments)
		{
			List<ISegmentPair> segmentPairs;
			if (exportSelectedSegments)
			{
				segmentPairs = document.GetSelectedSegmentPairs().ToList();
				if (segmentPairs.Count == 0)
				{
					var activeSegmentPair = document.GetActiveSegmentPair();
					if (activeSegmentPair != null)
					{
						segmentPairs = new List<ISegmentPair> { document.GetActiveSegmentPair() };
					}
				}
			}
			else
			{
				segmentPairs = document.FilteredSegmentPairs.ToList();
			}

			return segmentPairs;
		}

		public List<ProjectFile> GetProjectFiles(IEnumerable<ISegmentPair> segmentPairs)
		{
			var projectFiles = new List<ProjectFile>();
			foreach (var segmentPair in segmentPairs)
			{
				var projectFile = segmentPair.GetProjectFile();
				var projectFileId = projectFile.Id.ToString();
				if (!projectFiles.Exists(a => a.Id.ToString() == projectFileId))
				{
					projectFiles.Add(projectFile);
				}
			}

			return projectFiles;
		}

		public List<SegmentPairInfo> GetSegmentPairInfos(List<ProjectFileInfo> projectFiles, IEnumerable<ISegmentPair> selectedSegmentPairs)
		{
			var segmentPairInfos = new List<SegmentPairInfo>();

			foreach (var selectedSegmentPair in selectedSegmentPairs)
			{
				var segmentPairInfo =
					new SegmentPairInfo
					{
						ParagraphUnitId = selectedSegmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id,
						SegmentId = selectedSegmentPair.Properties.Id.Id
					};

				segmentPairInfo.FileId = GetProjectFileInfo(projectFiles, segmentPairInfo.ParagraphUnitId,
					segmentPairInfo.SegmentId)?.FileId;

				segmentPairInfos.Add(segmentPairInfo);
			}

			return segmentPairInfos;
		}

		public List<ProjectFileInfo> GetProjectFileInfos(List<ProjectFile> projectFiles)
		{
			var projectFileInfos = new List<ProjectFileInfo>();
			foreach (var documentFile in projectFiles)
			{
				var files = GetProjectFiles(documentFile.LocalFilePath);
				foreach (var projectFileInfo in files)
				{
					projectFileInfos.Add(projectFileInfo);
				}
			}

			return projectFileInfos;
		}

		public ProjectFileInfo GetProjectFileInfo(List<ProjectFileInfo> projectFiles, string paragraphId, string segmentId)
		{
			if (projectFiles == null)
			{
				return null;
			}

			foreach (var projectFile in projectFiles)
			{
				var paragraphInfo = projectFile.ParagraphInfos.FirstOrDefault(a => a.ParagraphId == paragraphId);
				var segmentInfo = paragraphInfo?.SegmentIdInfos.FirstOrDefault(a => a.OriginalSegmentId == segmentId);
				if (segmentInfo != null)
				{
					return projectFile;
				}
			}

			return null;
		}

		public string GetUniqueFileName(string filePath, string suffix)
		{
			var directoryName = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			var fileExtension = Path.GetExtension(fileName);
			var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName, fileExtension);

			var index = 1;
			var uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
															 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
															 + index.ToString().PadLeft(4, '0') + fileExtension);

			if (File.Exists(uniqueFilePath))
			{
				while (File.Exists(uniqueFilePath))
				{
					index++;
					uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
																 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
																 + index.ToString().PadLeft(4, '0') + fileExtension);
				}
			}

			return uniqueFilePath;
		}

		public string GetValidFolderPath(string initialPath)
		{
			if (string.IsNullOrWhiteSpace(initialPath))
			{
				return string.Empty;
			}

			var outputFolder = initialPath;
			if (Directory.Exists(outputFolder))
			{
				return outputFolder;
			}

			while (outputFolder.Contains("\\"))
			{
				outputFolder = outputFolder.Substring(0, outputFolder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(outputFolder))
				{
					return outputFolder;
				}
			}

			return outputFolder;
		}

		public string GetDateTimeToFilePartString(DateTime dateTime)
		{
			var value = (dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
				? dateTime.Year
				  + "" + dateTime.Month.ToString().PadLeft(2, '0')
				  + "" + dateTime.Day.ToString().PadLeft(2, '0')
				  + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
				  + "" + dateTime.Minute.ToString().PadLeft(2, '0')
				  + "" + dateTime.Second.ToString().PadLeft(2, '0')
				: "none";
			return value;
		}

		public string GetDateTimeToString(DateTime dateTime)
		{
			var value = (dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
				? dateTime.Year
				  + "-" + dateTime.Month.ToString().PadLeft(2, '0')
				  + "-" + dateTime.Day.ToString().PadLeft(2, '0')
				  + " " + dateTime.Hour.ToString().PadLeft(2, '0')
				  + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
				  + ":" + dateTime.Second.ToString().PadLeft(2, '0')
				: "[none]";
			return value;
		}
		
		public List<ProjectFileInfo> GetProjectFiles(string filePath)
		{
			var projectFileInfos = new List<ProjectFileInfo>();

			if (string.IsNullOrEmpty(filePath))
			{
				return projectFileInfos;
			}

			try
			{
				//var xml = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
				XDocument xml = null;
				// avoid System.Xml.XmlException Message: '?', hexadecimal value 0x1C, 0x1E, ... is an invalid character.
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { CheckCharacters = false };
				using (XmlReader xmlReader = XmlReader.Create(filePath, xmlReaderSettings))
				{
					xmlReader.MoveToContent();
					xml = XDocument.Load(xmlReader, LoadOptions.PreserveWhitespace);
				}
				
				var xliff = xml.Root;
				if (xliff != null &&
					string.Compare(xliff.Name.LocalName, "xliff", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var file in xliff.Elements())
					{
						if (string.Compare(file.Name.LocalName, "file", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var segmentIdCounter = 0;
							var projectFileInfo = ReadProjectFile(file, ref segmentIdCounter);
							projectFileInfos.Add(projectFileInfo);
						}
					}
				}
			}
			catch
			{
				// ignore; catch all
			}

			return projectFileInfos;
		}

		private static ProjectFileInfo ReadProjectFile(XElement file, ref int segmentIdCounter)
		{
			var projectFileInfo = new ProjectFileInfo();
			if (file.HasAttributes)
			{
				foreach (var attribute in file.Attributes())
				{
					if (string.Compare(attribute.Name.LocalName, "original", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						projectFileInfo.Original = attribute.Value;
					}

					if (string.Compare(attribute.Name.LocalName, "source-language", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						projectFileInfo.SourceLanguage = attribute.Value;
					}

					if (string.Compare(attribute.Name.LocalName, "target-language", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						projectFileInfo.TargetLanguage = attribute.Value;
					}
				}
			}

			ReadProjectFile(file, projectFileInfo, ref segmentIdCounter);

			return projectFileInfo;
		}

		private static void ReadProjectFile(XContainer file, ProjectFileInfo projectFileInfo, ref int segmentIdCounter)
		{
			foreach (var element in file.Elements())
			{
				if (string.Compare(element.Name.LocalName, "header", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var fileInfo in element.Elements())
					{
						if (string.Compare(fileInfo.Name.LocalName, "file-info", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							foreach (var value in fileInfo.Elements())
							{
								if (string.Compare(value.Name.LocalName, "value", StringComparison.InvariantCultureIgnoreCase) == 0)
								{
									if (!value.HasAttributes)
									{
										continue;
									}

									SetFileInfoAttributes(projectFileInfo, value);
								}
							}
						}

						if (string.Compare(fileInfo.Name.LocalName, "filetype-info", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var fileType = fileInfo.Elements().FirstOrDefault(a => string.Compare(a.Name.LocalName, "filetype-id", StringComparison.InvariantCultureIgnoreCase) == 0);
							if (fileType != null)
							{
								projectFileInfo.FileTypeId = fileType.Value;
							}
						}

						if (string.Compare(fileInfo.Name.LocalName, "cxt-defs", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							foreach (var cxtdef in fileInfo.Elements())
							{
								var contextDefinition = new ParagraphUnitContext();
								foreach (var attribute in cxtdef.Attributes())
								{
									if (string.Compare(attribute.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										contextDefinition.Id = attribute.Value;
									}

									if (string.Compare(attribute.Name.LocalName, "type", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										contextDefinition.ContextType = attribute.Value;
									}

									if (string.Compare(attribute.Name.LocalName, "code", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										contextDefinition.DisplayCode = attribute.Value;
									}

									if (string.Compare(attribute.Name.LocalName, "descr", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										contextDefinition.Description = attribute.Value;
									}

									if (string.Compare(attribute.Name.LocalName, "color", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										contextDefinition.DisplayName = attribute.Value;
									}
								}

								var props = cxtdef.Elements().FirstOrDefault(a => string.Compare(a.Name.LocalName, "props", StringComparison.InvariantCultureIgnoreCase) == 0);
								if (props != null)
								{
									foreach (var value in props.Elements())
									{
										if (string.Compare(value.Name.LocalName, "value", StringComparison.InvariantCultureIgnoreCase) == 0)
										{
											if (!value.HasAttributes)
											{
												continue;
											}

											foreach (var attribute in value.Attributes())
											{
												if (!contextDefinition.MetaData.ContainsKey(attribute.Value))
												{
													contextDefinition.MetaData.Add(attribute.Value, value.Value);
												}
											}
										}
									}

									projectFileInfo.ContextDefinitions.Add(contextDefinition);
								}
							}
						}
					}
				}

				if (string.Compare(element.Name.LocalName, "body", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					foreach (var group in element.Elements())
					{
						if (string.Compare(group.Name.LocalName, "group", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var contextDefinitions = new List<ParagraphUnitContext>();
							var contexts = group.Elements().FirstOrDefault(a => string.Compare(a.Name.LocalName, "cxts", StringComparison.InvariantCultureIgnoreCase) == 0);
							if (contexts != null)
							{
								foreach (var value in contexts.Elements())
								{
									if (string.Compare(value.Name.LocalName, "cxt", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										foreach (var attribute in value.Attributes())
										{
											if (string.Compare(attribute.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0)
											{
												var contextDefinition = projectFileInfo.ContextDefinitions.FirstOrDefault(a => a.Id == attribute.Value);
												if (contextDefinition != null)
												{
													contextDefinitions.Add(contextDefinition);
												}
											}
										}
									}
								}
							}

							var transUnits = group.Elements().Where(a =>
								string.Compare(a.Name.LocalName, "trans-unit", StringComparison.InvariantCultureIgnoreCase) == 0);
							foreach (var transUnit in transUnits)
							{
								var paragraphInfos = GetParagraphInfo(transUnit, ref segmentIdCounter);
								paragraphInfos.ContextDefinitions = contextDefinitions;
								projectFileInfo.ParagraphInfos.Add(paragraphInfos);
							}
						}
						if (string.Compare(group.Name.LocalName, "trans-unit", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							var paragraphInfos = GetParagraphInfo(group, ref segmentIdCounter);
							paragraphInfos.ContextDefinitions = new List<ParagraphUnitContext>();
							projectFileInfo.ParagraphInfos.Add(paragraphInfos);
						}
					}
				}
			}
		}

		private static ParagraphInfo GetParagraphInfo(XElement transUnit, ref int segmentIdCounter)
		{
			var paragraphInfo = new ParagraphInfo();

			foreach (var transUnitAttribute in transUnit.Attributes())
			{
				if (string.Compare(transUnitAttribute.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					paragraphInfo.ParagraphId = transUnitAttribute.Value;
					break;
				}
			}

			foreach (var xElement in transUnit.Elements())
			{
				if (string.Compare(xElement.Name.LocalName, "seg-defs", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var segmentIds = GetSegmentIds(xElement);

					foreach (var segmentId in segmentIds)
					{
						paragraphInfo.SegmentIdInfos.Add(new SegmentIdInfo
						{
							OriginalSegmentId = segmentId,
							SegmentId = (++segmentIdCounter).ToString()
						});
					}
				}

				if (string.Compare(xElement.Name.LocalName, "seg-source", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var mrkStack = new Stack<MrkInfo>();

					var reader = xElement.CreateReader();
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:

								if (string.Compare(reader.Name, "mrk", StringComparison.InvariantCultureIgnoreCase) == 0)
								{
									var mrkElement = new MrkInfo();
									if (reader.HasAttributes)
									{
										while (reader.MoveToNextAttribute())
										{
											if (string.Compare(reader.Name, "mtype", StringComparison.InvariantCultureIgnoreCase) == 0)
											{
												mrkElement.Mtype = reader.Value;
											}
											else if (string.Compare(reader.Name, "mid", StringComparison.InvariantCultureIgnoreCase) == 0)
											{
												mrkElement.Mid = reader.Value;
											}
										}
										reader.MoveToElement();
									}

									if (!reader.IsEmptyElement)
									{
										mrkStack.Push(mrkElement);
									}
								}
								break;
							case XmlNodeType.Text:
								{
									if (mrkStack.Count > 0)
									{
										var mrkElement = mrkStack.Peek();
										mrkElement.Content += reader.Value;
									}
									else if (reader.Value == "\n")
									{
										var mrkEmptyElement = new MrkInfo
										{
											Mtype = "structure",
											Mid = "-1",
											Content = reader.Value
										};

										paragraphInfo.SegmentationMrks.Add(mrkEmptyElement);
									}
								}
								break;
							case XmlNodeType.EndElement:
								{
									if (string.Compare(reader.Name, "mrk", StringComparison.InvariantCultureIgnoreCase) == 0)
									{
										var mrkElement = mrkStack.Pop();
										paragraphInfo.SegmentationMrks.Add(mrkElement);
									}
								}
								break;
						}
					}
				}
			}

			return paragraphInfo;
		}

		private static void SetFileInfoAttributes(ProjectFileInfo documentFileInfo, XElement value)
		{
			foreach (var attribute in value.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, "key", StringComparison.InvariantCultureIgnoreCase) == 0
					&& string.Compare(attribute.Value, "SDL:FileId", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					documentFileInfo.FileId = value.Value;
				}

				if (string.Compare(attribute.Name.LocalName, "key", StringComparison.InvariantCultureIgnoreCase) == 0
					&& string.Compare(attribute.Value, "SDL:OriginalFilePath", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					documentFileInfo.OriginalFilePath = value.Value;
				}
			}
		}

		private static IEnumerable<string> GetSegmentIds(XContainer segDefs)
		{
			var segmentIds = new List<string>();

			foreach (var seg in segDefs.Elements())
			{
				if (string.Compare(seg.Name.LocalName, "seg", StringComparison.InvariantCultureIgnoreCase) == 0 && seg.HasAttributes)
				{
					segmentIds.AddRange(
						from segAttribute in seg.Attributes()
						where string.Compare(segAttribute.Name.LocalName, "id", StringComparison.InvariantCultureIgnoreCase) == 0
						select segAttribute.Value);
				}
			}

			return segmentIds;
		}

		private string GetFileNameWithoutExtension(string fileName, string extension)
		{
			if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
			{
				return fileName;
			}

			if (extension.Length > fileName.Length || !fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return fileName;
			}

			return fileName.Substring(0, fileName.Length - extension.Length);
		}
	}
}
