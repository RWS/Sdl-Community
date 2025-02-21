using Multilingual.Excel.FileType.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.Excel.FileType.FileType.Settings
{
	public class DocumentInfoSettings : AbstractSettingsClass
	{
		private const string DocumentInfoSetting = "DocumentInfoSetting";
		private readonly List<DocumentInfo> _documentInfoDefault;

		public DocumentInfoSettings()
		{
			_documentInfoDefault = new List<DocumentInfo>();

			ResetToDefaults();
		}

		public List<DocumentInfo> DocumentInfo { get; set; }


		public override string SettingName => "MultilingualDocumentInfoSettings";

		public override void Read(IValueGetter valueGetter)
		{
			DocumentInfo = JsonConvert.DeserializeObject<List<DocumentInfo>>(valueGetter.GetValue(DocumentInfoSetting,
				JsonConvert.SerializeObject(_documentInfoDefault)));
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(DocumentInfoSetting, JsonConvert.SerializeObject(DocumentInfo),
				JsonConvert.SerializeObject(_documentInfoDefault));
		}

		public override object Clone()
		{
			return new DocumentInfoSettings
			{
				DocumentInfo = DocumentInfo
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is DocumentInfoSettings otherSetting &&
			       otherSetting.DocumentInfo == DocumentInfo;
		}

		public sealed override void ResetToDefaults()
		{
			DocumentInfo = _documentInfoDefault;
		}
	}
}