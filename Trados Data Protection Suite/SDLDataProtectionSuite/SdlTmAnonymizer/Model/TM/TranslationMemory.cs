using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.TM
{
	public class TranslationMemory
	{
		public int Id { get; set; }
		public string Guid { get; set; }
		public string Name { get; set; }
		public string SourceLangauge { get; set; }
		public string TargetLanguage { get; set; }
		public string Copyright { get; set; }
		public string Description { get; set; }
		public int Settings { get; set; }
		public string CreationUser { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime ExpirationDate { get; set; }
		public int FuzzyIndexes { get; set; }
		public DateTime LastReComputeDate { get; set; }
		public int LastReComputeSize { get; set; }
		public int Flags { get; set; }
		public int TuCount { get; set; }
	}
}
