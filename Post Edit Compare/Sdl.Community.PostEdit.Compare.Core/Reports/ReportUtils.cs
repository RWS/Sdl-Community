using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;

namespace Sdl.Community.PostEdit.Compare.Core.Reports
{
    public class ReportUtils
    {
        internal static string GetTERpPieChartTotalsLevel(decimal filesTotalTerp00NumEr, decimal filesTotalTerp01NumEr,
            decimal filesTotalTerp06NumEr, decimal filesTotalTerp10NumEr, decimal filesTotalTerp20NumEr,
            decimal filesTotalTerp30NumEr, decimal filesTotalTerp40NumEr, decimal filesTotalTerp50NumEr)
        {
            var strScript = "";


            strScript += "function drawChart_TERpPieChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified'],";
            strScript += "['0%', " + filesTotalTerp00NumEr + "],";
            strScript += "['01-05%', " + filesTotalTerp01NumEr + "],";
            strScript += "['06-09%', " + filesTotalTerp06NumEr + "],";
            strScript += "['10-19%', " + filesTotalTerp10NumEr + "],";
            strScript += "['20-29%', " + filesTotalTerp20NumEr + "],";
            strScript += "['30-39%', " + filesTotalTerp30NumEr + "],";
            strScript += "['40-49%', " + filesTotalTerp40NumEr + "],";
            strScript += "['50 +', " + filesTotalTerp50NumEr + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " legend: { position: 'right'},";
            strScript += " title: 'TERp (Errors)', pieHole: 0.35, sliceVisibilityThreshold: .2,";
            //sTotal8 += " slices: {0: {color: '#FA5858'}, 1: {color: '#FF8000'}, 2: {color: '#FFBF00'}, 3: {color: '#D7DF01'},4: {color: '#74DF00'},5: {color: '#01DFD7'},6: {color: '#0174DF'},7: {color: '#5F04B4'},8: {color: '#8904B1'}},"; 
            strScript += " is3D: false, ";
            strScript += " width: 220,";
            strScript += " pieSliceText: 'percentage', ";

            strScript += " chartArea: {left: 10, top: 30, height: 200, width: 200},";
            strScript += " };";

            strScript +=
                "var chart = new google.visualization.PieChart(document.getElementById('TERpPieChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_TERpPieChart);";
            return strScript;
        }

