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
			Name = ServerBasedTranslationMemory.GetServerBasedTranslationMemoryPath(uri);
			IsServerBasedTranslationMemory = isServerBasedTranslationMemory;			
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
