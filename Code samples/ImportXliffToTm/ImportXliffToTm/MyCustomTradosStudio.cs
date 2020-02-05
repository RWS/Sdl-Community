using System.Collections.Generic;
using System.IO;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ImportXliffToTm
{
	[ApplicationInitializer]
	public class MyCustomTradosStudio : IApplicationInitializer
	{
		public void Execute()
		{
			//ImportInTmWithoutFields(
			//	@"C:\Users\aghisa\Documents\Studio 2019\Translation Memories\SDLTMPortTestWithoutFields.sdltm");
			ImportInTmWithNameField(@"C:\Users\aghisa\Documents\Studio 2019\Translation Memories\SDLImportFields.sdltm");
		}

		private void ImportInTmWithoutFields(string tmPath)
		{
			var tm = new FileBasedTranslationMemory(tmPath);
			var tmImporter = new TranslationMemoryImporter(tm.LanguageDirection);
			tmImporter.BatchImported += TmImporter_BatchImported;
			tmImporter.ChunkSize = 20;
			var importSettings = tmImporter.ImportSettings;
			importSettings.ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge;
			ImportFiles(tmImporter);
		}

		private void ImportInTmWithNameField(string tmPath)
		{
			var tm = new FileBasedTranslationMemory(tmPath);
			var files = new List<string>
			{
				@"C:\Users\aghisa\Documents\Studio 2019\Projects\SDLTMImportEn-DE\de-de\first.txt.sdlxliff",
				@"C:\Users\aghisa\Documents\Studio 2019\Projects\SDLTMImportEn-DE\de-de\second.txt.sdlxliff"
			};

			foreach (var file in files)
			{
				var converter = DefaultFileTypeManager.CreateInstance(true).GetConverterToDefaultBilingual(file,file, null);
				var fileInfo = new FileInfo(file);
				var contentProcessor = new FileProcessor(tm,fileInfo.Name);
				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter.Parse();
			}
		

			//var tmImporter = new TranslationMemoryImporter(tm.LanguageDirection);
			//tmImporter.BatchImported += TmImporter_BatchImported;
			//tmImporter.ChunkSize = 20;
			//var importSettings = tmImporter.ImportSettings;
			//importSettings.ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge;
			//var fields = tm.FieldDefinitions;
		
			//tm.Save();
			//ImportFiles(tmImporter);
		}

		private void ImportFiles(TranslationMemoryImporter tmImporter)
		{
			tmImporter.Import(@"C:\Users\aghisa\Documents\Studio 2019\Projects\SDLTMImportEn-DE\de-de\first.txt.sdlxliff");
			tmImporter.Import(@"C:\Users\aghisa\Documents\Studio 2019\Projects\SDLTMImportEn-DE\de-de\second.txt.sdlxliff");
		}

		private void TmImporter_BatchImported(object sender, BatchImportedEventArgs e)
		{
			string info;
			var stats = e.Statistics;

			info = "Total read: " + stats.TotalRead + "\n";
			info += "Total imported: " + stats.TotalImported + "\n";
			info += "TUs added: " + stats.AddedTranslationUnits + "\n";
			info += "TUs discarded: " + stats.DiscardedTranslationUnits + "\n";
			info += "TUs merged: " + stats.MergedTranslationUnits + "\n";
			info += "Errors: " + stats.Errors + "\n";
		}
	}
}
