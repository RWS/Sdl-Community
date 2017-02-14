using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.iProperties;

namespace Sdl.Community.WPFListView
{
    public delegate void ColumnPropertiesChanged();

    /// <summary>
    /// Interaction logic for ListViewControl.xaml
    /// </summary>
    public partial class ListViewControl : UserControl
    {

        public event ColumnPropertiesChanged ColumnPropertiesChanged;

        private ObservableCollection<Record> _listViewItems = new ObservableCollection<Record>();
        public ObservableCollection<Record> ListViewItems
        {
            get { return _listViewItems; }
            set { _listViewItems = value; }
        }


        private ObservableCollection<ViewProperty> _listViewColumns = new ObservableCollection<ViewProperty>();
        public ObservableCollection<ViewProperty> ListViewColumns
        {
            get { return _listViewColumns; }
            set { _listViewColumns = value; }
        }


        private ObservableCollection<KeyStroke> _listViewKeyStroke = new ObservableCollection<KeyStroke>();
        public ObservableCollection<KeyStroke> ListViewKeyStroke
        {
            get { return _listViewKeyStroke; }
            set { _listViewKeyStroke = value; }
        }


        public CollectionViewSource ListingDataView { get; set; }
        public CollectionViewSource ListingDataViewColumns { get; set; }
        public CollectionViewSource ListingDataViewKeyStroke { get; set; }

        private GridViewColumnHeader _curSortCol;
        private SortAdorner _curAdorner;


        private readonly DataTemplate _dataTemplateColumnParagraphId;
        private readonly DataTemplate _dataTemplateColumnSegmentId;
        private readonly DataTemplate _dataTemplateColumnStarted;
        private readonly DataTemplate _dataTemplateColumnStopped;
        private readonly DataTemplate _dataTemplateColumnElapsed;
        private readonly DataTemplate _dataTemplateColumnStatus;
        private readonly DataTemplate _dataTemplateColumnMatch;
        private readonly DataTemplate _dataTemplateColumnSourceContent;
        private readonly DataTemplate _dataTemplateColumnTargetOriginalContent;
        private readonly DataTemplate _dataTemplateColumnTargetUpdatedContent;
        private readonly DataTemplate _dataTemplateColumnTargetTrackChangesContent;
        private readonly DataTemplate _dataTemplateColumnTargetComparisonContent;
        private readonly DataTemplate _dataTemplateColumnModificationsDistanceContent;
        private readonly DataTemplate _dataTemplateColumnPemPercentageContent;
        private readonly DataTemplate _dataTemplateColumnSourceWordCount;
        private readonly DataTemplate _dataTemplateColumnQualityMetricsContent;
        private readonly DataTemplate _dataTemplateColumnTargetCommentsContent;



        private readonly double _widthColumnParagraphId = 32;
        private readonly double _widthColumnSegmentId = 32;
        private readonly double _widthColumnStarted = 65;
        private readonly double _widthColumnStopped = 65;
        private readonly double _widthColumnElapsed = 80;
        private readonly double _widthColumnStatus = 75;
        private readonly double _widthColumnMatch = 60;
        private readonly double _widthColumnSourceContent = 230;
        private readonly double _widthColumnTargetOriginalContent = 230;
        private readonly double _widthColumnTargetUpdatedContent = 230;
        private readonly double _widthColumnTargetTrackChangesContent = 200;
        private readonly double _widthColumnTargetComparisonContent = 250;
        private readonly double _widthColumnModificationsDistanceContent = 75;
        private readonly double _widthColumnPemPercentageContent = 70;
        private readonly double _widthColumnSourceWordCount = 70;
        private readonly double _widthColumnTargetCommentsContent = 230;
        private readonly double _widthColumnQualityMetricsContent = 230;
        private double _heightColumnKeyStrokeData = 100;


        public int WpmTotalSecondsTimeOut { get; set; }
        public int WpmMinimumNumberOfChars { get; set; }


