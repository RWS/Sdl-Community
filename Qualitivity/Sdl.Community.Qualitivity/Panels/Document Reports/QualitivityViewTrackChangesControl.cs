using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Shapes;
using Sdl.Community.Comparison;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Projects.Activities;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;

namespace Sdl.Community.Qualitivity.Panels.Document_Reports
{
    public partial class QualitivityViewTrackChangesControl : UserControl
    {

        public QualitivityViewTrackChangesControl()
        {


            InitializeComponent();

            listViewControl1.ColumnPropertiesChanged += listViewControl1__ColumnPropertiesChanged;
        }


        private static void listViewControl1__ColumnPropertiesChanged()
        {
            //throw new NotImplementedException();
        }

        private void StudioTimeTrackerViewTrackChangesControl_Load(object sender, EventArgs e)
        {
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser2.Dock = DockStyle.Fill;

            webBrowser3.Dock = DockStyle.Fill;
            webBrowser4.Dock = DockStyle.Fill;



            webBrowser1.BringToFront();
            webBrowser3.BringToFront();


            KeyStrokeDataCount = 0;


        }


        #region  |  Document Reports  |

        private int KeyStrokeDataCount { get; set; }

        internal void SetupChartTimePerSegment(List<Record> records)
        {

            chart_segmentPerSecond.Series[0].Points.Clear();
            foreach (var record in records)
            {

                double seconds = record.TicksElapsed / 10000000;

                if (record.Started != null) chart_segmentPerSecond.Series[0].Points.AddXY(record.Started, seconds);

            }
            if (records.Count > 1)
            {
                panel_segment_per_second_properties.Visible = true;
       

                var mean = Math.Round(chart_segmentPerSecond.DataManipulator.Statistics.Mean("Series1"), 2);
                var median = Math.Round(chart_segmentPerSecond.DataManipulator.Statistics.Median("Series1"), 2);
                var variance = Math.Round(chart_segmentPerSecond.DataManipulator.Statistics.Variance("Series1", true), 2);

                chart_segmentPerSecond.Titles[0].Text = "Average Time per Segment (Average/Mean = " + mean + " seconds)";

                // Set Strip line item
                chart_segmentPerSecond.ChartAreas[0].AxisY.StripLines[0].IntervalOffset = mean - Math.Sqrt(variance);
                chart_segmentPerSecond.ChartAreas[0].AxisY.StripLines[0].StripWidth = 2.0 * Math.Sqrt(variance);

                // Set Strip line item
                chart_segmentPerSecond.ChartAreas[0].AxisY.StripLines[1].IntervalOffset = mean;

                // Set Strip line item
                chart_segmentPerSecond.ChartAreas[0].AxisY.StripLines[2].IntervalOffset = median;

                // Fill labels
                label2.Text = mean.ToString("G5") + @" (seconds)";
                label5.Text = median.ToString("G5") + @" (seconds)";
                label3.Text = Math.Round(Math.Sqrt(variance), 2).ToString("G5") + @" (seconds)";
                label8.Text = records.Count.ToString();             

            }
            else
            {
                panel_segment_per_second_properties.Visible = false;
                chart_segmentPerSecond.Titles[0].Text = "Average Time per Segment (Average/Mean = " + 0 + " seconds)";
            }

            chart_segmentPerSecond.Invalidate();
        }
        internal void SetupChartWordsPerMinute(List<Record> records)
        {
            var csgs = new List<KeySpeedGroup>();

            foreach (var record in records)
            {
                foreach (var ksg in GetCharSpeedGroups(record.TargetKeyStrokes))
                {
                    //check and elminate any duplicate values here
                    //this could happen if the user has created a clone of the same activity
                    //duplicate values might cause an error when initializing the MS graphic object
                    if (csgs.Exists(a => ksg.Stop != null && a.Stop != null && ksg.Start != null  && a.Start != null 
                        && a.Start.Value ==  ksg.Start.Value 
                        && a.Stop.Value == ksg.Stop.Value)) 
                        continue;
                    csgs.Add(ksg);
                }

            }


            KeyStrokeDataCount = csgs.Count;

            chart_words_per_minute.Series["DataSeries"].Points.Clear();
            if (csgs.Count > 1)
            {
                panel_words_per_min_control.Visible = true;
                chart_words_per_minute.ChartAreas["Box Chart Area"].Visible = true;
                

                label10.Text = csgs.Count.ToString();

                panel_words_per_min_control.Enabled = true;
                double min = -1;
                double max = -1;

                foreach (var csg in csgs)
                {
                    if (csg.Start != null)
                        chart_words_per_minute.Series["DataSeries"].Points.AddXY(csg.Start.Value, csg.WordsPerMinute);
                    if (min == -1 || csg.WordsPerMinute < min)
                        min = csg.WordsPerMinute;

                    if (csg.WordsPerMinute > max)
                        max = csg.WordsPerMinute;
                }
                if (min > -1 && max > -1)
                {
                    chart_words_per_minute.Series["BoxPlotSeries"]["BoxPlotSeries"] = "DataSeries";


                    comboBoxPercentiles.SelectedIndex = 2;


                    var total = max - min;
                    var less = Math.Round(0.10F * total, 2);

                    var minL = min - less;
                    var maxL = max + less;


                    chart_words_per_minute.ChartAreas["Box Chart Area"].Axes[1].Minimum = Math.Round(minL, 2);
                    chart_words_per_minute.ChartAreas["Box Chart Area"].Axes[1].Maximum = Math.Round(maxL, 2);

                    chart_words_per_minute.ChartAreas["Data Chart Area"].Axes[1].Minimum = Math.Round(minL, 2);
                    chart_words_per_minute.ChartAreas["Data Chart Area"].Axes[1].Maximum = Math.Round(maxL, 2);

                    UpdateChartSettings();

                    checkBoxShowMedian.Checked = false;
                }
            }
            else
            {
                panel_words_per_min_control.Visible = false;
                label10.Text = "0";
                panel_words_per_min_control.Enabled = false;
                chart_words_per_minute.ChartAreas["Box Chart Area"].Visible = false;
                                        
            }





        }

