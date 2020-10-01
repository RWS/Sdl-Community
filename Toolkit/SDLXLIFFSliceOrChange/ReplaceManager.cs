using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NLog;

namespace SDLXLIFFSliceOrChange
{
	public static class ReplaceManager
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public static void DoReplaceInFile(string file, ReplaceSettings settings, SDLXLIFFSliceOrChange sdlxliffSliceOrChange)
		{
			try
			{
				sdlxliffSliceOrChange.StepProcess("Replaceing in file: " + file + "...");

				string fileContent;
				using (var sr = new StreamReader(file))
				{
					fileContent = sr.ReadToEnd();
				}
				fileContent = Regex.Replace(fileContent, "\t", "");

				using (var sw = new StreamWriter(file, false))
				{
					sw.Write(fileContent);
				}

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
					ParseFileElements(fileList, settings);
				}

				Encoding encoding = new UTF8Encoding();
				if (!string.IsNullOrEmpty(xmlEncoding))
					encoding = Encoding.GetEncoding(xmlEncoding);

				using (var writer = new XmlTextWriter(file, encoding))
				{
					xDoc.Save(writer);
				}

				using (var sr = new StreamReader(file))
				{
					fileContent = sr.ReadToEnd();
				}
				fileContent = Regex.Replace(fileContent, "", "\t");

				using (var sw = new StreamWriter(file, false))
				{
					sw.Write(fileContent);
				}
				sdlxliffSliceOrChange.StepProcess("All information replaced in file: " + file + ".");
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		private static void ParseFileElements(XmlNodeList fileList, ReplaceSettings settings)
		{
			if (fileList != null)
			{
				foreach (var fileElement in fileList.OfType<XmlElement>())
				{
					var bodyElement = (XmlElement)(fileElement.GetElementsByTagName("body")[0]);
					var groupElements = bodyElement.GetElementsByTagName("group");

					foreach (var groupElement in groupElements.OfType<XmlElement>())
					{
						//look in segments
						var transUnits = groupElement.GetElementsByTagName("trans-unit");
						foreach (var transUnit in transUnits.OfType<XmlElement>())
						{
							var source = transUnit.GetElementsByTagName("source");
							if (source.Count > 0) //in mrk, g si innertext
								ReplaceAllChildsValue((XmlElement)source[0], settings);
							var segSource = transUnit.GetElementsByTagName("seg-source");
							if (segSource.Count > 0) //in mrk, g si innertext
								ReplaceAllChildsValue((XmlElement)segSource[0], settings);
							var target = transUnit.GetElementsByTagName("target");
							if (target.Count > 0) //in mrk, g si innertext
								ReplaceAllChildsValue((XmlElement)target[0], settings, false);
						}
					}

					//look in segments not located in groups
					var transUnitsInBody = bodyElement.ChildNodes;
					foreach (var transUnit in transUnitsInBody.OfType<XmlElement>())
					{
						if (transUnit.Name != "trans-unit")
							continue;
						var source = transUnit.GetElementsByTagName("source");
						if (source.Count > 0) //in mrk, g si innertext
							ReplaceAllChildsValue((XmlElement)source[0], settings);
						var segSource = transUnit.GetElementsByTagName("seg-source");
						if (segSource.Count > 0) //in mrk, g si innertext
							ReplaceAllChildsValue((XmlElement)segSource[0], settings);
						var target = transUnit.GetElementsByTagName("target");
						if (target.Count > 0) //in mrk, g si innertext
							ReplaceAllChildsValue((XmlElement)target[0], settings, false);
					}
				}
			}
		}

		private static void GetLastNode(ReplaceSettings settings, bool inSource, XmlElement currentNode)
		{
			var childNodes = currentNode.ChildNodes.OfType<XmlElement>().ToList();

			foreach (var innerChild in childNodes)
			{
				if (innerChild.Name.Equals("mrk") && innerChild.HasAttribute("mtype"))
				{
					ReplaceValue(settings, inSource, innerChild);
				}
				GetLastNode(settings, inSource, innerChild);
			}
		}

		private static void ReplaceAllChildsValue(XmlElement target, ReplaceSettings settings, bool inSource = true)
		{
			foreach (var child in target.ChildNodes.OfType<XmlElement>())
			{
				if (!child.IsEmpty)
				{
					if (child.Name.Equals("mrk") && child.HasAttribute("mtype"))
					{
						ReplaceValue(settings, inSource, child);
					}
					else
					{
						GetLastNode(settings, inSource, child);
					}
				}
			}
		}

		private static void ReplaceValue(ReplaceSettings settings, bool inSource, XmlElement child)
		{
			var segmentHtml = child.InnerXml;
			if (!string.IsNullOrEmpty(segmentHtml))
			{
				if (settings.UseRegEx)
				{
					var options = !settings.MatchCase ? RegexOptions.IgnoreCase & RegexOptions.Multiline : RegexOptions.None & RegexOptions.Multiline;

					if (!string.IsNullOrEmpty(segmentHtml))
					{
						ReplaceRegexText(settings, options, segmentHtml, child, inSource);
					}
				}
				else
				{
					var remove = Regex.Escape(inSource ? settings.SourceSearchText : settings.TargetSearchText);
					var pattern = settings.MatchWholeWord ? string.Format(@"(\b(?<!\w){0}\b|(?<=^|\s){0}(?=\s|$))", remove) : remove;
					var replace = inSource ? settings.SourceReplaceText : settings.TargetReplaceText;
					child.InnerXml = Regex.Replace(segmentHtml, pattern, replace, !settings.MatchCase ? RegexOptions.IgnoreCase : RegexOptions.None);
				}
			}
		}

		// Replace the text identified by Regex.
		// SegmentHtml needs to be encoded, because the settings.SourceReplaceText/settings.TargetReplaceText is encoded
		// and it should be the same for a correct replacement (ex: words which contains symbols)
		private static void ReplaceRegexText(ReplaceSettings settings, RegexOptions options, string segmentHtml, XmlElement child, bool inSource)
		{
			if (inSource && !string.IsNullOrEmpty(settings.SourceSearchText) && !string.IsNullOrEmpty(settings.SourceReplaceText))
			{
				var sourceRg = new Regex(settings.SourceSearchText, options);
				var replacedText = sourceRg.Replace(WebUtility.HtmlEncode(segmentHtml), settings.SourceReplaceText);
				child.InnerXml = replacedText;
			}
			if (!inSource && !string.IsNullOrEmpty(settings.TargetSearchText))
			{
				var targetRg = new Regex(settings.TargetSearchText, options);
				var replacedText = targetRg.Replace(WebUtility.HtmlEncode(segmentHtml), settings.TargetReplaceText);
				child.InnerXml = replacedText;
			}
		}
	}
}