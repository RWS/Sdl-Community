using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Sdl.Community.TMLifting.TranslationMemory
{
	class ServerBasedTranslationMemoryInfo
	{
		public Uri Uri { get; set; }

		public string Name { get; set; }

		public bool IsServerBasedTranslationMemory { get; set; }

		public ServerBasedTranslationMemoryInfo(Uri uri, bool isServerBasedTranslationMemory)
		{
			//FilePath = tmFilePath;
			Name = ServerBasedTranslationMemory.GetServerBasedTranslationMemoryPath(uri);
			IsServerBasedTranslationMemory = isServerBasedTranslationMemory;

			//var server = ServerBasedTranslationMemory.GetServerBasedTranslationMemoryPath(Uri);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
