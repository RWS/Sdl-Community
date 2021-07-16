using System;
using System.IO;
using System.Text;
using System.Xml;
using NLog;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
	{
		private IPersistentFileConversionProperties _originalFileProperties;
		private INativeOutputFileProperties _nativeFileProperties;
		private IDocumentProperties _documentInfo;
		private XmlDocument _targetFile;
		private TransitTextExtractor _textExtractor;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
			try
			{
				_targetFile = new XmlDocument {PreserveWhitespace = true};
				if (File.Exists(_originalFileProperties.OriginalFilePath))
				{
					_targetFile.Load(_originalFileProperties.OriginalFilePath);
				}
				else
				{
					//User changed the location of the project, we need to get the new path for source files, we can use the LastOpenedPath but there we
					//have the location on target folder, we need the path to source language folder
					var targetLanguageCode = _originalFileProperties.TargetLanguage.CultureInfo.Name;
					if (!string.IsNullOrEmpty(_documentInfo?.LastOpenedAsPath))
					{
						var lastOpenedPath = _documentInfo?.LastOpenedAsPath;
						var newRoothDirectory = lastOpenedPath.Substring(0, lastOpenedPath.LastIndexOf(targetLanguageCode, StringComparison.Ordinal));
						var fileName = Path.GetFileName(_originalFileProperties.OriginalFilePath);
						var newPath = Path.Combine(newRoothDirectory,
							_originalFileProperties.SourceLanguage.CultureInfo.Name,fileName);
						_targetFile.Load(newPath);
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e);
				throw;
			}
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_documentInfo = documentInfo;
			_textExtractor = new TransitTextExtractor();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var unitId = paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("UnitID");
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
					var dataValue = xmlUnit.Attributes["Data"].InnerText;
					xmlUnit.Attributes["Data"].InnerText = UpdateEditedStatus(dataValue, segmentPair.Properties.ConfirmationLevel);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		private string UpdateEditedStatus(string data, ConfirmationLevel unitLevel)
		{
			var status = string.Empty;
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

			var stringBytes = Encoding.Unicode.GetBytes(data);
			var sbBytes = new StringBuilder(stringBytes.Length * 2);
			foreach (var b in stringBytes)
			{
				sbBytes.AppendFormat("{0:X2}", b);
			}

			var changedData = status + sbBytes.ToString().Substring(2, sbBytes.ToString().Length - 2);
			var numberChars = changedData.Length;
			var bytes = new byte[numberChars / 2];
			for (int i = 0; i < numberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(changedData.Substring(i, 2), 16);
			}

			return Encoding.Unicode.GetString(bytes);
		}

		private void AddComment(XmlNode xmlUnit, string commentText)
		{
			try
			{
				var chunk = commentText.Split(';');

				var commentElement = _targetFile.CreateElement("comment");

				var creationid = _targetFile.CreateAttribute("creationid");
				var creationdate = _targetFile.CreateAttribute("creationdate");
				var type = _targetFile.CreateAttribute("type");

				var commentDate = GetCommentDate();

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
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		private string GetCommentDate()
		{
			string day;
			string month;

			if (DateTime.UtcNow.Month.ToString().Length == 1)
				month = "0" + DateTime.UtcNow.Month;
			else
				month = "0" + DateTime.UtcNow.Month;

			if (DateTime.UtcNow.Day.ToString().Length == 1)
				day = "0" + DateTime.UtcNow.Day;
			else
				day = "0" + DateTime.UtcNow.Day;

			return DateTime.UtcNow.Year + month + day + "T" +
				DateTime.UtcNow.Hour + DateTime.UtcNow.Minute +
				DateTime.UtcNow.Second + "Z";
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