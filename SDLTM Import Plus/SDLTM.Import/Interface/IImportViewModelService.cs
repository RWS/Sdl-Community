using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using SDLTM.Import.Model;

namespace SDLTM.Import.Interface
{
	public interface IImportViewModelService
	{
		Model.Import GetImportModel(WizardModel wizzardModel, List<TmDetails> tmsList, CultureInfo sourceLanguage,CultureInfo targetLanguage);
		void ProcessXliffFile(FileDetails fileDetails, TmDetails tmDetails, ITranslationMemoryService translationMemoryService);
		void ProcessTmxFile(FileDetails file, TmDetails tm, ITranslationMemoryService translationMemoryService);
		bool IsImportAvailable(ObservableCollection<Model.Import>importCollection);
	}
}
