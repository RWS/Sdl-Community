using Microsoft.Win32;
using System.IO;
using System.Reflection;


namespace ChangeScalingBehavior
{
	public class RegistryKeyManager
	{
		private const string SubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SideBySide";

		public bool IsRegistryKey()
		{
			using (var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			using (var registryKey = localMachine32.OpenSubKey(SubKey))
			{
				return registryKey?.GetValue("PreferExternalManifest") != null;
			}
		}

		public void AddExternalManifest()
		{
			using (var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				var keyx = localMachine32.OpenSubKey(SubKey, true);
				keyx?.SetValue("PreferExternalManifest", 1, RegistryValueKind.DWord);
			}
		}

		public void RemoveExternalManifest()
		{
			using (var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				var keyx = localMachine32.OpenSubKey(SubKey, true);
				keyx?.DeleteValue("PreferExternalManifest");
			}
		}

		public void ExtractResourceFile(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
		{
			var assembly = Assembly.GetCallingAssembly();
			using (var stream = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
			{
				if (stream == null)
				{
					return;
				}

				using (var bReader = new BinaryReader(stream))
				{
					using (var fStream = new FileStream(Path.Combine(outDirectory, resourceName), FileMode.OpenOrCreate))
					{
						using (var bWriter = new BinaryWriter(fStream))
						{
							bWriter.Write(bReader.ReadBytes((int)stream.Length));
						}
					}
				}
			}
		}
	}
}