        internal static string GetTERpBarChartTotalsLevel(decimal filesTotalTerp00Segments, decimal filesTotalTerp01Segments,
            decimal filesTotalTerp06Segments, decimal filesTotalTerp10Segments, decimal filesTotalTerp20Segments,
            decimal filesTotalTerp30Segments, decimal filesTotalTerp40Segments, decimal filesTotalTerp50Segments)
        {
            var strScript = "";


            strScript += "function drawChart_TERpColumnChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified'],";
            strScript += "['0%', " + filesTotalTerp00Segments + "],";
            strScript += "['01-05%', " + filesTotalTerp01Segments + "],";
            strScript += "['06-09%', " + filesTotalTerp06Segments + "],";
            strScript += "['10-19%', " + filesTotalTerp10Segments + "],";
            strScript += "['20-29%', " + filesTotalTerp20Segments + "],";
            strScript += "['30-39%', " + filesTotalTerp30Segments + "],";
            strScript += "['40-49%', " + filesTotalTerp40Segments + "],";
            strScript += "['50 +', " + filesTotalTerp50Segments + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " isStacked: false, ";
            strScript += " legend: { position: 'top'},";
            strScript += " title: 'TERp (Segments)',";
            //sTotal7 += " colors: ['#FA5858', '#FF8000', '#FFBF00', '#D7DF01', '#74DF00','#01DFD7','#0174DF','#5F04B4','#8904B1'],";
            strScript += " width: 210,";
            strScript += " chartArea: {left: 50, top: 20, height: 150, width: 170},";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.BarChart(document.getElementById('TERpColumnChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_TERpColumnChart);";
            return strScript;
        }

        internal static string GetConfirmationStatisticsBarChartTotalsLevel(decimal filesTotalNotTranslatedOriginal,
            decimal filesTotalNotTranslatedUpdated, decimal filesTotalDraftOriginal, decimal filesTotalDraftUpdated,
            decimal filesTotalTranslatedOriginal, decimal filesTotalTranslatedUpdated,
            decimal filesTotalTranslationRejectedOriginal, decimal filesTotalTranslationRejectedUpdated,
            decimal filesTotalTranslationApprovedOriginal, decimal filesTotalTranslationApprovedUpdated,
            decimal filesTotalSignOffRejectedOriginal, decimal filesTotalSignOffRejectedUpdated,
            decimal filesTotalSignedOffOriginal, decimal filesTotalSignedOffUpdated, decimal filesTotalStatusChangesPercentage)
        {
            var strScript = "";


            strScript += "function drawChart_ConfirmationStatisticsBarChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Original', 'Updated'],";
            strScript += "['Not Translated', " + filesTotalNotTranslatedOriginal + ", " + filesTotalNotTranslatedUpdated + "],";
            strScript += "['Draft', " + filesTotalDraftOriginal + ", " + filesTotalDraftUpdated + "],";
            strScript += "['Translated', " + filesTotalTranslatedOriginal + ", " + filesTotalTranslatedUpdated + "],";
            strScript += "['Trans. Rejected', " + filesTotalTranslationRejectedOriginal + ", " +
                       filesTotalTranslationRejectedUpdated + "],";
            strScript += "['Trans. Approved', " + filesTotalTranslationApprovedOriginal + ", " +
                       filesTotalTranslationApprovedUpdated + "],";
            strScript += "['Sign-off Rejected', " + filesTotalSignOffRejectedOriginal + ", " + filesTotalSignOffRejectedUpdated +
                       "],";
            strScript += "['Signed-off', " + filesTotalSignedOffOriginal + ", " + filesTotalSignedOffUpdated + "]";
            strScript += "]);";

            strScript += " var options = {";
            strScript += " title: 'Status Modifications (" + filesTotalStatusChangesPercentage + "%)',";
            strScript += " width: 235,";

            strScript += " chartArea: {left: 90, top: 25, height: 145, width: 170},";
            strScript += " };";

            strScript +=
                "var chart = new google.visualization.BarChart(document.getElementById('ConfirmationStatisticsBarChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_ConfirmationStatisticsBarChart);";
            return strScript;
        }

        internal static string GetPEMPieChartScriptTotalsLevel(decimal filesTotalPostEditExactWords,
            decimal filesTotalPostEditP99Words, decimal filesTotalPostEditP94Words, decimal filesTotalPostEditP84Words,
            decimal filesTotalPostEditP74Words, decimal filesTotalPostEditNewWords)
        {
            var strScript = "";


            strScript += "function drawChart_PEMpPieChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified'],";
            strScript += "['100%', " + filesTotalPostEditExactWords + "],";
            strScript += "['99-95%', " + filesTotalPostEditP99Words + "],";
            strScript += "['94-85%', " + filesTotalPostEditP94Words + "],";
            strScript += "['84-75%', " + filesTotalPostEditP84Words + "],";
            strScript += "['74-50%', " + filesTotalPostEditP74Words + "],";
            strScript += "['New', " + filesTotalPostEditNewWords + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " legend: { position: 'top'},";
            strScript += " title: 'Post-Edit (Words)',";
            strScript += " is3D: false, ";
            strScript += " pieSliceText: 'percentage', ";
            strScript +=
                "slices: {0: {color: 'Green'}, 1: {color: 'Pink'}, 2: {color: 'Brown'}, 3: {color: 'LightBlue'},4: {color: 'Orange'},5: {color: '#D3FF4F'}},";
            strScript += " chartArea: {left: 20, top: 40, height: 140, width: 120},";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.PieChart(document.getElementById('PEMpPieChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_PEMpPieChart);";
            return strScript;
        }


        internal static string GetPEMColumnChartScriptTotalsLevel(decimal filesTotalPostEditExactSegments,
            decimal filesTotalPostEditP99Segments, decimal filesTotalPostEditP94Segments, decimal filesTotalPostEditP84Segments,
            decimal filesTotalPostEditP74Segments, decimal filesTotalPostEditNewSegments)
        {
            var strScript = "";


            strScript += "function drawChart_PEMpColumnChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', '100%', '99-95%', '94-85%', '84-75%', '74-50%', 'New'],";
            strScript += "['Post-Edit Modifications', " + filesTotalPostEditExactSegments
                       + ", " + filesTotalPostEditP99Segments
                       + ", " + filesTotalPostEditP94Segments
                       + ", " + filesTotalPostEditP84Segments
                       + ", " + filesTotalPostEditP74Segments
                       + ", " + filesTotalPostEditNewSegments
                       + "]";

            strScript += "]);";


            strScript += " var options = {";
            strScript += " isStacked: false, ";
            strScript += " legend: { position: 'top'},";
            strScript += " title: 'Post-Edit (Segments)',";
            strScript += " colors: ['Green', 'Pink', 'Brown', 'LightBlue', 'Orange','#D3FF4F'],";
            strScript += " width: 250,";
            strScript += " chartArea: {left: 35, top: 20, height: 150, width: 250},";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.ColumnChart(document.getElementById('PEMpColumnChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_PEMpColumnChart);";
            return strScript;
        }


        internal static string GetTranslationModificationsColumnChartScriptTotalsLevel(decimal filesChangesPmSegments,
            decimal filesSourcePmSegments, decimal filesChangesCmSegments, decimal filesSourceCmSegments,
            decimal filesChangesRepsSegments, decimal filesSourceRepsSegments, decimal filesChangesExactSegments,
            decimal filesSourceExactSegments, decimal filesChangesFuzzy99Segments, decimal filesChangesFuzzy94Segments,
            decimal filesChangesFuzzy84Segments, decimal filesChangesFuzzy74Segments, decimal filesSourceFuzzy99Segments,
            decimal filesSourceFuzzy94Segments, decimal filesSourceFuzzy84Segments, decimal filesSourceFuzzy74Segments,
            decimal filesChangesNewSegments, decimal filesSourceNewSegments, decimal filesChangesAtSegments,
            decimal filesSoruceAtSegments, decimal filesTotalContentChangesPercentage)
        {
            var strScript = "";


            strScript += "function drawChart_TranslationModificaitonsColumnChart() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified', 'Total'],";
            strScript += "['PM', " + filesChangesPmSegments + ", " + filesSourcePmSegments + "],";
            strScript += "['CM', " + filesChangesCmSegments + ", " + filesSourceCmSegments + "],";
            strScript += "['Reps', " + filesChangesRepsSegments + ", " + filesSourceRepsSegments + "],";
            strScript += "['Exact', " + filesChangesExactSegments + ", " + filesSourceExactSegments + "],";
            strScript += "['Fuzzy', "
                       + (filesChangesFuzzy99Segments
                          + filesChangesFuzzy94Segments
                          + filesChangesFuzzy84Segments
                          + filesChangesFuzzy74Segments)
                       + ", "
                       + (filesSourceFuzzy99Segments
                          + filesSourceFuzzy94Segments
                          + filesSourceFuzzy84Segments
                          + filesSourceFuzzy74Segments) + "],";
            strScript += "['New', " + filesChangesNewSegments + ", " + filesSourceNewSegments + "],";
            strScript += "['AT', " + filesChangesAtSegments + ", " + filesSoruceAtSegments + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " width: 430,";
            strScript += " title: 'Translation Modifications (" + filesTotalContentChangesPercentage + "%)',";
            strScript += " colors: ['red','#004411'],";
            strScript += " chartArea: {left: 65, top: 30, height: 140, width: 270},";
            strScript += " vAxis: {title: 'Segments',  titleTextStyle: {color: 'red'}}";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.ColumnChart(document.getElementById('TranslationModificaitonsColumnChart'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_TranslationModificaitonsColumnChart);";
            return strScript;
        }


        internal static string GetTranslationModificationsColumnChartScriptFilesLevel(string fileId,
            Comparison.Comparer.FileUnitProperties fileUnitProperties, decimal filesTotalContentChangesPercentage)
        {
            var strScript = "";

            strScript += "function drawChart_TranslationModificaitonsColumnChart_" + fileId + "() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified', 'Total'],";
            strScript += "['PM', " + fileUnitProperties.TotalChangesPmSegments + ", " + fileUnitProperties.TotalSourcePmSegments + "],";
            strScript += "['CM', " + fileUnitProperties.TotalChangesCmSegments + ", " + fileUnitProperties.TotalSourceCmSegment + "],";
            strScript += "['Reps', " + fileUnitProperties.TotalChangesRepsSegments + ", " + fileUnitProperties.TotalSourceRepsSegments +
                 "],";
            strScript += "['Exact', " + fileUnitProperties.TotalChangesExactSegments + ", " +
                 fileUnitProperties.TotalSourceExactSegments + "],";
            strScript += "['Fuzzy', "
                 + (fileUnitProperties.TotalChangesFuzzy99Segments
                    + fileUnitProperties.TotalChangesFuzzy94Segments
                    + fileUnitProperties.TotalChangesFuzzy84Segments
                    + fileUnitProperties.TotalChangesFuzzy74Segments)
                 + ", "
                 + (fileUnitProperties.TotalSourceFuzzy99Segments
                    + fileUnitProperties.TotalSourceFuzzy94Segments
                    + fileUnitProperties.TotalSourceFuzzy84Segments
                    + fileUnitProperties.TotalSourceFuzzy74Segments) + "],";
            strScript += "['New', " + fileUnitProperties.TotalChangesNewSegments + ", " + fileUnitProperties.TotalSourceNewSegments +
                 "],";
            strScript += "['AT', " + fileUnitProperties.TotalChangesAtSegments + ", " + fileUnitProperties.TotalSourceAtSegments + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " width: 430,";
            strScript += " title: 'Translation Modifications (" + filesTotalContentChangesPercentage + "%)',";
            strScript += " colors: ['red','#004411'],";
            strScript += " chartArea: {left: 65, top: 30, height: 140, width: 270},";
            strScript += " vAxis: {title: 'Segments',  titleTextStyle: {color: 'red'}}";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.ColumnChart(document.getElementById('TranslationModificaitonsColumnChart_" +
                fileId + "'));";
            strScript += "chart.draw(data, options);";

            strScript += "}";


            strScript += "google.setOnLoadCallback(drawChart_TranslationModificaitonsColumnChart_" + fileId + ");";
            return strScript;
        }

        internal static string GetPEMColumnChartScriptFilesLevel(string fileId,
            decimal filesTotalPostEditExactSegments, decimal filesTotalPostEditP99Segments, decimal filesTotalPostEditP94Segments, decimal filesTotalPostEditP84Segments,
            decimal filesTotalPostEditP74Segments, decimal filesTotalPostEditNewSegments)
        {
            var strScript = "";

            strScript += "function drawChart_PEMpColumnChart_" + fileId + "() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', '100%', '99-95%', '94-85%', '84-75%', '74-50%', 'New'],";
            strScript += "['Post-Edit Modifications', " + filesTotalPostEditExactSegments
                       + ", " + filesTotalPostEditP99Segments
                       + ", " + filesTotalPostEditP94Segments
                       + ", " + filesTotalPostEditP84Segments
                       + ", " + filesTotalPostEditP74Segments
                       + ", " + filesTotalPostEditNewSegments
                       + "]";

            strScript += "]);";


            strScript += " var options = {";
            strScript += " isStacked: false, ";
            strScript += " legend: { position: 'top'},";
            strScript += " title: 'Post-Edit (Segments)',";
            strScript += " colors: ['Green', 'Pink', 'Brown', 'LightBlue', 'Orange','#D3FF4F'],";
            strScript += " width: 250,";
            strScript += " chartArea: {left: 35, top: 20, height: 150, width: 250},";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.ColumnChart(document.getElementById('PEMpColumnChart_" + fileId + "'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_PEMpColumnChart_" + fileId + ");";
            return strScript;
        }

        internal static string GetPEMPieChartScriptFilesLevel(string fileId, decimal filesTotalPostEditExactWords,
          decimal filesTotalPostEditP99Words, decimal filesTotalPostEditP94Words, decimal filesTotalPostEditP84Words,
          decimal filesTotalPostEditP74Words, decimal filesTotalPostEditNewWords)
        {
            var strScript = "";


            strScript += "function drawChart_PEMpPieChart_" + fileId + "() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Modified'],";
            strScript += "['100%', " + filesTotalPostEditExactWords + "],";
            strScript += "['99-95%', " + filesTotalPostEditP99Words + "],";
            strScript += "['94-85%', " + filesTotalPostEditP94Words + "],";
            strScript += "['84-75%', " + filesTotalPostEditP84Words + "],";
            strScript += "['74-50%', " + filesTotalPostEditP74Words + "],";
            strScript += "['New', " + filesTotalPostEditNewWords + "]";
            strScript += "]);";


            strScript += " var options = {";
            strScript += " legend: { position: 'top'},";
            strScript += " title: 'Post-Edit (Words)',";
            strScript += " is3D: false, ";
            strScript += " pieSliceText: 'percentage', ";
            strScript +=
                "slices: {0: {color: 'Green'}, 1: {color: 'Pink'}, 2: {color: 'Brown'}, 3: {color: 'LightBlue'},4: {color: 'Orange'},5: {color: '#D3FF4F'}},";
            strScript += " chartArea: {left: 20, top: 40, height: 140, width: 120},";
            strScript += " };";


            strScript +=
                "var chart = new google.visualization.PieChart(document.getElementById('PEMpPieChart_" + fileId + "'));";
            strScript += "chart.draw(data, options);";
            strScript += "}";
            strScript += "google.setOnLoadCallback(drawChart_PEMpPieChart_" + fileId + ");";
            return strScript;
        }


        internal static string GetConfirmationStatisticsBarChartFilesLevel(string fileId, Comparison.Comparer.FileUnitProperties fileUnitProperties)
        {
            var strScript = "";


            strScript += "function drawChart_ConfirmationStatisticsBarChart_" + fileId + "() {";
            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += "['Name', 'Original', 'Updated'],";
            strScript += "['Not Translated', " + fileUnitProperties.TotalNotTranslatedOriginal + ", " + fileUnitProperties.TotalNotTranslatedUpdated + "],";
            strScript += "['Draft', " + fileUnitProperties.TotalDraftOriginal + ", " + fileUnitProperties.TotalDraftUpdated + "],";
            strScript += "['Translated', " + fileUnitProperties.TotalTranslatedOriginal + ", " + fileUnitProperties.TotalTranslatedUpdated + "],";
            strScript += "['Trans. Rejected', " + fileUnitProperties.TotalTranslationRejectedOriginal + ", " +
                       fileUnitProperties.TotalTranslationRejectedUpdated + "],";
            strScript += "['Trans. Approved', " + fileUnitProperties.TotalTranslationApprovedOriginal + ", " +
                       fileUnitProperties.TotalTranslationApprovedUpdated + "],";
            strScript += "['Sign-off Rejected', " + fileUnitProperties.TotalSignOffRejectedOriginal + ", " + fileUnitProperties.TotalSignOffRejectedUpdated +
                       "],";
            strScript += "['Signed-off', " + fileUnitProperties.TotalSignedOffOriginal + ", " + fileUnitProperties.TotalSignedOffUpdated + "]";
            strScript += "]);";

            strScript += " var options = {";
            strScript += " title: 'Status Modifications (" + fileUnitProperties.TotalStatusChangesPercentage + "%)',";
            strScript += " width: 235,";

            strScript += " chartArea: {left: 90, top: 25, height: 145, width: 170},";
            strScript += " };";

            strScript += "var chart = new google.visualization.BarChart(document.getElementById('ConfirmationStatisticsBarChart_" + fileId + "'));";
            strScript += "chart.draw(data, options);";
            strScript += "};";
            strScript += "google.setOnLoadCallback(drawChart_ConfirmationStatisticsBarChart_" + fileId + ");";
            return strScript;
        }


        internal static string GetPEMLineChartFilesLevel(string fileId, List<string> analysisData, List<string> TERpAnalysisData)
        {
            var strScript = string.Empty;

            strScript += "function drawChart_PEMpLineChart_" + fileId + "() {";

            var sb = new StringBuilder();

            if (TERpAnalysisData.Count > 0)
            {
                sb.Append("['Source words', 'PEMp Edit-Distance', 'TERp Edit-Distance']");
                foreach (var row in TERpAnalysisData)
                    sb.Append("," + row);
            }
            else
            {
                sb.Append("['Source words', 'PEMp Edit-Distance']");
                if (analysisData.Count == 0)
                    sb.Append(",[0, 0]");
                foreach (var row in analysisData)
                    sb.Append("," + row); 
            }
           

            strScript += "var data = google.visualization.arrayToDataTable([";
            strScript += sb.ToString();
            strScript += "]);";

            strScript += " var options = {";
            strScript += "title: 'Segment words vs Edit-Distance', chart: {title: 'Segment words vs Edit-Distance',subtitle: 'based on character edit-distance'},";
            strScript += " hAxis: {title: 'Segment words'}, vAxis: {title: 'Edit-Distance'},";
            strScript += " width: 410,";
            strScript += " backgroundColor: 'white', ";
            strScript += " chartArea: {left: 40, top: 40, height: 130, width: 410}, legend: {position: 'in'}, titlePosition: 'out'";
            strScript += " };";


            strScript += "var chart = new google.visualization.ScatterChart(document.getElementById('PEMpLineChart_" + fileId + "'));";
            strScript += "chart.draw(data, options);";
            strScript += "};";
            strScript += "google.setOnLoadCallback(drawChart_PEMpLineChart_" + fileId + ");";
            return strScript;

        }

        internal static string GetTERpLineChartFilesLevel(string fileId, List<string> analysisData)
        {
            var strScript = string.Empty;

            strScript += "function drawChart_TERpLineChart_" + fileId + "() {";
            strScript += "var data = new google.visualization.DataTable();";

            strScript += "data.addColumn('number', 'X');";
            strScript += "data.addColumn('number', 'Ins');";
            strScript += "data.addColumn('number', 'Del');";
            strScript += "data.addColumn('number', 'Sub');";
            strScript += "data.addColumn('number', 'Shft');";


            var sb = new StringBuilder();
            var index = 0;
            foreach (var row in analysisData)
            {
                sb.Append((index > 0 ? "," : string.Empty) + row);
                index++;
            }

            strScript += "data.addRows([";
            strScript += sb.ToString();
            strScript += "]);";

            strScript += " var options = {";
            strScript += " title: 'TERp (Post-Edit Operations)',";
            strScript += " width: 415,";
            strScript += " hAxis: {title: 'Segments'  }, vAxis: {  title: 'Post-Edit Operations'}, backgroundColor: 'white', ";
            strScript += " chartArea: {left: 40, top: 30, height: 140, width: 300}";
            strScript += " };";


            strScript += "var chart = new google.visualization.LineChart(document.getElementById('TERpLineChart_" + fileId + "'));";
            strScript += "chart.draw(data, options);";
            strScript += "};";
            strScript += "google.setOnLoadCallback(drawChart_TERpLineChart_" + fileId + ");";
            return strScript;

        }

        internal static decimal GetDecimal(string str)
        {
            return NormalizeTerpStringValueToDecimal(str);
        }


        private static readonly Regex RegexPercentage = new Regex(@"[\d]+\%");
        private static decimal GetMatchValue(string matchPercentage)
        {
            decimal value = 0;
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

                strOut = strOut.Replace(dotIndex < commaIndex ? "." : ",", String.Empty);
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

            Decimal.TryParse(strOut, NumberStyles.Any, CultureInfo.InvariantCulture, out decOut);

            return decOut;
        }
        internal static void NormalizeValue(ref string strOut, char seperator)
        {
            var split = strOut.Split(seperator);
            if (split.Length != 2)
                return;

            strOut = split[0].Trim().TrimStart('0');
            var end = split[1].Trim().TrimEnd('0');
            strOut += end != String.Empty
                ? seperator + end
                : String.Empty;

            if (strOut.Trim() == String.Empty)
                strOut = "0";
        }


        internal static string NormalizedWords(string content)
        {
            var charsTemp = new List<string>();
            var strTemp = String.Empty;
            foreach (var _char in content.ToCharArray())
            {
                if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2)
                {
                    if (strTemp != String.Empty)
                        charsTemp.Add(strTemp);
                    strTemp = String.Empty;

                    charsTemp.Add(_char.ToString());
                }
                else
                    strTemp += _char.ToString();
            }
            if (strTemp != String.Empty)
                charsTemp.Add(strTemp);

            var charsBuilder = new StringBuilder();
            foreach (var chr in charsTemp)
            {
                var lastChar = String.Empty;
                if (charsBuilder.Length > 0)
                    lastChar = charsBuilder.ToString().Substring(charsBuilder.Length - 1, 1);

                if (charsBuilder.Length > 0 && lastChar != " " && chr != " ")
                    charsBuilder.Append(" ");
                charsBuilder.Append(chr);
            }


            charsBuilder.Replace("  ", " ");
            charsBuilder.Replace(" , ", ", ");

            return charsBuilder.ToString();

        }

        internal static readonly Regex RegexTagName = new Regex(@"\<\s*(|\/)\s*(?<tagName>\w+)", RegexOptions.IgnoreCase);
        internal static string GetTagNameVisual(string textEquivalent)
        {
            if (Processor.Settings.TagVisualStyle == Settings.TagVisual.Full)
                return textEquivalent;

            var tagType = GetTagType(textEquivalent);

            if (Processor.Settings.TagVisualStyle == Settings.TagVisual.Empty)
            {
                switch (tagType)
                {
                    case Settings.TagType.Opening:
                        return "<>";
                    case Settings.TagType.Closing:
                        return "</>";
                    case Settings.TagType.Standalone:
                        return "[/]";
                    case Settings.TagType.Undefined:
                        return "{}";
                    default:
                        return textEquivalent;
                }
            }

            try
            {
                if (tagType == Settings.TagType.Closing)
                {
                    var mRegxEnd = RegexTagName.Match(textEquivalent);
                    if (mRegxEnd.Success)
                    {
                        var name = mRegxEnd.Groups["tagName"].Value;
                        return "</" + name + ">";
                    }
                }

                // try to match the base pattern to retrive the tag name
                var mRegBase = RegexTagName.Match(textEquivalent);
                if (mRegBase.Success)
                {
                    var name = mRegBase.Groups["tagName"].Value;
                    return "<" + name + (tagType == Settings.TagType.Standalone ? "/" : "") + ">";
                }
                return textEquivalent;
            }
            catch
            {
                return textEquivalent;
            }

            // test is the tag name is valid
            //XmlConvert.VerifyName(tag.Name);


            // try to recover the attributes
            //var attributes = mRegBase.Groups["attributes"].Value;


            //if (string.IsNullOrEmpty(attributes))
            //return tag;


            //var mRegexId = regexId.Match(attributes);
            //if (mRegexId.Success)
            //    tag.Id = mRegexId.Groups["value"].Value;

            //var mRegexName = regexName.Match(attributes);
            //if (mRegexName.Success)
            //    tag.Name = mRegexName.Groups["value"].Value;



        }
        private static Settings.TagType GetTagType(string textEquivalent)
        {
            var tagTextTest = textEquivalent.Replace(" ", "");
            if (tagTextTest.StartsWith("</"))
                return Settings.TagType.Closing;
            if (tagTextTest.EndsWith("/>"))
                return Settings.TagType.Standalone;
            if (tagTextTest.StartsWith("<"))
                return Settings.TagType.Opening;

            return Settings.TagType.Undefined;
        }
        internal static bool IsFilterSegmentMatchPercentage(string filterOriginal, string filterUpdated, string matchOriginal, string matchUpdated)
        {
            var b = true;

            switch (filterOriginal)
            {
                case "{All}":
                    {
                        //do nothing
                    } break;
                case "PM {Perfect Match}":
                    {
                        if (String.Compare(matchOriginal, "PM", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "CM {Context Match}":
                    {
                        if (String.Compare(matchOriginal, "CM", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "AT {Automated Translation}":
                    {
                        if (String.Compare(matchOriginal, "AT", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "Exact Match":
                    {
                        if (String.Compare(matchOriginal, "100%", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "Fuzzy Match":
                    {
                        if (!RegexPercentage.Match(matchOriginal.Trim()).Success)
                            b = false;
                    } break;
                case "No Match":
                    {
                        if (matchOriginal.Trim() != String.Empty)
                            b = false;
                    } break;
            }

            if (!b) return b;
            switch (filterUpdated)
            {
                case "{All}":
                    {
                        //do nothing
                    } break;
                case "PM {Perfect Match}":
                    {
                        if (String.Compare(matchUpdated, "PM", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "CM {Context Match}":
                    {
                        if (String.Compare(matchUpdated, "CM", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "AT {Automated Translation}":
                    {
                        if (String.Compare(matchUpdated, "AT", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "Exact Match":
                    {
                        if (String.Compare(matchUpdated, "100%", StringComparison.OrdinalIgnoreCase) != 0)
                            b = false;
                    } break;
                case "Fuzzy Match":
                    {
                        if (!RegexPercentage.Match(matchUpdated.Trim()).Success)
                            b = false;
                    } break;
                case "No Match":
                    {
                        if (matchUpdated.Trim() != String.Empty)
                            b = false;
                    } break;
            }


            return b;
        }

        internal static List<string> GetTextSections(string str)
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

        internal static string GetVisualSegmentStatus(string segmentStatusId)
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

        internal static string GetMatchColor(string match, string originType)
        {

            var color = String.Empty;

            if (String.Compare(originType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                color = "#D3FF4F";
            else if (String.Compare(originType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                color = String.Empty;
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
                            if (match.Trim() != String.Empty)
                            {
                                color = String.Empty;
                            }
                        } break;
                }
            }



            return color;
        }

        internal static void TransformXmlReport(string reportFilePath)
        {
            if (reportFilePath == null) return;
            var filePathXslt = Path.Combine(Path.GetDirectoryName(reportFilePath), DefaultXsltName);



            var xsltSetting = new XsltSettings();
            xsltSetting.EnableDocumentFunction = true;
            xsltSetting.EnableScript = true;


            var myXPathDoc = new XPathDocument(reportFilePath);





            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(filePathXslt, xsltSetting, null);



            var myWriter = new XmlTextWriter(reportFilePath + ".html", Encoding.UTF8);
            myXslTrans.Transform(myXPathDoc, null, myWriter);


            myWriter.Flush();
            myWriter.Close();



            var xmlData = new StringBuilder();
            using (var reader = new StreamReader(reportFilePath, Encoding.UTF8))
            {
                xmlData.Append(reader.ReadToEnd());
                reader.Close();
            }
            var styleSheetVarialbe = String.Format(@"<?xml-stylesheet type='text/xsl' href='{0}'?>", DefaultXsltName);
            xmlData.Replace(styleSheetVarialbe, "");

            using (var writer = new StreamWriter(reportFilePath))
            {
                writer.Write(xmlData);
                writer.Flush();
                writer.Close();
            }

			//var myXslTrans1 = new XslCompiledTransform();
			//myXslTrans1.Load(filePathXslt, xsltSetting, null);
			//var myWriter1 = new XmlTextWriter(reportFilePath + ".csv", Encoding.UTF8);
			//myXslTrans1.Transform(myXPathDoc, null, myWriter1);


			//myWriter1.Flush();
			//myWriter1.Close();



			//var xmlData1 = new StringBuilder();
			//using (var reader = new StreamReader(reportFilePath, Encoding.UTF8))
			//{
			//	xmlData1.Append(reader.ReadToEnd());
			//	reader.Close();
			//}
			//var styleSheetVarialbe1 = String.Format(@"<?xml-stylesheet type='text/xsl' href='{0}'?>", DefaultXsltName);
			//xmlData1.Replace(styleSheetVarialbe1, "");

			//using (var writer1 = new StreamWriter(reportFilePath))
			//{
			//	writer1.Write(xmlData);
			//	writer1.Flush();
			//	writer1.Close();
			//}


			File.Delete(filePathXslt);
        }

        internal static void WriteReportResourcesToDirectory(string reportDirectory)
        {

            var filePathXslt = Path.Combine(reportDirectory, DefaultXsltName);

            if (Processor.Settings.UseCustomStyleSheet
                && File.Exists(Processor.Settings.FilePathCustomStyleSheet))
            {
                #region  |  custom report  |
                if (String.Compare(filePathXslt, Processor.Settings.FilePathCustomStyleSheet, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    try
                    {
                        File.Copy(Processor.Settings.FilePathCustomStyleSheet, filePathXslt, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to write the custom xlst file to the report directory!\r\n" + ex.Message);
                    }
                }
                #endregion
            }
            else
            {
                #region  |  default report  |

                var asb = Assembly.GetExecutingAssembly();


                const string templateXsltName = "Sdl.Community.PostEdit.Compare.Core.Reports.PostEdit.Compare.Report.01.xslt";

                using (var inputStream = asb.GetManifestResourceStream(templateXsltName))
                {

                    var outputStream = File.Open(filePathXslt, FileMode.Create);

                    if (inputStream != null)
                    {
                        var bsInput = new BufferedStream(inputStream);
                        var bsOutput = new BufferedStream(outputStream);

                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
                        {
                            bsOutput.Write(buffer, 0, bytesRead);
                        }

                        bsInput.Flush();
                        bsOutput.Flush();
                        bsInput.Close();
                        bsOutput.Close();
                    }
                }
                #endregion
            }

            UdpateStyleSheetInformation(filePathXslt);

        }

        internal const string DefaultXsltName = "Sdl.Community.PostEdit.Compare.Report.xslt";

        internal static string GetCompiledSegmentText(List<SegmentSection> tcrss, bool includeTags)
        {
            var text = String.Empty;
            foreach (var xSegmentSection in tcrss)
            {
                if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                        text += xSegmentSection.Content;
                    else if (includeTags)
                        text += xSegmentSection.Content;
                }
            }
            return text;
        }

        #region  |  Udpate Style Sheet Information  |

        private static readonly Regex RegexStyle = new Regex(@"(?<x1>\<style\s+[^\>]*\>)(?<x2>.*?)(?<x3>\<\/style\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RegexNewText = new Regex(@"(?<x1>span\.textNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RegexRemovedText = new Regex(@"(?<x1>span\.textRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RegexNewTag = new Regex(@"(?<x1>span\.tagNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RegexRemovedTag = new Regex(@"(?<x1>span\.tagRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static void UdpateStyleSheetInformation(string filePathXslt)
        {

            string str;
            using (var sr = new StreamReader(filePathXslt, Encoding.UTF8))
            {
                str = sr.ReadToEnd();
                sr.Close();
            }

            str = RegexStyle.Replace(str, new MatchEvaluator(MatchEvaluator_style));

            using (var sw = new StreamWriter(filePathXslt, false, Encoding.UTF8))
            {
                sw.Write(str);
                sw.Flush();
                sw.Close();
            }
        }

        private static string MatchEvaluator_style(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;

            str2 = RegexNewText.Replace(str2, (MatchEvaluator)MatchEvaluator_new_text);
            str2 = RegexRemovedText.Replace(str2, (MatchEvaluator)MatchEvaluator_removed_text);
            str2 = RegexNewTag.Replace(str2, (MatchEvaluator)MatchEvaluator_new_tag);
            str2 = RegexRemovedTag.Replace(str2, (MatchEvaluator)MatchEvaluator_removed_tag);

            return str1 + str2 + str3;
        }

        private static string MatchEvaluator_new_text(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleNewText);
            return str1 + str2 + str3;
        }

        private static string MatchEvaluator_removed_text(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleRemovedText);
            return str1 + str2 + str3;
        }

        private static string MatchEvaluator_new_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            var str4 = @"border: 2px solid #dadada; border-radius: 7px; color: Gray;";
            str2 = GetHtmlStyleText(Processor.Settings.StyleNewTag);
            return str1 + str2 + str4 + str3;
        }

        private static string MatchEvaluator_removed_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            var str4 = @"border: 2px solid #dadada; border-radius: 7px; color: Gray;";
            str2 = GetHtmlStyleText(Processor.Settings.StyleRemovedTag);
            return str1 + str2 + str4 + str3;
        }

        private static string GetHtmlStyleText(Settings.DifferencesFormatting style)
        {
            var str = String.Empty;

            if (style.FontSpecifyColor)
                str += "color: " + style.FontColor + "; ";
            if (style.FontSpecifyBackroundColor)
                str += "background-color: " + style.FontBackroundColor + "; ";

            if (style.StyleBold == "Activate")
                str += "font-weight: bold; ";
            if (style.StyleItalic == "Activate")
                str += "font-style: italic; ";
            if (style.StyleStrikethrough == "Activate")
                str += "text-decoration: line-through; ";
            if (style.StyleUnderline == "Activate")
                str += "text-decoration: underline; ";

            if (String.Compare(style.TextPosition, "Superscript", StringComparison.OrdinalIgnoreCase) == 0)
                str += "vertical-align: super; ";
            else if (String.Compare(style.TextPosition, "Subscript", StringComparison.OrdinalIgnoreCase) == 0)
                str += "vertical-align: sub; ";


            return str;
        }

        #endregion
    }
}
