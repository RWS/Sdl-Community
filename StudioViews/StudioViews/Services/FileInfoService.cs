using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sdl.Community.StudioViews.Model;

namespace Sdl.Community.StudioViews.Services
{
	public class FileInfoService
	{
		public List<ProjectFileInfo> GetProjectFiles(string filePath)
		{
			var projectFileInfos = new List<ProjectFileInfo>();

			if (string.IsNullOrEmpty(filePath))
			{
				return projectFileInfos;
			}

			try
			{
				var xml = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
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
						paragraphInfo.SegmentInfos.Add(new SegmentInfo
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
	}
}
