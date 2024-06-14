using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SDLTM.Import.Model;

namespace SDLTM.Import.Interface
{
	public interface ITranslationMemoryService
	{
		Settings Settings { get; set; }
		void ImportTranslationUnits(FileBasedTranslationMemory tm, TmDetails selectedTmDetails,TranslationUnit[]transaltioTranslationUnits, bool[]masks);
		ImportSummary ImportFile(FileBasedTranslationMemory tm, string filePath);
		void SetFieldsToTu(FileBasedTranslationMemory tm, TmDetails selectedTmDetails, TranslationUnit tu,string tuId, string fileName);
		void BackUpTm(string tmPath);
	}	
}