        public ListViewControl()
        {

            InitializeComponent();

            WpmTotalSecondsTimeOut = 5;
            WpmMinimumNumberOfChars = 5;

            ListingDataView = (CollectionViewSource)Resources["ListingDataView"];
            ListingDataViewColumns = (CollectionViewSource)Resources["ListingDataViewColumns"];
            ListingDataViewKeyStroke = (CollectionViewSource)Resources["ListingDataViewKeyStroke"];

            _dataTemplateColumnParagraphId = (DataTemplate)Resources["ColumnParagraphId"];
            _dataTemplateColumnSegmentId = (DataTemplate)Resources["ColumnSegmentId"];
            _dataTemplateColumnStarted = (DataTemplate)Resources["ColumnStarted"];
            _dataTemplateColumnStopped = (DataTemplate)Resources["ColumnStopped"];
            _dataTemplateColumnElapsed = (DataTemplate)Resources["ColumnElapsed"];
            _dataTemplateColumnStatus = (DataTemplate)Resources["ColumnStatus"];
            _dataTemplateColumnMatch = (DataTemplate)Resources["ColumnMatch"];
            _dataTemplateColumnSourceWordCount = (DataTemplate)Resources["ColumnSourceWordCount"];
            _dataTemplateColumnSourceContent = (DataTemplate)Resources["ColumnSourceContent"];
            _dataTemplateColumnTargetOriginalContent = (DataTemplate)Resources["ColumnTargetOriginalContent"];
            _dataTemplateColumnTargetUpdatedContent = (DataTemplate)Resources["ColumnTargetUpdatedContent"];
            _dataTemplateColumnTargetTrackChangesContent = (DataTemplate)Resources["ColumnTargetTrackChangesContent"];
            _dataTemplateColumnTargetComparisonContent = (DataTemplate)Resources["Column_targetComparisonContent"];
            _dataTemplateColumnModificationsDistanceContent = (DataTemplate)Resources["ColumnModificationsDistanceContent"];
            _dataTemplateColumnPemPercentageContent = (DataTemplate)Resources["ColumnPemPercentageContent"];
            _dataTemplateColumnQualityMetricsContent = (DataTemplate)Resources["ColumnQualityMetricsContent"];
            _dataTemplateColumnTargetCommentsContent = (DataTemplate)Resources["ColumnTargetCommentsContent"];

            GridSplitterKeyStroke.DragDelta += GridSplitter_keyStroke_DragDelta;
        }

        private void GridSplitter_keyStroke_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (GridKeyStroke.Visibility != Visibility.Collapsed) return;
            var myGridLengthConverter = new GridLengthConverter();
            var convertFromString = myGridLengthConverter.ConvertFromString("0");
            if (convertFromString != null)
            {
                var gl1 = (GridLength)convertFromString;
                RowDefinitionKeystroke.Height = gl1;
            }
            RowDefinitionKeystroke.MinHeight = 0;
            e.Handled = true;
        }

        private void ListViewControl1_Loaded(object sender, RoutedEventArgs e)
        {
            SetColumnsOnLoad();
        }


