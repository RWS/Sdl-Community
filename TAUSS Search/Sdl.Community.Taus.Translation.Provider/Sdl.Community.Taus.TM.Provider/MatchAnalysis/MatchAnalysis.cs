using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Settings;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.MatchAnalysis
{
    public class MatchAnalysis
    {
        public double GetPercentageLookUp(List<SegmentSection> sourceOriginal, List<SegmentSection> sourceServer, SearchSettings settings)
        {

            double matchPercentage = 100;

            var wordsSourceOriginal = GetSectionWords(sourceOriginal);
            var wordsSourceServer = GetSectionWords(sourceServer);

            var tagsSourceOriginal = GetSectionTags(sourceOriginal);
            var tagsSourceServer = GetSectionTags(sourceServer);


            var wordsContentOriginal = GetSectionText(sourceOriginal);
            var wordsContentSource = GetSectionText(sourceServer);

            var tagsContentOriginal = GetSectionTagText(sourceOriginal);
            var tagsContentSource = GetSectionTagText(sourceServer);


            decimal wordsPercentage = 0;
            decimal tagsPercentage = 0;

            double wordsPercentagePenalty = 0;  
            double differentFormattingPenalty = 0;
            double missingFormattingPenalty = 0;

            #region  |  Word Matching |

            var iWordsMatched = 0;
            foreach (var sSourceOriginal in wordsSourceOriginal)
            {
                foreach (var sSourceServer in wordsSourceServer)
                {
                    if (string.Compare(sSourceOriginal, sSourceServer, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    wordsSourceServer.Remove(sSourceServer);
                    iWordsMatched++;
                    break;
                }
            }
            if (wordsSourceOriginal.Count > 0)
            {
                if (iWordsMatched >= wordsSourceOriginal.Count)
                {
                    wordsPercentage = decimal.Divide(iWordsMatched, wordsSourceServer.Count + iWordsMatched) * 100;
                }
                else
                {
                    if (wordsSourceServer.Count + iWordsMatched > wordsSourceOriginal.Count)
                    {
                        wordsPercentage = decimal.Divide(iWordsMatched, wordsSourceServer.Count + iWordsMatched) * 100;                  
                    }
                    else
                    {
                        wordsPercentage = decimal.Divide(iWordsMatched, wordsSourceOriginal.Count) * 100;
                    }
                }
            }
            else if (wordsContentOriginal.Trim() == string.Empty && wordsContentSource.Trim() == string.Empty)
            {
                wordsPercentage = 100;
            }

            //return  value to the reference
            wordsPercentagePenalty = 100 - Convert.ToDouble(wordsPercentage);


            if (string.CompareOrdinal(wordsContentOriginal, wordsContentSource) != 0)
            {
                differentFormattingPenalty += settings.PenaltySettings.DifferentFormattingPenalty;
            }

            #endregion

            #region  |  Tags Matching |

            var iTagsMatched = 0;
            foreach (var sSourceOriginal in tagsSourceOriginal)
            {
                foreach (var sSourceServer in tagsSourceServer)
                {
                    if (string.Compare(sSourceOriginal, sSourceServer, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    tagsSourceServer.Remove(sSourceServer);
                    iTagsMatched++;
                    break;
                }
            }
            if (tagsSourceOriginal.Count > 0)
            {
                if (iTagsMatched >= tagsSourceOriginal.Count)
                {
                    tagsPercentage = decimal.Divide(iTagsMatched, tagsSourceServer.Count + iTagsMatched) * 100;
                }
                else
                {
                    if (tagsSourceServer.Count + iTagsMatched > tagsSourceOriginal.Count)
                    {
                        tagsPercentage = decimal.Divide(iTagsMatched, tagsSourceServer.Count + iTagsMatched) * 100;
                    }
                    else
                    {
                        tagsPercentage = decimal.Divide(iTagsMatched, tagsSourceOriginal.Count) * 100;
                    }
                }
            }
            else if (tagsContentOriginal.Trim() == string.Empty && tagsContentSource.Trim() == string.Empty)
            {
                tagsPercentage = 100;
            }


            missingFormattingPenalty = 100 - Convert.ToDouble(tagsPercentage);
            if (missingFormattingPenalty > 0)
            {
                missingFormattingPenalty = settings.PenaltySettings.MissingFormattingPenalty;
            }


            if (string.CompareOrdinal(tagsContentOriginal, tagsContentSource) != 0)
            {
                differentFormattingPenalty += settings.PenaltySettings.DifferentFormattingPenalty;
            }


            #endregion



            var totalPenalties = wordsPercentagePenalty + differentFormattingPenalty + missingFormattingPenalty;

            if (totalPenalties > 0)
            {
                matchPercentage = matchPercentage - totalPenalties;
            }





            return Math.Round(Convert.ToDouble(matchPercentage), 2);
        
        }

        public double GetPercentageConcordance(List<SegmentSection> sourceOriginal, List<SegmentSection> sourceServer, SearchSettings settings)
        {

            double matchPercentage = 100;


            var sourceOriginalComplete = GetSectionTextComplete(sourceOriginal);
            var sourceOriginalWot = GetSectionText(sourceOriginal);

            var sourceServerComplete = GetSectionTextComplete(sourceServer);
            var sourceServerWot = GetSectionText(sourceServer);


            //check if the original string exists in the server source
            if (sourceServerWot.ToLower().Contains(sourceOriginalWot.ToLower().Trim()))
            {
                //check for exact match
                if (sourceServerComplete.Contains(sourceOriginalComplete.Trim()))
                {
                    matchPercentage = 100;
                }
                //check for exact match with formatting difference
                else if (sourceServerComplete.ToLower().Contains(sourceOriginalComplete.ToLower().Trim()))
                {
                    matchPercentage = 99;
                }
                //check for exact match with multiple formating differences
                else
                {
                    matchPercentage = 98;
                }
            }
            else
            {
                //check individual words         
                var sourceOriginalWotList = GetSectionWords(sourceOriginal);
                var sourceServerWotList = GetSectionWords(sourceServer);

                var penalty = 0;
                decimal wordsMatched = 0;
                foreach (var word in sourceOriginalWotList)
                {
                    var foundExact = false;
                    var foundPenalty = false;
                    foreach (var wordServer in sourceServerWotList)
                    {
                        if (string.CompareOrdinal(word, wordServer) != 0) continue;
                        foundExact = true;
                        sourceServerWotList.Remove(wordServer);                            
                        break;
                    }
                    if (!foundExact)
                    {
                        foreach (var wordServer in sourceServerWotList)
                        {
                            if (string.Compare(word, wordServer, StringComparison.OrdinalIgnoreCase) != 0) continue;
                            foundPenalty = true;
                            sourceServerWotList.Remove(wordServer);
                            break;
                        }
                    }


                    if (foundExact)
                    {
                        wordsMatched++;
                    }
                    else if (foundPenalty)
                    {
                        wordsMatched++;
                        penalty++;
                    }
                }

                decimal wordsPercentage = 100;
                if (wordsMatched > 0 && sourceOriginalWotList.Count > 0)
                {
                    wordsPercentage = decimal.Divide(wordsMatched, Convert.ToDecimal(sourceOriginalWotList.Count)) * 100;
                }

                if (wordsPercentage >= penalty)
                    wordsPercentage = wordsPercentage - penalty;


                matchPercentage = Convert.ToDouble(wordsPercentage);
            }
            



            return Math.Round(Convert.ToDouble(matchPercentage), 2);

        }


        private static string GetSectionText(IEnumerable<SegmentSection> sections)
        {
            return (from section in sections where section.IsText select section.Content into sectionText select sectionText.Replace("‘", "'") into sectionText select sectionText.Replace("’", "'") into sectionText select sectionText.Replace("`", "'") into sectionText select sectionText.Replace("´", "'") into sectionText select sectionText.Replace("“", "\"") into sectionText select sectionText.Replace("”", "\"")).Aggregate(string.Empty, (current, sectionText) => current + sectionText);
        }
        private static string GetSectionTextComplete(IEnumerable<SegmentSection> sections)
        {
            return (from section in sections select section.Content into sectionText select sectionText.Replace("‘", "'") into sectionText select sectionText.Replace("’", "'") into sectionText select sectionText.Replace("`", "'") into sectionText select sectionText.Replace("´", "'") into sectionText select sectionText.Replace("“", "\"") into sectionText select sectionText.Replace("”", "\"")).Aggregate(string.Empty, (current, sectionText) => current + sectionText);
        }
        private static string GetSectionTagText(IEnumerable<SegmentSection> sections)
        {
            return sections.Where(section => !section.IsText).Aggregate(string.Empty, (current, section) => current + section.Content);
        }
        private List<string> GetSectionWords(IEnumerable<SegmentSection> sections)
        {
            var sectionWords = new List<string>();
            foreach (var section in sections)
            {
                if (section.IsText)
                {
                    var sectionText = section.Content;

                    sectionText = sectionText.Replace("‘", "'");
                    sectionText = sectionText.Replace("’", "'");
                    sectionText = sectionText.Replace("`", "'");
                    sectionText = sectionText.Replace("´", "'");
                    sectionText = sectionText.Replace("“", "\"");
                    sectionText = sectionText.Replace("”", "\"");

                    sectionWords.AddRange(GetWordList(sectionText));
                }
               
            }

            return sectionWords;
        }
        private static List<string> GetSectionTags(List<SegmentSection> sections)
        {
            var sectionWords = new List<string>();
            foreach (var section in sections)
            {
                if (section.IsText)
                {
                    //sectionWords.AddRange(getWordList(section.content));
                }
                else
                {
                    sectionWords.Add(section.Content);
                }
            }

            return sectionWords;
        }
        private readonly string[] _punctuations = { "!", ":", ".", ";", "?" };
        private IEnumerable<string> GetWordList(string text)
        {
            var wordsList = new List<string>();


            #region  |  create a list of words  |

            var words = text.Split(' ');

            foreach (var word in words)
            {
                #region  |  word  |

                var wordTmp = string.Empty;

                foreach (var _char in word.ToCharArray())
                {
                    if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters
                    {
                        if (wordTmp != string.Empty)
                            wordsList.Add(wordTmp);
                        wordTmp = string.Empty;

                        wordsList.Add(_char.ToString());
                    }
                    else
                        wordTmp += _char.ToString();
                }

                if (wordTmp != string.Empty)
                {
                    wordsList.Add(wordTmp);                   
                }
                #endregion
            }

            #endregion

            #region  |  get rid of the last punctuation mark at beginning & end  |


            // note we don't want to get rid of all punctuation marks
            if (wordsList.Count <= 0) return wordsList;
            var firstWord = wordsList[0];
            var lastWord = wordsList[wordsList.Count - 1];

               
            if (firstWord.Trim().StartsWith("¿") && lastWord.Trim().EndsWith("?"))
            {
                wordsList[0] = wordsList[0].Substring(wordsList[0].IndexOf("¿", StringComparison.Ordinal) + 1);
            }


            if (lastWord.Trim().EndsWith("..."))
            {
                wordsList[wordsList.Count - 1] = wordsList[wordsList.Count - 1].Substring(0, wordsList[wordsList.Count - 1].LastIndexOf("...", StringComparison.Ordinal));

                if (wordsList[wordsList.Count - 1].Trim() == string.Empty)
                {
                    wordsList.RemoveAt(wordsList.Count - 1);
                }
            }
            else
            {
                var punctuation = lastWord.Trim().Substring(lastWord.Trim().Length - 1);
                
                if (_punctuations.Contains(punctuation))
                {
                    wordsList[wordsList.Count - 1] = wordsList[wordsList.Count - 1].Substring(0, wordsList[wordsList.Count - 1].LastIndexOf(punctuation, StringComparison.Ordinal));
                }
            }

            #endregion

            return wordsList;
        }
    }
}
