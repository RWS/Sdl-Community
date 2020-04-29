using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.Utilities.TMTool.Task;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemapTMX
{
	public class RemapTMXTask : ITask
	{
		#region Fields

		/// <summary>
		/// TaskID field.
		/// </summary>
		private Guid taskID = new Guid("B58B57D8-36B5-4524-A04C-D4E72AFD9D8D");

		/// <summary>
		/// Extension supported by task.
		/// </summary>
		private string extension = "*.sdltm";

		/// <summary>
		/// Cultures that are present in SDL Trados Studio 2009, but not in Trados Workbench 2007.
		/// </summary>
		private string[] notSupportedCultures =
												{
													"fa-IR", "sw-KE", "zh-MO", "fr-MC", "en-ZW", "en-PH", "zh-CHT", "smj-NO",
													"bs-Cyrl-BA", "rm-CH", "moh-CA", "arn-CL", "smn-FI", "lb-LU", "sms-FI",
													"sma-SE", "ps-AF", "fy-NL", "smj-SE", "iu-Latn-CA"
												};

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the RemapTMXTask class.
		/// </summary>
		public RemapTMXTask()
		{
			this.SupportedFileTypes = new Dictionary<string, string>();
			this.SupportedFileTypes.Add(this.extension, string.Format(Properties.Resources.TmxDescription, this.extension));

			this.Control = new RemapTMXControl();
		}

		#endregion

		#region Events

		/// <summary>
		/// OnProgress event.
		/// </summary>
		public event OnProgressDelegate OnProgress;

		/// <summary>
		/// OnLogAdded event.
		/// </summary>
		public event OnAddLogDelegate OnLogAdded;

		#endregion

		#region Properties

		/// <summary>
		/// Gets TaskID. This task has B58B57D8-36B5-4524-A04C-D4E72AFD9D8D id.
		/// </summary>
		public Guid TaskID
		{
			get
			{
				return this.taskID;
			}
		}

		/// <summary>
		/// Gets task's friendly name. 
		/// </summary>
		public string TaskName
		{
			get
			{
				return Properties.Resources.TaskName;
			}
		}

		/// <summary>
		/// Gets pairs of supported file extensions.
		/// </summary>
		public Dictionary<string, string> SupportedFileTypes
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets <see cref="T:System.Windows.Forms.UserControl"/> to be displayed.
		/// </summary>
		public IControl Control
		{
			get;
			private set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Performs task.
		/// </summary>
		/// <param name="fileName">Physical path to file.</param>
		public void Execute(string fileName)
		{
			try
			{
				FileBasedTranslationMemory tm = new FileBasedTranslationMemory(fileName);
				RemapTMXSettings settings = (RemapTMXSettings)this.Control.Options;

				// check for unsupported languages
				string culture = string.Empty;
				bool containsUnsupported = false;

				if (this.notSupportedCultures.Contains(tm.LanguageDirection.SourceLanguage.Name))
				{
					culture = tm.LanguageDirection.SourceLanguage.EnglishName;
					containsUnsupported = true;
				}

				if (this.notSupportedCultures.Contains(tm.LanguageDirection.TargetLanguage.Name))
				{
					culture = tm.LanguageDirection.TargetLanguage.EnglishName;
					containsUnsupported = true;
				}

				if (containsUnsupported)
				{
					var result = MessageBox.Show(
									string.Format(
										Properties.Resources.NotSupportedCulture,
										culture),
									string.Empty,
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Information,
									MessageBoxDefaultButton.Button2);
					if (result == DialogResult.No)
					{
						this.OnProgressChanged(100, string.Empty);
						this.OnLogAddedChanged(string.Format(Properties.Resources.SkipTM, fileName));
						return;
					}
				}

				this.OnProgressChanged(0, string.Format(Properties.Resources.TMExportStarted, fileName));

				// Export into TMX
				TranslationMemoryExporter exporter = new TranslationMemoryExporter()
				{
					ChunkSize = Sdl.LanguagePlatform.TranslationMemoryApi.TranslationMemoryExporter.DefaultTranslationUnitChunkSize,
					TranslationMemoryLanguageDirection = tm.LanguageDirection
				};

				exporter.BatchExported += (object obj, BatchExportedEventArgs args) =>
				{
					StringBuilder info = new StringBuilder();

					info.AppendLine(string.Format(Properties.Resources.TotalTUProcessed, args.TotalProcessed));
					info.AppendLine(string.Format(Properties.Resources.TotalTUExported, args.TotalExported));

					this.OnLogAddedChanged(info.ToString());
					args.Cancel = false;
				};

				string targetTMXFile = string.Format(
					"{0}{1}{2}.tmx",
					settings.TargetFolder,
					Path.DirectorySeparatorChar,
					Path.GetFileNameWithoutExtension(fileName));
				exporter.Export(targetTMXFile, true);

				// convert TMX into required format

				var flavour = TranslationUnitFormat.TradosTranslatorsWorkbench;
				string targetTMXFlavouredFile = string.Empty;
				if (settings.SaveIntoTargetFolder)
				{
					targetTMXFlavouredFile = string.Format(
					"{0}{1}{2}_Trados2019.tmx",
					Path.GetDirectoryName(fileName),
					Path.DirectorySeparatorChar,
					Path.GetFileNameWithoutExtension(fileName));
				}
				else
				{
					targetTMXFlavouredFile = string.Format(
					"{0}{1}{2}_Trados2019.tmx",
					settings.TargetFolder,
					Path.DirectorySeparatorChar,
					Path.GetFileNameWithoutExtension(fileName));
				}

				this.Convert(targetTMXFile, targetTMXFlavouredFile, flavour);
				this.OnProgressChanged(0, string.Empty);
				this.OnLogAddedChanged(string.Format(Properties.Resources.TMXFlavouredConverted, targetTMXFile));

				File.Delete(targetTMXFile);

				this.OnProgressChanged(100, string.Empty);
				this.OnLogAddedChanged(string.Format(Properties.Resources.TMExportFinished, fileName));
			}
			catch (Exception ex)
			{
				this.OnProgressChanged(0, string.Empty);
				this.OnLogAddedChanged(string.Format(Properties.Resources.RemapException, ex.Message));
			}
		}

		#endregion

		#region Privates

		/// <summary>
		/// Handles progress changes.
		/// </summary>
		/// <param name="progress">New progress value.</param>
		/// <param name="message">Message to be displayed.</param>
		private void OnProgressChanged(double progress, string message)
		{
			if (this.OnProgress != null)
			{
				this.OnProgress(progress, message);
			}
		}

		/// <summary>
		/// Appends text message into log.
		/// </summary>
		/// <param name="message">String message to be written into log.</param>
		private void OnLogAddedChanged(string message)
		{
			if (this.OnLogAdded != null)
			{
				this.OnLogAdded(string.Format("{0}{1}", message, System.Environment.NewLine));

			}
		}

		/// <summary>
		/// Converts input file into required flavour.
		/// </summary>
		/// <param name="inputFile">Physical path to input file.</param>
		/// <param name="outputFile">Physical path to output file.</param>
		/// <param name="flavor">Required flavour.</param>
		private void Convert(string inputFile, string outputFile, Sdl.LanguagePlatform.TranslationMemory.TranslationUnitFormat flavor)
		{
			try
			{
				Sdl.LanguagePlatform.IO.Streams.TUStreamContext ctx
					= new Sdl.LanguagePlatform.IO.Streams.TUStreamContext();

				Sdl.LanguagePlatform.IO.TMX.TMXReaderSettings readerSettings
					= new Sdl.LanguagePlatform.IO.TMX.TMXReaderSettings(ctx, false, true, false);

				Sdl.LanguagePlatform.IO.TMX.TMXWriterSettings writerSettings
					= new Sdl.LanguagePlatform.IO.TMX.TMXWriterSettings(System.Text.Encoding.UTF8);

				writerSettings.TargetFormat = flavor;

				using (Sdl.LanguagePlatform.IO.TMX.TMXReader tmxReader
					= new Sdl.LanguagePlatform.IO.TMX.TMXReader(inputFile, readerSettings))
				{
					using (Sdl.LanguagePlatform.IO.TMX.TMXWriter tmxWriter
						= new Sdl.LanguagePlatform.IO.TMX.TMXWriter(outputFile, writerSettings))
					{
						Sdl.LanguagePlatform.IO.Streams.Event e;

						while ((e = tmxReader.Read()) != null)
						{
							tmxWriter.Emit(e);
						}
					}
				}
			}
			catch (Exception ex)
			{
				string error = string.Format("RemapTMX.Convert {0} to {1} with {2}{3}", inputFile, outputFile, flavor, System.Environment.NewLine);
				MessageBox.Show(error, "Informative message");
			}
		}

		#endregion
	}
}
