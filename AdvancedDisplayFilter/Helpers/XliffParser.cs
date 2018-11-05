using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class XliffParser
	{
		private readonly string _xliffPath;
		private readonly List<string> _segmentIds;
		private XmlDocument _xmlDoc;

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
			var fileElements = _xmlDoc.DocumentElement?.GetElementsByTagName("file");
			if (fileElements != null)
			{
				foreach (XmlElement fileElement in fileElements)
				{
					var bodyElement = (XmlElement)fileElement.GetElementsByTagName("body")[0];
					var groupElements = bodyElement.GetElementsByTagName("group");
					var removedGroups = new List<XmlNode>();

					foreach (var groupElement in groupElements.OfType<XmlElement>().ToList())
					{  
						SliceInBody( groupElement, removedGroups);
					}

					foreach (var group in removedGroups.OfType<XmlElement>())
					{
						bodyElement.RemoveChild(group);
					}
				}
				using (var writer = new XmlTextWriter(_xliffPath, Encoding.UTF8))
				{
					_xmlDoc.Save(writer);
				}
			}
		}

		private void SliceInBody(XmlElement groupElement, List<XmlNode> removedGroups)
		{
			var removedTransUnits = new List<XmlNode>();
			var transUnits = groupElement.ChildNodes.OfType<XmlElement>().Where(node => node.Name == "trans-unit").ToList();

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

					foreach (var segmentNode in removedSegments.OfType<XmlElement>())
					{
						//remove from seg-defs -> seg node
						segDef.RemoveChild(segmentNode);
						removedSegDefs.Add(segDef);
					}
					foreach (var rSegDef in removedSegDefs.OfType<XmlElement>())
					{   //remove from translation unit -> seg-defs
						if (rSegDef.ParentNode != null)
						{
							transUnit.RemoveChild(rSegDef);
							removedTransUnits.Add(transUnit);
						}
						
					}
					removedSegDefs.Clear();
					removedSegments.Clear();
				}

				foreach (var rTransaltionUnit in removedTransUnits.OfType<XmlElement>())
				{	//remove from group -> translation unit
					groupElement.RemoveChild(rTransaltionUnit);
					removedGroups?.Add(groupElement);
				}
			}
			removedTransUnits.Clear(); 
		}
	}
}
