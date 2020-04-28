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
			ImportInTmWithNameField(@"tm path");
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
			{@"files path",
				@""
			};

			foreach (var file in files)
			{
				var converter = DefaultFileTypeManager.CreateInstance(true).GetConverterToDefaultBilingual(file,file, null);
				var fileInfo = new FileInfo(file);
				var contentProcessor = new FileProcessor(tm,fileInfo.Name);
				converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
				converter.Parse();
			}
		}

		private void ImportFiles(TranslationMemoryImporter tmImporter)
		{
			tmImporter.Import(@"");
			tmImporter.Import(@"");
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
