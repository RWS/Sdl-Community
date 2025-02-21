using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using SDLTM.Import.FileService;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;

namespace SDLTM.Import.Service
{
	public class ImportViewModelService : IImportViewModelService
	{
		/// <summary>
		/// Creates the import model associated to each row in Import View data grid.
		/// We group the files and tms based on language pair and language variation selected on Settings view.
		/// </summary>
		public Model.Import GetImportModel(WizardModel wizardModel, List<TmDetails> tmsList, CultureInfo sourceLanguage,
			CultureInfo targetLanguage)
		{
			var filesCollection = new List<FileDetails>();
			var import = new Model.Import
			{
				SourceLanguage = sourceLanguage,
				TargetLanguage = targetLanguage,
				SourceFlag = new Language(sourceLanguage).GetFlagImage(),
				TargetFlag = new Language(targetLanguage).GetFlagImage(),
				TmsCollection = new ObservableCollection<TmDetails>(tmsList)
			};
			var tmxFiles = wizardModel.FilesList.Where(f => f.FileType == FileTypes.Tmx).ToList();
			var xliffFiles = wizardModel.FilesList.Where(f => f.FileType == FileTypes.Xliff).ToList();

			if (tmxFiles.Any())
			{
				filesCollection.AddRange(GetTmxImportFiles(wizardModel, sourceLanguage, targetLanguage, tmxFiles));
			}
			if (xliffFiles.Any())
			{
				filesCollection.AddRange(GetXliffImportFiles(wizardModel, sourceLanguage, targetLanguage, xliffFiles));
			}

			import.FilesCollection = new ObservableCollection<FileDetails>(filesCollection);
			return import;
		}

		public void ProcessXliffFile(FileDetails fileDetails, TmDetails tmDetails,ITranslationMemoryService translationMemoryService)
		{
			var converter = DefaultFileTypeManager.CreateInstance(true)
				.GetConverterToDefaultBilingual(fileDetails.Path, fileDetails.Path, null);
			var contentProcessor = new FileProcessor(tmDetails, fileDetails.Name, translationMemoryService);
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
			converter.Parse();
		}
			
		public void ProcessTmxFile(FileDetails file, TmDetails tm, ITranslationMemoryService translationMemoryService)
		{
			var tmExporter = new TmxProcessor(tm, file.Name, translationMemoryService);
			tmExporter.ImportFile(file.Path);
		}

		public bool IsImportAvailable(ObservableCollection<Model.Import> importCollection)
		{
			return importCollection.Any(importModel => importModel.FilesCollection.Any());
		}

		/// <summary>
		/// Get xliff files based on language variation selected
		/// </summary>
		private List<FileDetails> GetXliffImportFiles(WizardModel wizardModel, CultureInfo sourceLanguage,
			CultureInfo targetLanguage, List<FileDetails> xliffFiles)
		{
			if (!wizardModel.ImportSettings.ExcludeVariantsForXliff)
			{
				return GetFilesForIncludeVariants(xliffFiles, sourceLanguage, targetLanguage);
			}
			return GetFilesForExcludeVariants(xliffFiles, sourceLanguage, targetLanguage);
		}

		/// <summary>
		/// Get tmx files based on language variation selected
		/// </summary>
		private List<FileDetails> GetTmxImportFiles(WizardModel wizardModel, CultureInfo sourceLanguage,
			CultureInfo targetLanguage, List<FileDetails> tmxFiles)
		{
			if (!wizardModel.ImportSettings.ExcludeVariantsForTmx)
			{
				return GetFilesForIncludeVariants(tmxFiles, sourceLanguage, targetLanguage);
			}
			return GetFilesForExcludeVariants(tmxFiles, sourceLanguage, targetLanguage);
		}

		/// <summary>
		/// In case we want to include language variants we group using two letter iso
		/// </summary>
		private List<FileDetails> GetFilesForIncludeVariants(List<FileDetails> filesList, CultureInfo sourceLanguage,
			CultureInfo targetLanguage)
		{
			var filesCollection = filesList.Where(f =>
				f.SourceLanguage.TwoLetterISOLanguageName.Equals(sourceLanguage.TwoLetterISOLanguageName) &&
				f.TargetLanguage.TwoLetterISOLanguageName.Equals(targetLanguage.TwoLetterISOLanguageName));

			return filesCollection.ToList();
		}

		/// <summary>
		/// In case we want to exclude language variants we group using source language name
		/// </summary>
		private List<FileDetails> GetFilesForExcludeVariants(List<FileDetails> filesList, CultureInfo sourceLanguage,
			CultureInfo targetLanguage)
		{
			var filesCollection = filesList.Where(f =>
				f.SourceLanguage.Name.Equals(sourceLanguage.Name) &&
				f.TargetLanguage.Name.Equals(targetLanguage.Name));

			return filesCollection.ToList();
		}
	}
}
