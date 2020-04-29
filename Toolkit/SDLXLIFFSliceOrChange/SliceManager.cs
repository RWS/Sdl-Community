using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using log4net;

namespace SDLXLIFFSliceOrChange
{
    public class SliceManager
    {
        private SDLXLIFFSliceOrChange _sdlxliffSliceOrChange;
        private List<String> _fontsToBeAdded;
        private List<String> _cxtsToBeAdded;
        private List<String> _tagsToBeAdded;
        private List<String> _nodesToBeAdded;
        private List<String> _groupsToBeAdded;
        private ILog logger = LogManager.GetLogger(typeof (SliceManager));

        public SliceManager(SDLXLIFFSliceOrChange sdlxliffSliceOrChange)
        {
            _sdlxliffSliceOrChange = sdlxliffSliceOrChange;
        }

        public void MergeSplitFiles(List<KeyValuePair<string, List<string>>> filesPerLanguage)
        {
            List<Thread> threads = new List<Thread>();
            foreach (var keyValuePair in filesPerLanguage)
            {
                String language = keyValuePair.Key;
                List<String> files = keyValuePair.Value;
                Thread t = new Thread(() => MergeSplitFilesPerLanguage(language, files));
                t.Start();
                threads.Add(t);
            }

            foreach (var thread in threads)
                thread.Join();
        }

