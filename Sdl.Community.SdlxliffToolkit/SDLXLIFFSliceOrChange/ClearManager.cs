using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;

namespace Sdl.Community.SDLXLIFFSliceOrChange
{
    public class ClearManager
    {
        private static ILog logger = LogManager.GetLogger(typeof (ClearManager));
        public static void ClearFile(SliceInfo sliceInfo, SDLXLIFFSliceOrChange form)
        {
            try
            {
                String file = sliceInfo.File;
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                xDoc.Load(file);
                String xmlEncoding = "utf-8";
                try
                {
                    if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        // Get the encoding declaration.
                        XmlDeclaration decl = (XmlDeclaration)xDoc.FirstChild;
                        xmlEncoding = decl.Encoding;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
                XmlNodeList fileList = xDoc.DocumentElement.GetElementsByTagName("file");
                foreach (XmlElement fileElement in fileList.OfType<XmlElement>())
                {
                    XmlElement bodyElement = (XmlElement) (fileElement.GetElementsByTagName("body")[0]);
                    XmlNodeList groupElements = bodyElement.GetElementsByTagName("group");

                    foreach (var groupElement in groupElements.OfType<XmlElement>())
                    {
                        //look in segments
                        XmlNodeList transUnits = ((XmlElement) groupElement).GetElementsByTagName("trans-unit");
                        foreach (var transUnit in transUnits.OfType<XmlElement>())
                        {
                            ClearInTransUnit(sliceInfo, transUnit);
                        }
                        transUnits = null;
                    }

                    //look in segments
                    XmlNodeList transUnitsInBody = bodyElement.ChildNodes;
                    foreach (var transUnit in transUnitsInBody.OfType<XmlElement>())
                    {
                        if (((XmlNode)transUnit).Name != "trans-unit")
                            continue;
                        ClearInTransUnit(sliceInfo, transUnit);
                    }

                    bodyElement = null;
                    groupElements = null;
                    transUnitsInBody = null;
                }
                Encoding encoding = new UTF8Encoding();
                if (!String.IsNullOrEmpty(xmlEncoding))
                    encoding = Encoding.GetEncoding(xmlEncoding);

                using (var writer = new XmlTextWriter(file, encoding))
                {
                    //writer.Formatting = Formatting.None;
                    xDoc.Save(writer);
                }
                fileList = null; xDoc = null;

                form.StepProcess("Target segments from file: " + Path.GetFileName(sliceInfo.File) + " are empty.");
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static void ClearInTransUnit(SliceInfo sliceInfo, object transUnit)
        {
            String transUnitID = String.Empty;
            transUnitID = ((XmlElement) transUnit).Attributes["id"].Value;

            if (sliceInfo.Segments.All(seg => seg.Key != transUnitID))
                return;

            XmlNodeList segDefs = ((XmlElement) transUnit).GetElementsByTagName("sdl:seg-defs");

            foreach (var segDef in segDefs.OfType<XmlElement>())
            {
                XmlNodeList segments = ((XmlElement) segDef).GetElementsByTagName("sdl:seg");
                foreach (XmlElement segment in segments.OfType<XmlElement>())
                {
                    String segmentID = String.Empty;
                    segmentID = segment.Attributes["id"].Value;

                    if (
                        !sliceInfo.Segments.Any(
                            seg => seg.Key == transUnitID && seg.Value.Contains(segmentID)))
                        continue;

                    segment.RemoveAllAttributes();
                    segment.RemoveAll();
                    segment.SetAttribute("id", segmentID);
                }
                segments = null;
            }
            segDefs = null;
            KeyValuePair<String, List<String>> currentTransUnit =
                sliceInfo.Segments.FirstOrDefault(seg => seg.Key == transUnitID);

            XmlNodeList target = ((XmlElement) transUnit).GetElementsByTagName("target");
            if (target.Count > 0)
                ClearAllChildsInnerText((XmlElement) target[0], currentTransUnit.Value);
            target = null;
        }

        private static void ClearAllChildsInnerText(XmlElement target, List<String> ids)
        {
            foreach (XmlElement child in target.ChildNodes.OfType<XmlElement>())
            {
                if (child.Name == "mrk")
                {
                    if (!child.HasAttribute("mtype") || child.Attributes["mtype"].Value != "seg") 
                        continue;

                    String id = String.Empty;
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

        public static void CopyFile(SliceInfo sliceInfo, SDLXLIFFSliceOrChange form)
        {
            try
            {
                String file = sliceInfo.File;
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                xDoc.Load(file);

                String xmlEncoding = "utf-8";
                try
                {
                    if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        // Get the encoding declaration.
                        XmlDeclaration decl = (XmlDeclaration)xDoc.FirstChild;
                        xmlEncoding = decl.Encoding;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
                XmlNodeList fileList = xDoc.DocumentElement.GetElementsByTagName("file");
                foreach (XmlElement fileElement in fileList.OfType<XmlElement>())
                {
                    XmlElement bodyElement = (XmlElement) (fileElement.GetElementsByTagName("body")[0]);
                    XmlNodeList groupElements = bodyElement.GetElementsByTagName("group");

                    foreach (var groupElement in groupElements.OfType<XmlElement>())
                    {
                        //look in segments
                        XmlNodeList transUnits = ((XmlElement) groupElement).GetElementsByTagName("trans-unit");
                        foreach (var transUnit in transUnits.OfType<XmlElement>())
                        {
                            CopyInTransUnit(sliceInfo, transUnit);
                        }
                        transUnits = null;
                    }

                    //look in segments
                    XmlNodeList transUnitsInBody = ((XmlElement) bodyElement).ChildNodes;//.GetElementsByTagName("trans-unit");
                    foreach (var transUnit in transUnitsInBody.OfType<XmlElement>())
                    {
                        if (((XmlNode)transUnit).Name != "trans-unit")
                            continue;
                        CopyInTransUnit(sliceInfo, transUnit);
                    }
                    transUnitsInBody = null;
                    bodyElement = null;
                    groupElements = null;
                }
                fileList = null;

                Encoding encoding = new UTF8Encoding();
                if (!String.IsNullOrEmpty(xmlEncoding))
                    encoding = Encoding.GetEncoding(xmlEncoding);
                using (var writer = new XmlTextWriter(file, encoding))
                {
                    //writer.Formatting = Formatting.None;
                    xDoc.Save(writer);
                }
                xDoc = null;
                form.StepProcess("Source segments from file: " + Path.GetFileName(sliceInfo.File) + " are copied.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static void CopyInTransUnit(SliceInfo sliceInfo, object transUnit)
        {
            String transUnitID = String.Empty;
            transUnitID = ((XmlElement) transUnit).Attributes["id"].Value;

            if (sliceInfo.Segments.All(seg => seg.Key != transUnitID))
                return;

            XmlNodeList segDefs = ((XmlElement) transUnit).GetElementsByTagName("sdl:seg-defs");

            foreach (var segDef in segDefs.OfType<XmlElement>())
            {
                XmlNodeList segments = ((XmlElement) segDef).GetElementsByTagName("sdl:seg");
                foreach (XmlElement segment in segments.OfType<XmlElement>())
                {
                    String segmentID = String.Empty;
                    segmentID = segment.Attributes["id"].Value;

                    if (
                        !sliceInfo.Segments.Any(
                            seg => seg.Key == transUnitID && seg.Value.Contains(segmentID)))
                        continue;

                    segment.RemoveAllAttributes();
                    segment.SetAttribute("id", segmentID);
                    segment.SetAttribute("origin", "source");
                }
            }

            var target = ((XmlElement) transUnit).ChildNodes.OfType<XmlElement>()
                .FirstOrDefault(t => t.Name == "target");

            var segSource = ((XmlElement)transUnit).ChildNodes.OfType<XmlElement>()
                .FirstOrDefault(t => t.Name == "seg-source");

            if (target != null && segSource != null)
            {
                var currentTransUnitSliceInfo =
                sliceInfo.Segments.FirstOrDefault(seg => seg.Key == transUnitID);

                if (currentTransUnitSliceInfo.Value != null)
                {
                    foreach (var segmentID in currentTransUnitSliceInfo.Value)
                    {
                        var sourceSegment =
                            segSource.GetElementsByTagName("mrk").OfType<XmlElement>()
                                .FirstOrDefault(seg => seg.GetAttribute("mid") == segmentID);

                        var targetSegment = target.GetElementsByTagName("mrk").OfType<XmlElement>()
                            .FirstOrDefault(seg => seg.GetAttribute("mid") == segmentID);

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