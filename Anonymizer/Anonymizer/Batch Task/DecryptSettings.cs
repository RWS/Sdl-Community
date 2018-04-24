using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Anonymizer.Interfaces;
using Sdl.Core.Settings;

namespace Sdl.Community.Anonymizer.Batch_Task
{
	public class DecryptSettings:SettingsGroup,IDecryptSettings
	{
		public string EncryptionKey
		{
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = value;
		}
	}
}