        private void MergeSplitFilesPerLanguage(string language, List<string> files)
        {
            if (files.Count == 0)
                return;
            String finalFile = Path.Combine(_sdlxliffSliceOrChange._folderForSlicedFiles,
                                            String.Format("{0}_{1}.sdlxliff", language, "sliced"));
            File.Copy(files[0], finalFile, true);

            XmlDocument xFinalDoc = new XmlDocument();
            xFinalDoc.PreserveWhitespace = true;
            xFinalDoc.Load(finalFile);
            String xmlEncoding = "utf-8";
            try
            {
                if (xFinalDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                {
                    // Get the encoding declaration.
                    XmlDeclaration decl = (XmlDeclaration)xFinalDoc.FirstChild;
                    xmlEncoding = decl.Encoding;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
            XmlNodeList fileList = xFinalDoc.DocumentElement.GetElementsByTagName("file");
            foreach (XmlElement finalDocFileElement in fileList.OfType<XmlElement>())
            {
                XmlElement finalDocHeaderElement = (XmlElement) (finalDocFileElement.GetElementsByTagName("header")[0]);
                XmlNodeList referencesNodes = finalDocHeaderElement.GetElementsByTagName("reference");
                List<XmlNode> referencesToBeRemoved = new List<XmlNode>();
                foreach (var referencesNode in referencesNodes.OfType<XmlElement>())
                    referencesToBeRemoved.Add((XmlNode) referencesNode);
                foreach (var referenceToBeRemoved in referencesToBeRemoved.OfType<XmlElement>())
                    finalDocHeaderElement.RemoveChild(referenceToBeRemoved);
                referencesToBeRemoved.Clear();
                referencesNodes = null;
                finalDocHeaderElement = null;
            }
            fileList = null;

            Encoding encoding = new UTF8Encoding();
            if (!String.IsNullOrEmpty(xmlEncoding))
                encoding = Encoding.GetEncoding(xmlEncoding);
            using (var writer = new XmlTextWriter(finalFile, encoding))
            {
                //writer.Formatting = Formatting.None;
                xFinalDoc.Save(writer);
            }
            xFinalDoc = null;

            if (files.Count >= 2)
            {
                for (int i = 1; i < files.Count; i++)
                {
                    Merge2Files(finalFile, files[i]);
                }
            }
        }

        private void Merge2Files(string finalFile, string sourceFile)
        {
            _fontsToBeAdded = new List<string>();
            _cxtsToBeAdded = new List<string>();
            _tagsToBeAdded = new List<string>();
            _nodesToBeAdded = new List<string>();
            _groupsToBeAdded = new List<string>();

            XmlDocument xSourceDoc = new XmlDocument();
            xSourceDoc.PreserveWhitespace = true;
            xSourceDoc.Load(sourceFile);
            XmlNodeList fileList = xSourceDoc.DocumentElement.GetElementsByTagName("file");
            foreach (XmlElement sourceDocFileElement in fileList.OfType<XmlElement>())
            {
                XmlElement sourceDocBodyElement = (XmlElement) (sourceDocFileElement.GetElementsByTagName("body")[0]);
                XmlElement sourceDocHeaderElement =
                    (XmlElement) (sourceDocFileElement.GetElementsByTagName("header")[0]);

                //fmts
                Dictionary<string, string> newFmtIDs = null;
                Thread t = new Thread(() => 
                newFmtIDs = MergeFmts(finalFile, sourceDocHeaderElement));
                t.Start();
                t.Join();

                //cxt
                Dictionary<string, string> newCxtsIDs = null;
                Thread t1 = new Thread(() => 
                newCxtsIDs = MergeCxt(finalFile, sourceDocHeaderElement, newFmtIDs));
                t1.Start();

                //tags
                Dictionary<string, string> newTagsIDs = null;
                Thread t2 = new Thread(() => 
                newTagsIDs = MergeTags(finalFile, sourceDocHeaderElement, newFmtIDs));
                t2.Start();

                t1.Join();
                //tags
                Dictionary<string, string> newNodesIDs = null;
                Thread t3 = new Thread(() => 
                newNodesIDs = MergeNodes(finalFile, sourceDocHeaderElement, newCxtsIDs));
                t3.Start();

                //get all SegmentIds from finalDoc
                _sdlxliffSliceOrChange.StepProcess("Geting segments ... ", false);

                List<String> SegmentIds = new List<string>();
                using (XmlReader reader = XmlReader.Create(finalFile))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "sdl:seg-defs")
                        {
                            XmlDocument segDoc = new XmlDocument();
                            segDoc.PreserveWhitespace = true;
                            segDoc.LoadXml(reader.ReadOuterXml());
                            XmlNodeList segDefs = segDoc.DocumentElement.GetElementsByTagName("sdl:seg");

                            foreach (XmlElement seg in segDefs.OfType<XmlElement>())
                            {
                                string SegmentId = seg.Attributes["id"].Value;
                                if (!SegmentIds.Contains(SegmentId))
                                    SegmentIds.Add(SegmentId);
                            }
                        }
                    }
                }
                _sdlxliffSliceOrChange.StepProcess("Geting segments, done. ", false);

                t2.Join();
                t3.Join();
                sourceDocHeaderElement = null;

                _sdlxliffSliceOrChange.StepProcess("Merging body ... ", false);

                #region body segments

                //body - segments
                XmlNodeList sourceGroups = sourceDocBodyElement.GetElementsByTagName("group");
                _sdlxliffSliceOrChange.StepProcess(sourceGroups.Count.ToString() + " groups to process ... ", false);

                int itemsProcessed = 0;
                int itemsResetCounts = 1;
                foreach (XmlElement sourceGroup in sourceGroups.OfType<XmlElement>())
                {
                    itemsProcessed++;
                    if (itemsProcessed == 5000)
                    {
                        _sdlxliffSliceOrChange.StepProcess(
                            sourceGroups.Count - (itemsResetCounts*itemsProcessed) + " groups to process ... ", false);
                        itemsProcessed = 0;
                        itemsResetCounts++;
                    }

                    //update cxts and nodes
                    XmlNodeList cxtsElements = sourceGroup.GetElementsByTagName("sdl:cxts");
                    if (cxtsElements.Count > 0)
                    {
                        XmlElement cxts = (XmlElement) cxtsElements[0];

                        //cxt elements
                        XmlNodeList cxtElements = cxts.GetElementsByTagName("sdl:cxt");
                        foreach (var cxtElement in cxtElements.OfType<XmlElement>())
                        {
                            String id = ((XmlElement) cxtElement).Attributes["id"].Value;
                            if (Enumerable.Contains(newCxtsIDs.Keys, id))
                                ((XmlElement) cxtElement).SetAttribute("id", newCxtsIDs[id]);
                        }
                        cxtElements = null;
                        //node elements
                        XmlNodeList nodeElements = cxts.GetElementsByTagName("sdl:node");
                        foreach (var nodeElement in nodeElements.OfType<XmlElement>())
                        {
                            String id = ((XmlElement) nodeElement).Attributes["id"].Value;
                            if (Enumerable.Contains(newNodesIDs.Keys, id))
                                ((XmlElement) nodeElement).SetAttribute("id", newNodesIDs[id]);
                        }
                        nodeElements = null;
                    }
                    cxtsElements = null;
                    //update tags (x an g elements)
                    XmlNodeList transUnitElements = sourceGroup.GetElementsByTagName("trans-unit");
                    if (transUnitElements.Count > 0)
                    {
                        foreach (XmlElement transUnitElement in transUnitElements.OfType<XmlElement>())
                        {
                            XmlNodeList sourceElements = transUnitElement.GetElementsByTagName("source");
                            foreach (XmlElement sourceElement in sourceElements.OfType<XmlElement>())
                            {
                                UpdateGandX(sourceElement, newTagsIDs);
                            }
                            sourceElements = null;

                            XmlNodeList segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
                            foreach (XmlElement segSourceElement in segSourceElements.OfType<XmlElement>())
                            {
                                UpdateGandX(segSourceElement, newTagsIDs);
                            }
                            segSourceElements = null;

                            XmlNodeList targetElements = transUnitElement.GetElementsByTagName("target");
                            foreach (XmlElement targetElement in targetElements.OfType<XmlElement>())
                            {
                                UpdateGandX(targetElement, newTagsIDs);
                            }
                            targetElements = null;

                            XmlNodeList segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
                            foreach (XmlElement segTargetElement in segTargetElements.OfType<XmlElement>())
                            {
                                UpdateGandX(segTargetElement, newTagsIDs);
                            }
                            segTargetElements = null;

                            UpdateSegmentsIDs(transUnitElement, ref SegmentIds);
                        }
                    }
                    transUnitElements = null;
                    _groupsToBeAdded.Add(sourceGroup.OuterXml);
                }

                //update cxts and nodes
                XmlNodeList cxtsElementsInBody = sourceDocBodyElement.ChildNodes;//.GetElementsByTagName("sdl:cxts");
                if (cxtsElementsInBody.Count > 0)
                {
                    foreach (XmlElement cxts in cxtsElementsInBody.OfType<XmlElement>())
                    {
                        if (cxts.Name != "sdl:cxts")
                            continue;
                        //cxt elements
                        XmlNodeList cxtElements = cxts.GetElementsByTagName("sdl:cxt");
                        foreach (var cxtElement in cxtElements.OfType<XmlElement>())
                        {
                            String id = ((XmlElement)cxtElement).Attributes["id"].Value;
                            if (Enumerable.Contains(newCxtsIDs.Keys, id))
                                ((XmlElement)cxtElement).SetAttribute("id", newCxtsIDs[id]);
                        }
                        cxtElements = null;
                        //node elements
                        XmlNodeList nodeElements = cxts.GetElementsByTagName("sdl:node");
                        foreach (var nodeElement in nodeElements.OfType<XmlElement>())
                        {
                            String id = ((XmlElement)nodeElement).Attributes["id"].Value;
                            if (Enumerable.Contains(newNodesIDs.Keys, id))
                                ((XmlElement)nodeElement).SetAttribute("id", newNodesIDs[id]);
                        }
                        nodeElements = null;
                    }

                }
                cxtsElementsInBody = null;
                //update tags (x an g elements)
                XmlNodeList transUnitElementsInBody = sourceDocBodyElement.ChildNodes;//.GetElementsByTagName("trans-unit");
                if (transUnitElementsInBody.Count > 0)
                {
                    foreach (XmlElement transUnitElement in transUnitElementsInBody.OfType<XmlElement>())
                    {
                        if (transUnitElement.Name != "trans-unit")
                            continue;
                        XmlNodeList sourceElements = transUnitElement.GetElementsByTagName("source");
                        foreach (XmlElement sourceElement in sourceElements.OfType<XmlElement>())
                        {
                            UpdateGandX(sourceElement, newTagsIDs);
                        }
                        sourceElements = null;

                        XmlNodeList segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
                        foreach (XmlElement segSourceElement in segSourceElements.OfType<XmlElement>())
                        {
                            UpdateGandX(segSourceElement, newTagsIDs);
                        }
                        segSourceElements = null;

                        XmlNodeList targetElements = transUnitElement.GetElementsByTagName("target");
                        foreach (XmlElement targetElement in targetElements.OfType<XmlElement>())
                        {
                            UpdateGandX(targetElement, newTagsIDs);
                        }
                        targetElements = null;

                        XmlNodeList segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
                        foreach (XmlElement segTargetElement in segTargetElements.OfType<XmlElement>())
                        {
                            UpdateGandX(segTargetElement, newTagsIDs);
                        }
                        segTargetElements = null;

                        UpdateSegmentsIDs(transUnitElement, ref SegmentIds);
                        _groupsToBeAdded.Add(transUnitElement.OuterXml);
                    }
                }
                transUnitElementsInBody = null;
                _sdlxliffSliceOrChange.StepProcess("Merging body, done. ");
                sourceGroups = null;

                #endregion

                xSourceDoc = null;
                sourceDocBodyElement = null;
            }
            _sdlxliffSliceOrChange.StepProcess("Finnishing merge ... ");
            AddingElementsToFinalFile(finalFile);
            _sdlxliffSliceOrChange.StepProcess("Merging files, done. ");

            fileList = null;
        }