        private IEnumerable<KeySpeedGroup> GetCharSpeedGroups(List<KeyStroke> keyStrokes)
        {
            #region  |  calculate the char speed  |


            var charSpeedGroupList = new List<KeySpeedGroup>();
            var ksSorted = keyStrokes.OrderByDescending(k => k.Created).ToList();
            var ksGroup = new List<KeyStroke>();
            foreach (var t in ksSorted)
            {
                if (ksGroup.Count > 0)
                {
                    //check that the time span difference between last item in the list against the current
                    //keystroke is not greater than 5 seconds; otherwise it enters a seperate group
                    var dateTime = ksGroup[ksGroup.Count - 1].Created;
                    if (dateTime == null) continue;
                    if (t.Created == null) continue;
                    var ts = dateTime.Value.Subtract(t.Created.Value);
                    if (ts.TotalSeconds > 5)
                    {
                        //the translator needs to type more than 5 chars
                        if (ksGroup.Count > 5)
                            charSpeedGroupList.Add(GetCharSpeedGroup(ksGroup));

                        ksGroup = new List<KeyStroke> {t};
                    }
                    else
                    {
                        ksGroup.Add(t);
                    }
                }
                else
                {
                    ksGroup.Add(t);
                }
            }
            if (ksGroup.Count <= 0) return charSpeedGroupList;
            //the translator needs to type more than 5 chars
            if (ksGroup.Count > 5)
                charSpeedGroupList.Add(GetCharSpeedGroup(ksGroup));

            #endregion

            return charSpeedGroupList;
        }
        private static KeySpeedGroup GetCharSpeedGroup(List<KeyStroke> ksGroup)
        {
            var charSpeedGroup = new KeySpeedGroup
            {
                CharCount = ksGroup.Count,
                CharsPerSecond = 0,
                CharsPerMinute = 0,
                WordsPerMinute = 0,
                Elapsed = new TimeSpan(),
                Start = ksGroup[ksGroup.Count - 1].Created,
                Stop = ksGroup[0].Created,
                KeyStrokes = new List<KeyStroke>()
            };
            foreach (var ks in ksGroup)
                charSpeedGroup.KeyStrokes.Add((KeyStroke)ks.Clone());


            if (charSpeedGroup.Stop != null && charSpeedGroup.Start != null)
                    charSpeedGroup.Elapsed = charSpeedGroup.Stop.Value.Subtract(charSpeedGroup.Start.Value);
            if (charSpeedGroup.Elapsed != null)
                charSpeedGroup.CharsPerSecond = Math.Round(charSpeedGroup.CharCount / charSpeedGroup.Elapsed.Value.TotalSeconds, 2);
            charSpeedGroup.CharsPerMinute = Math.Round(charSpeedGroup.CharsPerSecond * 60, 2);
            charSpeedGroup.WordsPerMinute = Math.Round(charSpeedGroup.CharsPerMinute / 5, 2);

            return charSpeedGroup;
        }
        private void comboBoxPercentiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChartSettings();
        }
        private void UpdateChartSettings()
        {

            // Set whiskers percentile
            var whiskerPercentile = (3 - comboBoxPercentiles.SelectedIndex) * 5;
            chart_words_per_minute.Series["BoxPlotSeries"]["BoxPlotWhiskerPercentile"] = whiskerPercentile.ToString();

            // Show/Hide Average line
            chart_words_per_minute.Series["BoxPlotSeries"]["BoxPlotShowAverage"] = checkBoxShowAverage.Checked ? "true" : "false";

            // Show/Hide Median line
            chart_words_per_minute.Series["BoxPlotSeries"]["BoxPlotShowMedian"] = checkBoxShowMedian.Checked ? "true" : "false";

            // Show/Hide Unusual points
            chart_words_per_minute.Series["BoxPlotSeries"]["BoxPlotShowUnusualValues"] = checkBoxShowUnusual.Checked ? "true" : "false";
       
            chart_words_per_minute.Invalidate();


        }
        private void checkBoxShowAverage_CheckedChanged(object sender, EventArgs e)
        {
            UpdateChartSettings();
        }
        private void checkBoxShowMedian_CheckedChanged(object sender, EventArgs e)
        {
            UpdateChartSettings();
        }
        private void checkBoxShowUnusual_CheckedChanged(object sender, EventArgs e)
        {
            UpdateChartSettings();
        }
        private void chart1_PrePaint(object sender, ChartPaintEventArgs e)
        {
            if (e.ChartElement is Chart)
            {
                chart_words_per_minute.Titles["Title1"].Text = "Words Per Minute (Average/Mean = " + Math.Round(chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[4], 2) + " p/m)";

                // Position point chart type series on the points of the box plot to display labels
                chart_words_per_minute.Series["BoxPlotLabels"].Points[0].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[0];
                chart_words_per_minute.Series["BoxPlotLabels"].Points[1].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[1];
                chart_words_per_minute.Series["BoxPlotLabels"].Points[2].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[2];
                chart_words_per_minute.Series["BoxPlotLabels"].Points[3].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[3];
                chart_words_per_minute.Series["BoxPlotLabels"].Points[4].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[4];
                chart_words_per_minute.Series["BoxPlotLabels"].Points[5].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[5];

                chart_words_per_minute.Series["BoxPlotLabels"].Points[6].Label = "";
                chart_words_per_minute.Series["BoxPlotLabels"].Points[7].Label = "";


                if (KeyStrokeDataCount > 0)
                {
                    if (checkBoxShowUnusual.Checked)
                    {
                        if (chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues.Length > 6)
                        {
                            chart_words_per_minute.Series["BoxPlotLabels"].Points[6].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[6] - 3;                           
                        }
                        if (chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues.Length > 8)
                        {
                            chart_words_per_minute.Series["BoxPlotLabels"].Points[7].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[8] + 3;
                        }
                        else if (chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues.Length > 7)
                        {
                            chart_words_per_minute.Series["BoxPlotLabels"].Points[7].YValues[0] = chart_words_per_minute.Series["BoxPlotSeries"].Points[0].YValues[7] + 3;
                        }
                    }



                    // Define labels
                    var whiskerPercentile = (3 - comboBoxPercentiles.SelectedIndex) * 5;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[0].Label = whiskerPercentile + "th Percentile";
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[1].Label = 100 - whiskerPercentile + "th Percentile";
                    if (whiskerPercentile == 0)
                    {
                        chart_words_per_minute.Series["BoxPlotLabels"].Points[0].Label = "Minimum";
                        chart_words_per_minute.Series["BoxPlotLabels"].Points[1].Label = "Maximum";
                    }
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[2].Label = "25th Percentile (LQ)";
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[3].Label = "75th Percentile (UQ)";
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[4].Label = checkBoxShowAverage.Checked ? "Average/Mean (" + Math.Round(chart_words_per_minute.Series["BoxPlotLabels"].Points[4].YValues[0], 2) + " words p/m)" : "";
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[5].Label = checkBoxShowMedian.Checked ? "Median (" + Math.Round(chart_words_per_minute.Series["BoxPlotLabels"].Points[5].YValues[0], 2) + " words p/m)" : "";

                    // Add strip lines
                    chart_words_per_minute.ChartAreas["Data Chart Area"].AxisY.StripLines.Clear();
                    var stripLine = new StripLine
                    {
                        BackColor = Color.FromArgb(60, 252, 180, 65),
                        IntervalOffset = chart_words_per_minute.Series["BoxPlotLabels"].Points[2].YValues[0]
                    };
                    stripLine.StripWidth = chart_words_per_minute.Series["BoxPlotLabels"].Points[3].YValues[0] - stripLine.IntervalOffset;
                    stripLine.Text = "data points\n50% of";
                    stripLine.Font = new Font("Microsoft Sans Serif", 7);
                    stripLine.TextOrientation = TextOrientation.Rotated270;
                    stripLine.TextLineAlignment = StringAlignment.Center;
                    stripLine.TextAlignment = StringAlignment.Near;
                    chart_words_per_minute.ChartAreas["Data Chart Area"].AxisY.StripLines.Add(stripLine);


                    stripLine = new StripLine
                    {
                        BackColor = Color.FromArgb(60, 252, 180, 65),
                        IntervalOffset = chart_words_per_minute.Series["BoxPlotLabels"].Points[0].YValues[0]
                    };
                    stripLine.StripWidth = chart_words_per_minute.Series["BoxPlotLabels"].Points[1].YValues[0] - stripLine.IntervalOffset;
                    stripLine.ForeColor = Color.FromArgb(120, Color.Black);
                    stripLine.Text = 100 - whiskerPercentile * 2 + "% of data points";
                    stripLine.Font = new Font("Microsoft Sans Serif", 7);
                    stripLine.TextOrientation = TextOrientation.Rotated270;
                    stripLine.TextLineAlignment = StringAlignment.Center;
                    chart_words_per_minute.ChartAreas["Data Chart Area"].AxisY.StripLines.Add(stripLine);

                }
                else
                {
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[0].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[1].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[2].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[3].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[4].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[5].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[6].Label = string.Empty;
                    chart_words_per_minute.Series["BoxPlotLabels"].Points[7].Label = string.Empty;
                }

            }
        }

