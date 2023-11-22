using System.Collections.Generic;
using Multilingual.XML.FileType.Models;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Multilingual.XML.FileType.FileType.Settings
{
	public class CommentMappingSettings : AbstractSettingsClass
	{
		private bool _commentsProcess;

		private const string CommentsProcessSetting = "CommentProcess";
		private readonly bool _commentsProcessDefault;

		private const string CommentElementNameSetting = "CommentElementName";
		private readonly string _commentElementNameDefault;

		private const string CommentMappingsSetting = "CommentPropertyMappings";
		private readonly List<CommentPropertyMapping> _commentMappingsDefault;

		public CommentMappingSettings()
		{
			_commentsProcessDefault = true;
			_commentElementNameDefault = string.Empty;
			_commentMappingsDefault = new List<CommentPropertyMapping>();

			ResetToDefaults();
		}

		public bool CommentsProcess
		{
			get => _commentsProcess;
			set
			{
				if (_commentsProcess == value)
				{
					return;
				}

				_commentsProcess = value;
				OnPropertyChanged(nameof(CommentsProcess));
			}
		}

		public string CommentElementName { get; set; }

		public List<CommentPropertyMapping> CommentMappings { get; set; }

		public override string SettingName => "MultilingualXmlCommentMappingSettings";

		public override void Read(IValueGetter valueGetter)
		{
			CommentsProcess = valueGetter.GetValue(CommentsProcessSetting, _commentsProcessDefault);
			CommentElementName = valueGetter.GetValue(CommentElementNameSetting, _commentElementNameDefault);
			CommentMappings = JsonConvert.DeserializeObject<List<CommentPropertyMapping>>(valueGetter.GetValue(CommentMappingsSetting,
				JsonConvert.SerializeObject(_commentMappingsDefault)));
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(CommentsProcessSetting, CommentsProcess, _commentsProcessDefault);
			valueProcessor.Process(CommentElementNameSetting, CommentElementName, _commentElementNameDefault);
			valueProcessor.Process(CommentMappingsSetting, JsonConvert.SerializeObject(CommentMappings),
				JsonConvert.SerializeObject(_commentMappingsDefault));
		}

		public override object Clone()
		{
			return new CommentMappingSettings
			{
				CommentsProcess = CommentsProcess,
				CommentElementName = CommentElementName,
				CommentMappings = CommentMappings,
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is CommentMappingSettings otherSetting &&
				   otherSetting.CommentsProcess == CommentsProcess &&
				   otherSetting.CommentElementName == CommentElementName &&
				   otherSetting.CommentMappings == CommentMappings;
		}

		public sealed override void ResetToDefaults()
		{
			CommentsProcess = _commentsProcessDefault;
			CommentElementName = _commentElementNameDefault;
			CommentMappings = _commentMappingsDefault;
		}
	}
}