        private void AddingElementsToFinalFile(string finalFile)
        {
            String fileContent = String.Empty;
            using (StreamReader sr = new StreamReader(finalFile))
            {
                fileContent = sr.ReadToEnd();
            }
            String replaceWhat = @"</file-info>";
            if (fileContent.IndexOf(@"/sdl:filetype-info>") != -1)
                replaceWhat = @"</sdl:filetype-info>";

            if (_fontsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</fmt-defs>") == -1)
                {
                    fileContent = fileContent.Replace(replaceWhat, replaceWhat+@"<fmt-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></fmt-defs>");
                }
                String fonts = String.Format("{0}</fmt-defs>", String.Join("", _fontsToBeAdded));
                fileContent = fileContent.Replace("</fmt-defs>", fonts.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", String.Empty));
            }

            if (_cxtsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</cxt-defs>") == -1)
                {
                    if (fileContent.IndexOf("</fmt-defs>") != -1)
                        replaceWhat = @"</fmt-defs>";
                    fileContent = fileContent.Replace(replaceWhat, replaceWhat+@"<cxt-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></cxt-defs>");
                }
                String cxts = String.Format("{0}</cxt-defs>", String.Join("", _cxtsToBeAdded));
                fileContent = fileContent.Replace("</cxt-defs>", cxts.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", String.Empty));
            }

            if (_nodesToBeAdded.Any())
            {
                if (fileContent.IndexOf("</node-defs>") == -1)
                {
                    if (fileContent.IndexOf("</cxt-defs>") != -1)
                        replaceWhat = @"</cxt-defs>";
                    else
                    {
                        if (fileContent.IndexOf("</fmt-defs>") != -1)
                            replaceWhat = @"</fmt-defs>";
                    }
                    fileContent = fileContent.Replace(replaceWhat, replaceWhat + @"<node-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></node-defs>");
                }
                String nodes = String.Format("{0}</node-defs>", String.Join("", _nodesToBeAdded));
                fileContent = fileContent.Replace("</node-defs>", nodes.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", String.Empty));
            }

            if (_tagsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</tag-defs>") == -1)
                {
                    fileContent = fileContent.Replace("</header>", @"<tag-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></tag-defs></header>");
                }

                String tags = String.Format("{0}</tag-defs>", String.Join("", _tagsToBeAdded));
                fileContent = fileContent.Replace("</tag-defs>", tags.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", String.Empty));
            }

            if (_groupsToBeAdded.Any())
            {
                String groups = String.Format("{0}</body>", String.Join("", _groupsToBeAdded));
                fileContent = fileContent.Replace("</body>", groups);
            }

            using (StreamWriter sw = new StreamWriter(finalFile, false))
            {
                sw.Write(fileContent);
            }
            fileContent = null;
        }

        private void UpdateSegmentsIDs(XmlElement transUnitElement, ref List<string> SegmentIds)
        {
            int tempID = 0;
            String newId = (SegmentIds.Where(i => Int32.TryParse(i, out tempID)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
            XmlNodeList segDefElements = transUnitElement.GetElementsByTagName("sdl:seg-defs");
            if (segDefElements.Count > 0)
            {
                XmlElement segDefs = (XmlElement)segDefElements[0];
                XmlNodeList segments = segDefs.GetElementsByTagName("sdl:seg");
                foreach (XmlElement segment in segments.OfType<XmlElement>())
                {
                    String SegmentId = segment.Attributes["id"].Value;
                    if (SegmentIds.Contains(SegmentId))
                    {
                        //List<Thread> threads = new List<Thread>();
                        SegmentIds.Add(newId);
                        segment.SetAttribute("id", newId);
                        //Thread t1 = new Thread(() => 
                        //    {
                                XmlNodeList sourceElements = transUnitElement.GetElementsByTagName("source");
                                foreach (XmlElement sourceElement in sourceElements.OfType<XmlElement>())
                                {
                                    UpdateMrkMID(sourceElement, SegmentId, newId);
                                }
                                sourceElements = null;
                        //    });
                        //t1.Start();
                        //threads.Add(t1);

                        //Thread t2 = new Thread(() =>
                        //    {
                                XmlNodeList segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
                                foreach (XmlElement segSourceElement in segSourceElements.OfType<XmlElement>())
                                {
                                    UpdateMrkMID(segSourceElement, SegmentId, newId);
                                }
                                segSourceElements = null;
                        //    });
                        //t2.Start();
                        //threads.Add(t2);

                        //Thread t3 = new Thread(() =>
                        //    {
                                XmlNodeList targetElements = transUnitElement.GetElementsByTagName("target");
                                foreach (XmlElement targetElement in targetElements.OfType<XmlElement>())
                                {
                                    UpdateMrkMID(targetElement, SegmentId, newId);
                                }
                                targetElements = null;
                        //    });
                        //t3.Start();
                        //threads.Add(t3);

                        //Thread t4 = new Thread(() =>
                        //    {
                                XmlNodeList segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
                                foreach (XmlElement segTargetElement in segTargetElements.OfType<XmlElement>())
                                {
                                    UpdateMrkMID(segTargetElement, SegmentId, newId);
                                }
                                segTargetElements = null;
                        //    });
                        //t4.Start();
                        //threads.Add(t4);

                        //foreach (var thread in threads)
                        //    thread.Join();
                    }
                }
                segments = null;
                segDefs = null;
            }
            segDefElements = null;
        }

        private void UpdateMrkMID(XmlElement element, string oldId, string newId)
        {
            XmlNodeList mrks = element.GetElementsByTagName("mrk");
            foreach (XmlElement mrk in mrks.OfType<XmlElement>())
            {
                if (mrk.HasAttribute("mtype") && mrk.Attributes["mtype"].Value == "seg" && mrk.HasAttribute("mid"))
                {
                    String mId = mrk.Attributes["mid"].Value;
                    if (mId == oldId)
                    {
                        mrk.SetAttribute("mid", newId);
                    }
                }
            }
            mrks = null;
        }

        private static void UpdateGandX(XmlElement element, Dictionary<string, string> newTagsIDs)
        {
            XmlNodeList xList = element.GetElementsByTagName("x");
            foreach (XmlElement x in xList.OfType<XmlElement>())
            {
                if (x.HasAttribute("id") && newTagsIDs.Keys.Contains(x.Attributes["id"].Value))
                    x.SetAttribute("id", newTagsIDs[x.Attributes["id"].Value]);
            }
            xList = null;
            XmlNodeList gList = element.GetElementsByTagName("g");
            foreach (XmlElement g in gList.OfType<XmlElement>())
            {
                if (g.HasAttribute("id") && newTagsIDs.Keys.Contains(g.Attributes["id"].Value))
                    g.SetAttribute("id", newTagsIDs[g.Attributes["id"].Value]);
            }
            gList = null;
        }

        private Dictionary<string, string> MergeNodes(String finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newCxtsIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging Nodes ... ", false);
            Dictionary<String, String> newNodeIDs = new Dictionary<string, string>();

            XmlNodeList nodeDefinitions = sourceDocHeaderElement.GetElementsByTagName("node-defs");
            if (nodeDefinitions.Count == 0)
                return newNodeIDs;

            XmlElement sourceNodeDefs = (XmlElement)nodeDefinitions[0];

            List<String> finalNodeIDs = new List<String>();
            Dictionary<String, String> finalFormats = new Dictionary<string, string>();

            using (XmlReader reader = XmlReader.Create(finalFile))
            {
                while (reader.Read())
                {
                    if (reader.Name == "node-defs")
                    {
                        XmlDocument nodeDoc = new XmlDocument();
                        nodeDoc.PreserveWhitespace = true;
                        nodeDoc.LoadXml(reader.ReadOuterXml());
                        XmlNodeList nodeDefs = nodeDoc.DocumentElement.GetElementsByTagName("node-def");

                        foreach (XmlElement node in nodeDefs.OfType<XmlElement>())
                        {
                            string id = node.Attributes["id"].Value;
                            finalNodeIDs.Add(id);

                            Dictionary<String, String> formats = GetNodeFormats(node);
                            String format = GenerateFormat(formats);
                            if (!finalFormats.ContainsKey(format))
                                finalFormats.Add(format, id);
                        }
                    }
                }
            } 

            XmlNodeList sourceNodes = sourceNodeDefs.GetElementsByTagName("node-def");
            _sdlxliffSliceOrChange.StepProcess(sourceNodes.Count.ToString() + " nodes to process ... ", false);
            
            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (XmlElement sourceNode in sourceNodes.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceNodes.Count - (itemsResetCounts * itemsProcessed) + " nodes to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }
                String id = sourceNode.Attributes["id"].Value;
                String newId = id;
                //generating formats
                Dictionary<String, String> formats = GetNodeFormats((XmlElement) sourceNode);
                String format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newNodeIDs.Add(id, newId);
                }
                else
                {
                    if (finalNodeIDs.Contains(id))
                    {
                        int tempID = 0;
                        newId =
                            (finalNodeIDs.Where(i => Int32.TryParse(i, out tempID))
                                         .ToList()
                                         .ConvertAll(Convert.ToInt32)
                                         .Max() + 1).ToString();
                        finalNodeIDs.Add(newId);
                        newNodeIDs.Add(id, newId);
                    }

                    sourceNode.SetAttribute("id", newId);

                    var nodeCxts = sourceNode.GetElementsByTagName("cxt");
                    foreach (var nodeCxt in nodeCxts.OfType<XmlElement>())
                    {
                        String fmtId = ((XmlElement) nodeCxt).Attributes["id"].Value;
                        if (newCxtsIDs.Keys.Contains(fmtId))
                            ((XmlElement) nodeCxt).SetAttribute("id", newCxtsIDs[fmtId]);
                    }
                    nodeCxts = null;
                    _nodesToBeAdded.Add(sourceNode.OuterXml);
                }
            }

            foreach (XmlElement sourceNode in sourceNodes.OfType<XmlElement>())
            {
                if (sourceNode.HasAttribute("parent"))
                {
                    String parentID = sourceNode.Attributes["parent"].Value;
                    if (newNodeIDs.ContainsKey(parentID))
                    {
                        sourceNode.SetAttribute("parent", newNodeIDs[parentID]);
                    }
                }
            }

            sourceNodes = null;
            sourceNodeDefs = null;
            _sdlxliffSliceOrChange.StepProcess("Merging Nodes, done. ", false);

            return newNodeIDs;
        }

        private Dictionary<string, string> GetNodeFormats(XmlElement node)
        {
            List<String> attributes = new List<string>() { "force-name", "parent" };
            Dictionary<String, String> formats = new Dictionary<string, string>();
            foreach (var attribute in attributes)
            {
                if (node.HasAttribute(attribute))
                    formats.Add(attribute, node.Attributes[attribute].Value);
            }
            XmlNodeList cxts = node.GetElementsByTagName("cxt");
            List<String> cxtIDs = new List<string>();
            foreach (XmlElement cxt in cxts.OfType<XmlElement>())
            {
                cxtIDs.Add(cxt.Attributes["id"].Value);
            }
            cxts = null;
            formats.Add("cxt", String.Join(",", cxtIDs.ToArray()));

            return formats;
        }

        private Dictionary<string, string> MergeTags(String finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newFmtIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging tags ... ", false);
            Dictionary<String, String> newTagIDs = new Dictionary<string, string>();

            XmlNodeList tagDefinitions = sourceDocHeaderElement.GetElementsByTagName("tag-defs");
            if (tagDefinitions.Count == 0)
                return newTagIDs;

            XmlElement sourceTagsDefs = (XmlElement)tagDefinitions[0];

           // XmlNodeList finalTags = finalTagsDefs.GetElementsByTagName("tag");
            List<String> finalTagIDs = new List<String>();
            Dictionary<String, String> finalFormats = new Dictionary<string, string>();

            using (XmlReader reader = XmlReader.Create(finalFile))
            {
                while (reader.Read())
                {
                    if (reader.Name == "tag-defs")
                    {
                        XmlDocument tagDoc = new XmlDocument();
                        tagDoc.PreserveWhitespace = true;
                        tagDoc.LoadXml(reader.ReadOuterXml());
                        XmlNodeList tagDefs = tagDoc.DocumentElement.GetElementsByTagName("tag");

                        foreach (XmlElement tag in tagDefs.OfType<XmlElement>())
                        {
                            string id = tag.Attributes["id"].Value;
                            finalTagIDs.Add(id);

                            Dictionary<String, String> formats = GetTagFormats(tag);
                            String format = GenerateFormat(formats);
                            if (!finalFormats.ContainsKey(format))
                                finalFormats.Add(format, id);
                        }
                    }
                }
            } 

            XmlNodeList sourceTags = sourceTagsDefs.GetElementsByTagName("tag");
            _sdlxliffSliceOrChange.StepProcess(sourceTags.Count.ToString() + " tags to process ... ", false);
            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (XmlElement sourceTag in sourceTags.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceTags.Count - (itemsResetCounts * itemsProcessed) + " tags to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }

                String id = sourceTag.Attributes["id"].Value;
                String newId = id;
                //generating formats
                Dictionary<String, String> formats = GetTagFormats(sourceTag);
                String format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newTagIDs.Add(id, newId);
                }
                else
                {
                    if (finalTagIDs.Contains(id))
                    {
                        int tempID = 0;
                        newId =
                            (finalTagIDs.Where(i => Int32.TryParse(i, out tempID))
                                        .ToList()
                                        .ConvertAll(Convert.ToInt32)
                                        .Max() + 1).ToString();
                        finalTagIDs.Add(newId);
                        newTagIDs.Add(id, newId);
                    }

                    sourceTag.SetAttribute("id", newId);

                    var tagFmts = sourceTag.GetElementsByTagName("fmt");
                    foreach (XmlElement TagFmt in tagFmts.OfType<XmlElement>())
                    {
                        String fmtId = TagFmt.Attributes["id"].Value;
                        if (newFmtIDs.Keys.Contains(fmtId))
                            TagFmt.SetAttribute("id", newFmtIDs[fmtId]);
                    }
                    tagFmts = null;
                    _tagsToBeAdded.Add(sourceTag.OuterXml);
                }
            }
            sourceTags = null;
            sourceTagsDefs = null;
            _sdlxliffSliceOrChange.StepProcess("Merging tags, done. ", false);
            return newTagIDs;
        }

        private Dictionary<string, string> GetTagFormats(XmlElement tag)
        {
            Dictionary<String, String> formats = new Dictionary<string, string>();
            List<String> elements = new List<string>() {"bpt", "ept", "ph", "st"};
            List<String> props = new List<string>() { "bpt-props", "ept-props", "props" };

            foreach (var element in elements)
            {
                XmlNodeList childElements = tag.GetElementsByTagName(element);
                if (childElements.Count > 0)
                {
                    XmlElement childElement = (XmlElement)childElements[0];
                    foreach (XmlAttribute attribute in childElement.Attributes)
                    {
                        if (attribute.Name == "id")
                            continue;
                        formats.Add(String.Format("{0}-{1}", element, attribute.Name), attribute.Value);
                    }
                    childElement = null;
                }
                childElements = null;
            }

            foreach (var prop in props)
            {
                XmlNodeList properties = tag.GetElementsByTagName(prop);
                if (properties.Count > 0)
                {
                    XmlElement neededProp = (XmlElement) properties[0];
                    XmlNodeList values = neededProp.GetElementsByTagName("value");
                    foreach (XmlElement value in values.OfType<XmlElement>())
                    {
                        if (value.HasAttribute("key"))
                            formats.Add(String.Format("{0}-{1}",prop, value.Attributes["key"].Value), value.InnerText);
                    }
                    values = null;
                }
                properties = null;
            }
            //fmt
            XmlNodeList fmts = tag.GetElementsByTagName("fmt");
            List<String> fmtIDs = new List<string>();
            foreach (XmlElement fmt in fmts.OfType<XmlElement>())
            {
                fmtIDs.Add(fmt.Attributes["id"].Value);
            }
            fmts = null;
            formats.Add("fmt", String.Join(",", fmtIDs.ToArray()));

            return formats;
        }

        private Dictionary<string, string> MergeCxt(String finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newFmtIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging Cxt ... ", false);
            Dictionary<String, String> newCxtIDs = new Dictionary<string, string>();

            XmlNodeList cxtDefinitions = sourceDocHeaderElement.GetElementsByTagName("cxt-defs");
            if (cxtDefinitions.Count == 0)
                return newCxtIDs;
            XmlElement sourceCxtDefs = (XmlElement)cxtDefinitions[0];

            List<String> finalCxtIDs = new List<String>();
            Dictionary<String, String> finalFormats = new Dictionary<string, string>();

            using (TextReader tr = new StreamReader(finalFile))
            {
                using (XmlReader reader = XmlReader.Create(tr))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "cxt-defs")
                        {
                            XmlDocument ctxDoc = new XmlDocument();
                            ctxDoc.PreserveWhitespace = true;
                            ctxDoc.LoadXml(reader.ReadOuterXml());
                            XmlNodeList cxtDefs = ctxDoc.DocumentElement.GetElementsByTagName("cxt-def");

                            foreach (XmlElement cxt in cxtDefs.OfType<XmlElement>())
                            {
                                string id = cxt.Attributes["id"].Value;
                                finalCxtIDs.Add(id);

                                Dictionary<String, String> formats = GetCxtsFormats(cxt);
                                String format = GenerateFormat(formats);
                                if (!finalFormats.ContainsKey(format))
                                    finalFormats.Add(format, id);
                            }
                        }
                    }
                }
            }
            XmlNodeList sourceCxts = sourceCxtDefs.GetElementsByTagName("cxt-def");
            _sdlxliffSliceOrChange.StepProcess(sourceCxts.Count.ToString() + " cxts to process ... ", false);

            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (XmlElement sourceCxt in sourceCxts.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceCxts.Count - (itemsResetCounts * itemsProcessed) + " cxts to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }

                String id = sourceCxt.Attributes["id"].Value;
                String newId = id;

                //generating formats
                Dictionary<String, String> formats = GetCxtsFormats(sourceCxt);
                String format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newCxtIDs.Add(id, newId);
                }
                else
                {
                    if (finalCxtIDs.Contains(id))
                    {
                        int tempID = 0;
                        newId =
                            (finalCxtIDs.Where(i => Int32.TryParse(i, out tempID))
                                        .ToList()
                                        .ConvertAll(Convert.ToInt32)
                                        .Max() + 1).ToString();
                        finalCxtIDs.Add(newId);
                        newCxtIDs.Add(id, newId);
                    }

                    sourceCxt.SetAttribute("id", newId);

                    var cxtFmts = sourceCxt.GetElementsByTagName("fmt");
                    foreach (var cxtFmt in cxtFmts.OfType<XmlElement>())
                    {
                        String fmtId = ((XmlElement) cxtFmt).Attributes["id"].Value;
                        if (newFmtIDs.Keys.Contains(fmtId))
                            ((XmlElement) cxtFmt).SetAttribute("id", newFmtIDs[fmtId]);
                    }
                    cxtFmts = null;
                    _cxtsToBeAdded.Add(sourceCxt.OuterXml);
                }
            }
            sourceCxts = null;
            sourceCxtDefs = null;
            _sdlxliffSliceOrChange.StepProcess("Merging Cxt, done. ", false);
            return newCxtIDs;
        }

