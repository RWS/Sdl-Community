using System;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
	{
		private IPersistentFileConversionProperties _originalFileProperties;
		private INativeOutputFileProperties _nativeFileProperties;
		private XmlDocument _targetFile;
		private TransitTextExtractor _textExtractor;

		public static readonly Log Log = Log.Instance;

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			_originalFileProperties = fileProperties;
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_nativeFileProperties = properties;
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_targetFile = new XmlDocument();
			_targetFile.PreserveWhitespace = true;
			_targetFile.Load(_originalFileProperties.OriginalFilePath);
		}


		public void Initialize(IDocumentProperties documentInfo)
		{
			_textExtractor = new TransitTextExtractor();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			string unitId = paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("UnitID");
			var xmlUnit = _targetFile.SelectSingleNode("//Seg[@SegID='" + unitId + "']");

			CreateParagraphUnit(paragraphUnit, xmlUnit);
		}

		private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode xmlUnit)
		{
			try
			{
				// iterate all segment pairs
				foreach (var segmentPair in paragraphUnit.SegmentPairs)
				{
					var source = xmlUnit.SelectSingleNode(".");

					source.InnerXml = _textExtractor.GetPlainText(segmentPair.Source);

					Byte matchPercent;
					if (segmentPair.Properties.TranslationOrigin != null)
					{
						matchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
					}
					else
					{
						matchPercent = 0;
					}
					XmlNode target;
					target = xmlUnit.SelectSingleNode(".");
					target.InnerXml = _textExtractor.GetPlainText(segmentPair.Target);
					//update modified status  
					string dataValue = xmlUnit.Attributes["Data"].InnerText;
					xmlUnit.Attributes["Data"].InnerText = UpdateEditedStatus(dataValue, segmentPair.Properties.ConfirmationLevel);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateParagraphUnit method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private string UpdateEditedStatus(string data, ConfirmationLevel unitLevel)
		{
			try
			{
				string status = string.Empty;
				switch (unitLevel)
				{
					case ConfirmationLevel.Translated:
						status = "0a";
						break;
					case ConfirmationLevel.Draft:
						status = "02";
						break;
					case ConfirmationLevel.Unspecified:
						status = "02";
						break;
					case ConfirmationLevel.ApprovedTranslation:
						status = "0e";
						break;
					case ConfirmationLevel.ApprovedSignOff:
						status = "0f";
						break;
					case ConfirmationLevel.RejectedSignOff:
						status = "02";
						break;
					case ConfirmationLevel.RejectedTranslation:
						status = "02";
						break;
					default:
						status = "0a";
						break;
				}

				Byte[] stringBytes = Encoding.Unicode.GetBytes(data);
				var sbBytes = new StringBuilder(stringBytes.Length * 2);
				foreach (byte b in stringBytes)
				{
					sbBytes.AppendFormat("{0:X2}", b);
				}

				string changedData = status + sbBytes.ToString().Substring(2, sbBytes.ToString().Length - 2);
				int numberChars = changedData.Length;
				byte[] bytes = new byte[numberChars / 2];
				for (int i = 0; i < numberChars; i += 2)
				{
					bytes[i / 2] = Convert.ToByte(changedData.Substring(i, 2), 16);
				}

				return Encoding.Unicode.GetString(bytes);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UpdateEditedStatus method: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
		}

		private void AddComment(XmlNode xmlUnit, string commentText)
		{
			try
			{
				string[] chunk = commentText.Split(';');

				var commentElement = _targetFile.CreateElement("comment");

				var creationid = _targetFile.CreateAttribute("creationid");
				var creationdate = _targetFile.CreateAttribute("creationdate");
				var type = _targetFile.CreateAttribute("type");

				string commentDate = this.GetCommentDate();

				creationdate.Value = commentDate;
				creationid.Value = chunk[2];

				switch (chunk[3])
				{
					case "Medium":
						type.Value = "text";
						break;

					case "High":
						type.Value = "important";
						break;

					default:
						type.Value = "text";
						break;
				}

				commentElement.Attributes.Append(creationid);
				commentElement.Attributes.Append(creationdate);
				commentElement.Attributes.Append(type);

				commentElement.InnerText = chunk[0];
				xmlUnit.AppendChild(commentElement);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AddComment method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		private string GetCommentDate()
		{
			string day;
			string month;

			if (DateTime.UtcNow.Month.ToString().Length == 1)
				month = "0" + DateTime.UtcNow.Month.ToString();
			else
				month = "0" + DateTime.UtcNow.Month.ToString();

			if (DateTime.UtcNow.Day.ToString().Length == 1)
				day = "0" + DateTime.UtcNow.Day.ToString();
			else
				day = "0" + DateTime.UtcNow.Day.ToString();

			return DateTime.UtcNow.Year.ToString() + month + day + "T" +
				DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString() +
				DateTime.UtcNow.Second.ToString() + "Z";
		}

		public void FileComplete()
		{
			_targetFile.Save(_nativeFileProperties.OutputFilePath);
			_targetFile = null;
		}

		public void Complete()
		{

		}

		public void Dispose()
		{
			//don't need to dispose of anthing
		}
	}
}