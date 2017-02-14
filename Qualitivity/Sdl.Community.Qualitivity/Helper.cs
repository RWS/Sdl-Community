using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.Parser;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.TM.Database;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Brushes = System.Windows.Media.Brushes;
using DocumentActivities = Sdl.Community.Structures.Projects.Activities.DocumentActivities;
using RevisionMarker = Sdl.Community.Structures.Documents.Records.RevisionMarker;
using TranslationOrigin = Sdl.Community.Parser.TranslationOrigin;

namespace Sdl.Community.Qualitivity
{
    public class Helper
    {

        public static bool BackUpMyDatabasesNow(string backupFolderStr)
        {
            var success = false;


            var dtNow = DateTime.Now;

            try
            {
                var backupFolderYear = Path.Combine(backupFolderStr.Trim(), DateTime.Now.Year.ToString());
                if (!Directory.Exists(backupFolderYear))
                    Directory.CreateDirectory(backupFolderYear);

                var backupFolderMonth = Path.Combine(backupFolderYear, DateTime.Now.Month.ToString().PadLeft(2, '0'));
                if (!Directory.Exists(backupFolderMonth))
                    Directory.CreateDirectory(backupFolderMonth);

                var newFilePathSettings = Path.Combine(backupFolderMonth, "Settings.sqlite");
                var newFilePathProjects = Path.Combine(backupFolderMonth, "Projects.sqlite");

                var projectDatabases = new Dictionary<string, string>();
                foreach (var tp in Tracked.TrackingProjects.TrackerProjects)
                {
                    if (!File.Exists(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath +
                                     "_" + tp.Id.ToString().PadLeft(6, '0'))) continue;
                    var newFilePathProject = Path.Combine(backupFolderMonth, "Projects.sqlite" + "_" + tp.Id.ToString().PadLeft(6, '0'));

                    projectDatabases.Add(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + tp.Id.ToString().PadLeft(6, '0')
                        , newFilePathProject);
                }
                var zipName = Path.Combine(backupFolderMonth, "Qualitivity.Backup" + "." + dtNow.Year + "." + dtNow.Month.ToString().PadLeft(2, '0') + "." + dtNow.Day.ToString().PadLeft(2, '0') + ".zip");



                File.Copy(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, newFilePathSettings, true);
                File.Copy(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath, newFilePathProjects, true);
                foreach (var kvp in projectDatabases)
                    File.Copy(kvp.Key, kvp.Value, true);

                try
                {
                    if (File.Exists(zipName))
                        File.Delete(zipName);

                    using (var newFile = ZipFile.Open(zipName, ZipArchiveMode.Create))
                    {
                        newFile.CreateEntryFromFile(newFilePathSettings, "Settings.sqlite");
                        newFile.CreateEntryFromFile(newFilePathProjects, "Projects.sqlite");
                        foreach (var kvp in projectDatabases)
                            newFile.CreateEntryFromFile(kvp.Value, Path.GetFileName(kvp.Value), CompressionLevel.Fastest);
                    }

                    success = true;
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    try
                    {
                        File.Delete(newFilePathSettings);
                        File.Delete(newFilePathProjects);
                        foreach (var kvp in projectDatabases)
                            File.Delete(kvp.Value);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                Tracked.Settings.GetBackupProperty("backupLastDate").Value = Sdl.Community.TM.Database.Helper.DateTimeToSQLite(dtNow);

                var query = new Query();
                query.SaveBackupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, Tracked.Settings.BackupSettings.BackupProperties);
            }
            catch (Exception ex)
            {
                throw new Exception("Backup Databases failed! " + ex.Message);
            }




            return success;
        }

        public static string GetVisualSegmentStatus(string segmentStatusId)
        {
            switch (segmentStatusId)
            {
                case "Unspecified": return "Not Translated";
                case "Draft": return "Draft";
                case "Translated": return "Translated";
                case "RejectedTranslation": return "Translation Rejected";
                case "ApprovedTranslation": return "Translation Approved";
                case "RejectedSignOff": return "Sign-off Rejected";
                case "ApprovedSignOff": return "Signed Off";
                default: return "Unknown";
            }
        }
        public static string GetMatchColor(string match, string originType)
        {

            var color = string.Empty;

            if (string.Compare(originType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                color = "#D3FF4F";
            else if (string.Compare(originType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                //color = "#CFCFCF";
                color = "#FFFFFF";
            else
            {
                switch (match)
                {
                    case "PM": color = "#DFBFFF;"; break;
                    case "CM": color = "#B3ECB3"; break;
                    case "AT": color = "#00B8F4"; break;
                    case "100%": color = "#B3ECB3"; break;
                    default:
                        {
                            if (match.Trim() != string.Empty)
                            {
                                color = string.Empty;
                            }
                        } break;
                }
            }

            return color;
        }

        public static Span CreateSpanObject(List<ContentSection> sections, bool includeTagsInComparison)
        {
            var span = new Span();
            span.Inlines.Clear();

            foreach (var section in sections)
            {
                if (section.HasRevision && section.RevisionMarker != null && section.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore here
                }
                else
                {
                    if (section.CntType == ContentSection.ContentType.Text)
                    {
                        span.Inlines.Add(new Run(section.Content));
                    }
                    else if (includeTagsInComparison)
                    {
                        var run = new Run(section.Content) { Foreground = Brushes.Gray };
                        span.Inlines.Add(run);
                    }
                }
            }
            return span;


        }
        public static Span CreateSpanObject(List<ComparisonUnit> comparisonUnits, bool includeTagsInComparison, ComparerSettings comparerSettings)
        {
            var span = new Span();
            span.Inlines.Clear();

            foreach (var tccu in comparisonUnits)
            {
                if (tccu.Type == ComparisonUnit.ComparisonType.Identical)
                {
                    foreach (var trgu in tccu.Section)
                    {
                        if (trgu.CntType == ContentSection.ContentType.Text)
                        {
                            span.Inlines.Add(new Run(tccu.Text));
                        }
                        else if (includeTagsInComparison)
                        {
                            var run = new Run(tccu.Text) { Foreground = Brushes.Gray };
                            span.Inlines.Add(run);
                        }
                    }
                }
                else
                {
                    foreach (var trgu in tccu.Section)
                    {
                        if (trgu.CntType == ContentSection.ContentType.Text)
                        {
                            if (tccu.Type == ComparisonUnit.ComparisonType.New)
                            {
                                var run = new Run(tccu.Text);
                                run = GetRunFormatting(run, comparerSettings.StyleNewText);
                                span.Inlines.Add(run);
                            }
                            else
                            {
                                var run = new Run(tccu.Text);
                                run = GetRunFormatting(run, comparerSettings.StyleRemovedText);
                                span.Inlines.Add(run);
                            }
                        }
                        else if (includeTagsInComparison)
                        {
                            if (tccu.Type == ComparisonUnit.ComparisonType.New)
                            {
                                var run = new Run(tccu.Text);
                                run = GetRunFormatting(run, comparerSettings.StyleNewTag);
                                span.Inlines.Add(run);
                            }
                            else
                            {
                                var run = new Run(tccu.Text);
                                run = GetRunFormatting(run, comparerSettings.StyleRemovedTag);
                                span.Inlines.Add(run);
                            }
                        }
                    }
                }
            }
            return span;
        }

        public static Run GetRunFormatting(Run run, DifferencesFormatting differencesFormatting)
        {

            if (differencesFormatting.FontSpecifyColor && differencesFormatting.FontColor.Trim() != string.Empty)
            {
                var color = ColorTranslator.FromHtml(differencesFormatting.FontColor.Trim());
                var converted = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                run.Foreground = new SolidColorBrush(converted);
            }
            if (differencesFormatting.FontSpecifyBackroundColor && differencesFormatting.FontBackroundColor.Trim() != string.Empty)
            {
                var color = ColorTranslator.FromHtml(differencesFormatting.FontBackroundColor.Trim());
                var converted = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                run.Background = new SolidColorBrush(converted);
            }

            if (string.Compare(differencesFormatting.StyleBold, "Activate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.FontWeight = FontWeights.Bold;
            }
            if (string.Compare(differencesFormatting.StyleItalic, "Activate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.FontStyle = FontStyles.Italic;
            }
            if (string.Compare(differencesFormatting.StyleStrikethrough, "Activate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.TextDecorations.Add(TextDecorations.Strikethrough);
            }
            if (string.Compare(differencesFormatting.StyleUnderline, "Activate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.TextDecorations.Add(TextDecorations.Underline);
            }
            if (string.Compare(differencesFormatting.TextPosition, "Superscript", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.Typography.Variants = FontVariants.Superscript;
            }
            else if (string.Compare(differencesFormatting.TextPosition, "Subscript", StringComparison.OrdinalIgnoreCase) == 0)
            {
                run.Typography.Variants = FontVariants.Subscript;
            }

            return run;
        }

        public static bool QualitivityProjectInStudioProjectList(ProjectsController projectsController, string id)
        {
            var projects = projectsController.GetProjects().ToList();

            return projects.Select(proj => proj.GetProjectInfo()).Any(pi => pi.Id.ToString() == id);
        }


        public static List<QualityMetric> GetAllQualityMetricRecordsFromDocument(int projectId, string documentId)
        {
            var qms = new List<QualityMetric>();
            if (projectId == -1) return qms;
            var query = new Query();
            qms = query.GetAllQualityMetrics(
                Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + projectId.ToString().PadLeft(6, '0')
                , documentId);
            return qms;

        }
        public static List<DocumentActivity> GetDocumentActivityObjects(Activity activity)
        {
            var query = new Query();
            var documentActivities = query.GetDocumentActivities(
                Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath + "_" + activity.ProjectId.ToString().PadLeft(6, '0')
                , Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseProjectsPath
                , activity.Id, null);
            return documentActivities;
        }


        public static Project GetProjectFromId(int projectId)
        {
            return projectId > -1 ? Tracked.TrackingProjects.TrackerProjects.Find(a => a.Id == projectId) : null;
        }
        public static CompanyProfile GetClientFromId(int companyId)
        {
            return companyId > -1 ? Tracked.Settings.CompanyProfiles.Find(a => a.Id == companyId) : null;
        }



        public static string GetMonthName(DateTime givenDate)
        {
            var formatInfoinfo = new DateTimeFormatInfo();
            var monthName = formatInfoinfo.MonthNames;

            return monthName[givenDate.Month - 1];
        }


        public static List<string> GetTextSections(string str)
        {
            var strList = new List<string>();

            var regexDoubleSpaces = new Regex(@"\s{2,}", RegexOptions.Singleline);
            var mcRegexDoubleSpaces = regexDoubleSpaces.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexDoubleSpaces in mcRegexDoubleSpaces)
            {
                if (mRegexDoubleSpaces.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexDoubleSpaces.Index - previousStart);
                    if (startText.Length > 0)
                        strList.Add(startText);
                }


                var tagText = mRegexDoubleSpaces.Value.Replace(" ", ((char)160).ToString());
                if (tagText.Length > 0)
                    strList.Add(tagText);


                previousStart = mRegexDoubleSpaces.Index + mRegexDoubleSpaces.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                strList.Add(endText);


            return strList;
        }


        public static string GetUniqueName(string baseName, List<string> existingNames)
        {
            var rs = string.Empty;


            for (var i = 0; i < 1000; i++)
            {
                var newName = baseName + "_" + i.ToString().PadLeft(4, '0');
                var foundName = existingNames.Any(name => string.Compare(name, newName, StringComparison.OrdinalIgnoreCase) == 0);
                if (foundName) continue;
                rs = newName;
                break;
            }

            return rs;
        }

        public static string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
                + ":" + dateTime.Second.ToString().PadLeft(2, '0');
        }
        public static string GetStringFromDateTimeMilli(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
                + ":" + dateTime.Second.ToString().PadLeft(2, '0')
                + "." + dateTime.Millisecond.ToString().PadLeft(3, '0');
        }
        public static DateTime GetDateTimeFromString(string strDateTime)
        {
            var dateTime = DateTime.Now;

            //2012-05-17
            var rDateTime = new Regex(@"(?<x1>\d{4})\-(?<x2>\d{2})\-(?<x3>\d{2})T(?<x4>\d{2})(\.|\:)(?<x5>\d{2})(\.|\:)(?<x6>\d{2})", RegexOptions.IgnoreCase);

            var mRDateTime = rDateTime.Match(strDateTime);
            if (!mRDateTime.Success) return dateTime;
            try
            {
                var yy = Convert.ToInt32(mRDateTime.Groups["x1"].Value);
                var mm = Convert.ToInt32(mRDateTime.Groups["x2"].Value);
                var dd = Convert.ToInt32(mRDateTime.Groups["x3"].Value);
                var hh = Convert.ToInt32(mRDateTime.Groups["x4"].Value);
                var MM = Convert.ToInt32(mRDateTime.Groups["x5"].Value);
                var ss = Convert.ToInt32(mRDateTime.Groups["x6"].Value);

                dateTime = new DateTime(yy, mm, dd, hh, MM, ss);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dateTime;
        }

        public static int CalculateWordsForPercentage(SegmentSection segmentSection, ref int placeables, ref string updatedContent)
        {
            var wordCounter = 0;
            var placeablecounter = 0;
            var strWords = new List<string>();


            var strContent = segmentSection as Text;


            if (strContent == null)
                return wordCounter;

            if (strContent.Revision != null && strContent.Revision.RevType == Sdl.Community.Parser.RevisionMarker.RevisionType.Delete)
            {
                //ignore
            }
            else
            {
                //date  {and time?}
                if (strContent.Value.IndexOfAny(new[] { '-', '/', ':', '.', '\\' }) > -1)
                {

                    strContent.Value = Regex.Replace(strContent.Value, @"(?<b1>\b)(?<xdf>\d{2,4}[\/\-\\]\d{1,2}[\/\-\\]\d{2,4})(?<b2>\b)", delegate(Match match)
                    {
                        placeablecounter++;
                        var b1 = match.Groups["b1"].Value;
                        var xdf = match.Groups["xdf"].Value;
                        var b2 = match.Groups["b2"].Value;

                        return b1 + "-_-_-_date_-_-_-" + b2;
                    });
                }
                strContent.Value = strContent.Value.Replace("-_-_-_date_-_-_-", string.Empty);


                //url
                if (strContent.Value.ToLower().IndexOf("http", StringComparison.Ordinal) > -1
                    || strContent.Value.ToLower().IndexOf("https", StringComparison.Ordinal) > -1
                    || strContent.Value.ToLower().IndexOf("ftp", StringComparison.Ordinal) > -1)
                {
                    strContent.Value = Regex.Replace(strContent.Value, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,4}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", delegate
                    {
                        placeablecounter++;
                        return "-_-_-_webpage_-_-_-";
                    });
                }
                strContent.Value = strContent.Value.Replace("-_-_-_webpage_-_-_-", string.Empty);

                //curr and digit
                if (strContent.Value.IndexOfAny(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) > -1)
                {
                    strContent.Value = Regex.Replace(strContent.Value, @"(p{Sc}|)\d+[\.\,\d]*", delegate
                    {
                        placeablecounter++;
                        return "-_-_-_number_-_-_-";
                    });
                }
                strContent.Value = strContent.Value.Replace("-_-_-_number_-_-_-", string.Empty);

                placeables = placeablecounter;
                updatedContent = strContent.Value;

                var charArray = strContent.Value.ToCharArray();
                var strWord = string.Empty;
                foreach (var chr in charArray)
                {
                    if (char.IsControl(chr) || char.IsWhiteSpace(chr) || Encoding.UTF8.GetByteCount(chr.ToString()) > 2)
                    {
                        strWords.Add(strWord);
                        strWord = string.Empty;
                    }
                    else if (chr == '/' || chr == '\\')
                    {
                        if (strWord.Trim() != string.Empty)
                        {
                            strWords.Add(strWord);
                            strWord = string.Empty;
                        }
                        else
                        {
                            strWord += chr.ToString();
                        }
                    }
                    else //if (!char.IsPunctuation(chr))
                    {
                        strWord += chr.ToString();
                    }
                }


                if (strWord.Trim() != string.Empty)
                {
                    strWords.Add(strWord);
                    strWord = string.Empty;
                }


                foreach (var word in strWords)
                {
                    if (word.Length == 1)
                    {
                        var _char = Convert.ToChar(word);

                        //var b1 = char.IsControl(_char);
                        //var b2 = char.IsWhiteSpace(_char);
                        //var b3 = char.IsPunctuation(_char);
                        //var b4 = char.IsSeparator(_char);

                        if (!char.IsControl(_char) && !char.IsWhiteSpace(_char) && !char.IsPunctuation(_char))
                        {
                            wordCounter++;
                        }

                    }
                    else if (word.Trim() != string.Empty)
                    {
                        wordCounter++;
                    }
                }
            }


            return wordCounter;

        }
        public static void GetStatisticalContentCounts(List<SegmentSection> segmentSections, ref int words, ref int chars, ref int tags, ref int placeholders)
        {

            foreach (var segmentSection in segmentSections)
            {
                if (segmentSection.GetType() == typeof(Text))
                {
                    var objText = segmentSection as Text;
                    if (objText != null && objText.Revision != null && objText.Revision.RevType == Sdl.Community.Parser.RevisionMarker.RevisionType.Delete)
                    {
                        //ignore from the comparison process
                    }
                    else
                    {

                        var placeables = 0;
                        var updatedContent = objText != null ? objText.Value : string.Empty;

                        words += CalculateWordsForPercentage(segmentSection, ref placeables, ref updatedContent);

                        placeholders += placeables;
                        chars += updatedContent.ToCharArray().Length;
                    }
                }
                else
                {
                    var objTag = segmentSection as Tag;

                    if (objTag != null && objTag.Revision != null && objTag.Revision.RevType == Sdl.Community.Parser.RevisionMarker.RevisionType.Delete)
                    {
                        //ignore from the comparison process
                    }
                    else
                    {
                        if (objTag != null && objTag.SectionType == Tag.Type.Standalone)
                            placeholders++;
                        else
                            tags++;
                    }
                }
            }
        }



        public static long GetTotalWordsFromActivityRecords(List<Record> tcrs)
        {
            long count = 0;
            var index = new List<string>();
            foreach (var tcr in tcrs)
            {
                if (index.Contains(tcr.ParagraphId + tcr.SegmentId)) continue;
                count += tcr.WordCount;
                index.Add(tcr.ParagraphId + tcr.SegmentId);
            }
            return count;
        }
        public static long GetTotalTicksFromActivityDocuments(List<DocumentActivities> tds)
        {
            return tds.Sum(td => td.DocumentActivityTicks);
        }

        public static long GetTotalTicksFromActivityRecords(List<Record> tcrs)
        {
            return tcrs.Sum(tcr => tcr.TicksElapsed);
        }


        public static string GetTranslationStatus(ITranslationOrigin translationOrigin)
        {


            var match = string.Empty;
            if (translationOrigin == null) return match;
            if (string.Compare(translationOrigin.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // nothing to do here
            }
            else if (string.Compare(translationOrigin.OriginType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
            {
                match = "REPS";
            }
            else
            {
                if (translationOrigin.MatchPercent >= 100)
                {
                    if (
                        string.Compare(translationOrigin.OriginType, "document-match",
                            StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        match = "PM";
                    }
                    else if (translationOrigin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget)
                    {
                        match = "CM";
                    }
                    else if (string.Compare(translationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                          || string.Compare(translationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        match = "AT";
                    }
                    else
                    {
                        match = translationOrigin.MatchPercent + "%";
                    }
                }
                else if (string.Compare(translationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                         || string.Compare(translationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    match = "AT";
                }
                else if (translationOrigin.MatchPercent > 0)
                {
                    match = translationOrigin.MatchPercent + "%";
                }
                else
                {
                    match = string.Empty;
                }
            }
            return match;
        }
        public static decimal GetMatchValue(string matchPercentage)
        {
            decimal value = 0;
            if (string.IsNullOrEmpty(matchPercentage))
                return value;
            var mRegexPercentage = RegexPercentage.Match(matchPercentage);
            if (mRegexPercentage.Success)
                value = NormalizeTerpStringValueToDecimal(mRegexPercentage.Groups["x1"].Value);

            return value;
        }
        private static decimal NormalizeTerpStringValueToDecimal(string str)
        {
            var strOut = str.Trim();
            decimal decOut;

            var hasDot = str.Contains(".");
            var hasComma = str.Contains(",");

            if (hasDot && hasComma)
            {
                var dotIndex = str.IndexOf(".", StringComparison.Ordinal);
                var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

                strOut = strOut.Replace(dotIndex < commaIndex ? "." : ",", string.Empty);
                strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            }
            else if (hasDot)
            {
                NormalizeValue(ref strOut, '.');
            }
            else if (hasComma)
            {
                NormalizeValue(ref strOut, ',');
                strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            }

            decimal.TryParse(strOut, NumberStyles.Any, CultureInfo.InvariantCulture, out decOut);

            return decOut;
        }
        private static void NormalizeValue(ref string strOut, char seperator)
        {
            var split = strOut.Split(seperator);
            if (split.Length != 2)
                return;

            strOut = split[0].Trim().TrimStart('0');
            var end = split[1].Trim().TrimEnd('0');
            strOut += end != string.Empty
                ? seperator + end
                : string.Empty;

            if (strOut.Trim() == string.Empty)
                strOut = "0";
        }

        private static readonly Regex RegexPercentage = new Regex(@"(?<x1>[\d]+)\%");

        public static string GetTranslationStatus(TranslationOrigin translationOrigin)
        {

            var match = string.Empty;
            if (translationOrigin != null)
            {
                if (string.Compare(translationOrigin.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //match = translationOrigin.matchPercentage.ToString();
                }
                else
                {
                    if (translationOrigin.MatchPercentage >= 100)
                    {
                        if (string.Compare(translationOrigin.OriginType, "document-match", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            match = "PM";
                        }
                        else if (string.Compare(translationOrigin.TextContextMatchLevel, "SourceAndTarget", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            match = "CM";
                        }
                        else if (string.Compare(translationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                           || string.Compare(translationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            match = "AT";
                        }
                        else
                        {
                            match = translationOrigin.MatchPercentage + "%";
                        }
                    }
                    else if (string.Compare(translationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                        || string.Compare(translationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        match = "AT";
                    }
                    else if (translationOrigin.MatchPercentage > 0)
                    {
                        match = translationOrigin.MatchPercentage + "%";
                    }
                    else
                    {
                        match = string.Empty;
                    }
                }
            }
            return match;
        }
        public static string GetCompiledSegmentText(List<ContentSection> tcrss, bool includeTags)
        {
            var text = string.Empty;
            foreach (var tcrs in tcrss)
            {
                if (tcrs.RevisionMarker != null && tcrs.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (tcrs.CntType == ContentSection.ContentType.Text)
                        text += tcrs.Content;
                    else if (includeTags)
                        text += tcrs.Content;
                }
            }

            return text;
        }



        public static Regex RegDecimalNumbers = new Regex(@"[\.\,]+(?<x1>.*)", RegexOptions.IgnoreCase);
        public static decimal RoundUp(decimal d1, decimal d2, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1 * d2, ilen + 1).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;


            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);

            return rd;
        }
        public static decimal RoundUp(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, ilen + 1).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;

            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);

            return rd;
        }
        /// <summary>
        /// Round up (AwayFromZero)
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static decimal RoundUp(decimal d1)
        {
            decimal rd = 0;

            d1 = Math.Round(d1, 1, MidpointRounding.AwayFromZero);

            //get the value up to 4 decimal points
            var s1 = Math.Round(d1, 1).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;



            if (roundUpLastDigit)
            {

                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";

            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, 0, MidpointRounding.AwayFromZero);

            return rd;
        }
        public static decimal RoundDown(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, ilen + 1).ToString(CultureInfo.InvariantCulture);


            var roundDownLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundDownLastDigit = false;
            }
            else
                roundDownLastDigit = false;


            if (roundDownLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "0";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.ToEven);



            return rd;
        }
        public static decimal Round(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, ilen + 1).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;


            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                //if (Convert.ToInt32(s1_lastDecimalPoint) == 5)
                //    s1_returnValue += "5";
                //else
                s1ReturnValue += s1LastDecimalPoint;
                //if (Convert.ToInt32(s1_lastDecimalPoint) >=5)
                //    s1_returnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);



            return rd;
        }



















    }
}