        private Dictionary<string, string> GetCxtsFormats(XmlElement cxt)
        {
            List<String> attributes = new List<string>() { "type", "code", "name", "descr", "color", "purpose" };
            Dictionary<String, String> formats = new Dictionary<string, string>();
            foreach (var attribute in attributes)
            {
                if (cxt.HasAttribute(attribute))
                    formats.Add(attribute, cxt.Attributes[attribute].Value);
            }
            XmlNodeList fmts = cxt.GetElementsByTagName("fmt");
            List<String> fmtIDs = new List<string>();
            foreach (XmlElement fmt in fmts.OfType<XmlElement>())
            {
                fmtIDs.Add(fmt.Attributes["id"].Value);
            }
            fmts = null;
            formats.Add("fmt", String.Join(",", fmtIDs.ToArray()));
            XmlNodeList props = cxt.GetElementsByTagName("props");
            if (props.Count > 0)
            {
                XmlNodeList values = ((XmlElement)props[0]).GetElementsByTagName("value");
                foreach (XmlElement value in values.OfType<XmlElement>())
                {
                    if (value.HasAttribute("key"))
                        formats.Add(value.Attributes["key"].Value, value.InnerText);
                }
                values = null;
            }
            props = null;

            return formats;
        }

        private Dictionary<string, string> MergeFmts(String finalFile, XmlElement sourceDocHeaderElement)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging fonts ... ", false);
            Dictionary<String, String> newFmtIDs = new Dictionary<string, string>();

