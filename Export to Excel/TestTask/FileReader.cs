using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace ExportToExcel
{
    public class FileReader : AbstractBilingualContentProcessor
    {
        private readonly DataExtractor _dataExtractor;
        private readonly GeneratorSettings _convertSettings;
        private readonly string _originalFilePath;
        private ExcelSuperWriter _excelSuperWriter;

        public FileReader(DataExtractor dataExtr, GeneratorSettings settings, string filePath)
        {
            _dataExtractor = dataExtr;
            _convertSettings = settings;
            _originalFilePath = filePath;
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (paragraphUnit.IsStructure)
            {
                return;
            }

            foreach (ISegmentPair item in paragraphUnit.SegmentPairs)
            {

                if (SkipSegment(item))
                {
                    continue;
                }

                _dataExtractor.Process(item.Source);
                var sourceText = _dataExtractor.PlainText.ToString();
                List<Token> sourceTokens = CloneList(_dataExtractor.Tokens);
                _dataExtractor.Process(item.Target);
                string targetText = _dataExtractor.PlainText.ToString();
                List<Token> targetTokens = CloneList(_dataExtractor.Tokens);
                List<string> comments = _dataExtractor.Comments;
                
                _excelSuperWriter.WriteEntry(item.Properties.Id.Id, sourceText, targetText, comments,
                    item.Properties);
            }
        }

        private bool SkipSegment(ISegmentPair segmentPair)
        {
            ITranslationOrigin origin = segmentPair.Properties.TranslationOrigin;
            ConfirmationLevel confLevel = segmentPair.Properties.ConfirmationLevel;

			if(_convertSettings.ExcludeExportType == GeneratorSettings.ExclusionType.Locked 
				&& segmentPair.Properties.IsLocked)
			{
				return true;
			}
			else
			{
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
						if (_convertSettings.DontExportNoMatch)
						{
							return true;
						}
						else
						{
							return false;
						}
					}

					if ((origin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget) &&
						_convertSettings.DontExportContext)
					{

						return true;
					}
					else
					{
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
				}
			}
            
            return false;
        }

        private List<Token> CloneList(List<Token> list)
        {
            List<Token> result = new List<Token>();
            foreach (var item in list)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Start of new file
        /// </summary>
        /// <param name="fileInfo"></param>
        public override void SetFileProperties(IFileProperties fileInfo)
        {


            //get output file name
            FileInfo info = new FileInfo(_originalFilePath);
            string targetfilename = info.DirectoryName + Path.DirectorySeparatorChar + _convertSettings.FileNamePrefix +
                                    info.Name.Substring(0, info.Name.IndexOf(info.Extension));

            //initialize Excel writer

            _excelSuperWriter = new ExcelSuperWriter();
            _excelSuperWriter.Initialize(targetfilename + ".Preview.xlsx", _convertSettings);


        }


        public override void FileComplete()
        {
            //One file complete, but we may have more
            
            base.FileComplete();
        }

        /// <summary>
        /// Using Complete insead of FileComplete for merged files.
        /// </summary>
        public override void Complete()
        {
            _excelSuperWriter.Complete();
        }
    }
}
