using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

//TODO use XML Writer
//TODO offer option to clean up elements with too many tags

namespace Sdl.Community.TMOptimizerLib
{
	public class Cleaner : ProcessorBase
    {
        private readonly TmxFile _inputFile;
        private readonly Settings _settings;
        private readonly TmxFile _outputFile;
        private int? _remainingTuProcessingQuota;
        
        public Cleaner(TmxFile inputFile, TmxFile outputFile, Settings settings, int? remainingTuProcessingQuota)
        {
            _inputFile = inputFile;
            _outputFile = outputFile;
            _settings = settings;
            _remainingTuProcessingQuota = remainingTuProcessingQuota;
        }

        public int TusRead
        {
            get;
            set;
        }

        public int TusCleaned
        {
            get;
            set;
        }

        public int TagsUpdated
        {
            get;
            set;
        }

        public int TagsRemoved
        {
            get;
            set;
        }

        public void Execute()
        {
            var tusNode = from el in ReadFromTmxFile(_inputFile.FilePath) select el;

            var outputWriter = new OutputWriter(_outputFile.FilePath);
            outputWriter.InitializeWorkbenchTmx(_inputFile.DetectInfo.OriginalSourceLanguage);


            int tuIndex = 0;
            foreach (var tu in tusNode)
            {
                var source = tu.Elements("tuv").First().Element("seg");
                var target = tu.Elements("tuv").Last().Element("seg");

                if (source == null || target == null)
                {
                    continue;
                }

                int tagsUpdated = 0;
                int tagsRemoved = 0;
                    
                if ((source.Elements("bpt").Any() || target.Elements("bpt").Any()))
                {
                    StripFontTags(source, ref tagsUpdated, ref tagsRemoved);
                    StripFontTags(target, ref tagsUpdated, ref tagsRemoved);
                }

                if (_settings.RemoveOrphan && !source.Elements("bpt").Any())
                {
                    tagsRemoved += target.Elements("bpt").Count();
                    tagsRemoved += target.Elements("ept").Count();
                    target.Elements("bpt").Remove();
                    target.Elements("ept").Remove();
                }

                if(_settings.ReplaceSoftHyphen)
                {
                    if (source.Elements("ph").Any())
                    {
                        ReplaceSoftHyphen(source, ref tagsUpdated);
                    }

                    if (target.Elements("ph").Any())
                    {
                        ReplaceSoftHyphen(target, ref tagsUpdated);
                    }
                }

                // update statistics
                tuIndex++;
                TusRead++;
                if (tagsUpdated > 0 || tagsRemoved > 0)
                {
                    TusCleaned++;
                    TagsUpdated += tagsUpdated;
                    TagsRemoved += tagsRemoved;
                }

                // write TU
                outputWriter.Write(tu.ToString());
                                
                ReportProgress((int)(100.0 * tuIndex / _inputFile.GetDetectInfo().TuCount));

                if (_remainingTuProcessingQuota != null)
                {
                    _remainingTuProcessingQuota = _remainingTuProcessingQuota.Value - 1;

                    if (_remainingTuProcessingQuota.Value == 0)
                    {
                        break;
                    }
                }
            }

            outputWriter.Complete();
            
            // output file has same properties as input file
            _outputFile.DetectInfo = _inputFile.GetDetectInfo().Clone();
            _outputFile.DetectInfo.TuCount = TusRead;
                        
            ReportProgress(100);
        }

        /// <summary>
        /// Method will replace the softhyphen placeholder tag with custom value which will be converted in Studio to tag instaed to plain text
        /// </summary>
        /// <param name="tu"></param>
        /// <param name="tagsUpdated"></param>
        private static void ReplaceSoftHyphen(XElement tu, ref int tagsUpdated)
        {
            foreach (var element in tu.Elements("ph").Where(element => element.Value == "\\-"))
            {
                element.Value = "softhyphen";
                tagsUpdated++;
            }
        }

        /// <summary>
        /// Any font markup will be removed, combined markup will be cleaned from fonts.
        /// </summary>
        /// <param name="originalSegment"></param>
        /// <param name="tagsUpdated"></param>
        /// <param name="tagsRemoved"></param>
        /// <returns></returns>
        private static void StripFontTags(XElement originalSegment, ref int tagsUpdated, ref int tagsRemoved)
        {
            var listOfTagsToRemove = new List<string>();
            foreach (var element in originalSegment.Elements("bpt"))
            {
                var newValue = RemoveExtraFormatting(element.Value);
                if (String.IsNullOrEmpty(newValue.Trim()))
                {
                    listOfTagsToRemove.Add(element.Attribute("i").Value);
                    tagsRemoved++;
                }
                else if (element.Value != newValue)
                {
                    if (!newValue.StartsWith("{"))
                    {
                        element.Value = "{" + newValue;
                    }
                    else
                    {
                        element.Value = newValue;
                    }
                    tagsUpdated++;
                }
            }

            foreach (var el in listOfTagsToRemove)
            {
                var nodes = from bptept in originalSegment.Elements() where (string)bptept.Attribute("i") == el select bptept;
                nodes.Remove();
            }
        }

        /// <summary>
        /// Removes extra fonts and formatting which is not necessary to preserve:
        /// - kerning
        /// - scaling
        /// - expansion (between characters and twips)
        /// </summary>
        /// <param name="tagValue">Tag string value to clean up</param>
        /// <returns>Cleaned tag content</returns>
        private static string RemoveExtraFormatting(string tagValue)
        {
            return Regex.Replace(tagValue, "{?\\\\(f|kerning|charscalex|expnd|expndtw|lang)\\p{N}+", String.Empty);
        }


        /// <summary>
        /// Main function to read the input file in streamed manner
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        static IEnumerable<XElement> ReadFromTmxFile(string fileLocation)
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };

            using (var reader = XmlReader.Create(fileLocation, settings))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "tu")
                            {
                                var el = XNode.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }
    }
}