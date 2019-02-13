using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class XliffParser
	{
		private readonly string _xliffPath;
		private readonly List<string> _segmentIds;
		private readonly XmlDocument _xmlDoc;

		public XliffParser(string xliffPath, List<string> segmentIds)
		{
			_xliffPath = xliffPath;
			_segmentIds = segmentIds;
			_xmlDoc = new XmlDocument
			{
				PreserveWhitespace = true
			};
			_xmlDoc.Load(xliffPath);
		}

		public void GenerateXliff()
		{
			try
			{
				var fileElements = _xmlDoc.DocumentElement?.GetElementsByTagName("file");
				if (fileElements != null)
				{
					foreach (XmlElement fileElement in fileElements)
					{
						RemoveInternalFileInfo(fileElement);

						var bodyElement = (XmlElement)fileElement.GetElementsByTagName("body")[0];
						var groupElements = bodyElement.GetElementsByTagName("group");

						var removedGroups = new List<XmlNode>();

						if (groupElements.Count > 0)
						{
							foreach (var groupElement in groupElements.OfType<XmlElement>().ToList())
							{
								SliceInBody(groupElement, removedGroups);
							}

							foreach (var group in removedGroups.OfType<XmlElement>())
							{
								bodyElement.RemoveChild(group);
							}
						}
						else
						{
							SliceInBody(bodyElement, removedGroups, true);	
						}
					}

					using (var writer = new XmlTextWriter(_xliffPath, Encoding.UTF8))
					{
						_xmlDoc.Save(writer);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void RemoveInternalFileInfo(XmlElement fileElement)
		{
			try
			{
				var headerElement = (XmlElement)fileElement.GetElementsByTagName("header")[0];
				var referenceElement = headerElement.GetElementsByTagName("reference");
				foreach (var reference in referenceElement.OfType<XmlElement>())
				{
					var internalFile = reference.ChildNodes.OfType<XmlElement>().Where(node => node.Name == "internal-file")
						.ToList()[0];
					internalFile.InnerText = string.Empty;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private void SliceInBody(XmlElement groupElement, List<XmlNode> removedGroups, bool isSingleGroup = false)
		{
			List<XmlElement> transUnits;

			var removedTransUnits = new List<XmlNode>();

			if (isSingleGroup)
			{
				transUnits = groupElement.ChildNodes.OfType<XmlElement>().ToList();
			}
			else
			{
				transUnits = groupElement.ChildNodes.OfType<XmlElement>().Where(node => node.Name == "trans-unit")
					.ToList();
			}

			foreach (var transUnit in transUnits.ToList())
			{
				// Skip structure translation units.
				if (transUnit.GetAttribute("translate") == "no")
				{
					continue;
				}

				var segDefList = transUnit.GetElementsByTagName("sdl:seg-defs");
				var removedSegDefs = new List<XmlNode>();

				foreach (var segDef in segDefList.OfType<XmlElement>().ToList())
				{
					var segs = segDef.GetElementsByTagName("sdl:seg");
					var removedSegments = new List<XmlNode>();

					//the id of the segment is in mrk elements
					if (segs.Count > 1)
					{
						RemoveSegmentFromSourceOrTarget(transUnit, "source");
						RemoveSegmentFromSourceOrTarget(transUnit, "target");
						RemoveSegmentFromSourceOrTarget(transUnit, "seg-source");
						RemoveSegmentFromSourceOrTarget(transUnit, "target-source");
					}
					else
					{
						foreach (XmlNode seg in segs)
						{
							var id = seg.Attributes?["id"]?.Value;
							if (!string.IsNullOrEmpty(id))
							{
								var idExists = _segmentIds.Any(s => s.Equals(id));
								if (!idExists)
								{
									removedSegments.Add(seg);
								}
							}
						}
					}

					foreach (var segmentNode in removedSegments.OfType<XmlElement>())
					{
						//remove from seg-defs -> seg node
						segDef.RemoveChild(segmentNode);
						removedSegDefs.Add(segDef);
					}

					foreach (var rSegDef in removedSegDefs.OfType<XmlElement>())
					{
						//remove from translation unit -> seg-defs
						if (rSegDef.ParentNode != null)
						{
							transUnit.RemoveChild(rSegDef);
							removedTransUnits.Add(transUnit);
						}
					}

					removedSegDefs.Clear();
					removedSegments.Clear();
				}

				foreach (var rTranslationUnit in removedTransUnits.OfType<XmlElement>())
				{
					//remove from group -> translation unit
					groupElement.RemoveChild(rTranslationUnit);
					removedGroups?.Add(groupElement);
				}

				removedTransUnits.Clear();
			}
		}

		private void RemoveSegmentFromSourceOrTarget(XmlElement transUnit, string tagName)
		{
			var segments = transUnit.GetElementsByTagName(tagName);
			if (segments.Count > 0)
			{
				var firstSeg = (XmlElement)segments[0];
				var mrks = firstSeg.GetElementsByTagName("mrk");
				var removedMrks = new List<XmlNode>();
				foreach (var mrk in mrks.OfType<XmlElement>())
				{
					if (!mrk.HasAttribute("mtype") || mrk.Attributes["mtype"].Value != "seg")
						continue;

					string mrkId;
					if (mrk.HasAttribute("mid"))
					{
						mrkId = mrk.Attributes["mid"].Value;
						if (!string.IsNullOrEmpty(mrkId))
						{
							var idExists = _segmentIds.Any(s => s.Equals(mrkId));
							if (!idExists)
							{
								removedMrks.Add(mrk);
							}
						}
					}
				}
				//remove mrk element from translation unit
				foreach (var xmlNode in removedMrks.OfType<XmlElement>())
				{
					xmlNode.ParentNode?.RemoveChild(xmlNode);
				}
				removedMrks.Clear();
			}
		}
	}
}
