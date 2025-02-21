using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;


namespace Sdl.Community.Utilities.TMTool.Lib.TMHelpers
{
	public class Exporter
	{
		private TranslationMemoryExporter _exporter;
		private int TUs;

		public delegate void OnProgressDelegate(double progress, int operationType);
		public event OnProgressDelegate OnProgress;

		/// <summary>
		/// number of translation units exported
		/// </summary>
		public int TUsExported
		{
			get;
			private set;
		}

		/// <summary>
		/// TMX file TM was exported to
		/// </summary>
		public string TMXPath
		{
			get;
			private set;
		}

		/// <summary>
		/// creates new exporter
		/// </summary>
		public Exporter()
		{
			_exporter = new TranslationMemoryExporter();
		}

		/// <summary>
		/// perform TUs export operation from TM file to TMX
		/// </summary>
		/// <param name="memoryData">TM file to export from</param>
		public void Export(FileBasedTranslationMemory memoryData)
		{
			TUs = memoryData.GetTranslationUnitCount();
			TUsExported = 0;
			ProgressExport(0);

			_exporter.TranslationMemoryLanguageDirection = memoryData.LanguageDirection;
			_exporter.BatchExported += new EventHandler<BatchExportedEventArgs>(exporter_BatchExported);

			// set TMX file name
			TMXPath = string.Format(@"{0}\{1}.tmx",
				Path.GetDirectoryName(memoryData.FilePath),
				Path.GetFileNameWithoutExtension(memoryData.FilePath));
			TMXPath = FileHelper.ChangeFileName(TMXPath, @"{0}\{1}_{2}.tmx");

			// begin export
			_exporter.Export(TMXPath, true);

			ProgressExport(100);
		}

		void exporter_BatchExported(object sender, BatchExportedEventArgs e)
		{
			TUsExported = e.TotalExported;

			if (TUs > 0)
				ProgressExport(TUsExported * 100 / TUs);
		}

		private void ProgressExport(double progress)
		{
			if (this.OnProgress != null)
			{
				this.OnProgress(progress, 0);
			}
		}
	}
}