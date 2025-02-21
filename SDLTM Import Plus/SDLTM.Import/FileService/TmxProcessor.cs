using Sdl.LanguagePlatform.IO.Streams;
using Sdl.LanguagePlatform.IO.TMX;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SDLTM.Import.Interface;
using System.Collections.Generic;
using System.Linq;
using SDLTM.Import.Model;

namespace SDLTM.Import.FileService
{
	public class TmxProcessor
	{
		private readonly TmDetails _tmDetails;
		private readonly string _fileName;
		private readonly FileBasedTranslationMemory _tm;
		private readonly ITranslationMemoryService _translationMemoryService;
		private readonly List<TranslationUnit> _translationUnits;
		private readonly bool[] _masks;

		public TmxProcessor(TmDetails tmDetails,string fileName,ITranslationMemoryService translationMemoryService)
		{
			_tmDetails = tmDetails;
			_fileName = fileName;
			_translationMemoryService = translationMemoryService;
			_tm = tmDetails.TranslationMemory;
			_masks = new[] { false, true };
			_translationUnits = new List<TranslationUnit>();
		}

		public void ImportFile(string filePath)
		{
			var settings = new TMXReaderSettings
			{
				ValidateAgainstSchema = false,
				ResolveNeutralCultures = true
			};

			var result = new ScanResult();

			using (var rdr = new TMXReader(filePath, settings))
			{
				Event ev;

				var cancelled = false;

				var read = 0;

				while ((ev = rdr.Read()) != null && !cancelled)
				{
					TUEvent tuv;
					TMXStartOfInputEvent soi;

					if ((soi = ev as TMXStartOfInputEvent) != null)
					{
						result.TmxHeader = soi;
					}
					else if ((tuv = ev as TUEvent) != null)
					{
						++read;
						_tmDetails.ImportSummary.ReadTusCount++;
						_translationMemoryService.SetFieldsToTu(_tm, _tmDetails, tuv.TranslationUnit, $"# {read}", _fileName);

						if (_translationUnits.Count == 0)
						{
							_translationMemoryService.ImportTranslationUnits(_tm, _tmDetails, new[] { tuv.TranslationUnit }, new[] { true });
						}
						else
						{
							var transUnits = new[] { _translationUnits.Last(), tuv.TranslationUnit };
							_translationMemoryService.ImportTranslationUnits(_tm, _tmDetails, transUnits, _masks);
						}
						_translationUnits.Add(tuv.TranslationUnit);
					}
				}
				_tm.Save();
			}
		}
	}
}