            XmlNodeList fmtDefinitions = sourceDocHeaderElement.GetElementsByTagName("fmt-defs");
            if (fmtDefinitions.Count == 0)
                return newFmtIDs;

            XmlElement sourceFmtDefs = (XmlElement)fmtDefinitions[0];

            //XmlNodeList finalFmts = finalFmtDefs.GetElementsByTagName("fmt-def");
            List<String> finalFmtIDs = new List<String>();
            Dictionary<String, String> finalFmtFormats = new Dictionary<string, string>();


            using (XmlReader reader = XmlReader.Create(finalFile))
            {
                while (reader.Read())
                {
                    if (reader.Name == "fmt-defs")
                    {
                        XmlDocument fmtDoc = new XmlDocument();
                        fmtDoc.PreserveWhitespace = true;
                        fmtDoc.LoadXml(reader.ReadOuterXml());
                        XmlNodeList fmtDefs = fmtDoc.DocumentElement.GetElementsByTagName("fmt-def");

                        foreach (XmlElement fmt in fmtDefs.OfType<XmlElement>())
                        {
                            string id = fmt.Attributes["id"].Value;
                            finalFmtIDs.Add(id);

                            XmlNodeList fmtValues = fmt.GetElementsByTagName("value");
                            Dictionary<String, String> formats = fmtValues.Cast<XmlElement>().ToDictionary(fmtValue => fmtValue.Attributes["key"].Value, fmtValue => fmtValue.InnerText);

                            String format = GenerateFormat(formats);
                            if (!finalFmtFormats.Keys.Contains(format))
                                finalFmtFormats.Add(format, id);
                        }
                    }
                }
            } 

