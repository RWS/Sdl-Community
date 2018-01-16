using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Sdl.Community.TMLifting.TranslationMemory
{
	class ServerBasedTranslationMemoryInfo
	{
		public string Name { get; set; }

		public bool IsServerBasedTranslationMemory { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
