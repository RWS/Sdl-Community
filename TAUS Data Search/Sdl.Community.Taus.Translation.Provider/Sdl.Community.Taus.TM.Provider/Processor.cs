using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Settings;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider
{
    public class Processor
    {
        public enum ScoreType
        {
            Lookup,
            Concordance
        }

        private static readonly string SettingsFilePath;
        static Processor()
        {
            SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Taus.Translation.Provider");

            if (!Directory.Exists(SettingsFilePath))
                Directory.CreateDirectory(SettingsFilePath);

            SettingsFilePath = Path.Combine(SettingsFilePath, "Taus.Translation.Provider.settings.xml");


        }

        public SearchSegmentResult SearchSegment(SearchSettings settings, ScoreType matchScoringType)
        {

            var searchSegmentResult = new SearchSegmentResult();


            var result = GetSearchSegmentResult(settings);


            if (result.Trim() != string.Empty)
            {
                var resultSegmentParser = new ResultSegmentParser();
                searchSegmentResult = resultSegmentParser.ReadResult(result);

                //apply the analysis percentage            
                searchSegmentResult = SetMatchPercentage(searchSegmentResult, settings, matchScoringType);

                //sort on match %
                searchSegmentResult.Segments.Sort((c1, c2) => c2.MatchPercentage.CompareTo(c1.MatchPercentage));

                //respect the limit
                var limitIndex = settings.Limit - 1;
                var totalSegmentIndex = searchSegmentResult.Segments.Count - 1;

                if (settings.Limit > 0 && settings.Limit < searchSegmentResult.Segments.Count)
                {
                    searchSegmentResult.Segments.RemoveRange(limitIndex, totalSegmentIndex - limitIndex);
                }
            }
            else
            {
                searchSegmentResult.Status = "timed out";
                searchSegmentResult.Reason = "timed out";
            }

            return searchSegmentResult;
        }


        public static Dictionary<string, string> GetAttributeListings(string authKey, string attribute, string forType, string appKey)
        {
            var items = new Dictionary<string, string>();

            string result = string.Empty;

            var buffer = Encoding.ASCII.GetBytes("action=login");
            var webrequest = (HttpWebRequest)WebRequest.Create(new Uri("https://www.tausdata.org/api/attr/" + attribute + ".xml?for=" + forType + ""));
            webrequest.KeepAlive = false;
            webrequest.Method = @"GET";
            //webrequest.Proxy = null;
            webrequest.Headers.Add("X-TDA-App-Key", appKey);
            webrequest.Headers.Add("X-TDA-Auth-Key", authKey);
            webrequest.Timeout = 10000; //10 seconds
            webrequest.ContentLength = 0;
            using (var webresponse = webrequest.GetResponse() as HttpWebResponse)
            {
                if (webresponse != null)
                {
                    var reader = new StreamReader(webresponse.GetResponseStream());
                    result = reader.ReadToEnd();
                }
            }


            var resultSegmentParser = new ResultSegmentParser();
            var searchSegmentResult = resultSegmentParser.ReadResult(result);

            if (searchSegmentResult.Status == "200")
            {
                var rAttribute = new Regex(@"\<" + attribute + @"\>(?<x1>.*?|)\<\/" + attribute + @"\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var rAttributeName = new Regex(@"\<name\>(?<x1>.*?|)\<\/name\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var rAttributeId = new Regex(@"\<id\>(?<x1>.*?|)\<\/id\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                var mcAttributes = rAttribute.Matches(result);
                foreach (Match mAttribute in mcAttributes)
                {
                    var mId = rAttributeId.Match(mAttribute.Groups["x1"].Value);
                    var mName = rAttributeName.Match(mAttribute.Groups["x1"].Value);
                    if (!mId.Success || !mName.Success) continue;
                    if (!items.ContainsKey(mId.Groups["x1"].Value))
                        items.Add(mId.Groups["x1"].Value, mName.Groups["x1"].Value);
                }
            }
            else
            {
                throw new Exception("Status: " + searchSegmentResult.Status + "\r\nReason: " + searchSegmentResult.Reason);
            }

            return items;
        }


        public static string GetAuthorizationKey(string userName, string password, string appKey)
        {
            string authKey;

			var result = string.Empty;

            var buffer = Encoding.UTF8.GetBytes("action=login");
            var webrequest = (HttpWebRequest) WebRequest.Create(new Uri("https://www.tausdata.org/api/auth_key.xml"));
            webrequest.KeepAlive = false;
            webrequest.Method = @"POST";


            webrequest.ContentLength = buffer.Length;
            webrequest.Credentials = new NetworkCredential(userName, password);

            webrequest.Headers.Add("X-TDA-App-Key", appKey);


            webrequest.ContentType = "application/x-www-form-urlencoded";

            var postData = webrequest.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();

            using (var webresponse = webrequest.GetResponse() as HttpWebResponse)
            {
                if (webresponse != null)
                {
                    var reader = new StreamReader(webresponse.GetResponseStream());
                    result = reader.ReadToEnd();
                }
            }

            var resultSegmentParser = new ResultSegmentParser();
            var searchSegmentResult = resultSegmentParser.ReadResult(result);


            if (searchSegmentResult.Status == "201")
            {
                authKey = searchSegmentResult.AuthKey.Id;
            }

            else
            {
                throw new Exception("Status: " + searchSegmentResult.Status + "\r\nReason: " +
                                    searchSegmentResult.Reason);
            }

			return authKey;
		}
		
        public static void DeleteSettingsFile()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                    File.Delete(SettingsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region  |  private methods  |






        private string GetSearchSegmentResult(SearchSettings settings)
        {

            var result = string.Empty;

            var webrequest = (HttpWebRequest)WebRequest.Create(new Uri(GetSearchString(settings)));
            webrequest.KeepAlive = false;
            webrequest.Method = @"GET";
            //webrequest.Proxy = null;
            webrequest.Headers.Add("X-TDA-App-Key", settings.AppKey);
            webrequest.ContentLength = 0;
            if (settings.AuthKey != string.Empty)
                webrequest.Headers.Add("X-TDA-Auth-Key", settings.AuthKey);
            else
            {
                webrequest.Credentials = new NetworkCredential(settings.UserName, settings.Password);
            }

            webrequest.Timeout = Convert.ToInt32(settings.Timeout.ToString() + "000");


            try
            {
                using (var webresponse = webrequest.GetResponse() as HttpWebResponse)
                {
                    if (webresponse != null)
                    {
                        var reader = new StreamReader(webresponse.GetResponseStream());
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("timed out", StringComparison.Ordinal) <= -1)
                    throw;
            }
            return result;
        }




        private static SearchSegmentResult SetMatchPercentage(SearchSegmentResult searchSegmentResult, SearchSettings settings, ScoreType matchScoringType)
        {
            var matchAnalysis = new MatchAnalysis.MatchAnalysis();

            foreach (var segment in searchSegmentResult.Segments)
            {
                segment.MatchPercentage = matchScoringType == ScoreType.Lookup ? matchAnalysis.GetPercentageLookUp(settings.SearchSections, segment.SourceSections, settings) : matchAnalysis.GetPercentageConcordance(settings.SearchSections, segment.SourceSections, settings);
            }

            return searchSegmentResult;
        }

        private string GetSearchString(SearchSettings settings)
        {
            var searchString = "https://www.tausdata.org/api/segment.xml";
            searchString += "?source_lang=" + settings.SourceLanguageId;
            searchString += "&target_lang=" + settings.TargetLanguageId;
			var sectionText = GetSectionText(settings.SearchSections);

			searchString += "&q=" + GetTausCompatibleSearchText(sectionText);

            if (settings.OwnerId != null && settings.OwnerId.Trim() != string.Empty)
                searchString += "&owner=" + settings.OwnerId;

            if (settings.ContentTypeId != null && settings.ContentTypeId.Trim() != string.Empty)
                searchString += "&content_type=" + settings.ContentTypeId;

            if (settings.ProductId != null && settings.ProductId.Trim() != string.Empty)
                searchString += "&product=" + settings.ProductId;

            if (settings.ProviderId != null && settings.ProviderId.Trim() != string.Empty)
                searchString += "&provider=" + settings.ProviderId;

            if (settings.IndustryId != null && settings.IndustryId.Trim() != string.Empty)
                searchString += "&industry=" + settings.IndustryId;

            if (settings.Limit < 35)
                settings.Limit = 35;
                
            searchString += "&limit=" + settings.Limit.ToString();


            // tests have shown that we need at >= 30 segment limit for this server
            // I will shorten the return list to respect the limit specified by the user
            // searchString += "&limit=35";


            return searchString;
        }

        private readonly string[] _punctuations = { "!", ":", ".", ";", "?" };

        private string GetTausCompatibleSearchText(string searchText)
        {

            #region  |  create a list of words  |
            var searchWords = new List<string>();
            var words = searchText.Split(' ');

            foreach (var word in words)
            {
                #region  |  word  |

                var wordTmp = string.Empty;

                foreach (var _char in word.ToCharArray())
                {

                    // need to confirm if we separate the dbl byte chars
                    // from the documentation it suggests this, but I need to confirm
                    if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters
                    {
                        if (wordTmp != string.Empty)
                            searchWords.Add(wordTmp);
                        wordTmp = string.Empty;

                        searchWords.Add(_char.ToString());
                    }
                    else
                        wordTmp += _char.ToString();


                }
                if (wordTmp != string.Empty)
                {
                    searchWords.Add(wordTmp);
                }
                #endregion
            }
            #endregion

            #region  |  get rid of the last punctuation mark at beginning & end  |


            // note we don't want to get rid of all punctuation marks
            if (searchWords.Count > 0)
            {
                var firstWord = searchWords[0];
                var lastWord = searchWords[searchWords.Count - 1];

             
                if (firstWord.Trim().StartsWith("¿") && lastWord.Trim().EndsWith("?"))
                {
                    searchWords[0] = searchWords[0].Substring(searchWords[0].IndexOf("¿", StringComparison.Ordinal) + 1);
                }


                if (lastWord.Trim().EndsWith("..."))
                {
                    searchWords[searchWords.Count - 1] = searchWords[searchWords.Count - 1].Substring(0, searchWords[searchWords.Count - 1].LastIndexOf("...", StringComparison.Ordinal));
                }
                else
                {
                    var punctuation = lastWord.Trim().Substring(lastWord.Trim().Length - 1);
                   
                    if (_punctuations.Contains(punctuation))
                    {
                        searchWords[searchWords.Count - 1] = searchWords[searchWords.Count - 1].Substring(0, searchWords[searchWords.Count - 1].LastIndexOf(punctuation, StringComparison.Ordinal));
                    }
                }
            }
            #endregion

            #region  |  seperate words with +  |
            var tausSearchText = string.Empty;

            foreach (var word in searchWords)
            {
                if (word.Trim() != string.Empty)
                {
                    if (tausSearchText.Trim() != string.Empty && !tausSearchText.EndsWith("+"))
                        tausSearchText += "+";

                    tausSearchText += word.Trim();
                }
                else
                {
                    if (!tausSearchText.EndsWith("+"))
                        tausSearchText += "+";
                }
            }
            #endregion

            #region  |  get rid of the HTTP parameter control chars (i.e. ?&=)  |
            searchText = ReplaceParamChars(tausSearchText);
            #endregion

            return searchText;
        }

        private string ReplaceParamChars(string searchText)
        {
            searchText = searchText.Replace("&", string.Empty);
            searchText = searchText.Replace("?", string.Empty);
            searchText = searchText.Replace("=", string.Empty);

            return searchText;
        }

        private string GetSectionText(List<SegmentSection> sections)
        {
            return sections.Where(section => section.IsText).Aggregate(string.Empty, (current, section) => current + section.Content);
        }

        #endregion
		
        #region  |  settings serialization  |

        private readonly string _cryptoKey = "m8jEh6axKnD9/eodQpVZyIMeyHKzdITO";
        private readonly string _cryptoIv = "ZhbkOSFLj80=";
        private SymmetricAlgorithm _mCsp;
        public string EncryptString(string value)
        {
            _mCsp = new TripleDESCryptoServiceProvider();

            var bKey = Convert.FromBase64CharArray(_cryptoKey.ToCharArray(), 0, _cryptoKey.Length);
            var bIv = Convert.FromBase64CharArray(_cryptoIv.ToCharArray(), 0, _cryptoIv.Length);

            var ct = _mCsp.CreateEncryptor(bKey, bIv);

            var byt = Encoding.UTF8.GetBytes(value);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }
        public string DecryptString(string value)
        {
            MemoryStream ms;

            _mCsp = new TripleDESCryptoServiceProvider();

            try
            {
                var bKey = Convert.FromBase64CharArray(_cryptoKey.ToCharArray(), 0, _cryptoKey.Length);
                var bIv = Convert.FromBase64CharArray(_cryptoIv.ToCharArray(), 0, _cryptoIv.Length);

                var ct = _mCsp.CreateDecryptor(bKey, bIv);
                var byt = Convert.FromBase64String(value);
                ms = new MemoryStream();
                var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();

                cs.Close();
            }
            catch
            {
                return value;
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public void SaveSettings(SearchSettings settings)
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SearchSettings));
                stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, settings);
            }
            finally
            {
                if (stream != null)
                    stream.Close();


                #region  |  Encrypt  |
                try
                {
                    string str;
                    using (var r = new StreamReader(SettingsFilePath, true))
                    {
                        str = r.ReadToEnd();
                        r.Close();
                    }

                    str = EncryptString(str);

                    using (var w = new StreamWriter(SettingsFilePath, false))
                    {
                        w.Write(str);
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    try
                    {
                        File.Delete(SettingsFilePath);
                    }
                    catch (Exception ex1)
                    {
                        Console.WriteLine(ex1.Message);
                    }
                }
                #endregion
            }


       
        }
        public SearchSettings ReadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                var settings = new SearchSettings();
                SaveSettings(settings);
            }


            FileStream stream = null;
            try
            {
                #region  |  Decrypt  |

                try
                {
                    var str = string.Empty;
                    using (var r = new StreamReader(SettingsFilePath, true))
                    {
                        str = r.ReadToEnd();
                        r.Close();
                    }

                    str = DecryptString(str);

                    using (var w = new StreamWriter(SettingsFilePath, false))
                    {
                        w.Write(str);
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    try
                    {
                        File.Delete(SettingsFilePath);
                    }
                    catch (Exception ex1)
                    {
                        Console.WriteLine(ex1.Message);
                    }
                }
                #endregion


                var serializer = new XmlSerializer(typeof(SearchSettings));
                stream = new FileStream(SettingsFilePath, FileMode.Open);
                var settings = (SearchSettings)serializer.Deserialize(stream) ?? new SearchSettings();

                return settings;

            }
            finally
            {
                if (stream != null)
                    stream.Close();

                #region  |  Encrypt  |
                try
                {
                    string str;
                    using (var r = new StreamReader(SettingsFilePath, true))
                    {
                        str = r.ReadToEnd();
                        r.Close();
                    }

                    str = EncryptString(str);

                    using (var w = new StreamWriter(SettingsFilePath, false))
                    {
                        w.Write(str);
                        w.Flush();
                        w.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    try
                    {
                        File.Delete(SettingsFilePath);
                    }
                    catch (Exception ex1)
                    {
                        Console.WriteLine(ex1.Message);
                    }
                }
                #endregion
            }



         
        }

        #endregion		
    }
}