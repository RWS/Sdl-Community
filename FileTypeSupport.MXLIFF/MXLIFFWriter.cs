using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sdl.Community.FileTypeSupport.MXLIFF.Utils;

namespace Sdl.Community.FileTypeSupport.MXLIFF
{
	internal class MXLIFFWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
	{
		private INativeOutputFileProperties _nativeFileProperties;
		private XmlNamespaceManager _nsmgr;
		private IPersistentFileConversionProperties _originalFileProperties;
		private XmlDocument _targetFile;
		private MXLIFFTextExtractor _textExtractor;
		private readonly Dictionary<string, string> _users = new Dictionary<string, string>();
		private readonly Helper _helper;
	
		public MXLIFFWriter()
		{
			_helper = new Helper();
		}

		public void Complete()
		{
		}

		public void Dispose()
		{
			// Don't need to dispose of anything
		}

		public void FileComplete()
		{
			using (var wr = new XmlTextWriter(_nativeFileProperties.OutputFilePath, Encoding.UTF8))
			{
				wr.Formatting = Formatting.None;
				_targetFile.Save(wr);
				_targetFile = null;
			}
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			_originalFileProperties = fileProperties;
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_textExtractor = new MXLIFFTextExtractor();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var id = paragraphUnit.Properties.Contexts.Contexts[0].GetMetaData("ID");
			var xmlUnit = _targetFile.SelectSingleNode("//x:trans-unit[@id='" + id + "']", _nsmgr);

			CreateParagraphUnit(paragraphUnit, xmlUnit);
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_targetFile = new XmlDocument
			{
				PreserveWhitespace = false
			};

			LoadFile();

			_nsmgr = new XmlNamespaceManager(_targetFile.NameTable);
			_nsmgr.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");
			_nsmgr.AddNamespace("m", "http://www.memsource.com/mxlf/2.0");

			// Acquire users
			var memsourceUsers = _targetFile.SelectNodes("//m:user", _nsmgr);

			if (memsourceUsers is null) return;
			foreach (XmlElement user in memsourceUsers)
			{
				var id = user.Attributes["id"]?.Value;
				var username = user.Attributes["username"]?.Value;

				if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(id) && _users.ContainsKey(username))
				{
					_users.Add(username, id);
				}
			}
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_nativeFileProperties = properties;
		}

		private void LoadFile()
		{
			if (_originalFileProperties != null)
			{
				var backupFile = _helper.GetBackupFile(Path.GetFileNameWithoutExtension(_originalFileProperties.OriginalFilePath));
				if (File.Exists(_originalFileProperties.OriginalFilePath))
				{
					_targetFile.Load(_originalFileProperties.OriginalFilePath);
				}

				else if (!string.IsNullOrWhiteSpace(backupFile) && File.Exists(backupFile))
				{
					_targetFile.Load(backupFile);
				}
			}
		}

		private static void UpdateConfirmedAttribute(XmlNode transUnit, ISegmentPair segmentPair)
		{
			if (transUnit is null) return;
			if (transUnit.Attributes is null) return;
			if (segmentPair.Target?.Properties == null) return;
			switch (segmentPair.Target.Properties.ConfirmationLevel)
			{
				case ConfirmationLevel.Unspecified:
					transUnit.Attributes["m:confirmed"].Value = "0";
					break;

				case ConfirmationLevel.Draft:
					transUnit.Attributes["m:confirmed"].Value = "0";
					break;

				case ConfirmationLevel.Translated:
					transUnit.Attributes["m:confirmed"].Value = "1";
					break;

				case ConfirmationLevel.RejectedTranslation:
					transUnit.Attributes["m:confirmed"].Value = "0";
					break;

				case ConfirmationLevel.ApprovedTranslation:
					transUnit.Attributes["m:confirmed"].Value = "1";
					break;

				case ConfirmationLevel.RejectedSignOff:
					transUnit.Attributes["m:confirmed"].Value = "0";
					break;

				case ConfirmationLevel.ApprovedSignOff:
					transUnit.Attributes["m:confirmed"].Value = "1";
					break;
			}
		}

