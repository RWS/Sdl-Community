using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using NLog;

namespace SDLXLIFFSliceOrChange
{
	public class ClearManager
    {
	    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static void ClearFile(SliceInfo sliceInfo, SdlxliffSliceOrChange form)
        {
            try
            {
                var file = sliceInfo.File;
				var xDoc = new XmlDocument
				{
					PreserveWhitespace = true
				};
				xDoc.Load(file);
                var xmlEncoding = "utf-8";
                if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
	                // Get the encoding declaration.
	                var decl = (XmlDeclaration) xDoc.FirstChild;
	                xmlEncoding = decl.Encoding;
                }

                var fileList = xDoc.DocumentElement?.GetElementsByTagName("file");
                if (fileList != null)
                {
	                foreach (var fileElement in fileList.OfType<XmlElement>())
	                {
		                var bodyElement = (XmlElement) (fileElement.GetElementsByTagName("body")[0]);
		                var groupElements = bodyElement.GetElementsByTagName("group");

		                foreach (var groupElement in groupElements.OfType<XmlElement>())
		                {
			                //look in segments
			                var transUnits = groupElement.GetElementsByTagName("trans-unit");
			                foreach (var transUnit in transUnits.OfType<XmlElement>())
			                {
				                ClearInTransUnit(sliceInfo, transUnit);
			                }
		                }

		                //look in segments
		                var transUnitsInBody = bodyElement.ChildNodes;
		                foreach (var transUnit in transUnitsInBody.OfType<XmlElement>())
		                {
			                if (transUnit.Name != "trans-unit")
				                continue;
			                ClearInTransUnit(sliceInfo, transUnit);
		                }
	                }
                }

                Encoding encoding = new UTF8Encoding();
                if (!string.IsNullOrEmpty(xmlEncoding))
                    encoding = Encoding.GetEncoding(xmlEncoding);

                using (var writer = new XmlTextWriter(file, encoding))
                {
                    xDoc.Save(writer);
                }
                form.StepProcess();
            }
            catch(Exception ex)
            {
	            _logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
            }
		}

        private static void ClearInTransUnit(SliceInfo sliceInfo, object transUnit)
        {
            var transUnitID = ((XmlElement) transUnit).Attributes["id"].Value;

            if (sliceInfo.Segments.All(seg => seg.Key != transUnitID))
                return;

            var segDefs = ((XmlElement) transUnit).GetElementsByTagName("sdl:seg-defs");

            foreach (var segDef in segDefs.OfType<XmlElement>())
            {
                var segments = segDef.GetElementsByTagName("sdl:seg");
                foreach (var segment in segments.OfType<XmlElement>())
                {
                    var segmentId = segment.Attributes["id"].Value;

                    if (!sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(segmentId)))
                        continue;

                    segment.RemoveAllAttributes();
                    segment.RemoveAll();
                    segment.SetAttribute("id", segmentId);
                }
            }
            var currentTransUnit = sliceInfo.Segments.FirstOrDefault(seg => seg.Key == transUnitID);

