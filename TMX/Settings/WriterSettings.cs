using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;

namespace Sdl.Community.FileType.TMX.Settings
{
	public class WriterSettings : AbstractSettingsClass
	{
		private const string WriteChangeDateSetting = "WriteChangeDate";
		private const string WriteUserIDSetting = "WriteUserID";

		public bool WriteChangeDate { get; set; } = false;
		public bool WriteUserID { get; set; } = false;

		public override void Read(IValueGetter valueGetter)
		{
			WriteChangeDate = valueGetter.GetValue(WriteChangeDateSetting, false);
			WriteUserID = valueGetter.GetValue(WriteUserIDSetting, false);
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(WriteChangeDateSetting, WriteChangeDate, false);
			valueProcessor.Process(WriteUserIDSetting, WriteUserID, false);
		}

		public override object Clone()
		{
			return new WriterSettings
			{
				WriteChangeDate = WriteChangeDate, WriteUserID = WriteUserID,
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			if (other is WriterSettings o)
				return WriteChangeDate == o.WriteChangeDate && WriteUserID == o.WriteUserID;
			return false;
		}

		public override string SettingName => "TMXFileWriterSettings";

		public override void ResetToDefaults()
		{
			WriteChangeDate = false;
			WriteUserID = false;
		}
	}
}