            XmlNodeList sourceFmts = sourceFmtDefs.GetElementsByTagName("fmt-def");
            _sdlxliffSliceOrChange.StepProcess(sourceFmts.Count.ToString() + " fmts to process ... ", false);

            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (XmlElement sourceFmt in sourceFmts.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceFmts.Count - (itemsResetCounts * itemsProcessed) + " fmts to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }
                String id = sourceFmt.Attributes["id"].Value;
                String newId = id;

                Dictionary<String, String> formats = new Dictionary<string, string>();
                XmlNodeList values = sourceFmt.GetElementsByTagName("value");
                foreach (XmlElement value in values.OfType<XmlElement>())
                {
                    if (value.HasAttribute("key"))
                        formats.Add(value.Attributes["key"].Value, value.InnerText);
                }
                values = null;
                String format = GenerateFormat(formats);
                if (finalFmtFormats.ContainsKey(format))
                {
                    newId = finalFmtFormats[format];
                    newFmtIDs.Add(id, newId);
                }
                else
                {
                    if (finalFmtIDs.Contains(id))
                    {
                        int tempID = 0;
                        newId =
                            (finalFmtIDs.Where(i => Int32.TryParse(i, out tempID))
                                        .ToList()
                                        .ConvertAll(Convert.ToInt32)
                                        .Max() + 1).ToString();
                        finalFmtIDs.Add(newId);
                        newFmtIDs.Add(id, newId);
                    }

                    sourceFmt.SetAttribute("id", newId);
                    _fontsToBeAdded.Add(sourceFmt.OuterXml);
                }
            }