        private void SortClick(object sender, RoutedEventArgs e)
        {
            var column = sender as GridViewColumnHeader;
            if (column == null) return;
            var field = column.Tag as string;

            if (field != null && field.Trim() == string.Empty) return;
            if (_curSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(_curSortCol).Remove(_curAdorner);
                ListViewGrid.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;
            if (Equals(_curSortCol, column) && _curAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _curSortCol = column;
            _curAdorner = new SortAdorner(_curSortCol, newDir);
            AdornerLayer.GetAdornerLayer(_curSortCol).Add(_curAdorner);
            ListViewGrid.Items.SortDescriptions.Add(
                new SortDescription(field, newDir));
        }

        public void _SetHeaderTemplate_Source(string text, string flagPath)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(flagPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            HeaderImageSource.Source = bi;
        }
        public void _SetHeaderTemplate_Target(string text, string flagPath)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(flagPath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            HeaderImageTargetOriginal.Source = bi;
            HeaderImageTargetUpdated.Source = bi;
            HeaderImageTargetComparison.Source = bi;

            if (ListViewGrid.SelectedItems != null && ListViewGrid.SelectedItems.Count == 0)
                ListViewGrid.SelectedIndex = 0;


        }


        private void GetSelectedItemFromGrid(object sender)
        {
            if (sender.GetType() == typeof(RichTextBox))
            {
                var border = (Border)((RichTextBox)sender).Parent;
                var record = (Record)border.DataContext;
                ListViewGrid.SelectedItem = record;
            }
            else if (sender.GetType() == typeof(RichTextBox))
            {
                var border = (Border)((TextBlock)sender).Parent;
                var record = (Record)border.DataContext;
                ListViewGrid.SelectedItem = record;
            }
        }

        private void source_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void target_original_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void target_updated_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void target_comparison_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void segment_id_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void started_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void stopped_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }


        private void match_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }



        private void status_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void paragraph_id_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void elapsed_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void target_track_changes_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void modificationsDistance_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void pemPercentage_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void source_word_count_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void target_comments_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void quality_metrics_content_GotFocus(object sender, RoutedEventArgs e)
        {
            GetSelectedItemFromGrid(sender);
        }