            var target = ((XmlElement) transUnit).GetElementsByTagName("target");
            if (target.Count > 0)
                ClearAllChildsInnerText((XmlElement) target[0], currentTransUnit.Value);
        }

        private static void ClearAllChildsInnerText(XmlElement target, List<string> ids)
        {
            foreach (var child in target.ChildNodes.OfType<XmlElement>())
            {
                if (child.Name == "mrk")
                {
                    if (!child.HasAttribute("mtype") || child.Attributes["mtype"].Value != "seg") 
                        continue;

                    var id = string.Empty;
                    if (child.HasAttribute("mid"))
                        id = child.Attributes["mid"].Value;
                    if (!ids.Contains(id))
                        continue;

                    child.RemoveAll();
                    child.SetAttribute("mid", id);
                    child.SetAttribute("mtype", "seg");
                }
                else
                {
                    ClearAllChildsInnerText(child, ids);
                }
            }
        }

        public static void CopyFile(SliceInfo sliceInfo, SdlxliffSliceOrChange form)
        {
            try
            {
                var file = sliceInfo.File;
                var xDoc = new XmlDocument {PreserveWhitespace = true};
                xDoc.Load(file);

                var xmlEncoding = "utf-8";

                if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
	                // Get the encoding declaration.
	                var decl = (XmlDeclaration) xDoc.FirstChild;
	                xmlEncoding = decl.Encoding;
                }

                var fileList = xDoc.DocumentElement?.GetElementsByTagName("file");
                if (fileList != null)
                {
	                foreach (var fileElement in fileList.OfType<XmlElement>())
	                {
		                var bodyElement = (XmlElement) (fileElement.GetElementsByTagName("body")[0]);
		                var groupElements = bodyElement.GetElementsByTagName("group");

		                foreach (var groupElement in groupElements.OfType<XmlElement>())
		                {
			                //look in segments
			                var transUnits = groupElement.GetElementsByTagName("trans-unit");
			                foreach (var transUnit in transUnits.OfType<XmlElement>())
			                {
				                CopyInTransUnit(sliceInfo, transUnit);
			                }
		                }

		                //look in segments
		                var transUnitsInBody = bodyElement.ChildNodes;
		                foreach (var transUnit in transUnitsInBody.OfType<XmlElement>())
		                {
			                if (transUnit.Name != "trans-unit")
				                continue;
			                CopyInTransUnit(sliceInfo, transUnit);
		                }
	                }
                }

                Encoding encoding = new UTF8Encoding();
                if (!string.IsNullOrEmpty(xmlEncoding))
                    encoding = Encoding.GetEncoding(xmlEncoding);
                using (var writer = new XmlTextWriter(file, encoding))
                {
                    xDoc.Save(writer);
                }
                form.StepProcess();
            }
            catch (Exception ex)
            {
	            _logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
            }
		}

        private static void CopyInTransUnit(SliceInfo sliceInfo, object transUnit)
        {
            var transUnitID = ((XmlElement) transUnit).Attributes["id"].Value;

            if (sliceInfo.Segments.All(seg => seg.Key != transUnitID))
                return;

            var segDefs = ((XmlElement) transUnit).GetElementsByTagName("sdl:seg-defs");

            foreach (var segDef in segDefs.OfType<XmlElement>())
            {
	            var segments = segDef.GetElementsByTagName("sdl:seg");
                foreach (var segment in segments.OfType<XmlElement>())
                {
                    var segmentId = segment.Attributes["id"].Value;
                    if (!sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(segmentId)))
                        continue;

                    segment.RemoveAllAttributes();
                    segment.SetAttribute("id", segmentId);
                    segment.SetAttribute("origin", "source");
                }
            }
            var target = ((XmlElement) transUnit).ChildNodes.OfType<XmlElement>().FirstOrDefault(t => t.Name == "target");
            var segSource = ((XmlElement)transUnit).ChildNodes.OfType<XmlElement>().FirstOrDefault(t => t.Name == "seg-source");

            if (target != null && segSource != null)
            {
                var currentTransUnitSliceInfo = sliceInfo.Segments.FirstOrDefault(seg => seg.Key == transUnitID);

                if (currentTransUnitSliceInfo.Value != null)
                {
                    foreach (var segmentId in currentTransUnitSliceInfo.Value)
                    {
                        var sourceSegment = segSource.GetElementsByTagName("mrk").OfType<XmlElement>().FirstOrDefault(seg => seg.GetAttribute("mid") == segmentId);
                        var targetSegment = target.GetElementsByTagName("mrk").OfType<XmlElement>().FirstOrDefault(seg => seg.GetAttribute("mid") == segmentId);

                        if (sourceSegment != null && targetSegment != null)
                        {
                            targetSegment.InnerXml = sourceSegment.InnerXml;
                        }
                    }
                }
            }
        }
    }
}