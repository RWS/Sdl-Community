using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;

namespace SDLTM.Import.FileService
{
	public class FileProcessor : AbstractBilingualContentProcessor
	{
		private readonly TmDetails _tmDetails;
		private readonly FileBasedTranslationMemory _tm;
		private readonly ITranslationMemoryService _translationMemoryService;
		private readonly bool[] _masks;
		private readonly string _fileName;
		public static readonly Log Log = Log.Instance;
		private readonly List<TranslationUnit> _translationUnits;

		public FileProcessor(TmDetails tmDetails, string fileName,ITranslationMemoryService translationMemoryService)
		{
			_tmDetails = tmDetails;
			_tm = tmDetails.TranslationMemory;
			_translationMemoryService = translationMemoryService;
			_fileName = fileName;
			_masks = new []{false,true};
			_translationUnits = new List<TranslationUnit>();
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			try
			{
				base.ProcessParagraphUnit(paragraphUnit);
				if (paragraphUnit.IsStructure)
				{
					return;
				}

				foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
				{
					var languagePair = new LanguagePair(_tm.LanguageDirection.SourceLanguage, _tm.LanguageDirection.TargetLanguage);
					var tu = TuConverter.BuildLinguaTranslationUnit(languagePair, segmentPair, paragraphUnit.Properties, false, false);
					_tmDetails.ImportSummary.ReadTusCount++;
					_translationMemoryService.SetFieldsToTu(_tm,_tmDetails,tu, "#"+segmentPair.Properties.Id.Id,_fileName);

					if (_translationUnits.Count == 0)
					{
						_translationMemoryService.ImportTranslationUnits(_tm,_tmDetails,new []{tu},new []{true});
					}
					else
					{
						var transUnits = new[] { _translationUnits.Last(), tu};
						_translationMemoryService.ImportTranslationUnits(_tm, _tmDetails, transUnits, _masks);
					}
					_translationUnits.Add(tu);
				}
				_tm.Save();
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Process ParagraphUnit: {e.Message}\n {e.StackTrace}");
			}
		}
		
	}
}
