using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using NLog;

namespace SDLXLIFFSliceOrChange
{
    public class SliceManager
    {
        private SDLXLIFFSliceOrChange _sdlxliffSliceOrChange;
        private List<string> _fontsToBeAdded;
        private List<string> _cxtsToBeAdded;
        private List<string> _tagsToBeAdded;
        private List<string> _nodesToBeAdded;
        private List<string> _groupsToBeAdded;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public SliceManager(SDLXLIFFSliceOrChange sdlxliffSliceOrChange)
        {
            _sdlxliffSliceOrChange = sdlxliffSliceOrChange;
        }

        public void MergeSplitFiles(List<KeyValuePair<string, List<string>>> filesPerLanguage)
        {
           var threads = new List<Thread>();
            foreach (var keyValuePair in filesPerLanguage)
            {
	            var language = keyValuePair.Key;
                var files = keyValuePair.Value;
                var t = new Thread(() => MergeSplitFilesPerLanguage(language, files));
                t.Start();
                threads.Add(t);
            }

            foreach (var thread in threads)
                thread.Join();
        }

        private void MergeSplitFilesPerLanguage(string language, List<string> files)
        {
	        try
	        {
		        if (files !=null && files.Count != 0)
		        {
			        var finalFile = Path.Combine(_sdlxliffSliceOrChange._folderForSlicedFiles, $"{language}_{"sliced"}.sdlxliff");
			        File.Copy(files[0], finalFile, true);

			        var xFinalDoc = new XmlDocument {PreserveWhitespace = true};
			        xFinalDoc.Load(finalFile);
			        var xmlEncoding = "utf-8";

			        if (xFinalDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
			        {
				        // Get the encoding declaration.
				        var decl = (XmlDeclaration) xFinalDoc.FirstChild;
				        xmlEncoding = decl.Encoding;
			        }

			        var fileList = xFinalDoc.DocumentElement.GetElementsByTagName("file");
			        foreach (var finalDocFileElement in fileList.OfType<XmlElement>())
			        {
				        var finalDocHeaderElement = (XmlElement)finalDocFileElement.GetElementsByTagName("header")[0];
				        var referencesNodes = finalDocHeaderElement.GetElementsByTagName("reference");
				        var referencesToBeRemoved = new List<XmlNode>();
				        foreach (var referencesNode in referencesNodes.OfType<XmlElement>())
					        referencesToBeRemoved.Add(referencesNode);
				        foreach (var referenceToBeRemoved in referencesToBeRemoved.OfType<XmlElement>())
					        finalDocHeaderElement.RemoveChild(referenceToBeRemoved);
				        referencesToBeRemoved.Clear();
			        }

			        Encoding encoding = new UTF8Encoding();
			        if (!string.IsNullOrEmpty(xmlEncoding))
				        encoding = Encoding.GetEncoding(xmlEncoding);
			        using (var writer = new XmlTextWriter(finalFile, encoding))
			        {
				        xFinalDoc.Save(writer);
			        }

			        if (files.Count >= 2)
			        {
				        for (int i = 1; i < files.Count; i++)
				        {
					        Merge2Files(finalFile, files[i]);
				        }
			        }
		        }
	        }
	        catch (Exception ex)
	        {
		        _logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
	        }
        }

        private void Merge2Files(string finalFile, string sourceFile)
        {
            _fontsToBeAdded = new List<string>();
            _cxtsToBeAdded = new List<string>();
            _tagsToBeAdded = new List<string>();
            _nodesToBeAdded = new List<string>();
            _groupsToBeAdded = new List<string>();

            var xSourceDoc = new XmlDocument();
            xSourceDoc.PreserveWhitespace = true;
            xSourceDoc.Load(sourceFile);
            var fileList = xSourceDoc.DocumentElement?.GetElementsByTagName("file");
            if (fileList != null)
            {
	            foreach (var sourceDocFileElement in fileList.OfType<XmlElement>())
	            {
		            var sourceDocBodyElement = (XmlElement) sourceDocFileElement.GetElementsByTagName("body")[0];
		            var sourceDocHeaderElement = (XmlElement) sourceDocFileElement.GetElementsByTagName("header")[0];

					// fmts
					Dictionary<string, string> newFmtIDs = null;
					var t = new Thread(() => newFmtIDs = MergeFmts(finalFile, sourceDocHeaderElement));
					t.Start();
					t.Join();

					// cxt
					Dictionary<string, string> newCxtsIDs = null;
					var t1 = new Thread(() => newCxtsIDs = MergeCxt(finalFile, sourceDocHeaderElement, newFmtIDs));
					t1.Start();

					// tags
					Dictionary<string, string> newTagsIDs = null;
					var t2 = new Thread(() => newTagsIDs = MergeTags(finalFile, sourceDocHeaderElement, newFmtIDs));
					t2.Start();
					t1.Join();

					// nodes
					Dictionary<string, string> newNodesIDs = null;
					var t3 = new Thread(() => newNodesIDs = MergeNodes(finalFile, sourceDocHeaderElement, newCxtsIDs));
					t3.Start();

					//get all SegmentIds from finalDoc
					_sdlxliffSliceOrChange.StepProcess("Geting segments ... ");

		            var segmentIds = GetSegmentIds(finalFile);

		            _sdlxliffSliceOrChange.StepProcess("Geting segments, done. ");

		            t2.Join();
		            t3.Join();
		            sourceDocHeaderElement = null;
		            _sdlxliffSliceOrChange.StepProcess("Merging body ... ");
					
		            //body - segments
		            var sourceGroups = sourceDocBodyElement.GetElementsByTagName("group");
		            _sdlxliffSliceOrChange.StepProcess(sourceGroups.Count.ToString() + " groups to process ... ");

		            var itemsProcessed = 0;
		            var itemsResetCounts = 1;
		            foreach (var sourceGroup in sourceGroups.OfType<XmlElement>())
		            {
			            itemsProcessed++;
			            if (itemsProcessed == 5000)
			            {
				            _sdlxliffSliceOrChange.StepProcess(sourceGroups.Count - (itemsResetCounts * itemsProcessed) + " groups to process ... ");
				            itemsProcessed = 0;
				            itemsResetCounts++;
			            }

						// update cxts and nodes
						UpdateCXT(sourceGroup, newCxtsIDs, newNodesIDs);
						
						//update tags (x an g elements)
						UpdateTags(sourceGroup, newTagsIDs, ref segmentIds);
						
						_groupsToBeAdded.Add(sourceGroup.OuterXml);
		            }

					// update cxts and nodes
					UpdateCxtsElements(sourceDocBodyElement, newCxtsIDs, newNodesIDs);

					 // update tags (x an g elements)
					UpdateTagsElements(sourceDocBodyElement, newTagsIDs, ref segmentIds);
		            _sdlxliffSliceOrChange.StepProcess("Merging body, done. ");
	            }
            }

            _sdlxliffSliceOrChange.StepProcess("Finnishing merge ... ");
            AddingElementsToFinalFile(finalFile);
            _sdlxliffSliceOrChange.StepProcess("Merging files, done. ");
        }

		private List<string> GetSegmentIds(string finalFile)
        {
	        var segmentIds = new List<string>();
	        using (var reader = XmlReader.Create(finalFile))
	        {
		        while (reader.Read())
		        {
			        if (reader.Name == "sdl:seg-defs")
			        {
				        var segDoc = new XmlDocument();
				        segDoc.PreserveWhitespace = true;
				        segDoc.LoadXml(reader.ReadOuterXml());
				        var segDefs = segDoc.DocumentElement?.GetElementsByTagName("sdl:seg");
				        if (segDefs != null)
				        {
					        foreach (var seg in segDefs.OfType<XmlElement>())
					        {
						        var segmentId = seg.Attributes["id"].Value;
						        if (!segmentIds.Contains(segmentId))
							        segmentIds.Add(segmentId);
					        }
				        }
			        }
		        }
	        }

	        return segmentIds;
        }

        private void UpdateCXT(XmlElement sourceGroup, Dictionary<string, string> newCxtsIDs, Dictionary<string, string> newNodesIDs)
        {
	        var cxtsElements = sourceGroup.GetElementsByTagName("sdl:cxts");
	        if (cxtsElements.Count > 0)
	        {
		        var cxts = (XmlElement)cxtsElements[0];

		        //cxt elements
		        var cxtElements = cxts.GetElementsByTagName("sdl:cxt");
		        foreach (var cxtElement in cxtElements.OfType<XmlElement>())
		        {
			        var id = cxtElement.Attributes["id"].Value;
			        if (Enumerable.Contains(newCxtsIDs.Keys, id))
				        cxtElement.SetAttribute("id", newCxtsIDs[id]);
		        }

		        //node elements
		        var nodeElements = cxts.GetElementsByTagName("sdl:node");
		        foreach (var nodeElement in nodeElements.OfType<XmlElement>())
		        {
			        var id = nodeElement.Attributes["id"].Value;
			        if (Enumerable.Contains(newNodesIDs.Keys, id))
				        nodeElement.SetAttribute("id", newNodesIDs[id]);
		        }
	        }
		}

        private void UpdateTags(XmlElement sourceGroup, Dictionary<string,string> newTagsIDs,ref List<string> segmentIds)
        {
	        var transUnitElements = sourceGroup.GetElementsByTagName("trans-unit");
	        if (transUnitElements.Count > 0)
	        {
		        foreach (var transUnitElement in transUnitElements.OfType<XmlElement>())
		        {
			        var sourceElements = transUnitElement.GetElementsByTagName("source");
			        foreach (var sourceElement in sourceElements.OfType<XmlElement>())
			        {
				        UpdateGandX(sourceElement, newTagsIDs);
			        }
			        var segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
			        foreach (var segSourceElement in segSourceElements.OfType<XmlElement>())
			        {
				        UpdateGandX(segSourceElement, newTagsIDs);
			        }

			        var targetElements = transUnitElement.GetElementsByTagName("target");
			        foreach (var targetElement in targetElements.OfType<XmlElement>())
			        {
				        UpdateGandX(targetElement, newTagsIDs);
			        }

			        var segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
			        foreach (var segTargetElement in segTargetElements.OfType<XmlElement>())
			        {
				        UpdateGandX(segTargetElement, newTagsIDs);
			        }

			        UpdateSegmentsIDs(transUnitElement, ref segmentIds);
		        }
	        }
		}
        private void UpdateCxtsElements(XmlElement sourceDocBodyElement, Dictionary<string,string> newCxtsIDs, Dictionary<string, string> newNodesIDs)
        {
	        var cxtsElementsInBody = sourceDocBodyElement.ChildNodes;
			
	        if (cxtsElementsInBody!=null && cxtsElementsInBody.Count > 0)
	        {
		        foreach (var cxts in cxtsElementsInBody.OfType<XmlElement>())
		        {
			        if (cxts.Name != "sdl:cxts") continue;

			        var cxtElements = cxts.GetElementsByTagName("sdl:cxt");
			        foreach (var cxtElement in cxtElements.OfType<XmlElement>())
			        {
				        var id = cxtElement.Attributes["id"].Value;
				        if (Enumerable.Contains(newCxtsIDs.Keys, id))
					        cxtElement.SetAttribute("id", newCxtsIDs[id]);
			        }

			        //node elements
			        var nodeElements = cxts.GetElementsByTagName("sdl:node");
			        foreach (var nodeElement in nodeElements.OfType<XmlElement>())
			        {
				        var id = nodeElement.Attributes["id"].Value;
				        if (Enumerable.Contains(newNodesIDs.Keys, id))
					        nodeElement.SetAttribute("id", newNodesIDs[id]);
			        }
		        }
	        }
		}

        private void UpdateTagsElements(XmlElement sourceDocBodyElement, Dictionary<string,string> newTagsIDs, ref List<string> segmentIds)
        {
	        var transUnitElementsInBody = sourceDocBodyElement.ChildNodes;
	        if (transUnitElementsInBody.Count > 0)
	        {
		        foreach (var transUnitElement in transUnitElementsInBody.OfType<XmlElement>())
		        {
			        if (transUnitElement.Name != "trans-unit")
				        continue;
			        var sourceElements = transUnitElement.GetElementsByTagName("source");
			        foreach (var sourceElement in sourceElements.OfType<XmlElement>())
			        {
				        UpdateGandX(sourceElement, newTagsIDs);
			        }

			        var segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
			        foreach (var segSourceElement in segSourceElements.OfType<XmlElement>())
			        {
				        UpdateGandX(segSourceElement, newTagsIDs);
			        }

			        var targetElements = transUnitElement.GetElementsByTagName("target");
			        foreach (var targetElement in targetElements.OfType<XmlElement>())
			        {
				        UpdateGandX(targetElement, newTagsIDs);
			        }

			        var segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
			        foreach (var segTargetElement in segTargetElements.OfType<XmlElement>())
			        {
				        UpdateGandX(segTargetElement, newTagsIDs);
			        }
			        UpdateSegmentsIDs(transUnitElement, ref segmentIds);
			        _groupsToBeAdded.Add(transUnitElement.OuterXml);
		        }
	        }
		}

        private void AddingElementsToFinalFile(string finalFile)
        {
            string fileContent;
            using (var sr = new StreamReader(finalFile))
            {
                fileContent = sr.ReadToEnd();
            }
            var replaceWhat = @"</file-info>";
            if (fileContent.IndexOf(@"/sdl:filetype-info>") != -1)
                replaceWhat = @"</sdl:filetype-info>";

            if (_fontsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</fmt-defs>") == -1)
                {
                    fileContent = fileContent.Replace(replaceWhat, replaceWhat+@"<fmt-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></fmt-defs>");
                }
                var fonts = $"{string.Join("", _fontsToBeAdded)}</fmt-defs>";
                fileContent = fileContent.Replace("</fmt-defs>", fonts.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", string.Empty));
            }

            if (_cxtsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</cxt-defs>") == -1)
                {
                    if (fileContent.IndexOf("</fmt-defs>") != -1)
                        replaceWhat = @"</fmt-defs>";
                    fileContent = fileContent.Replace(replaceWhat, replaceWhat+@"<cxt-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></cxt-defs>");
                }
                var cxts = $"{string.Join("", _cxtsToBeAdded)}</cxt-defs>";
                fileContent = fileContent.Replace("</cxt-defs>", cxts.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", string.Empty));
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
                var nodes = $"{string.Join("", _nodesToBeAdded)}</node-defs>";
                fileContent = fileContent.Replace("</node-defs>", nodes.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", string.Empty));
            }

            if (_tagsToBeAdded.Any())
            {
                if (fileContent.IndexOf("</tag-defs>") == -1)
                {
                    fileContent = fileContent.Replace("</header>", @"<tag-defs xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""></tag-defs></header>");
                }

                var tags = $"{string.Join("", _tagsToBeAdded)}</tag-defs>";
                fileContent = fileContent.Replace("</tag-defs>", tags.Replace(@"xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0""", string.Empty));
            }

            if (_groupsToBeAdded.Any())
            {
	            var groups = $"{string.Join("", _groupsToBeAdded)}</body>";
                fileContent = fileContent.Replace("</body>", groups);
            }

            using (var sw = new StreamWriter(finalFile, false))
            {
                sw.Write(fileContent);
            }
        }

        private void UpdateSegmentsIDs(XmlElement transUnitElement, ref List<string> segmentIds)
        {
	        var newId = (segmentIds.Where(i => int.TryParse(i, out _)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
	        var segDefElements = transUnitElement.GetElementsByTagName("sdl:seg-defs");
	        if (segDefElements.Count > 0)
	        {
		        var segDefs = (XmlElement) segDefElements[0];
		        var segments = segDefs.GetElementsByTagName("sdl:seg");
		        foreach (var segment in segments.OfType<XmlElement>())
		        {
			        var segmentId = segment.Attributes["id"].Value;
			        if (segmentIds.Contains(segmentId))
			        {
				        segmentIds.Add(newId);
				        segment.SetAttribute("id", newId);

				        var sourceElements = transUnitElement.GetElementsByTagName("source");
				        foreach (var sourceElement in sourceElements.OfType<XmlElement>())
				        {
					        UpdateMrkMID(sourceElement, segmentId, newId);
				        }

				        var segSourceElements = transUnitElement.GetElementsByTagName("seg-source");
				        foreach (var segSourceElement in segSourceElements.OfType<XmlElement>())
				        {
					        UpdateMrkMID(segSourceElement, segmentId, newId);
				        }

				        var targetElements = transUnitElement.GetElementsByTagName("target");
				        foreach (var targetElement in targetElements.OfType<XmlElement>())
				        {
					        UpdateMrkMID(targetElement, segmentId, newId);
				        }

				        var segTargetElements = transUnitElement.GetElementsByTagName("seg-target");
				        foreach (var segTargetElement in segTargetElements.OfType<XmlElement>())
				        {
					        UpdateMrkMID(segTargetElement, segmentId, newId);
				        }
			        }
		        }
	        }
        }

        private void UpdateMrkMID(XmlElement element, string oldId, string newId)
        {
            var mrks = element.GetElementsByTagName("mrk");
            foreach (var mrk in mrks.OfType<XmlElement>())
            {
                if (mrk.HasAttribute("mtype") && mrk.Attributes["mtype"].Value == "seg" && mrk.HasAttribute("mid"))
                {
	                var mId = mrk.Attributes["mid"].Value;
                    if (mId == oldId)
                    {
                        mrk.SetAttribute("mid", newId);
                    }
                }
            }
        }

        private static void UpdateGandX(XmlElement element, Dictionary<string, string> newTagsIDs)
        {
            var xList = element.GetElementsByTagName("x");
            foreach (var x in xList.OfType<XmlElement>())
            {
                if (x.HasAttribute("id") && newTagsIDs.Keys.Contains(x.Attributes["id"].Value))
                    x.SetAttribute("id", newTagsIDs[x.Attributes["id"].Value]);
            }
            var gList = element.GetElementsByTagName("g");
            foreach (var g in gList.OfType<XmlElement>())
            {
                if (g.HasAttribute("id") && newTagsIDs.Keys.Contains(g.Attributes["id"].Value))
                    g.SetAttribute("id", newTagsIDs[g.Attributes["id"].Value]);
            }
        }

        private Dictionary<string, string> MergeNodes(string finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newCxtsIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging Nodes ... ");
            var newNodeIDs = new Dictionary<string, string>();

            var nodeDefinitions = sourceDocHeaderElement.GetElementsByTagName("node-defs");
            if (nodeDefinitions.Count == 0)
                return newNodeIDs;

            var sourceNodeDefs = (XmlElement)nodeDefinitions[0];

            var finalNodeIDs = new List<string>();
            var finalFormats = new Dictionary<string, string>();

            using (var reader = XmlReader.Create(finalFile))
            {
                while (reader.Read())
                {
                    if (reader.Name == "node-defs")
                    {
	                    var nodeDoc = new XmlDocument();
                        nodeDoc.PreserveWhitespace = true;
                        nodeDoc.LoadXml(reader.ReadOuterXml());
                        var nodeDefs = nodeDoc.DocumentElement.GetElementsByTagName("node-def");

                        foreach (var node in nodeDefs.OfType<XmlElement>())
                        {
	                        var id = node.Attributes["id"].Value;
                            finalNodeIDs.Add(id);

                            var formats = GetNodeFormats(node);
                            var format = GenerateFormat(formats);
                            if (!finalFormats.ContainsKey(format))
                                finalFormats.Add(format, id);
                        }
                    }
                }
            }

            var sourceNodes = sourceNodeDefs.GetElementsByTagName("node-def");
            _sdlxliffSliceOrChange.StepProcess(sourceNodes.Count.ToString() + " nodes to process ... ");
            
            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (var sourceNode in sourceNodes.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceNodes.Count - (itemsResetCounts * itemsProcessed) + " nodes to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }
                var id = sourceNode.Attributes["id"].Value;
                var newId = id;
				//generating formats
				var formats = GetNodeFormats(sourceNode);
				var format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newNodeIDs.Add(id, newId);
                }
                else
                {
                    if (finalNodeIDs.Contains(id))
                    {
                        newId = (finalNodeIDs.Where(i => int.TryParse(i, out _)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
                        finalNodeIDs.Add(newId);
                        newNodeIDs.Add(id, newId);
                    }

                    sourceNode.SetAttribute("id", newId);

                    var nodeCxts = sourceNode.GetElementsByTagName("cxt");
                    foreach (var nodeCxt in nodeCxts.OfType<XmlElement>())
                    {
	                    var fmtId = nodeCxt.Attributes["id"].Value;
                        if (newCxtsIDs.Keys.Contains(fmtId))
                            nodeCxt.SetAttribute("id", newCxtsIDs[fmtId]);
                    }
                    _nodesToBeAdded.Add(sourceNode.OuterXml);
                }
            }

            foreach (var sourceNode in sourceNodes.OfType<XmlElement>())
            {
                if (sourceNode.HasAttribute("parent"))
                {
	                var parentID = sourceNode.Attributes["parent"].Value;
                    if (newNodeIDs.ContainsKey(parentID))
                    {
                        sourceNode.SetAttribute("parent", newNodeIDs[parentID]);
                    }
                }
            }
            _sdlxliffSliceOrChange.StepProcess("Merging Nodes, done. ", false);

            return newNodeIDs;
        }

        private Dictionary<string, string> GetNodeFormats(XmlElement node)
        {
	        var attributes = new List<string> { "force-name", "parent" };
	        var formats = new Dictionary<string, string>();
            foreach (var attribute in attributes)
            {
                if (node.HasAttribute(attribute))
                    formats.Add(attribute, node.Attributes[attribute].Value);
            }
            var cxts = node.GetElementsByTagName("cxt");
            var cxtIDs = new List<string>();
            foreach (var cxt in cxts.OfType<XmlElement>())
            {
                cxtIDs.Add(cxt.Attributes["id"].Value);
            }
            formats.Add("cxt", string.Join(",", cxtIDs.ToArray()));

            return formats;
        }

        private Dictionary<string, string> MergeTags(string finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newFmtIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging tags ... ", false);
            var newTagIDs = new Dictionary<string, string>();

            var tagDefinitions = sourceDocHeaderElement.GetElementsByTagName("tag-defs");
            if (tagDefinitions.Count == 0)
                return newTagIDs;

            var sourceTagsDefs = (XmlElement)tagDefinitions[0];

			var finalTagIDs = new List<string>();
			var finalFormats = new Dictionary<string, string>();

            using (var reader = XmlReader.Create(finalFile))
            {
                while (reader.Read())
                {
	                if (reader.Name == "tag-defs")
	                {
		                var tagDoc = new XmlDocument();
		                tagDoc.PreserveWhitespace = true;
		                tagDoc.LoadXml(reader.ReadOuterXml());
		                var tagDefs = tagDoc.DocumentElement?.GetElementsByTagName("tag");
		                if (tagDefs != null)
		                {
			                foreach (var tag in tagDefs.OfType<XmlElement>())
			                {
								var id = tag.Attributes["id"].Value;
				                finalTagIDs.Add(id);

				                var formats = GetTagFormats(tag);
				                var format = GenerateFormat(formats);
				                if (!finalFormats.ContainsKey(format))
					                finalFormats.Add(format, id);
			                }
		                }
	                }
                }
            }

            var sourceTags = sourceTagsDefs.GetElementsByTagName("tag");
            _sdlxliffSliceOrChange.StepProcess(sourceTags.Count.ToString() + " tags to process ... ");
            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (var sourceTag in sourceTags.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceTags.Count - (itemsResetCounts * itemsProcessed) + " tags to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }

                var id = sourceTag.Attributes["id"].Value;
                var newId = id;
				//generating formats
				var formats = GetTagFormats(sourceTag);
				var format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newTagIDs.Add(id, newId);
                }
                else
                {
                    if (finalTagIDs.Contains(id))
                    {
                        newId = (finalTagIDs.Where(i => int.TryParse(i, out _)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
                        finalTagIDs.Add(newId);
                        newTagIDs.Add(id, newId);
                    }

                    sourceTag.SetAttribute("id", newId);

                    var tagFmts = sourceTag.GetElementsByTagName("fmt");
                    foreach (var TagFmt in tagFmts.OfType<XmlElement>())
                    {
	                    var fmtId = TagFmt.Attributes["id"].Value;
                        if (newFmtIDs.Keys.Contains(fmtId))
                            TagFmt.SetAttribute("id", newFmtIDs[fmtId]);
                    }
                    _tagsToBeAdded.Add(sourceTag.OuterXml);
                }
            }
            _sdlxliffSliceOrChange.StepProcess("Merging tags, done. ");
            return newTagIDs;
        }

        private Dictionary<string, string> GetTagFormats(XmlElement tag)
        {
	        var formats = new Dictionary<string, string>();
	        var elements = new List<string> {"bpt", "ept", "ph", "st"};
	        var props = new List<string> { "bpt-props", "ept-props", "props" };

            foreach (var element in elements)
            {
	            var childElements = tag.GetElementsByTagName(element);
                if (childElements.Count > 0)
                {
	                var childElement = (XmlElement)childElements[0];
                    foreach (XmlAttribute attribute in childElement.Attributes)
                    {
                        if (attribute.Name == "id")
                            continue;
                        formats.Add($"{element}-{attribute.Name}", attribute.Value);
                    }
                }
            }

            foreach (var prop in props)
            {
	            var properties = tag.GetElementsByTagName(prop);
                if (properties.Count > 0)
                {
	                var neededProp = (XmlElement) properties[0];
	                var values = neededProp.GetElementsByTagName("value");
                    foreach (var value in values.OfType<XmlElement>())
                    {
                        if (value.HasAttribute("key"))
                            formats.Add($"{prop}-{value.Attributes["key"].Value}", value.InnerText);
                    }
                }
            }

			var fmts = tag.GetElementsByTagName("fmt");
			var fmtIDs = new List<string>();
            foreach (XmlElement fmt in fmts.OfType<XmlElement>())
            {
                fmtIDs.Add(fmt.Attributes["id"].Value);
            }
            formats.Add("fmt", string.Join(",", fmtIDs.ToArray()));

            return formats;
        }

        private Dictionary<string, string> MergeCxt(string finalFile, XmlElement sourceDocHeaderElement, Dictionary<string, string> newFmtIDs)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging Cxt ... ");
            var newCxtIDs = new Dictionary<string, string>();

            var cxtDefinitions = sourceDocHeaderElement.GetElementsByTagName("cxt-defs");
            if (cxtDefinitions.Count == 0)
                return newCxtIDs;
            var sourceCxtDefs = (XmlElement)cxtDefinitions[0];
            var finalCxtIDs = new List<string>();
            var finalFormats = new Dictionary<string, string>();

            using (var tr = new StreamReader(finalFile))
            {
                using (var reader = XmlReader.Create(tr))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "cxt-defs")
                        {
	                        var ctxDoc = new XmlDocument();
                            ctxDoc.PreserveWhitespace = true;
                            ctxDoc.LoadXml(reader.ReadOuterXml());
                            var cxtDefs = ctxDoc.DocumentElement?.GetElementsByTagName("cxt-def");
                            if (cxtDefs != null)
                            {
	                            foreach (var cxt in cxtDefs.OfType<XmlElement>())
	                            {
		                            var id = cxt.Attributes["id"].Value;
		                            finalCxtIDs.Add(id);

		                            var formats = GetCxtsFormats(cxt);
		                            var format = GenerateFormat(formats);
		                            if (!finalFormats.ContainsKey(format))
			                            finalFormats.Add(format, id);
	                            }
                            }
                        }
                    }
                }
            }
            var sourceCxts = sourceCxtDefs.GetElementsByTagName("cxt-def");
            _sdlxliffSliceOrChange.StepProcess(sourceCxts.Count.ToString() + " cxts to process ... ", false);

            int itemsProcessed = 0;
            int itemsResetCounts = 1;
            foreach (var sourceCxt in sourceCxts.OfType<XmlElement>())
            {
                itemsProcessed++;
                if (itemsProcessed == 5000)
                {
                    _sdlxliffSliceOrChange.StepProcess(sourceCxts.Count - (itemsResetCounts * itemsProcessed) + " cxts to process ... ", false);
                    itemsProcessed = 0;
                    itemsResetCounts++;
                }

                var id = sourceCxt.Attributes["id"].Value;
                var newId = id;

				//generating formats
				var formats = GetCxtsFormats(sourceCxt);
				var format = GenerateFormat(formats);
                if (finalFormats.ContainsKey(format))
                {
                    newId = finalFormats[format];
                    newCxtIDs.Add(id, newId);
                }
                else
                {
                    if (finalCxtIDs.Contains(id))
                    {
                        newId = (finalCxtIDs.Where(i => int.TryParse(i, out _)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
                        finalCxtIDs.Add(newId);
                        newCxtIDs.Add(id, newId);
                    }

                    sourceCxt.SetAttribute("id", newId);

                    var cxtFmts = sourceCxt.GetElementsByTagName("fmt");
                    foreach (var cxtFmt in cxtFmts.OfType<XmlElement>())
                    {
	                    var fmtId = cxtFmt.Attributes["id"].Value;
                        if (newFmtIDs.Keys.Contains(fmtId))
                            cxtFmt.SetAttribute("id", newFmtIDs[fmtId]);
                    }
                    _cxtsToBeAdded.Add(sourceCxt.OuterXml);
                }
            }
            _sdlxliffSliceOrChange.StepProcess("Merging Cxt, done. ");
            return newCxtIDs;
        }

        private Dictionary<string, string> GetCxtsFormats(XmlElement cxt)
        {
	        var attributes = new List<string> { "type", "code", "name", "descr", "color", "purpose" };
	        var formats = new Dictionary<string, string>();
            foreach (var attribute in attributes)
            {
                if (cxt.HasAttribute(attribute))
                    formats.Add(attribute, cxt.Attributes[attribute].Value);
            }
            var fmts = cxt.GetElementsByTagName("fmt");
            var fmtIDs = new List<string>();
            foreach (var fmt in fmts.OfType<XmlElement>())
            {
                fmtIDs.Add(fmt.Attributes["id"].Value);
            }
           
            formats.Add("fmt", string.Join(",", fmtIDs.ToArray()));
            var props = cxt.GetElementsByTagName("props");
            if (props.Count > 0)
            {
	            var values = ((XmlElement)props[0]).GetElementsByTagName("value");
                foreach (var value in values.OfType<XmlElement>())
                {
                    if (value.HasAttribute("key"))
                        formats.Add(value.Attributes["key"].Value, value.InnerText);
                }
            }

            return formats;
        }

        private Dictionary<string, string> MergeFmts(string finalFile, XmlElement sourceDocHeaderElement)
        {
            _sdlxliffSliceOrChange.StepProcess("Merging fonts ... ");
            var newFmtIDs = new Dictionary<string, string>();

            var fmtDefinitions = sourceDocHeaderElement.GetElementsByTagName("fmt-defs");
            if (fmtDefinitions.Count == 0)
                return newFmtIDs;

            var sourceFmtDefs = (XmlElement)fmtDefinitions[0];

			var finalFmtIDs = new List<string>();
			var finalFmtFormats = new Dictionary<string, string>();
			
			ProcessFinalFile(finalFile, finalFmtIDs,finalFmtFormats);

            var sourceFmts = sourceFmtDefs.GetElementsByTagName("fmt-def");
            _sdlxliffSliceOrChange.StepProcess(sourceFmts.Count.ToString() + " fmts to process ... ");
            
            GenerateFormat(sourceFmts, finalFmtFormats, newFmtIDs, finalFmtIDs);
		
			_sdlxliffSliceOrChange.StepProcess("Merging fonts, done! ");

            return newFmtIDs;
        }

        private void GenerateFormat(XmlNodeList sourceFmts, Dictionary<string,string> finalFmtFormats, Dictionary<string,string> newFmtIDs, List<string> finalFmtIDs)
		{
			var itemsProcessed = 0;
			var itemsResetCounts = 1;

			foreach (var sourceFmt in sourceFmts.OfType<XmlElement>())
	        {
		        itemsProcessed++;
		        if (itemsProcessed == 5000)
		        {
			        _sdlxliffSliceOrChange.StepProcess(sourceFmts.Count - (itemsResetCounts * itemsProcessed) + " fmts to process ... ");
			        itemsProcessed = 0;
			        itemsResetCounts++;
		        }
		        var id = sourceFmt.Attributes["id"].Value;
		        var newId = id;

		        var formats = new Dictionary<string, string>();
		        var values = sourceFmt.GetElementsByTagName("value");
		        foreach (var value in values.OfType<XmlElement>())
		        {
			        if (value.HasAttribute("key"))
				        formats.Add(value.Attributes["key"].Value, value.InnerText);
		        }
		        var format = GenerateFormat(formats);
		        if (finalFmtFormats.ContainsKey(format))
		        {
			        newId = finalFmtFormats[format];
			        newFmtIDs.Add(id, newId);
		        }
		        else
		        {
			        if (finalFmtIDs.Contains(id))
			        {
				        newId = (finalFmtIDs.Where(i => int.TryParse(i, out _)).ToList().ConvertAll(Convert.ToInt32).Max() + 1).ToString();
				        finalFmtIDs.Add(newId);
				        newFmtIDs.Add(id, newId);
			        }

			        sourceFmt.SetAttribute("id", newId);
			        _fontsToBeAdded.Add(sourceFmt.OuterXml);
		        }
	        }
		}

        private void ProcessFinalFile(string finalFile, List<string> finalFmtIDs, Dictionary<string,string> finalFmtFormats)
        {
	        using (var reader = XmlReader.Create(finalFile))
	        {
		        while (reader.Read())
		        {
			        if (reader.Name == "fmt-defs")
			        {
				        var fmtDoc = new XmlDocument();
				        fmtDoc.PreserveWhitespace = true;
				        fmtDoc.LoadXml(reader.ReadOuterXml());
				        var fmtDefs = fmtDoc.DocumentElement?.GetElementsByTagName("fmt-def");
				        if (fmtDefs != null)
				        {
					        foreach (var fmt in fmtDefs.OfType<XmlElement>())
					        {
						        var id = fmt.Attributes["id"].Value;
						        finalFmtIDs.Add(id);

						        var fmtValues = fmt.GetElementsByTagName("value");
						        var formats = fmtValues.Cast<XmlElement>().ToDictionary(fmtValue => fmtValue.Attributes["key"].Value, fmtValue => fmtValue.InnerText);

						        var format = GenerateFormat(formats);
						        if (!finalFmtFormats.Keys.Contains(format))
							        finalFmtFormats.Add(format, id);
					        }
				        }
			        }
		        }
	        }
		}

        private string GenerateFormat(Dictionary<string, string> formats)
        {
	        var sb = new StringBuilder();
	        var orderedFormats = formats.OrderBy(style => style.Key);
            foreach (var orderedFormat in orderedFormats)
            {
                sb.Append($"{orderedFormat.Key}:{orderedFormat.Value};");
            }
            return sb.ToString();
        }

        public void SliceFile(string file, SliceInfo sliceInfo)
        {
	        try
	        {
		        _sdlxliffSliceOrChange.StepProcess("Sliceing file: " + Path.GetFileName(file) + ".", false);

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

				        var removedGroups = new List<XmlNode>();
				        foreach (var groupElement in groupElements.OfType<XmlElement>())
				        {
					        SliceInBody(sliceInfo, groupElement, removedGroups);
				        }

				        foreach (var xmlNode in removedGroups.OfType<XmlElement>())
					        bodyElement.RemoveChild(xmlNode);

				        SliceInBody(sliceInfo, bodyElement, null);
			        }
		        }

		        Encoding encoding = new UTF8Encoding();
		        if (!string.IsNullOrEmpty(xmlEncoding))
			        encoding = Encoding.GetEncoding(xmlEncoding);
		        using (var writer = new XmlTextWriter(file, encoding))
		        {
			        xDoc.Save(writer);
		        }
	        }
	        catch (Exception ex)
	        {
		        _logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
	        }
        }

        private static void SliceInBody(SliceInfo sliceInfo, object groupElement, List<XmlNode> removedGroups)
        {
			// look in the segments
            var transUnits = ((XmlElement) groupElement).ChildNodes.OfType<XmlElement>().Where(node => node.Name == "trans-unit").ToList();

            var transUnitCount = transUnits.Count;
            var removedTransUnits = new List<XmlNode>();

            foreach (var transUnit in transUnits)
            {
                // Skip structure translation units.
                if (transUnit.GetAttribute("translate") == "no")
                {
                    continue;
                }

                var transUnitID = transUnit.Attributes["id"].Value;

                var removeAllSegments = sliceInfo.Segments.All(seg => seg.Key != transUnitID);

                var segDefs = transUnit.GetElementsByTagName("sdl:seg-defs");
                var segDefsCount = segDefs.Count;
                var removedSegDefs = new List<XmlNode>();
                foreach (var segDef in segDefs.OfType<XmlElement>())
                {
	                var segments = segDef.GetElementsByTagName("sdl:seg");
	                var segmentsCount = segments.Count;
	                var removedSegments = new List<XmlNode>();
                    foreach (var segment in segments.OfType<XmlElement>())
                    {
	                    var segmentId = segment.Attributes["id"].Value;

                        if (removeAllSegments ||
                            !sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(segmentId)))
                            removedSegments.Add((XmlNode) segment);
                    }
                    foreach (var xmlNode in removedSegments.OfType<XmlElement>())
                        segDef.RemoveChild(xmlNode);
                    if (segmentsCount == removedSegments.Count)
                        removedSegDefs.Add(segDef);
                    removedSegments.Clear();
                }

                foreach (var xmlNode in removedSegDefs.OfType<XmlElement>())
                    transUnit.RemoveChild(xmlNode);
                if (segDefsCount == removedSegDefs.Count)
                {
                    removedTransUnits.Add(transUnit);
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

        private static void RemoveSegmentFromSourceOrTarget(SliceInfo sliceInfo, object transUnit, string transUnitID, string tagName)
        {
	        var segs = ((XmlElement)transUnit).GetElementsByTagName(tagName);
            if (segs.Count > 0)
            {
	            var firstSeg = (XmlElement) segs[0];
	            var mrks = firstSeg.GetElementsByTagName("mrk");
	            var removedMrks = new List<XmlNode>();
                foreach (var mrk in mrks.OfType<XmlElement>())
                {
                    if (!mrk.HasAttribute("mtype") || mrk.Attributes["mtype"].Value != "seg")
                        continue;
					
                    if (mrk.HasAttribute("mid"))
                    {
                        var mrkID = mrk.Attributes["mid"].Value;

                        if (!sliceInfo.Segments.Any(seg => seg.Key == transUnitID && seg.Value.Contains(mrkID)))
                            removedMrks.Add(mrk);
                    }
                }
                foreach (var xmlNode in removedMrks.OfType<XmlElement>())
                {
                    xmlNode.ParentNode?.RemoveChild(xmlNode);
                }
                removedMrks.Clear();
            }
        }
    }
}