		private void AddComments(XmlNode xmlUnit, List<IComment> comments)
		{
			var text = string.Empty;
			var comment = comments.First();

			// We concatenate all comment text if there are multiple
			foreach (var c in comments)
			{
				text += c.Text + " ";
			}

			var createdat = _targetFile.CreateAttribute("created-at");
			var createdby = _targetFile.CreateAttribute("created-by");
			var modifiedat = _targetFile.CreateAttribute("modified-at");
			modifiedat.Value = "0";

			var modifiedby = _targetFile.CreateAttribute("modified-by");

			var resolved = _targetFile.CreateAttribute("resolved");
			resolved.Value = "false";

			// Convert DateTime to Unix timestamp in milliseconds
			var milliseconds = (long)(TimeZoneInfo.ConvertTimeToUtc(comment.Date) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
			createdat.Value = milliseconds.ToString();

			// Try to find the user id by author name
			// If it fails, just leave it blank
			createdby.Value = _users.ContainsKey(comment.Author) ? _users[comment.Author] : string.Empty;

			xmlUnit.Attributes?.Append(createdat);
			xmlUnit.Attributes?.Append(createdby);
			xmlUnit.Attributes?.Append(modifiedat);
			xmlUnit.Attributes?.Append(modifiedby);
			xmlUnit.Attributes?.Append(resolved);

			xmlUnit.InnerText = text;
		}

		private XmlNode CreateNewNode(XmlNode parent, XmlNode transUnit)
		{
			var segDoc = new XmlDocument();
			var nodeContent = parent.OuterXml;
			segDoc.LoadXml(nodeContent);
			var tuNode = segDoc.CreateNode(XmlNodeType.Element, "trans-unit", _nsmgr.LookupNamespace("x"));

			var importNode = parent.OwnerDocument?.ImportNode(tuNode, true);
			parent.InsertAfter(importNode ?? throw new InvalidOperationException(), transUnit);

			return importNode;
		}

		private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode transUnit)
		{
			if (transUnit == null)
			{
				return;
			}

			if (paragraphUnit.SegmentPairs.Count() == 1)
			{
				UpdateSegment(transUnit, paragraphUnit.SegmentPairs.First());
			}
			else if (paragraphUnit.SegmentPairs.Count() > 1)
			{
				UpdateSegment(transUnit, paragraphUnit.SegmentPairs.First());

				if (transUnit.Attributes == null) return;
				transUnit.Attributes["id"].Value = UpdateTopId(transUnit.Attributes["id"].Value);

				var topId = transUnit.Attributes["id"].Value;
				var count = (int)char.GetNumericValue(topId.Last()) + 1;
				// Iterate all segment pairs
				foreach (var segmentPair in paragraphUnit.SegmentPairs.Skip(1).Reverse())
				{
					var parent = transUnit.ParentNode;

					var newNode = CreateNewNode(parent, transUnit);
					AddAttributes(transUnit, newNode);
					AddElements(transUnit, newNode);

					if (newNode.Attributes != null)
					{
						newNode.Attributes["id"].Value = UpdateId(topId, count);
					}

					UpdateSegment(newNode, segmentPair);

					count++;
				}
			}

		}

		private string UpdateTopId(string value)
		{
			if (Regex.IsMatch(value, @".*?(:\d+?:\d+)"))
			{
				return value;
			}
			return value + ":0";
		}

		private string UpdateId(string id, int count)
		{
			return id.Remove(id.Length - 1) + count;
		}

