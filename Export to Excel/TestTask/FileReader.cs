using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace ExportToExcel
{
    public class FileReader : IBilingualContentProcessor
	{
        private readonly DataExtractor _dataExtractor;
        private readonly GeneratorSettings _convertSettings;
        private readonly string _originalFilePath;
        private ExcelSuperWriter _excelSuperWriter;
		private IBilingualContentHandler _output;

		public FileReader(DataExtractor dataExtr, GeneratorSettings settings, string filePath)
        {
            _dataExtractor = dataExtr;
            _convertSettings = settings;
            _originalFilePath = filePath;
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (paragraphUnit.IsStructure)
            {
                return;
            }
	      
            foreach (var item in paragraphUnit.SegmentPairs)
            {

                if (SkipSegment(item))
                {
                    continue;
                }

                var sourceText = ReadSegment(item.Source, out var sourceTokens);			
	            var targetText = ReadSegment(item.Target, out var targetTokens);
                var comments = _dataExtractor.Comments;
                
                _excelSuperWriter.WriteEntry(item.Properties.Id.Id, sourceText, targetText, comments,
                    item.Properties);
            }
        }

		private string ReadSegment(ISegment item, out List<Token> tokens)
		{
			_dataExtractor.Process(item);

			var text = _dataExtractor.PlainText.ToString();
			tokens = CloneList(_dataExtractor.Tokens);

			return text;
		}

		private bool SkipSegment(ISegmentPair segmentPair)
        {
            var origin = segmentPair.Properties.TranslationOrigin;
            var confLevel = segmentPair.Properties.ConfirmationLevel;

			if(_convertSettings.ExcludeExportType == GeneratorSettings.ExclusionType.Locked 
				&& segmentPair.Properties.IsLocked)
			{
				return true;
			}

	        if (_convertSettings.ExcludeExportType == GeneratorSettings.ExclusionType.Status)
	        {
		        if (_convertSettings.ExcludedStatuses.Contains(confLevel))
		        {
			        return true;
		        }
	        }
	        else
	        {
		        if (origin == null)
		        {
			        return _convertSettings.DontExportNoMatch;
		        }

		        if ((origin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget) &&
		            _convertSettings.DontExportContext)
		        {

			        return true;
		        }

		        if (origin.MatchPercent == 100 && _convertSettings.DontExportExact)
		        {
			        return true;
		        }
		        if ((origin.MatchPercent > 0 && origin.MatchPercent < 100) && _convertSettings.DontExportFuzzy)
		        {
			        return true;
		        }
		        if (origin.MatchPercent == 0 && _convertSettings.DontExportNoMatch)
		        {
			        return true;
		        }
	        }

	        return false;
        }

        private static List<Token> CloneList(IEnumerable<Token> list)
        {
	        return list.ToList();
        }

		/// <summary>
		/// Start of new file
		/// </summary>
		/// <param name="fileInfo"></param>
		public void SetFileProperties(IFileProperties fileInfo)
		{
			// not required with this implementation
		}


		public void FileComplete()
        {
	        // not required with this implementation						
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			//get output file name
			var info = new FileInfo(_originalFilePath);
			var targetfilename = info.DirectoryName + Path.DirectorySeparatorChar + _convertSettings.FileNamePrefix +
			                        info.Name.Substring(0, info.Name.IndexOf(info.Extension, StringComparison.Ordinal));

			//initialize Excel writer
			_excelSuperWriter = new ExcelSuperWriter();
			_excelSuperWriter.Initialize(targetfilename + ".Preview.xlsx", _convertSettings);

		}

		/// <summary>
		/// Using Complete insead of FileComplete for merged files.
		/// </summary>
		public void Complete()
        {
			_excelSuperWriter.Complete();
		}

		public IBilingualContentHandler Output
		{
			get => _output;
			set => _output = value;
		}
	}
}