            sourceFmtDefs = null;
            _sdlxliffSliceOrChange.StepProcess("Merging fonts, done! ", false);

            return newFmtIDs;
        }

        private string GenerateFormat(Dictionary<string, string> formats)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<KeyValuePair<string, string>> orderedFormats = formats.OrderBy(style => style.Key);
            foreach (var orderedFormat in orderedFormats)
            {
                sb.Append(String.Format("{0}:{1};", orderedFormat.Key, orderedFormat.Value));
            }
            return sb.ToString();
        }

        public void SliceFile(string file, SliceInfo sliceInfo)
        {
            try
            {
                _sdlxliffSliceOrChange.StepProcess("Sliceing file: " + Path.GetFileName(file) + ".", false); 

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

                    List<XmlNode> removedGroups = new List<XmlNode>();
                    foreach (var groupElement in groupElements.OfType<XmlElement>())
                    {
                        SliceInBody(sliceInfo, groupElement, removedGroups);
                    }

                    foreach (var xmlNode in removedGroups.OfType<XmlElement>())
                        ((XmlElement) bodyElement).RemoveChild(xmlNode);

                    SliceInBody(sliceInfo, bodyElement, null);
                    bodyElement = null;
                    groupElements = null;
                }
                Encoding encoding = new UTF8Encoding();
                if (!String.IsNullOrEmpty(xmlEncoding))
                    encoding = Encoding.GetEncoding(xmlEncoding);
                using (var writer = new XmlTextWriter(file, encoding))
                {
                    //writer.Formatting = Formatting.None;
                    xDoc.Save(writer);
                }

                xDoc = null;
                fileList = null;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static void SliceInBody(SliceInfo sliceInfo, object groupElement, List<XmlNode> removedGroups)
        {
//look in segments
            var transUnits =
                ((XmlElement) groupElement).ChildNodes.OfType<XmlElement>()
                    .Where(node => node.Name == "trans-unit")
                    .ToList();

            int transUnitCount = transUnits.Count;
            var removedTransUnits = new List<XmlNode>();

            foreach (var transUnit in transUnits)
            {
                // Skip structure translation units.
                if (transUnit.GetAttribute("translate") == "no")
                {
                    continue;
                }

                String transUnitID = ((XmlElement) transUnit).Attributes["id"].Value;

                bool removeAllSegments = sliceInfo.Segments.All(seg => seg.Key != transUnitID);

                XmlNodeList segDefs = ((XmlElement) transUnit).GetElementsByTagName("sdl:seg-defs");
                int segDefsCount = segDefs.Count;
                List<XmlNode> removedSegDefs = new List<XmlNode>();
                foreach (var segDef in segDefs.OfType<XmlElement>())
                {
                    XmlNodeList segments = ((XmlElement) segDef).GetElementsByTagName("sdl:seg");
                    int segmentsCount = segments.Count;
                    List<XmlNode> removedSegments = new List<XmlNode>();
                    foreach (var segment in segments.OfType<XmlElement>())
                    {
                        String SegmentId = ((XmlElement) segment).Attributes["id"].Value;

                        if (removeAllSegments ||
                            !sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(SegmentId)))
                            removedSegments.Add((XmlNode) segment);
                    }
                    segments = null;
                    foreach (var xmlNode in removedSegments.OfType<XmlElement>())
                        ((XmlElement) segDef).RemoveChild(xmlNode);
                    if (segmentsCount == removedSegments.Count)
                        removedSegDefs.Add((XmlNode) segDef);
                    removedSegments.Clear();
                }

                segDefs = null;
                foreach (var xmlNode in removedSegDefs.OfType<XmlElement>())
                    ((XmlElement) transUnit).RemoveChild(xmlNode);
                if (segDefsCount == removedSegDefs.Count)
                {
                    removedTransUnits.Add((XmlNode) transUnit);
                }
                else //only if I don't delete the entire trans-unit
                {
                    RemoveSegmentFromSourceOrTarget(sliceInfo, transUnit, transUnitID, "source");
                    RemoveSegmentFromSourceOrTarget(sliceInfo, transUnit, transUnitID, "target");
                    RemoveSegmentFromSourceOrTarget(sliceInfo, transUnit, transUnitID, "seg-source");
                    RemoveSegmentFromSourceOrTarget(sliceInfo, transUnit, transUnitID, "target-source");
                }
                removedSegDefs.Clear();
            }

            foreach (var xmlNode in removedTransUnits.OfType<XmlElement>())
            {
                ((XmlElement) groupElement).RemoveChild(xmlNode);
            }

            if (removedGroups != null)
            {
                if (transUnitCount == removedTransUnits.Count)
                {
                    removedGroups.Add((XmlNode) groupElement);
                }
            }

            removedTransUnits.Clear();
        }

        private static void RemoveSegmentFromSourceOrTarget(SliceInfo sliceInfo, object transUnit, string transUnitID, String tagName)
        {
            XmlNodeList segs = ((XmlElement)transUnit).GetElementsByTagName(tagName);
            if (segs.Count > 0)
            {
                XmlElement firstSeg = (XmlElement) segs[0];
                XmlNodeList mrks = firstSeg.GetElementsByTagName("mrk");
                List<XmlNode> removedMrks = new List<XmlNode>();
                foreach (var mrk in mrks.OfType<XmlElement>())
                {
                    if (!((XmlElement) mrk).HasAttribute("mtype") || ((XmlElement) mrk).Attributes["mtype"].Value != "seg")
                        continue;

                    String mrkID = String.Empty;
                    if (((XmlElement) mrk).HasAttribute("mid"))
                    {
                        mrkID = ((XmlElement) mrk).Attributes["mid"].Value;

                        if (!sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(mrkID)))
                            removedMrks.Add((XmlNode) mrk);
                    }
                }
                mrks = null;
                foreach (var xmlNode in removedMrks.OfType<XmlElement>())
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
                removedMrks.Clear();
            }
            segs = null;
        }
    }
}