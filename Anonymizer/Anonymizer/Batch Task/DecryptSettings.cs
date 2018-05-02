using Sdl.Community.projectAnonymizer.Interfaces;
using Sdl.Core.Settings;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class DecryptSettings:SettingsGroup,IDecryptSettings
	{
		public string EncryptionKey
		{
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = value;
		}
		public bool IgnoreEncrypted
		{
			get => GetSetting<bool>(nameof(IgnoreEncrypted));
			set => GetSetting<bool>(nameof(IgnoreEncrypted)).Value = value;
		}
	}
}