        private void showHideColumnPropertiesGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ShowHideColumnPropertiesGrid.Visibility != Visibility.Collapsed)
            {
                ShowHideColumnPropertiesGrid.Visibility = Visibility.Collapsed;
            }

        }

        private void showHideColumnPropertiesGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ShowHideColumnPropertiesGrid.Visibility == Visibility.Collapsed)
            {
                ShowHideColumnPropertiesGrid.Visibility = Visibility.Visible;
            }
        }

        private static void _ShowColumnSource(GridViewColumn column, DataTemplate template, double width)
        {
            try
            {
                var style = new Style(typeof(GridViewColumnHeader));
                style.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));

                column.HeaderContainerStyle = style;
                column.Width = width;
                column.CellTemplate = template;
            }
            catch
            {
                // ignored
            }
        }
        private static void _HideColumnSource(GridViewColumn column)
        {
            try
            {
                var style = new Style(typeof(GridViewColumnHeader));
                style.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));



                column.HeaderContainerStyle = style;
                column.Width = 0;
                column.CellTemplate = null;

            }
            catch
            {
                // ignored
            }
        }

        private void column_names_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            HandleColumns(cb.IsChecked == true ? true : false, cb.Content);
            if (ColumnPropertiesChanged != null)
                ColumnPropertiesChanged();
        }

        private void column_names_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            Debug.Assert(cb != null, "cb != null");
            HandleColumns(cb.IsChecked == true ? true : false, cb.Content);
            if (ColumnPropertiesChanged != null)
                ColumnPropertiesChanged();
        }

        private void SetColumnsOnLoad()
        {
            foreach (var drc in ListViewColumns)
            {
                if (!Convert.ToBoolean(drc.Value))
                    HandleColumns(Convert.ToBoolean(drc.Value), drc.Text);
            }
        }

        private void HandleColumns(bool isChecked, object content)
        {


            if (string.Compare(content.ToString(), "Paragraph ID", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnParagraphId, _dataTemplateColumnParagraphId, _widthColumnParagraphId);
                else
                    _HideColumnSource(ColumnParagraphId);
            }
            else if (string.Compare(content.ToString(), "Segment ID", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnSegmentId, _dataTemplateColumnSegmentId, _widthColumnSegmentId);
                else
                    _HideColumnSource(ColumnSegmentId);
            }
            else if (string.Compare(content.ToString(), "Date Started", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnStarted, _dataTemplateColumnStarted, _widthColumnStarted);
                else
                    _HideColumnSource(ColumnStarted);
            }
            else if (string.Compare(content.ToString(), "Date Stopped", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnStopped, _dataTemplateColumnStopped, _widthColumnStopped);
                else
                    _HideColumnSource(ColumnStopped);
            }
            else if (string.Compare(content.ToString(), "Elapsed Time", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnElapsed, _dataTemplateColumnElapsed, _widthColumnElapsed);
                else
                    _HideColumnSource(ColumnElapsed);
            }
            else if (string.Compare(content.ToString(), "Translation Match", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnMatch, _dataTemplateColumnMatch, _widthColumnMatch);
                else
                    _HideColumnSource(ColumnMatch);
            }
            else if (string.Compare(content.ToString(), "Translation Status", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnStatus, _dataTemplateColumnStatus, _widthColumnStatus);
                else
                    _HideColumnSource(ColumnStatus);
            }
            else if (string.Compare(content.ToString(), "Source Word Count", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnSourceWordCount, _dataTemplateColumnSourceWordCount, _widthColumnSourceWordCount);
                else
                    _HideColumnSource(ColumnSourceWordCount);
            }
            else if (string.Compare(content.ToString(), "Source Content", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnSourceContent, _dataTemplateColumnSourceContent, _widthColumnSourceContent);
                else
                    _HideColumnSource(ColumnSourceContent);
            }
            else if (string.Compare(content.ToString(), "Target Content {Original}", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnTargetOriginalContent, _dataTemplateColumnTargetOriginalContent, _widthColumnTargetOriginalContent);
                else
                    _HideColumnSource(ColumnTargetOriginalContent);
            }
            else if (string.Compare(content.ToString(), "Target Content {Updated}", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnTargetUpdatedContent, _dataTemplateColumnTargetUpdatedContent, _widthColumnTargetUpdatedContent);
                else
                    _HideColumnSource(ColumnTargetUpdatedContent);
            }
            else if (string.Compare(content.ToString(), "Target {Track Changes}", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnTargetTrackChangesContent, _dataTemplateColumnTargetTrackChangesContent, _widthColumnTargetTrackChangesContent);
                else
                    _HideColumnSource(ColumnTargetTrackChangesContent);
            }
            else if (string.Compare(content.ToString(), "Target {Comparison}", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnTargetComparisonContent, _dataTemplateColumnTargetComparisonContent, _widthColumnTargetComparisonContent);
                else
                    _HideColumnSource(ColumnTargetComparisonContent);
            }
            else if (string.Compare(content.ToString(), "Modifications Distance", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnModificationsDistanceContent, _dataTemplateColumnModificationsDistanceContent, _widthColumnModificationsDistanceContent);
                else
                    _HideColumnSource(ColumnModificationsDistanceContent);

            }
            else if (string.Compare(content.ToString(), "PEM Percentage", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnPemPercentageContent, _dataTemplateColumnPemPercentageContent, _widthColumnPemPercentageContent);
                else
                    _HideColumnSource(ColumnPemPercentageContent);

            }
            else if (string.Compare(content.ToString(), "Quality Metrics", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnQualityMetricsContent, _dataTemplateColumnQualityMetricsContent, _widthColumnQualityMetricsContent);
                else
                    _HideColumnSource(ColumnQualityMetricsContent);
            }
            else if (string.Compare(content.ToString(), "Target Comments", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                    _ShowColumnSource(ColumnTargetCommentsContent, _dataTemplateColumnTargetCommentsContent, _widthColumnTargetCommentsContent);
                else
                    _HideColumnSource(ColumnTargetCommentsContent);
            }
            else if (string.Compare(content.ToString(), "Keystroke data", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (isChecked)
                {
                    var myGridLengthConverter = new GridLengthConverter();
                    var convertFromString = myGridLengthConverter.ConvertFromString(_heightColumnKeyStrokeData.ToString(CultureInfo.InvariantCulture));
                    if (convertFromString != null)
                    {
                        var gl1 = (GridLength)convertFromString;
                        RowDefinitionKeystroke.Height = gl1;
                    }
                    RowDefinitionKeystroke.MinHeight = 100;
                    GridKeyStroke.Visibility = Visibility.Visible;

                }
                else
                {
                    _heightColumnKeyStrokeData = GridKeyStroke.ActualHeight;
                    var myGridLengthConverter = new GridLengthConverter();
                    var convertFromString = myGridLengthConverter.ConvertFromString("0");
                    if (convertFromString != null)
                    {
                        var gl1 = (GridLength)convertFromString;
                        RowDefinitionKeystroke.Height = gl1;
                    }
                    RowDefinitionKeystroke.MinHeight = 0;
                    GridKeyStroke.Visibility = Visibility.Collapsed;

                }
            }


        }


        private static KeySpeedGroup GetAverageCharSpeedGroup(IReadOnlyList<KeySpeedGroup> charSpeedGroupList)
        {
            var charSpeedGroup = new KeySpeedGroup();

            if (charSpeedGroupList.Count > 1)
            {
                #region  |  get average  |
                var charsPerSecond = new List<double>();
                charSpeedGroup.KeyStrokes = new List<KeyStroke>();
                charSpeedGroup.Elapsed = new TimeSpan();
                foreach (var keySpeedGroup in charSpeedGroupList)
                {
                    charSpeedGroup.Elapsed += keySpeedGroup.Elapsed;
                    charSpeedGroup.KeyStrokes.AddRange(keySpeedGroup.KeyStrokes);
                }
                charSpeedGroup.Start = charSpeedGroupList[charSpeedGroupList.Count - 1].Start;
                charSpeedGroup.Stop = charSpeedGroupList[0].Stop;
                charSpeedGroup.CharCount = charSpeedGroup.KeyStrokes.Count;


                if (charSpeedGroup.Elapsed != null)
                    charSpeedGroup.CharsPerSecond = Math.Round(charSpeedGroup.CharCount / charSpeedGroup.Elapsed.Value.TotalSeconds, 2);
                charSpeedGroup.CharsPerMinute = Math.Round(charSpeedGroup.CharsPerSecond * 60, 2);
                charSpeedGroup.WordsPerMinute = Math.Round(charSpeedGroup.CharsPerMinute / 5, 2);
                #endregion
            }
            else if (charSpeedGroupList.Count == 1)
            {
                charSpeedGroup = charSpeedGroupList[0];
            }

            return charSpeedGroup;
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


            if (charSpeedGroup.Start != null && charSpeedGroup.Stop != null)
                charSpeedGroup.Elapsed = charSpeedGroup.Stop.Value.Subtract(charSpeedGroup.Start.Value);
            if (charSpeedGroup.Elapsed != null)
                charSpeedGroup.CharsPerSecond = Math.Round(charSpeedGroup.CharCount / charSpeedGroup.Elapsed.Value.TotalSeconds, 2);
            charSpeedGroup.CharsPerMinute = Math.Round(charSpeedGroup.CharsPerSecond * 60, 2);
            charSpeedGroup.WordsPerMinute = Math.Round(charSpeedGroup.CharsPerMinute / 5, 2);

            return charSpeedGroup;
        }




        private void ListView_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeySpeedGroup charSpeedGroupReturn = null;
            List<KeySpeedGroup> charSpeedGroupList = null;
            ListViewKeyStroke = new ObservableCollection<KeyStroke>();
            if (ListViewGrid.SelectedItems.Count > 0)
            {
                var record = (Record)ListViewGrid.SelectedItems[0];

                if (record.TargetKeyStrokes != null && record.TargetKeyStrokes.Count > 0)
                {
                    foreach (var ks in record.TargetKeyStrokes)
                    {
                        if (ks.Key == " ")
                            ks.Key = "[Space]";

                        ListViewKeyStroke.Add(ks);
                    }

                    try
                    {
                        #region  |  calculate the char speed  |

                        //WPM_TotalSecondsTimeOut = 5;
                        //WPM_MinimumNumberOfChars = 5;
                        charSpeedGroupList = new List<KeySpeedGroup>();
                        var ksSorted = record.TargetKeyStrokes.OrderByDescending(k => k.Created).ToList();
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
                                if (ts.TotalSeconds > WpmTotalSecondsTimeOut)
                                {
                                    //the translator needs to type more than 5 chars
                                    if (ksGroup.Count > WpmMinimumNumberOfChars)
                                        charSpeedGroupList.Add(GetCharSpeedGroup(ksGroup));

                                    ksGroup = new List<KeyStroke> { t };
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
                        if (ksGroup.Count > 0)
                        {
                            //the translator needs to type more than 5 chars
                            if (ksGroup.Count > WpmMinimumNumberOfChars)
                                charSpeedGroupList.Add(GetCharSpeedGroup(ksGroup));
                        }

                        #endregion

                        //if more than on group was return, then get an accurate average!
                        charSpeedGroupReturn = GetAverageCharSpeedGroup(charSpeedGroupList);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            ListingDataViewKeyStroke.Source = ListViewKeyStroke;
            if (ListViewKeyStroke.Count > 0)
                ListViewGridKeyStroke.SelectedIndex = 0;

            TextBoxWordSpeedData.Inlines.Clear();
            if (charSpeedGroupReturn == null || !(charSpeedGroupReturn.CharCount > 0)) return;
            try
            {
                TextBoxWordSpeedData.Padding = new Thickness(1, 5, 1, 1);

                var run = new Run
                {
                    Text = "Estimated Typing Speed",
                    TextDecorations = TextDecorations.Underline,
                    Foreground = Brushes.Black
                };
                run.FontSize = run.FontSize + 0;
                TextBoxWordSpeedData.Inlines.Add(run);
                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);
                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Block Count:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                run = new Run { Text = charSpeedGroupList.Count.ToString() };
                TextBoxWordSpeedData.Inlines.Add(run);
                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Start Time:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                if (charSpeedGroupReturn.Start != null)
                    run = new Run { Text = charSpeedGroupReturn.Start.Value.ToLongTimeString() };
                TextBoxWordSpeedData.Inlines.Add(run);
                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Stop Time:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                if (charSpeedGroupReturn.Stop != null)
                    run = new Run { Text = charSpeedGroupReturn.Stop.Value.ToLongTimeString() };
                TextBoxWordSpeedData.Inlines.Add(run);

                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Total Seconds:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                if (charSpeedGroupReturn.Elapsed != null)
                    run = new Run { Text = Math.Round(charSpeedGroupReturn.Elapsed.Value.TotalSeconds, 2).ToString(CultureInfo.InvariantCulture) };
                TextBoxWordSpeedData.Inlines.Add(run);

                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Characters:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                run = new Run { Text = charSpeedGroupReturn.CharCount.ToString(CultureInfo.InvariantCulture) };
                TextBoxWordSpeedData.Inlines.Add(run);


                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Characters p/m:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                run = new Run
                {
                    Text = charSpeedGroupReturn.CharsPerMinute.ToString(CultureInfo.InvariantCulture),
                    Foreground = Brushes.SteelBlue
                };

                TextBoxWordSpeedData.Inlines.Add(run);

                TextBoxWordSpeedData.Inlines.Add(Environment.NewLine);

                run = new Run { Text = "Words p/m:\t" };
                TextBoxWordSpeedData.Inlines.Add(run);

                run = new Run
                {
                    Text = charSpeedGroupReturn.WordsPerMinute.ToString(CultureInfo.InvariantCulture),
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkGreen
                };
                run.FontSize = run.FontSize + 1;
                TextBoxWordSpeedData.Inlines.Add(run);
            }
            catch
            {
                // ignored
            }
        }
    }
}