		private void AddElements(XmlNode transUnit, XmlNode newNode)
		{
			var segDoc = new XmlDocument();
			var nodeContent = transUnit.OuterXml;
			segDoc.LoadXml(nodeContent);
			var source = segDoc.CreateNode(XmlNodeType.Element, "source", _nsmgr.LookupNamespace("x"));
			var target = segDoc.CreateNode(XmlNodeType.Element, "target", _nsmgr.LookupNamespace("x"));

			if (transUnit.OwnerDocument != null)
			{
				var sourceNode = transUnit.OwnerDocument.ImportNode(source, true);
				var targetNode = transUnit.OwnerDocument.ImportNode(target, true);

				newNode.AppendChild(sourceNode);
				newNode.AppendChild(targetNode);

				// alt-trans
				var altTrans = segDoc.CreateNode(XmlNodeType.Element, "alt-trans", _nsmgr.LookupNamespace("x"));

				var altTransNode = transUnit.OwnerDocument.ImportNode(altTrans, true);
				if (altTransNode.Attributes != null)
				{
					altTransNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("origin"));
					altTransNode.Attributes["origin"].Value = "machine-trans";
				}
				altTransNode.AppendChild(targetNode.Clone());

				newNode.AppendChild(altTransNode);
			}

			// m:tunit-metadata
			// m:tunit-target-metadata
			var meta1 = segDoc.CreateNode(XmlNodeType.Element, "m:tunit-metadata", _nsmgr.LookupNamespace("m"));
			var meta2 = segDoc.CreateNode(XmlNodeType.Element, "m:tunit-target-metadata", _nsmgr.LookupNamespace("m"));

			if (transUnit.OwnerDocument != null)
			{
				var m1 = transUnit.OwnerDocument.ImportNode(meta1, true);
				var m2 = transUnit.OwnerDocument.ImportNode(meta2, true);

				newNode.AppendChild(m1);
				newNode.AppendChild(m2);
			}

			// m:editing-stats
			var mstats = segDoc.CreateNode(XmlNodeType.Element, "m:editing-stats", _nsmgr.LookupNamespace("m"));
			var thinkingtime = segDoc.CreateNode(XmlNodeType.Element, "m:thinking-time", _nsmgr.LookupNamespace("m"));
			thinkingtime.InnerText = "0";
			var editingtime = segDoc.CreateNode(XmlNodeType.Element, "m:editing-time", _nsmgr.LookupNamespace("m"));
			editingtime.InnerText = "0";

			mstats.AppendChild(thinkingtime);
			mstats.AppendChild(editingtime);

			if (transUnit.OwnerDocument == null) return;
			var mstatsNode = transUnit.OwnerDocument.ImportNode(mstats, true);