        #endregion


        internal List<DocumentActivity> DocumentActivities { get; set; }
        internal Activity Activity { get; set; }

        public void UpdateDocumentOverview(ref string reportOverview)
        {
            if (QualitivityViewTrackChangesController.NavigationTreeView.SelectedNode != null
                && QualitivityViewTrackChangesController.ObjectListView.SelectedObjects.Count > 0)
            {

                var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Activity.Documents.xml.html");

                if (reportOverview.Trim() == string.Empty)
                {

                    var reports = new Processor();
                    var cpi = Helper.GetClientFromId(Activity.CompanyProfileId);

                    var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Activity.Documents.xml");
                    reports.CreateTrackChangesReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, DocumentActivities, Activity, cpi);

                    using (var r = new StreamReader(htmlFileFullPath, Encoding.UTF8))
                    {
                        reportOverview = r.ReadToEnd();
                    }
                }
                else
                {

                    using (var w = new StreamWriter(htmlFileFullPath, false, Encoding.UTF8))
                    {
                        w.Write(reportOverview);
                    }
                }
                webBrowser1.BringToFront();
                webBrowser1.Navigate(new Uri(Path.Combine("file://", htmlFileFullPath)));

                var records = DocumentActivities.OrderBy(a => a.Started).SelectMany(da => da.Records.OrderBy(b => b.Started)).ToList();


                SetupChartWordsPerMinute(records);
                SetupChartTimePerSegment(records);

                QualitivityViewTrackChangesController.ObjectListView.Focus();
            }
            else
            {
                SetupChartWordsPerMinute(new List<Record>());
                SetupChartTimePerSegment(new List<Record>());
                webBrowser2.BringToFront();
            }
        }


        public void UpdateQualityMetricsReport(ref string reportMetrics)
        {
            if (QualitivityViewTrackChangesController.NavigationTreeView.SelectedNode != null
                && QualitivityViewTrackChangesController.ObjectListView.SelectedObjects.Count > 0)
            {


                var htmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Quality.Metrics.xml.html");


                if (reportMetrics == string.Empty)
                {
                    var reports = new Processor();


                    var cpi = Helper.GetClientFromId(Activity.CompanyProfileId);
                    var project = Helper.GetProjectFromId(Activity.ProjectId);

                    var projectName = string.Empty;
                    if (project != null)
                        projectName = project.Name;

                    var xmlFileFullPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Qualitivity.Quality.Metrics.xml");
                    reports.CreateQualityMetricsReport(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, xmlFileFullPath, projectName, DocumentActivities, Activity, cpi);

                    using (var r = new StreamReader(htmlFileFullPath, Encoding.UTF8))
                    {
                        reportMetrics = r.ReadToEnd();
                    }
                }
                else
                {
                    using (var w = new StreamWriter(htmlFileFullPath, false, Encoding.UTF8))
                    {
                        w.Write(reportMetrics);
                    }
                }
                webBrowser3.BringToFront();
                webBrowser3.Navigate(new Uri(Path.Combine("file://", htmlFileFullPath)));
            }
            else
            {
                webBrowser4.BringToFront();
            }
        }


        public void UpdateDocumentRecords()
        {

            treeView_navigation.Nodes.Clear();
            listViewControl1.ListViewItems.Clear();

            var flagsPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Flags");
            var documentSourceLanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");
            var documentTargetlanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");


            listViewControl1._SetHeaderTemplate_Source(string.Empty, documentSourceLanguageFlagPath);
            listViewControl1._SetHeaderTemplate_Target(string.Empty, documentTargetlanguageFlagPath);

            if (QualitivityViewTrackChangesController.NavigationTreeView.SelectedNode != null
                && QualitivityViewTrackChangesController.ObjectListView.SelectedObjects.Count > 0)
            {
                try
                {
                    treeView_navigation.BeginUpdate();
                    treeView_navigation.Nodes.Clear();

                    var documentActivitiesDict = new Dictionary<string, List<DocumentActivity>>();
                    foreach (var documentActivity in DocumentActivities)
                        if (!documentActivitiesDict.ContainsKey(documentActivity.DocumentId))
                            documentActivitiesDict.Add(documentActivity.DocumentId, new List<DocumentActivity> { documentActivity });
                        else
                            documentActivitiesDict[documentActivity.DocumentId].Add(documentActivity);

                    foreach (var kvp in documentActivitiesDict)
                    {
                        var tn0 = treeView_navigation.Nodes.Add(kvp.Value[0].TranslatableDocument.DocumentName);
                        tn0.Tag = kvp.Value;
                        tn0.ImageKey = @"Bookmark";
                        tn0.SelectedImageKey = tn0.ImageKey;

                        foreach (var da in kvp.Value)
                        {
                            if (da.Started == null) continue;
                            var tn1 = tn0.Nodes.Add(da.Started.Value.ToString(CultureInfo.InvariantCulture));
                            tn1.Tag = da;
                            tn1.ImageKey = @"Calendar";
                            tn1.SelectedImageKey = tn1.ImageKey;
                        }
                    }

                    if (treeView_navigation.Nodes.Count > 0)
                    {
                        treeView_navigation.SelectedNode = treeView_navigation.Nodes[0];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    treeView_navigation.EndUpdate();
                }
            }
            else
            {
                treeView_navigation.Nodes.Clear();
            }
        }




        private static void SetRecordReportInfo(Record record
            , Activity activity
            , TextComparer textComparer
            , ComparerSettings comparerSettings)
        {
            record.SegmentIdIndex = record.SegmentId.PadLeft(4, '0');


            var elapsedSpan = new TimeSpan(record.TicksElapsed);
            record.ElapsedTime = elapsedSpan.Hours.ToString().PadLeft(2, '0')
                + ":" + elapsedSpan.Minutes.ToString().PadLeft(2, '0')
                + ":" + elapsedSpan.Seconds.ToString().PadLeft(2, '0')
                + "." + elapsedSpan.Milliseconds.ToString().PadLeft(3, '0');

            var dld = new EditDistance(record, activity);

            record.EditDistance = "D=" + Math.Round(dld.Edits, 2) + "/" + Math.Round(dld.EditDistanceRelative, 2);



            record.PemPercentageLines = new Span();
            if (string.Compare(record.TranslationOrigins.Updated.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var run = new Run(Math.Round(dld.PemPercentage, 2) + "%") { Foreground = Brushes.Black };
                record.PemPercentageLines.Inlines.Add(run);
            }
            else
            {
                var trgo = Sdl.Community.Report.Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, activity.ComparisonOptions.IncludeTagsInComparison);
                var trgu = Sdl.Community.Report.Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, activity.ComparisonOptions.IncludeTagsInComparison);

                if ((trgo != trgu ? Convert.ToInt32(dld.PemPercentage) : 100) != 100)
                {
                    var runRemoved = new Run(Math.Round(dld.PemPercentage, 2) + "%");

                    var colorRemoved = Color.Red;
                    var convertedRemoved = System.Windows.Media.Color.FromArgb(colorRemoved.A, colorRemoved.R, colorRemoved.G, colorRemoved.B);
                    runRemoved.Foreground = new SolidColorBrush(convertedRemoved);
                    runRemoved.TextDecorations.Add(TextDecorations.Strikethrough);

                    record.PemPercentageLines.Inlines.Add(runRemoved);
                    record.PemPercentageLines.Inlines.Add(Environment.NewLine);

                    var runNew = new Run("100%");
                    var colorNewA = Color.Blue;
                    var convertedNewA = System.Windows.Media.Color.FromArgb(colorNewA.A, colorNewA.R, colorNewA.G, colorNewA.B);
                    runNew.Foreground = new SolidColorBrush(convertedNewA);
                    var colorNewB = ColorTranslator.FromHtml("#FFFF66");
                    var convertedB = System.Windows.Media.Color.FromArgb(colorNewB.A, colorNewB.R, colorNewB.G, colorNewB.B);
                    runNew.Background = new SolidColorBrush(convertedB);
                    runNew.TextDecorations.Add(TextDecorations.Underline);

                    record.PemPercentageLines.Inlines.Add(runNew);
                }
                else
                {
                    var run = new Run("100%") { Foreground = Brushes.Black };
                    record.PemPercentageLines.Inlines.Add(run);
                }
            }

            record.ContentSections.SourceSectionLines = new Span();
            record.ContentSections.SourceSectionLines = Helper.CreateSpanObject(record.ContentSections.SourceSections, activity.ComparisonOptions.IncludeTagsInComparison);
            record.ContentSections.SourceIndex = Helper.GetCompiledSegmentText(record.ContentSections.SourceSections, false);


            record.ContentSections.TargetOriginalSectionLines = new Span();
            record.ContentSections.TargetOriginalSectionLines = Helper.CreateSpanObject(record.ContentSections.TargetOriginalSections, activity.ComparisonOptions.IncludeTagsInComparison);
            record.ContentSections.TargetOriginalIndex = Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, false);


            record.ContentSections.TargetUpdatedSectionLines = new Span();
            record.ContentSections.TargetUpdatedSectionLines = Helper.CreateSpanObject(record.ContentSections.TargetUpdatedSections, activity.ComparisonOptions.IncludeTagsInComparison);
            record.ContentSections.TargetUpdatedIndex = Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, false);


            var tccus = textComparer.GetComparisonTextUnits(record.ContentSections.TargetOriginalSections, record.ContentSections.TargetUpdatedSections, activity.ComparisonOptions.ConsolidateChanges);
            record.ContentSections.TargetCompareSectionLines = Helper.CreateSpanObject(tccus, activity.ComparisonOptions.IncludeTagsInComparison, comparerSettings);
            record.ContentSections.TargetCompareIndex = string.Empty;
            foreach (var cu in tccus)
                foreach (var sec in cu.Section)
                    if (sec.CntType == ContentSection.ContentType.Text)
                        record.ContentSections.TargetCompareIndex += cu.Text;


            #region  |  target (Updated) - Revision Markers  |

            var rmCount = record.ContentSections.TargetUpdatedSections.Count(trgu => trgu.RevisionMarker != null);

            record.ContentSections.TargetTrackChangesLines = new Span();
            if (rmCount > 0)
            {


                foreach (var trgu in record.ContentSections.TargetUpdatedSections)
                {
                    #region  |  add revision marker  |

                    if (trgu.RevisionMarker == null) continue;
                    var span = new Span();

                    if (record.ContentSections.TargetTrackChangesLines.Inlines.Count > 0)
                    {
                        span.Inlines.Add(new LineBreak());
                        var ln = new Line
                        {
                            Stroke = Brushes.LightSteelBlue,
                            X1 = 0,
                            X2 = 1,
                            Y1 = 0,
                            Y2 = 0,
                            Stretch = Stretch.Fill,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Center,
                            StrokeThickness = 1
                        };
                        span.Inlines.Add(ln);
                        span.Inlines.Add(new LineBreak());
                    }

                    var run = new Run { FontWeight = FontWeights.Bold };
                    switch (trgu.RevisionMarker.RevType)
                    {
                        case RevisionMarker.RevisionType.Delete:
                            run.Text = "Deleted";
                            run.Foreground = Brushes.DarkRed;
                            break;
                        case RevisionMarker.RevisionType.Insert:
                            run.Text = "Inserted";
                            run.Foreground = Brushes.DarkBlue;
                            break;
                        default:
                            run.Text = "Unchanged";
                            run.Foreground = Brushes.Black;
                            break;
                    }
                    span.Inlines.Add(run);


                    if (trgu.RevisionMarker.Created != null)
                    {
                        var dt = trgu.RevisionMarker.Created.Value.Year
                                 + "-" + trgu.RevisionMarker.Created.Value.Month.ToString().PadLeft(2, '0')
                                 + "-" + trgu.RevisionMarker.Created.Value.Day.ToString().PadLeft(2, '0')
                                 + " " + trgu.RevisionMarker.Created.Value.Hour.ToString().PadLeft(2, '0')
                                 + ":" + trgu.RevisionMarker.Created.Value.Minute.ToString().PadLeft(2, '0')
                                 + ":" + trgu.RevisionMarker.Created.Value.Second.ToString().PadLeft(2, '0');

                        run = new Run(" (" + dt + ")");
                    }
                    run.Foreground = Brushes.DarkBlue;
                    run.FontStyle = FontStyles.Italic;
                    span.Inlines.Add(run);
                    span.Inlines.Add(Environment.NewLine);


                    run = new Run("By: ") {Foreground = Brushes.Black};
                    span.Inlines.Add(run);

                    run = new Run(trgu.RevisionMarker.Author)
                    {
                        Foreground = Brushes.DarkSlateGray,
                        FontStyle = FontStyles.Italic
                    };
                    span.Inlines.Add(run);

                    span.Inlines.Add(Environment.NewLine);

                    switch (trgu.RevisionMarker.RevType)
                    {
                        case RevisionMarker.RevisionType.Unchanged:

                            #region  |  trgu.rm.type == Structures.Documents.Records.RevisionMarker.RevisionType.Unchanged  |
                            if (trgu.CntType == ContentSection.ContentType.Text)
                            {
                                span.Inlines.Add(new Run(trgu.Content));
                            }
                            else
                            {
                                run = new Run(trgu.Content) {Foreground = Brushes.Gray};
                                span.Inlines.Add(run);
                            }
                            break;

                            #endregion

                        case RevisionMarker.RevisionType.Insert:

                            #region  |  trgu.rm.type == Structures.Documents.Records.RevisionMarker.RevisionType.Insert  |
                            if (trgu.CntType == ContentSection.ContentType.Text)
                            {
                                run = new Run(trgu.Content);
                                run = Helper.GetRunFormatting(run, comparerSettings.StyleNewText);
                                span.Inlines.Add(run);
                            }
                            else
                            {
                                run = new Run(trgu.Content);
                                run = Helper.GetRunFormatting(run, comparerSettings.StyleNewTag);
                                span.Inlines.Add(run);
                            }
                            break;

                            #endregion

                        case RevisionMarker.RevisionType.Delete:

                            #region  |  trgu.rm.type == Structures.Documents.Records.RevisionMarker.RevisionType.Delete  |

                            if (trgu.CntType == ContentSection.ContentType.Text)
                            {
                                run = new Run(trgu.Content);
                                run = Helper.GetRunFormatting(run, comparerSettings.StyleRemovedText);
                                span.Inlines.Add(run);
                            }
                            else
                            {
                                run = new Run(trgu.Content);
                                run = Helper.GetRunFormatting(run, comparerSettings.StyleRemovedTag);
                                span.Inlines.Add(run);
                            }
                            break;

                            #endregion
                    }



                    record.ContentSections.TargetTrackChangesLines.Inlines.Add(span);

                    #endregion
                }
            }




            #endregion


            #region  |  Translation  Status  |

            record.StatusLines = new Span();
            if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var run = new Run(Helper.GetVisualSegmentStatus(record.TranslationOrigins.Updated.ConfirmationLevel))
                {
                    Foreground = Brushes.Black
                };
                record.StatusLines.Inlines.Add(run);
            }
            else
            {
                var runRemoved = new Run(Helper.GetVisualSegmentStatus(record.TranslationOrigins.Original.ConfirmationLevel));

                var colorRemoved = Color.Red;
                var convertedRemoved = System.Windows.Media.Color.FromArgb(colorRemoved.A, colorRemoved.R, colorRemoved.G, colorRemoved.B);
                runRemoved.Foreground = new SolidColorBrush(convertedRemoved);
                runRemoved.TextDecorations.Add(TextDecorations.Strikethrough);

                record.StatusLines.Inlines.Add(runRemoved);
                record.StatusLines.Inlines.Add(Environment.NewLine);

                var runNew = new Run(Helper.GetVisualSegmentStatus(record.TranslationOrigins.Updated.ConfirmationLevel));
                var colorNewA = Color.Blue;
                var convertedNewA = System.Windows.Media.Color.FromArgb(colorNewA.A, colorNewA.R, colorNewA.G, colorNewA.B);
                runNew.Foreground = new SolidColorBrush(convertedNewA);
                var colorNewB = ColorTranslator.FromHtml("#FFFF66");
                var convertedB = System.Windows.Media.Color.FromArgb(colorNewB.A, colorNewB.R, colorNewB.G, colorNewB.B);
                runNew.Background = new SolidColorBrush(convertedB);
                runNew.TextDecorations.Add(TextDecorations.Underline);

                record.StatusLines.Inlines.Add(runNew);
            }
            #endregion


            #region  |  Translation Match  |

            record.MatchLines = new Span();
            record.MatchColor = Helper.GetMatchColor(record.TranslationOrigins.Original.TranslationStatus, record.TranslationOrigins.Original.OriginType);

            if (string.Compare(record.TranslationOrigins.Original.TranslationStatus, record.TranslationOrigins.Updated.TranslationStatus, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (record.TranslationOrigins.Original.TranslationStatus.Trim() != string.Empty)
                {

                    var run = new Run(record.TranslationOrigins.Original.TranslationStatus) { Foreground = Brushes.Black };
                    record.MatchLines.Inlines.Add(run);
                }
            }
            else
            {
                if (record.TranslationOrigins.Original.TranslationStatus.Trim() != string.Empty)
                {

                    var runRemoved = new Run(record.TranslationOrigins.Original.TranslationStatus);

                    var colorRemoved = Color.Red;
                    var convertedRemoved = System.Windows.Media.Color.FromArgb(colorRemoved.A, colorRemoved.R, colorRemoved.G, colorRemoved.B);
                    runRemoved.Foreground = new SolidColorBrush(convertedRemoved);
                    runRemoved.TextDecorations.Add(TextDecorations.Strikethrough);
                    record.MatchLines.Inlines.Add(runRemoved);

                    if (record.TranslationOrigins.Updated.TranslationStatus.Trim() != string.Empty)
                    {
                        record.MatchLines.Inlines.Add(Environment.NewLine);
                    }

                }

                if (record.TranslationOrigins.Updated.TranslationStatus.Trim() != string.Empty)
                {
                    var runNew = new Run(record.TranslationOrigins.Updated.TranslationStatus);
                    var colorNewA = Color.Blue;
                    var convertedNewA = System.Windows.Media.Color.FromArgb(colorNewA.A, colorNewA.R, colorNewA.G, colorNewA.B);
                    runNew.Foreground = new SolidColorBrush(convertedNewA);
                    var colorNewB = ColorTranslator.FromHtml("#FFFF66");
                    var convertedB = System.Windows.Media.Color.FromArgb(colorNewB.A, colorNewB.R, colorNewB.G, colorNewB.B);
                    runNew.Background = new SolidColorBrush(convertedB);
                    runNew.TextDecorations.Add(TextDecorations.Underline);

                    record.MatchLines.Inlines.Add(runNew);
                }


            }
            #endregion


            #region  |  Quality Metrics  |

            record.QualityMetricsLines = new Span();

            if (record.QualityMetrics != null && record.QualityMetrics.Count > 0)
            {
                foreach (var qm in record.QualityMetrics)
                {
                    var span = new Span();

                    if (record.QualityMetricsLines.Inlines.Count > 0)
                    {
                        span.Inlines.Add(new LineBreak());
                        var ln = new Line
                        {
                            Stroke = Brushes.LightSteelBlue,
                            X1 = 0,
                            X2 = 1,
                            Y1 = 0,
                            Y2 = 0,
                            Stretch = Stretch.Fill,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Center,
                            StrokeThickness = 1
                        };
                        span.Inlines.Add(ln);
                        span.Inlines.Add(new LineBreak());
                    }


                    var run = new Run(qm.Name)
                    {
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Black
                    };
                    span.Inlines.Add(run);

                    run = new Run(" ");
                    span.Inlines.Add(run);

                    run = new Run(" ( ") { Foreground = Brushes.DarkGray };
                    span.Inlines.Add(run);

                    run = new Run(qm.SeverityName)
                    {
                        Foreground = Brushes.DarkSlateGray,
                        FontWeight = FontWeights.Bold
                    };
                    span.Inlines.Add(run);

                    run = new Run(" ");
                    span.Inlines.Add(run);

                    run = new Run(qm.SeverityValue.ToString())
                    {
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };
                    span.Inlines.Add(run);

                    run = new Run(" )") { Foreground = Brushes.DarkGray };
                    span.Inlines.Add(run);


                    span.Inlines.Add(Environment.NewLine);
                    run = new Run(qm.Status.ToString()) { FontWeight = FontWeights.Bold };
                    switch (qm.Status)
                    {
                        case QualityMetric.ItemStatus.Open:
                            run.Foreground = Brushes.DarkBlue;
                            break;
                        case QualityMetric.ItemStatus.Resolved:
                            run.Foreground = Brushes.DarkGreen;
                            break;
                        default:
                            run.Foreground = Brushes.DarkRed;
                            break;
                    }

                    span.Inlines.Add(run);


                    if (qm.Modified != null && qm.Modified.Value != null)
                    {
                        var dt = qm.Modified.Value.Year
                                 + "-" + qm.Modified.Value.Month.ToString().PadLeft(2, '0')
                                 + "-" + qm.Modified.Value.Day.ToString().PadLeft(2, '0')
                                 + " " + qm.Modified.Value.Hour.ToString().PadLeft(2, '0')
                                 + ":" + qm.Modified.Value.Minute.ToString().PadLeft(2, '0')
                                 + ":" + qm.Modified.Value.Second.ToString().PadLeft(2, '0');

                        run = new Run("  " + dt + "");
                    }
                    run.Foreground = Brushes.Black;
                    run.FontStyle = FontStyles.Italic;
                    span.Inlines.Add(run);


                    span.Inlines.Add(Environment.NewLine);
                    run = new Run(@"By: ")
                    {
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Normal
                    };
                    span.Inlines.Add(run);

                    run = new Run(qm.UserName)
                    {
                        Foreground = Brushes.DarkSlateGray,
                        FontWeight = FontWeights.Bold
                    };
                    span.Inlines.Add(run);


                    span.Inlines.Add(Environment.NewLine);

                    run = new Run(@"Content: ")
                    {
                        Foreground = Brushes.DarkGray,
                        FontWeight = FontWeights.Normal
                    };
                    span.Inlines.Add(run);

                    run = new Run(qm.Content) { Foreground = Brushes.Black };
                    span.Inlines.Add(run);

                    if (qm.Comment.Trim() != string.Empty)
                    {
                        span.Inlines.Add(Environment.NewLine);


                        run = new Run("Comment: ")
                        {
                            Foreground = Brushes.DarkGray,
                            FontWeight = FontWeights.Normal
                        };
                        span.Inlines.Add(run);

                        run = new Run(qm.Comment) { Foreground = Brushes.Black };
                        span.Inlines.Add(run);
                    }


                    record.QualityMetricsLines.Inlines.Add(span);
                }
            }
            #endregion


            #region  |  comments  |

            record.CommentLines = new Span();

            if (record.Comments == null || record.Comments.Count <= 0) return;
            {
                foreach (var comment in record.Comments)
                {
                    var span = new Span();

                    if (record.CommentLines.Inlines.Count > 0)
                    {
                        span.Inlines.Add(new LineBreak());
                        var ln = new Line
                        {
                            Stroke = Brushes.LightSteelBlue,
                            X1 = 0,
                            X2 = 1,
                            Y1 = 0,
                            Y2 = 0,
                            Stretch = Stretch.Fill,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Center,
                            StrokeThickness = 1
                        };
                        span.Inlines.Add(ln);
                        span.Inlines.Add(new LineBreak());
                    }



                    var run = new Run(comment.Severity);
                    run.FontWeight = FontWeights.Bold;
                    if (string.Compare(comment.Severity, "High", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        run.Foreground = Brushes.DarkRed;
                    }
                    else if (string.Compare(comment.Severity, "Medium", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        run.Foreground = Brushes.DarkBlue;
                    }
                    else if (string.Compare(comment.Severity, "Low", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        run.Foreground = Brushes.Black;
                    }
                    else
                    {
                        run.Foreground = Brushes.Black;
                    }
                    span.Inlines.Add(run);


                    if (comment.Created != null)
                    {
                        var dt = comment.Created.Value.Year
                                 + "-" + comment.Created.Value.Month.ToString().PadLeft(2, '0')
                                 + "-" + comment.Created.Value.Day.ToString().PadLeft(2, '0')
                                 + " " + comment.Created.Value.Hour.ToString().PadLeft(2, '0')
                                 + ":" + comment.Created.Value.Minute.ToString().PadLeft(2, '0')
                                 + ":" + comment.Created.Value.Second.ToString().PadLeft(2, '0');

                        run = new Run(" (" + dt + ")");
                    }
                    run.Foreground = Brushes.DarkBlue;
                    run.FontStyle = FontStyles.Italic;
                    span.Inlines.Add(run);


                    span.Inlines.Add(Environment.NewLine);
                    run = new Run("By: ") { Foreground = Brushes.Black };
                    span.Inlines.Add(run);

                    run = new Run(comment.Author)
                    {
                        Foreground = Brushes.DarkSlateGray,
                        FontStyle = FontStyles.Italic
                    };
                    span.Inlines.Add(run);

                    span.Inlines.Add(Environment.NewLine);
                    run = new Run(comment.Content) { Foreground = Brushes.Black };
                    span.Inlines.Add(run);


                    record.CommentLines.Inlines.Add(span);
                }
            }

            #endregion

        }


        private void treeView_navigation_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {


                listViewControl1.ListViewItems.Clear();
                listViewControl1.ListViewColumns.Clear();

                foreach (var drc in Tracked.Settings.ViewSettings.ViewProperties)
                    listViewControl1.ListViewColumns.Add(drc);
                listViewControl1.ListingDataViewColumns.Source = listViewControl1.ListViewColumns;

                var flagsPath = Path.Combine(Tracked.Settings.ApplicationPaths.ApplicationTrackChangesReportPath, "Flags");
                var documentSourceLanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");
                var documentTargetlanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");


                listViewControl1._SetHeaderTemplate_Source(string.Empty, documentSourceLanguageFlagPath);
                listViewControl1._SetHeaderTemplate_Target(string.Empty, documentTargetlanguageFlagPath);

                if (e.Node != null)
                {
                    var sourceLanguage = string.Empty;
                    var targetLanguage = string.Empty;

                    var tpa = (Activity)QualitivityViewTrackChangesController.ObjectListView.SelectedObjects[0];

                    var cp = Helper.GetClientFromId(tpa.CompanyProfileId);
                    var cs = new ComparerSettings();
                    if (cp != null)
                        cs = cp.ComparerOptions;

                    var tcc = new TextComparer { Type = TextComparer.ComparisonType.Words };

                    if (e.Node.Tag.GetType() == typeof(List<DocumentActivity>))
                    {
                        var das = (List<DocumentActivity>)e.Node.Tag;
                        foreach (var da in das)
                        {
                            sourceLanguage = da.TranslatableDocument.SourceLanguage;
                            targetLanguage = da.TranslatableDocument.TargetLanguage;

                            foreach (var record in da.Records.OrderBy(a => a.Started))
                            {
                                SetRecordReportInfo(record, tpa, tcc, cs);
                                listViewControl1.ListViewItems.Add(record);
                            }
                        }
                    }
                    else if (e.Node.Tag.GetType() == typeof(DocumentActivity))
                    {
                        var da = (DocumentActivity)e.Node.Tag;
                        sourceLanguage = da.TranslatableDocument.SourceLanguage;
                        targetLanguage = da.TranslatableDocument.TargetLanguage;
                        foreach (var record in da.Records.OrderBy(a => a.Started))
                        {
                            SetRecordReportInfo(record, tpa, tcc, cs);
                            listViewControl1.ListViewItems.Add(record);
                        }
                    }

                    documentSourceLanguageFlagPath = Path.Combine(flagsPath, sourceLanguage + ".gif");
                    documentTargetlanguageFlagPath = Path.Combine(flagsPath, targetLanguage + ".gif");

                    if (!File.Exists(documentSourceLanguageFlagPath))
                        documentSourceLanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");
                    if (!File.Exists(documentTargetlanguageFlagPath))
                        documentTargetlanguageFlagPath = Path.Combine(flagsPath, "empty" + ".gif");

                    listViewControl1._SetHeaderTemplate_Source("", documentSourceLanguageFlagPath);
                    listViewControl1._SetHeaderTemplate_Target("", documentTargetlanguageFlagPath);

                }


                listViewControl1.ListingDataView.Source = listViewControl1.ListViewItems;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckEmptyControls()
        {
            try
            {
                if (QualitivityViewTrackChangesController.NavigationTreeView.SelectedNode != null &&
                    QualitivityViewTrackChangesController.ObjectListView.SelectedObjects.Count != 0) return;
                webBrowser2.BringToFront();
                webBrowser4.BringToFront();

                SetupChartWordsPerMinute(new List<Record>());
                SetupChartTimePerSegment(new List<Record>());

                treeView_navigation.Nodes.Clear();
                listViewControl1.ListViewItems.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void tabControl1_VisibleChanged(object sender, EventArgs e)
        {
            CheckEmptyControls();
        }

    }
}
