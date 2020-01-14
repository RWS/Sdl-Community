using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using log4net;
//using Sdl.Utilities.BatchSearchReplace.Lib;

namespace SDLXLIFFSliceOrChange
{
    public static class ReplaceManager
    {
        private static ILog logger = LogManager.GetLogger(typeof (ReplaceManager));
        public static void DoReplaceInFile(string file, ReplaceSettings settings, SDLXLIFFSliceOrChange sdlxliffSliceOrChange)
        {
            try
            {
                sdlxliffSliceOrChange.StepProcess("Replaceing in file: "+file+"...");

                String fileContent = String.Empty;
                using (StreamReader sr = new StreamReader(file))
                {
                    fileContent = sr.ReadToEnd();
                }
                fileContent = Regex.Replace(fileContent, "\t", "");

                using (StreamWriter sw = new StreamWriter(file, false))
                {
                    sw.Write(fileContent);
                }

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
                        foreach (XmlElement transUnit in transUnits.OfType<XmlElement>())
                        {
                            XmlNodeList source = transUnit.GetElementsByTagName("source");
                            if (source.Count > 0) //in mrk, g si innertext
                                ReplaceAllChildsValue((XmlElement) source[0], settings);
                            source = null;
                            XmlNodeList segSource = transUnit.GetElementsByTagName("seg-source");
                            if (segSource.Count > 0) //in mrk, g si innertext
                                ReplaceAllChildsValue((XmlElement) segSource[0], settings);
                            segSource = null;
                            XmlNodeList target = transUnit.GetElementsByTagName("target");
                            if (target.Count > 0) //in mrk, g si innertext
                                ReplaceAllChildsValue((XmlElement) target[0], settings, false);
                            target = null;
                        }
                    }

                    //look in segments not located in groups
                    XmlNodeList transUnitsInBody = bodyElement.ChildNodes;//.GetElementsByTagName("trans-unit");
                    foreach (XmlElement transUnit in transUnitsInBody.OfType<XmlElement>())
                    {
                        if (transUnit.Name != "trans-unit")
                            continue;
                        XmlNodeList source = transUnit.GetElementsByTagName("source");
                        if (source.Count > 0) //in mrk, g si innertext
                            ReplaceAllChildsValue((XmlElement)source[0], settings);
                        source = null;
                        XmlNodeList segSource = transUnit.GetElementsByTagName("seg-source");
                        if (segSource.Count > 0) //in mrk, g si innertext
                            ReplaceAllChildsValue((XmlElement)segSource[0], settings);
                        segSource = null;
                        XmlNodeList target = transUnit.GetElementsByTagName("target");
                        if (target.Count > 0) //in mrk, g si innertext
                            ReplaceAllChildsValue((XmlElement)target[0], settings, false);
                        target = null;
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
                    ////writer.Formatting = Formatting.None;
                    xDoc.Save(writer);
                }

                fileContent = String.Empty;
                using (StreamReader sr = new StreamReader(file))
                {
                    fileContent = sr.ReadToEnd();
                }
                fileContent = Regex.Replace(fileContent, "", "\t");

                using (StreamWriter sw = new StreamWriter(file, false))
                {
                    sw.Write(fileContent);
                }

                xDoc = null;
                fileList = null;
                sdlxliffSliceOrChange.StepProcess("All information replaced in file: " + file + ".");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        private static void ReplaceAllChildsValue(XmlElement target, ReplaceSettings settings, bool inSource = true)
        {
            foreach (XmlElement child in target.ChildNodes.OfType<XmlElement>())
            {
                if (!child.IsEmpty)
                {
                    if (child.Name == "mrk" && child.HasAttribute("mtype"))
                    {
                        ReplaceTheVaue(settings, inSource, child);
                    }
                    else
                    {
                        GetLastNode(settings, inSource, child);
                    }
                    
                    
                }
              
            }
        }

        private static void GetLastNode(ReplaceSettings settings,bool inSource,XmlElement currentNode)
        {
            var childNodes = currentNode.ChildNodes.OfType<XmlElement>().ToList();

            foreach (var innerChild in childNodes)
            {
                if (innerChild.Name == "mrk" && innerChild.HasAttribute("mtype"))
                {
                    ReplaceTheVaue(settings, inSource, innerChild);

                }
                 GetLastNode(settings,inSource,innerChild);
            }

        }





        private static void ReplaceTheVaue(ReplaceSettings settings, bool inSource, XmlElement child)
        {
			var segmentHtml = child.InnerXml;
            
            if (settings.UseRegEx)
            {
                var options = RegexOptions.None&RegexOptions.Multiline;
                
                if (!settings.MatchCase)
                    options = RegexOptions.IgnoreCase&RegexOptions.Multiline;
                
                if (segmentHtml != string.Empty)
                {
                    if (inSource && settings.SourceSearchText != string.Empty &&
                        settings.SourceReplaceText != string.Empty)
                    {
                        var sourceRg = new Regex(settings.SourceSearchText,options);
                        var replacedText = sourceRg.Replace(segmentHtml, settings.SourceReplaceText);
                        child.InnerXml = replacedText;               
                    }
                    if (!inSource && settings.TargetSearchText != string.Empty)
                    {
                        var targetRg = new Regex(settings.TargetSearchText, options);
                        var replacedText = targetRg.Replace(segmentHtml, settings.TargetReplaceText);
                        child.InnerXml = replacedText;
                    }

                }
            }
            else
            {
                if (segmentHtml != string.Empty)
                {
                    var remove = Regex.Escape(inSource ? settings.SourceSearchText : settings.TargetSearchText);
                    var pattern = settings.MatchWholeWord
                                             ? string.Format(@"(\b(?<!\w){0}\b|(?<=^|\s){0}(?=\s|$))", remove)
                                             : remove;
                    var replace = inSource ? settings.SourceReplaceText : settings.TargetReplaceText;
                    child.InnerXml = Regex.Replace(segmentHtml, pattern, replace,
                                                        !settings.MatchCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                }
     
            }

        }
    }
}