			newNode.AppendChild(mstatsNode);
		}

		private void AddAttributes(XmlNode transUnit, XmlNode newNode)
		{
			if (newNode.Attributes is null) return;
			if (transUnit.OwnerDocument != null)
			{
				newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("id"));

				newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:para-id", _nsmgr.LookupNamespace("m")));
				if (transUnit.Attributes != null)
				{
					newNode.Attributes["m:para-id"].Value = transUnit.Attributes["m:para-id"].Value;

					newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("xml:space"));
					newNode.Attributes["xml:space"].Value = "preserve";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:trans-origin", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:trans-origin"].Value = "null";

					newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:score", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:score"].Value = "0";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:gross-score", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:gross-score"].Value = "0";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:confirmed", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:confirmed"].Value = "0";

					newNode.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:locked", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:locked"].Value = "false";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:level-edited", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:level-edited"].Value = "true";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:created-by", _nsmgr.LookupNamespace("m")));
					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:created-at", _nsmgr.LookupNamespace("m")));
					newNode.Attributes["m:created-at"].Value = "0";

					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:modified-by", _nsmgr.LookupNamespace("m")));
					newNode.Attributes.Append(
						transUnit.OwnerDocument.CreateAttribute("m:modified-at", _nsmgr.LookupNamespace("m")));
				}
			}
			newNode.Attributes["m:modified-at"].Value = "0";
		}

		private void FillSegment(ISegmentPair segmentPair, XmlNode node, bool source)
		{
			var seg = source ? segmentPair.Source : segmentPair.Target;

			node.InnerText = _textExtractor.GetPlainText(seg);
		}

		private void UpdateSegment(XmlNode transUnit, ISegmentPair segmentPair)
		{
			var matchPercent = 0;

			var source = transUnit.SelectSingleNode("x:source", _nsmgr);

			if (source != null)
			{
				FillSegment(segmentPair, source, true);
			}

			if (segmentPair.Properties.TranslationOrigin != null)
			{
				matchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
			}

			if (transUnit.SelectSingleNode("x:target", _nsmgr) == null)
			{
				var segDoc = new XmlDocument();
				var nodeContent = transUnit.OuterXml;
				segDoc.LoadXml(nodeContent);
				var trgNode = segDoc.CreateNode(XmlNodeType.Element, "target", _nsmgr.LookupNamespace("x"));
				trgNode.InnerText = string.Empty;

				if (transUnit.OwnerDocument != null)
				{
					var importNode = transUnit.OwnerDocument.ImportNode(trgNode, true);
					transUnit.AppendChild(importNode);
				}

				var target = transUnit.SelectSingleNode("x:target", _nsmgr);

				FillSegment(segmentPair, target, false);
			}
			else
			{
				var target = transUnit.SelectSingleNode("x:target", _nsmgr);
				FillSegment(segmentPair, target, false);
			}

			// Add comments (if applicable)
			var comments = _textExtractor.GetSegmentComment(segmentPair.Target);
			if (comments.Count > 0 && transUnit.SelectSingleNode("m:comment", _nsmgr) == null)
			{
				var commentElement = _targetFile.CreateElement("m:comment", _nsmgr.LookupNamespace("m"));

				var tunitMetaData = transUnit.SelectSingleNode("m:tunit-metadata", _nsmgr);
				if (tunitMetaData != null)
				{
					transUnit.InsertBefore(commentElement, tunitMetaData);
					AddComments(transUnit.SelectSingleNode("m:comment", _nsmgr), comments);
				}
			}

			// Update score value
			var dbl = matchPercent / 100.0;
			if (transUnit.Attributes?["m:score"] != null && transUnit.Attributes["m:gross-score"] != null && transUnit.Attributes["m:trans-origin"] != null)
			{
				transUnit.Attributes["m:score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
				transUnit.Attributes["m:gross-score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				if (transUnit.Attributes != null)
				{
					if (transUnit.OwnerDocument != null)
					{
						transUnit.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:score"));
						transUnit.Attributes.Append(transUnit.OwnerDocument.CreateAttribute("m:gross-score"));
					}
					transUnit.Attributes["m:score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
					transUnit.Attributes["m:gross-score"].Value = dbl.ToString(CultureInfo.InvariantCulture);
				}
			}

			var transOrigin = segmentPair.Target?.Properties?.TranslationOrigin;
			if (transOrigin != null)
			{
				var memSourceTransOrigin = transUnit.Attributes?["m:trans-origin"];

				if (memSourceTransOrigin != null)
				{
					if (transOrigin.OriginType == DefaultTranslationOrigin.TranslationMemory)
					{
						memSourceTransOrigin.Value = DefaultTranslationOrigin.TranslationMemory;
					}
					else if (transOrigin.OriginType == DefaultTranslationOrigin.MachineTranslation)
					{
						memSourceTransOrigin.Value = DefaultTranslationOrigin.MachineTranslation;
					}
				}
			}

			// Update m:locked
			if (transUnit.Attributes != null && transUnit.Attributes["m:locked"] != null)
			{
				var isLocked = segmentPair.Target?.Properties?.IsLocked.ToString();
				transUnit.Attributes["m:locked"].Value = isLocked?.ToLower() ?? "false";
			}

			// Update m:confirmed
			if (transUnit.Attributes["m:confirmed"] != null)
			{
				UpdateConfirmedAttribute(transUnit, segmentPair);
			}
		}
	}
}