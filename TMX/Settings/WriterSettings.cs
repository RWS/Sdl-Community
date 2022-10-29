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
		public override void Read(IValueGetter valueGetter)
		{
			// FIXME
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			// FIXME
		}

		public override object Clone()
		{
			return new WriterSettings
			{
				// FIXME
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return false;
		}

		public override string SettingName => "TMXFileWriterSettings";

		public override void ResetToDefaults()
		{
			// FIXME
		}
	}
}
