using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.XliffReadWrite.SDLXLIFF;
using Sdl.Community.XliffToLegacyConverter.Core;

namespace Sdl.Community.XliffToLegacyConverter
{
    public partial class FormMain : Form
    {
        private StringBuilder _report = new StringBuilder();



        public FormMain()
        {
            InitializeComponent();

            comboBox_OuputFormat.Items.Add(new ComboBoxExItem("Word 97-2003 Document (*.doc)", 0));
            comboBox_OuputFormat.Items.Add(new ComboBoxExItem("Word Document (*.docx)", 1));

            comboBox_OuputFormat.Items.Add(new ComboBoxExItem("TRADOStag Documents (*.ttx)",2));
            comboBox_OuputFormat.Items.Add(new ComboBoxExItem("TMX 1.4 (*.tmx)  -  Export only", 3));

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
	        Text = @"Legacy Converter";


            textBox_reportFileName.Text = @"SDLXLIFF to Legacy Converter Report "
                + DateTime.Now.Year
                + @"-" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + @"-" + DateTime.Now.Day.ToString().PadLeft(2, '0')+ @".log";

           
            comboBox_OuputFormat.SelectedIndex = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsConverToFormat;

            checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
            checkBox_reverseLanguageDirection.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsReverseLanguageDirection;
            checkBox_viewReportWhenProcessingFinished.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsViewReportWhenProcessingFinished;

            checkBox_createBAKfile.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsCreateBaKfile;
            checkBox_excludeTags.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsExcludeTags;


            CheckEnableRun();

        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CreateBackupOfSdlXliffFile = checkBox_createBAKfile.Checked;

            Sdl.Community.XliffReadWrite.Processor.SaveSettings();
        }

        private void CheckEnableRun()
        {

            if (textBox_reportFileName.Text.Trim() != string.Empty && Directory.Exists(textBox_reportDirectory.Text))
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    if (listView_export.Items.Count > 0)
                    {
                        toolStripButton_Run.Enabled = true;
                        runToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        toolStripButton_Run.Enabled = false;
                        runToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    if (listView_import.Items.Count > 0)
                    {
                        toolStripButton_Run.Enabled = true;
                        runToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        toolStripButton_Run.Enabled = false;
                        runToolStripMenuItem.Enabled = false;
                    }
                }
            }
            else
            {
                toolStripButton_Run.Enabled = false;
                runToolStripMenuItem.Enabled = false;
            }
        }


        private static CultureInfo GetTargetCultureInfo(string fileFullPath, CultureInfo sourceCi, CultureInfo targetCi)
        {
            if (string.Compare(sourceCi.Name, targetCi.Name, StringComparison.OrdinalIgnoreCase) != 0) 
                return targetCi;

            var directoryName = Path.GetDirectoryName(fileFullPath);
            if (directoryName == null) 
                return targetCi;

            var directory = directoryName.Trim();
            if (directory.IndexOf("\\", StringComparison.Ordinal) <= -1) 
                return targetCi;

            var tmpTargetStr = directory.Substring(directory.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            var rCultureInfo = new Regex(@"^[a-z]{2}\-[A-Z]{2}$", RegexOptions.None);
            if (!rCultureInfo.Match(tmpTargetStr).Success)
                return targetCi;

            try
            {
                var tmpTargetCi = new CultureInfo(tmpTargetStr);
                targetCi = tmpTargetCi;
            }
            catch
            {
                //ignore
            }

            return targetCi;
        }

        private void RunExport()
        {
            #region  |  set cursor  |

            Enabled = false;
            Cursor = Cursors.WaitCursor;

            #endregion
            #region  |  set progress info  |
            toolStripProgressBar1.Maximum = 10;
            toolStripProgressBar1.Value = 0;

            toolStripStatusLabel_Progress_Percentage.Text = @"0%";
            toolStripStatusLabel_Message.Text = @"...";
            toolStripStatusLabel_Progress_Files.Text = string.Empty;
            toolStripStatusLabel_Status.Text = @"Processing...";
            
            Application.DoEvents();
            #endregion

            
            try
            {
                _report = new StringBuilder();
                _report.Append(Application.ProductName + " (" + Application.ProductVersion + ")\r\n\r\n");

                _report.Append("Report Name\t: " + textBox_reportFileName.Text + "\r\n");
                _report.Append("Processing Type\t: Export\r\n");
                _report.Append("Conversion Type\t: " + comboBox_OuputFormat.Text + "\r\n\r\n");
                _report.Append("Start Processing: " + DateTime.Now + "\r\n\r\n");

                _report.Append("".PadRight(90, '-')+"\r\n\r\n");

                var sdlXliffReader = new Sdl.Community.XliffReadWrite.Processor();
                var converter = new Processor();


                try
                {
                    _report.Append("Total files processed: " + listView_export.Items.Count + "\r\n\r\n");
                    _report.Append("".PadRight(90, '-') + "\r\n\r\n");
                    sdlXliffReader.Progress += sdlXliffReader_Progress;

                    var iFile = 1;
                    foreach (ListViewItem itm in listView_export.Items)
                    {
                        toolStripStatusLabel_Progress_Files.Text = " " + iFile++ + @" of " + listView_export.Items.Count + @" files";

                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal = itm.Text;
                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathUpdated = string.Empty;

                      

                        _report.Append("Original file\t: " + itm.Text + "\r\n");

                        if (comboBox_OuputFormat.SelectedIndex==3) //export to TMX
                            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations = true;

                        var fileParagraphUnits = sdlXliffReader.ReadFileParagraphUnits();
                        try
                        {
                            if (String.Compare(sdlXliffReader.SourceLanguageCultureInfo.Name, sdlXliffReader.TargetLanguageCultureInfo.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal.IndexOf("\\", StringComparison.Ordinal) > -1)
                                {
                                    var path = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal.Trim().Substring(0, Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal.Trim().LastIndexOf("\\", StringComparison.Ordinal));
                                    if (path.IndexOf("\\", StringComparison.Ordinal) > -1)
                                    {
                                        var cultureInfoNew = path.Substring(path.LastIndexOf("\\", StringComparison.Ordinal) + 1);

                                        var ciNew = new CultureInfo(cultureInfoNew);

                                        sdlXliffReader.TargetLanguageCultureInfo = ciNew;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        switch (comboBox_OuputFormat.SelectedIndex)
                        {
                            case 0:
                                {                                   
                                    _report.Append("Converted file\t: " + itm.Text + ".doc" + "\r\n\r\n");
                                    converter.WriteRtf(itm.Text + ".rtf", Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, fileParagraphUnits, sdlXliffReader.SourceLanguageCultureInfo, sdlXliffReader.TargetLanguageCultureInfo, false, checkBox_includeLegacyStructure.Checked);                                    
                                } break;
                            case 1:
                                {
                                    _report.Append("Converted file\t: " + itm.Text + ".docx" + "\r\n\r\n");
                                    converter.WriteRtf(itm.Text + ".rtf", Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, fileParagraphUnits, sdlXliffReader.SourceLanguageCultureInfo, sdlXliffReader.TargetLanguageCultureInfo, true, checkBox_includeLegacyStructure.Checked);
                                } break;
                            case 2:
                                {
                                    _report.Append("Converted file\t: " + itm.Text + ".ttx" + "\r\n\r\n");
                                    converter.WriteTtx(itm.Text + ".ttx", Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, fileParagraphUnits, sdlXliffReader.SourceLanguageCultureInfo, sdlXliffReader.TargetLanguageCultureInfo);
                                } break;
                            case 3:
                                {                                    
                                    _report.Append("Converted file\t: " + itm.Text + ".tmx" + "\r\n\r\n");
                                    converter.WriteTmx(itm.Text + ".tmx", Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, fileParagraphUnits, sdlXliffReader.SourceLanguageCultureInfo, sdlXliffReader.TargetLanguageCultureInfo, checkBox_excludeTags.Checked, checkBox_reverseLanguageDirection.Checked);
                                } break;
                        }

                        sdlXliffReader.TargetLanguageCultureInfo = GetTargetCultureInfo(itm.Text ,sdlXliffReader.SourceLanguageCultureInfo, sdlXliffReader.TargetLanguageCultureInfo);

                        _report.Append("Source Language\t: " + sdlXliffReader.SourceLanguageCultureInfo.Name + " - " + sdlXliffReader.SourceLanguageCultureInfo.EnglishName + "\r\n");
                        _report.Append("Target Language\t: " + sdlXliffReader.TargetLanguageCultureInfo.Name + " - " + sdlXliffReader.TargetLanguageCultureInfo.EnglishName + "\r\n\r\n");

                        
                        _report.Append("Total Segments\r\n");
                        _report.Append("Read\t\t: " + (converter.SegmentsExported + converter.SegmentsNotExported) + "\r\n");
                        _report.Append("Exported\t: " + converter.SegmentsExported + "\r\n");
                        _report.Append("Ignored\t\t: " + converter.SegmentsNotExported + "\r\n\r\n");

                        if (converter.SegmentsExported + converter.SegmentsNotExported == 0)
                        {
                            _report.Append("Warning\t: no segment pairs found; it is required to use the target version of the SDLXLIFF file when exporting.\r\n\r\n");
                        }

                        _report.Append("".PadRight(90, '-') + "\r\n\r\n");

                       
                    }
                  
                    _report.Append("End Processing: " + DateTime.Now + "\r\n\r\n");

                    _report.Append("".PadRight(90, '-') + "\r\n\r\n");
                }
                finally
                {
                    sdlXliffReader.Progress -= sdlXliffReader_Progress;
                }
            }
            catch (Exception ex)
            {
                _report.Append("\r\nException Error\t: " + ex.Message + "\r\n");
                #region  |  set cursor  |

                Enabled = true;
                Cursor = Cursors.Default;

                #endregion
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    CreateReport();
                }
                catch
                {
                    // ignored
                }

                #region  |  set cursor  |

                Enabled = true;
                Cursor = Cursors.Default;

                #endregion
                #region  |  set progress info  |


                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;

                toolStripStatusLabel_Progress_Percentage.Text = @"100%";
                toolStripStatusLabel_Message.Text = string.Empty;
                toolStripStatusLabel_Status.Text = @"Ready";
                toolStripStatusLabel_Progress_Files.Text = @"Finished Processing";
                Application.DoEvents();

                #endregion

                
            }
        }

        private void RunImport()
        {

            
            Enabled = false;
            Cursor = Cursors.WaitCursor;

          
            toolStripProgressBar1.Maximum = 10;
            toolStripProgressBar1.Value = 0;

            toolStripStatusLabel_Progress_Percentage.Text = @"0%";
            toolStripStatusLabel_Message.Text = @"Initializing interop.word";
            toolStripStatusLabel_Status.Text = @"Processing...";
            toolStripStatusLabel_Progress_Files.Text = string.Empty;
            Application.DoEvents();
           
            try
            {
                _report = new StringBuilder();
                var sdlXliffWriter = new Sdl.Community.XliffReadWrite.Processor();


                _report = new StringBuilder();
                _report.Append(Application.ProductName + " (" + Application.ProductVersion + ")\r\n\r\n");

                _report.Append("Report Name\t: " + textBox_reportFileName.Text + "\r\n");
                _report.Append("Processing Type\t: Import\r\n\r\n");
                
                _report.Append("Start Processing: " + DateTime.Now + "\r\n\r\n");

                _report.Append("".PadRight(90, '-') + "\r\n\r\n");

                var processingFilesTmp = new Dictionary<string, string>();
                var processingFilePathTmp = string.Empty;
                var sbReportBody = new StringBuilder();
                try
                {
                    sbReportBody.Append("Total files processed: " + listView_import.Items.Count + "\r\n\r\n");
                    sbReportBody.Append("".PadRight(90, '-') + "\r\n\r\n");

                    sdlXliffWriter.Progress += sdlXliffWriter_Progress;

                    var iFile = 1;
                    foreach (ListViewItem itm in listView_import.Items)
                    {
                        processingFilePathTmp = Path.Combine(itm.Tag.ToString(), itm.SubItems[1].Text);

                        toolStripStatusLabel_Progress_Files.Text = " " + iFile++ + @" of " + listView_import.Items.Count + @" files";

                        Dictionary<string, ParagraphUnit> paragraphUnits;

                        sbReportBody.Append("Original file\t: " + Path.Combine(itm.Tag.ToString(), itm.SubItems[1].Text) + "\r\n");
                        sbReportBody.Append("Bilingual file\t: " + Path.Combine(itm.Tag.ToString(), itm.Text) + "" + "\r\n\r\n");                        
 
                        var converter = new Processor();
                        try
                        {
                            converter.Progress += converter_Progress;
                            paragraphUnits = itm.Text.ToLower().Trim().EndsWith(".ttx") 
                                ? converter.ReadTtx(Path.Combine(itm.Tag.ToString(), itm.Text)) 
                                : converter.ReadWord(Path.Combine(itm.Tag.ToString(), itm.Text));
                        }
                        finally
                        {
                            converter.Progress -= converter_Progress;
                        }
                    

                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal = Path.Combine(itm.Tag.ToString(), itm.SubItems[1].Text);
                        
                        if (!File.Exists(Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal))
                            throw new Exception("Unable to locate the associated sdlxliff file for the bilingual file\r\n" + itm.Text);



                        var sdlXliffReader = new Sdl.Community.XliffReadWrite.Processor();
                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal;
                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathUpdated = string.Empty;
                      
                        var fileParagraphUnitsOriginal = sdlXliffReader.ReadFileParagraphUnits();


                        #region  |  importing from file without legacy structure  |
                        if (paragraphUnits.Count == 1 && paragraphUnits.ContainsKey(string.Empty))
                        {                  
                            var segmentCountOriginal = (from fileParagraphUnit in fileParagraphUnitsOriginal 
                                                        from paragraphUnit in fileParagraphUnit.Value 
                                                            from segmentPair in paragraphUnit.Value.SegmentPairs 
                                                            select segmentPair).Count();
                            var segmentCountNew = paragraphUnits[string.Empty].SegmentPairs.Count;

                            if (segmentCountOriginal != segmentCountNew)
                            {
                                throw new Exception("Unable to perform import; found a discrepancy in the segment counts:\r\n"
                                                + "Segments found in the SDLXLIFF file:\t" + segmentCountOriginal + "\r\n"
                                                + "Segments found in the Legacy file:\t" + segmentCountNew + "\r\n");
                            }
                            var segmentIndex = 0;
                            var paragraphUnitsNew = new Dictionary<string, ParagraphUnit>();
                            foreach (var fileParagraphUnit in fileParagraphUnitsOriginal)
                            {

                                foreach (var paragraphUnit in fileParagraphUnit.Value)
                                {
                                    var unit = new ParagraphUnit
                                    {
                                        FileName = paragraphUnit.Value.FileName,
                                        ParagraphUnitId = paragraphUnit.Value.ParagraphUnitId
                                    };

                                    foreach (var segmentPair in paragraphUnit.Value.SegmentPairs)
                                    {
                                        segmentPair.Target = paragraphUnits[string.Empty].SegmentPairs[segmentIndex].Target;
                                        segmentPair.TargetSections = paragraphUnits[string.Empty].SegmentPairs[segmentIndex].TargetSections;

                                        unit.SegmentPairs.Add(segmentPair);
                                        segmentIndex++;
                                    }
                                    paragraphUnitsNew.Add(paragraphUnit.Key, unit);
                                }
                            }

                            paragraphUnits = paragraphUnitsNew;
                        }
                        #endregion

                        #region  |  check for missing keys  |

                        var paragraphKeysNotFound = new List<string>();
                        var segmentKeysNotFound = new List<string>();
                        foreach (var paragraphUnitNew in paragraphUnits)
                        {
                            var foundParagraphKey = false;
                            foreach (var fileParagraphUnitOriginal in fileParagraphUnitsOriginal)
                            {
                                foreach (var paragraphUnitOriginal in fileParagraphUnitOriginal.Value)
                                {
                                    if (string.Compare(paragraphUnitNew.Key, paragraphUnitOriginal.Key, StringComparison.OrdinalIgnoreCase) != 0)
                                        continue;
                                    foundParagraphKey = true;
                                    segmentKeysNotFound.AddRange(from segmentPairNew in paragraphUnitNew.Value.SegmentPairs 
                                                                 let foundSegmentKey = paragraphUnitOriginal.Value.SegmentPairs.Any(segmentPairOriginal => 
                                                                     string.Compare(segmentPairNew.Id, segmentPairOriginal.Id, StringComparison.OrdinalIgnoreCase) == 0) 
                                                                 where !foundSegmentKey select segmentPairNew.Id);
                                    break;
                                }
                            }

                            if (!foundParagraphKey)
                                paragraphKeysNotFound.Add(paragraphUnitNew.Key);


                            
                            if (paragraphKeysNotFound.Count > 1)
                                break;

                            if (segmentKeysNotFound.Count > 1)
                                break;
                        }


                        var originalCountSegments = fileParagraphUnitsOriginal.SelectMany(fileParagraphUnitOriginal => fileParagraphUnitOriginal.Value).Count();

                        if (paragraphKeysNotFound.Count > 0
                            && originalCountSegments == paragraphUnits.Count)
                        {
                            //try to match the segment ids instead
                            paragraphKeysNotFound = new List<string>();
                            segmentKeysNotFound = new List<string>();

                            var tempParagraphs = paragraphUnits.Select(paragraphUnitNew => paragraphUnitNew.Value).ToList();

                            var paragraphUnitsNewer = new Dictionary<string, ParagraphUnit>();
                            var i = 0;
                            foreach (var fileParagraphUnitOriginal in fileParagraphUnitsOriginal)
                            {
                                foreach (var paragraphUnitOriginal in fileParagraphUnitOriginal.Value)
                                {
                                    var xParagraphUnitTemp = tempParagraphs[i];
                                    xParagraphUnitTemp.ParagraphUnitId = paragraphUnitOriginal.Key;
                                    paragraphUnitsNewer.Add(paragraphUnitOriginal.Key, xParagraphUnitTemp);
                                    i++;
                                }
                            }
                            paragraphUnits = paragraphUnitsNewer;
                        }

                        if (paragraphKeysNotFound.Count > 0 || segmentKeysNotFound.Count > 0)
                        {
                            var paragraphKeys = paragraphKeysNotFound.Aggregate(string.Empty, (current, key) => current + (current != string.Empty ? ", " : string.Empty) + key);
                            var segmentKeys = segmentKeysNotFound.Aggregate(string.Empty, (current, key) => current + (current != string.Empty ? ", " : string.Empty) + key);

                            throw new Exception("Unable to perform import; found a discrepancy in the keys:\r\n"
                                + (paragraphKeys != string.Empty ? "Unknown paragraph keys: " + paragraphKeys + "\r\n" : string.Empty)
                                + (segmentKeys != string.Empty ? "Unknown segment keys: " + segmentKeys + "\r\n" : string.Empty)
                                + "\r\n");
                        }
                        #endregion




                        Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathUpdated = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal + "_updated.sdlxliff";

                        var fileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>
                        {
                            {string.Empty, paragraphUnits}
                        };
                        sdlXliffWriter.WriteFileParagraphUnits(fileParagraphUnits);

                        sdlXliffWriter.TargetLanguageCultureInfo = GetTargetCultureInfo(Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, sdlXliffWriter.SourceLanguageCultureInfo, sdlXliffWriter.TargetLanguageCultureInfo);

                        sbReportBody.Append("Source Language\t: " + sdlXliffWriter.SourceLanguageCultureInfo.Name + " - " + sdlXliffWriter.SourceLanguageCultureInfo.EnglishName + "\r\n");
                        sbReportBody.Append("Target Language\t: " + sdlXliffWriter.TargetLanguageCultureInfo.Name + " - " + sdlXliffWriter.TargetLanguageCultureInfo.EnglishName + "\r\n\r\n");

                        sbReportBody.Append("Original file\r\n");
                        sbReportBody.Append("Total segments\t\t: " + sdlXliffWriter.TotalSegmentsOriginalFile + "\r\n\r\n");

                        sbReportBody.Append("Bilingual file\r\n");
                        sbReportBody.Append("Total segments\t\t: " + (converter.SegmentsImported + converter.SegmentsNotImported) + "\r\n");
                        sbReportBody.Append("Read\t\t\t: " + converter.SegmentsImported + "\r\n");
                        sbReportBody.Append("Ignored\t\t\t: " + converter.SegmentsNotImported + "\r\n\r\n");

                        sbReportBody.Append("Contant changes\r\n");
                        sbReportBody.Append("Translations Updated\t: " + sdlXliffWriter.TotalContentChanges + "\r\n\r\n");

                        sbReportBody.Append("Segment status changes\r\n");
                        sbReportBody.Append("With content changes\t: " + sdlXliffWriter.TotalStatusChangesWithContentChanges + "\r\n");
                        sbReportBody.Append("Without content changes\t: " + sdlXliffWriter.TotalStatusChangesWithoutContentChanges + "\r\n");
                        sbReportBody.Append("No changes\t\t: " + (sdlXliffWriter.TotalSegmentsOriginalFile - (sdlXliffWriter.TotalStatusChangesWithContentChanges + sdlXliffWriter.TotalStatusChangesWithoutContentChanges)) + "\r\n\r\n");


                        if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ReportDifferencesInTagsPlaceables)
                        {
                            if (sdlXliffWriter.TagUnitWarnings.Count > 0)
                            {
                                sbReportBody.Append("Tag and Placeable differences\r\n");

                                foreach (var tagUnitWarning in sdlXliffWriter.TagUnitWarnings)
                                {
                                    sbReportBody.Append("Segment ID: " + tagUnitWarning.Key + "\r\n");
                                    foreach (var unitWarning in tagUnitWarning.Value)
                                    {
                                        sbReportBody.Append(unitWarning.WarningMessage + "\r\n");
                                        foreach (var tagUnit in unitWarning.TagUnits)
                                        {
                                            sbReportBody.Append(tagUnit.Content + "\r\n");
                                        }
                                        sbReportBody.Append("\r\n");
                                    }
                                    sbReportBody.Append("\r\n");
                                    sbReportBody.Append("\r\n");                                
                                }
                                sbReportBody.Append("\r\n");
                            }
                        }

                        if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CreateBackupOfSdlXliffFile)
                        {
                            sbReportBody.Append("Created backup of the original file\r\n" + Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal + ".bak" + "\r\n\r\n");
                        }

                        sbReportBody.Append("".PadRight(90, '-') + "\r\n\r\n");

                        processingFilesTmp.Add(Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathOriginal, Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.FilePathUpdated);
                    }



                    #region  |  finalize files that were processed  |
                    foreach (var processingFile in processingFilesTmp)
                    {

                        if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CreateBackupOfSdlXliffFile)
                        {
                            if (File.Exists(processingFile.Key + ".bak"))
                                File.Delete(processingFile.Key + ".bak");

                            File.Move(processingFile.Key, processingFile.Key + ".bak");                          
                        }
                        else
                            File.Delete(processingFile.Key);

                        File.Move(processingFile.Value, processingFile.Key);
                    }
                    #endregion

                    _report.Append(sbReportBody);
                }
                  
                catch (Exception ex)
                {
                    if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.UndoAllChangesOnException)
                    {
                        //clean up the processing files if an exception occurs
                        foreach (var processingFile in processingFilesTmp)
                            File.Delete(processingFile.Value);
                    }
                    else
                    {
                        #region  |  finalize files that were processed  |
                        foreach (var processingFile in processingFilesTmp)
                        {

                            if (Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CreateBackupOfSdlXliffFile)
                            {
                                if (File.Exists(processingFile.Key + ".bak"))
                                    File.Delete(processingFile.Key + ".bak");

                                File.Move(processingFile.Key, processingFile.Key + ".bak");
                            }
                            else
                                File.Delete(processingFile.Key);

                            File.Move(processingFile.Value, processingFile.Key);
                        }
                        #endregion

                        _report.Append(sbReportBody);
                    }

                    throw new Exception("Error occurred while processing the file:\r\n" + processingFilePathTmp + "\r\n\r\n" + ex.Message);
                }
                finally
                {                   
                    sdlXliffWriter.Progress -= sdlXliffWriter_Progress;                  
                }


            }
            catch (Exception ex)
            {

                _report.Append("\r\nException Error\t: " + ex.Message + "\r\n\r\n");
                _report.Append("".PadRight(90, '-') + "\r\n\r\n");
               
                Enabled = true;
                Cursor = Cursors.Default;

               
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                   
                    _report.Append("End Processing: " + DateTime.Now + "\r\n\r\n");
                    _report.Append("".PadRight(90, '-') + "\r\n\r\n");


                    CreateReport();
                }
                catch
                {
                    // ignored
                }

             
                Enabled = true;
                Cursor = Cursors.Default;

               
               

                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;

                toolStripStatusLabel_Progress_Percentage.Text = @"100%";
                toolStripStatusLabel_Message.Text = string.Empty;
                toolStripStatusLabel_Status.Text = @"Ready";
                toolStripStatusLabel_Progress_Files.Text = @"Finished Processing";
                Application.DoEvents();
             
            }

        }


        private void CreateReport()
        {

            using (var sw = new StreamWriter(Path.Combine(textBox_reportDirectory.Text, textBox_reportFileName.Text), false, Encoding.UTF8))
            {
                sw.WriteLine(_report);
                sw.Flush();
                sw.Close();
            }

            if (checkBox_viewReportWhenProcessingFinished.Checked)
                System.Diagnostics.Process.Start(Path.Combine(textBox_reportDirectory.Text, textBox_reportFileName.Text));
        }

        private void LoadSettings()
        {
            var settings = new FormSettings
            {
                checkBox_copySourceToTargetEmptyTranslations =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CopySourceToTargetEmptyTranslations
                },
                checkBox_ReportDifferencesInTags =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ReportDifferencesInTagsPlaceables
                },
                checkBox_ignoreEmptyTranslations =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations
                },
                checkBox_UndoChangesToAllFileOnException =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.UndoAllChangesOnException
                },
                checkBox_pm_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportPerfectMatch
                },
                checkBox_cm_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportContextMatch
                },
                checkBox_em_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportExactMatch
                },
                checkBox_fm_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportFuzzyMatch
                },
                checkBox_nm_doNotExport = {Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNoMatch},
                checkBox_locked_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportLocked
                },
                checkBox_unlocked_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportUnLocked
                },
                checkBox_notTranslated_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNotTranslated
                },
                checkBox_draft_doNotExport = {Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportDraft},
                checkBox_translated_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslated
                },
                checkBox_translationApproved_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationApproved
                },
                checkBox_translationRejected_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationRejected
                },
                checkBox_signOffRejected_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOffRejected
                },
                checkBox_signOff_doNotExport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOff
                },
                checkBox_pm_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportPerfectMatch
                },
                checkBox_cm_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportContextMatch
                },
                checkBox_em_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportExactMatch
                },
                checkBox_fm_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportFuzzyMatch
                },
                checkBox_nm_doNotImport = {Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNoMatch},
                checkBox_locked_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportLocked
                },
                checkBox_unlocked_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportUnLocked
                },
                checkBox_notTranslated_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNotTranslated
                },
                checkBox_draft_doNotImport = {Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportDraft},
                checkBox_translated_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslated
                },
                checkBox_translationApproved_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationApproved
                },
                checkBox_translationRejected_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationRejected
                },
                checkBox_signOffRejected_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOffRejected
                },
                checkBox_signOff_doNotImport =
                {
                    Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOff
                },
                comboBox_UpdatedStatus =
                {
                    SelectedItem =
                    Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ChangedTranslationStatus != string.Empty
                        ? Processor.GetVisualSegmentStatus(
                            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ChangedTranslationStatus)
                        : "Don't Change"
                },
                comboBox_NotUpdatedStatus =
                {
                    SelectedItem =
                    Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotChangedTranslationStatus != string.Empty
                        ? Processor.GetVisualSegmentStatus(
                            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotChangedTranslationStatus)
                        : "Don't Change"
                },
                comboBox_NotImportedStatus =
                {
                    SelectedItem =
                    Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotImportedTranslationStatus != string.Empty
                        ? Processor.GetVisualSegmentStatus(
                            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotImportedTranslationStatus)
                        : "Don't Change"
                }
            };




            settings.comboBox_OuputFormat.Items.Clear();
             settings.comboBox_OuputFormat.Items.Add(new ComboBoxExItem("Word 97-2003 Document (*.doc)", 0));
             settings.comboBox_OuputFormat.Items.Add(new ComboBoxExItem("Word Document (*.docx)", 1));

             settings.comboBox_OuputFormat.Items.Add(new ComboBoxExItem("TRADOStag Documents (*.ttx)", 2));
             settings.comboBox_OuputFormat.Items.Add(new ComboBoxExItem("TMX 1.4 (*.tmx)  -  Export only", 3));

             settings.comboBox_OuputFormat.SelectedIndex = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsConverToFormat;

             settings.checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
             settings.checkBox_reverseLanguageDirection.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsReverseLanguageDirection;
             settings.checkBox_viewReportWhenProcessingFinished.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsViewReportWhenProcessingFinished;
             settings.checkBox_createBAKfile.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsCreateBaKfile;
             settings.checkBox_excludeTags.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsExcludeTags;
          

            settings.ShowDialog();

            if (!settings.IsSaved)
                return;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsConverToFormat = settings.comboBox_OuputFormat.SelectedIndex;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure = settings.checkBox_includeLegacyStructure.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsReverseLanguageDirection = settings.checkBox_reverseLanguageDirection.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsViewReportWhenProcessingFinished = settings.checkBox_viewReportWhenProcessingFinished.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsCreateBaKfile = settings.checkBox_createBAKfile.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsExcludeTags = settings.checkBox_excludeTags.Checked;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations = settings.checkBox_ignoreEmptyTranslations.Checked;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CopySourceToTargetEmptyTranslations = settings.checkBox_copySourceToTargetEmptyTranslations.Checked;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ReportDifferencesInTagsPlaceables = settings.checkBox_ReportDifferencesInTags.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.UndoAllChangesOnException = settings.checkBox_UndoChangesToAllFileOnException.Checked;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportPerfectMatch = settings.checkBox_pm_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportContextMatch = settings.checkBox_cm_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportExactMatch = settings.checkBox_em_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportFuzzyMatch = settings.checkBox_fm_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNoMatch = settings.checkBox_nm_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportLocked = settings.checkBox_locked_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportUnLocked = settings.checkBox_unlocked_doNotExport.Checked;


            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNotTranslated = settings.checkBox_notTranslated_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportDraft = settings.checkBox_draft_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslated = settings.checkBox_translated_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationApproved = settings.checkBox_translationApproved_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationRejected = settings.checkBox_translationRejected_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOffRejected = settings.checkBox_signOffRejected_doNotExport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOff = settings.checkBox_signOff_doNotExport.Checked;

  

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportPerfectMatch = settings.checkBox_pm_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportContextMatch = settings.checkBox_cm_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportExactMatch = settings.checkBox_em_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportFuzzyMatch = settings.checkBox_fm_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNoMatch = settings.checkBox_nm_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportLocked = settings.checkBox_locked_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportUnLocked = settings.checkBox_unlocked_doNotImport.Checked;

            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNotTranslated = settings.checkBox_notTranslated_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportDraft = settings.checkBox_draft_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslated = settings.checkBox_translated_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationApproved = settings.checkBox_translationApproved_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationRejected = settings.checkBox_translationRejected_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOffRejected = settings.checkBox_signOffRejected_doNotImport.Checked;
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOff = settings.checkBox_signOff_doNotImport.Checked;



            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.ChangedTranslationStatus = string.Compare(settings.comboBox_UpdatedStatus.SelectedItem.ToString(), "Don't Change", StringComparison.OrdinalIgnoreCase) == 0 
                ? string.Empty 
                : Processor.GetSegmentStatusFromVisual(settings.comboBox_UpdatedStatus.SelectedItem.ToString());
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotChangedTranslationStatus = string.Compare(settings.comboBox_NotUpdatedStatus.SelectedItem.ToString(), "Don't Change", StringComparison.OrdinalIgnoreCase) == 0 
                ? string.Empty 
                : Processor.GetSegmentStatusFromVisual(settings.comboBox_NotUpdatedStatus.SelectedItem.ToString());
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.NotImportedTranslationStatus = string.Compare(settings.comboBox_NotImportedStatus.SelectedItem.ToString(), "Don't Change", StringComparison.OrdinalIgnoreCase) == 0 
                ? string.Empty 
                : Processor.GetSegmentStatusFromVisual(settings.comboBox_NotImportedStatus.SelectedItem.ToString());
            Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.CreateBackupOfSdlXliffFile = checkBox_createBAKfile.Checked;                

            Sdl.Community.XliffReadWrite.Processor.SaveSettings();
        }


        private void DeleteFilesFromExportListView()
        {
            foreach (ListViewItem itm in listView_export.Items)
            {
                if (itm.Selected)
                    itm.Remove();
            }
            label_itemCount_export.Text = @"files: " + listView_export.Items.Count;
            CheckEnableRun();
        }
        private void DeleteAllFilesFromExportListView()
        {
            foreach (ListViewItem itm in listView_export.Items)
            {             
                    itm.Remove();
            }
            label_itemCount_export.Text = @"files: " + listView_export.Items.Count;
            CheckEnableRun();
        }
        private void AddFilesToExportListView(List<FileInfo> files)
        {
            foreach (var file in files)
            {
                var foundInList = listView_export.Items.Cast<ListViewItem>().Any(itm => string.Compare(itm.Text, file.FullName, StringComparison.OrdinalIgnoreCase) == 0);
                if (foundInList) continue;
                {
                    var itm = listView_export.Items.Add(file.FullName);
                    AutoUpdateReportFileLocation();
                }
            }
            label_itemCount_export.Text = @"files: " + listView_export.Items.Count;

            CheckEnableRun();
        }


        private void DeleteFilesFromImportListView()
        {
            foreach (ListViewItem itm in listView_import.Items)
            {
                if (itm.Selected)
                    itm.Remove();
            }
            label_itemCount_import.Text = @"files: " + listView_import.Items.Count;
            CheckEnableRun();
        }
        private void DeleteAllFilesFromImportListView()
        {
            foreach (ListViewItem itm in listView_import.Items)
            {              
                    itm.Remove();
            }
            label_itemCount_import.Text = @"files: " + listView_import.Items.Count;
            CheckEnableRun();
        }
        private void AddFilesToImportListView(List<FileInfo> files)
        {
            try
            {
                foreach (var file in files)
                {
                    var foundInList = (from ListViewItem itm in listView_import.Items let path = itm.Tag.ToString() 
                                       where string.Compare(Path.Combine(path, itm.Text), file.FullName, StringComparison.OrdinalIgnoreCase) == 0 
                                       select itm).Any();
                    if (foundInList) 
                        continue;

                    var sdlxliffFileName = file.Name.Trim().Substring(0, file.Name.LastIndexOf(".", StringComparison.Ordinal));              
                    var sdlxliffFilePath = file.DirectoryName;
                    if (!sdlxliffFileName.ToLower().EndsWith(".sdlxliff"))
                    {
                        throw new Exception("Unable to locate the original sdlxliff file for the file:\r\n" + file.FullName + "\r\n\r\nNote: The original sdlxliff file should be located in the same folder as the bilingual file and have the same name without the (*.rtf, *.doc, *.docx, *.ttx) extension");
                    }
                    if (sdlxliffFilePath != null && !File.Exists(Path.Combine(sdlxliffFilePath, sdlxliffFileName)))
                    {
                        throw new Exception("Unable to locate the original sdlxliff file for the file:\r\n" + file.FullName + "\r\n\r\nNote: The original sdlxliff file should be located in the same folder as the bilingual file and have the same name without the (*.rtf, *.doc, *.docx, *.ttx) extension");
                    }

                    var item = listView_import.Items.Add(file.Name);
                    item.SubItems.Add(sdlxliffFileName);
                    item.SubItems.Add(sdlxliffFilePath);
                    item.Tag = file.DirectoryName;

                    AutoUpdateReportFileLocation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            label_itemCount_import.Text = @"files: " + listView_import.Items.Count;
            CheckEnableRun();
        }

        private static IEnumerable<string> ReadProjectSettingsFile(string filePath, bool isImport)
        {
            var fileList = new List<string>();
            string fileText;
            using (var sr = new StreamReader(filePath, Encoding.UTF8))
            {
                fileText = sr.ReadToEnd();
                sr.Close();
            }
            var regProjectFiles = new Regex(@"\<ProjectFiles\>(?<ProjectFiles>(.*?|))\<\/ProjectFiles\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regLanguageFiles = new Regex(@"\<LanguageFiles\>(?<LanguageFiles>(.*?|))\<\/LanguageFiles\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regLanguageFile = new Regex(@"\<LanguageFile\s+(?<LanguageFileElement>[^\>]*)\>(?<LanguageFile>(.*?|))\<\/LanguageFile\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regFileVersion = new Regex(@"\<FileVersion\s+(?<FileVersion>[^\>]*)\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regFileName = new Regex(@"\s+FileName=""(?<FileName>[^\""]*)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regPhysicalPath = new Regex(@"\s+PhysicalPath=""(?<PhysicalPath>[^\""]*)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var regLanguageCode = new Regex(@"\s+LanguageCode=""(?<LanguageCode>[^\""]*)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);           
            var regSourceLanguageCode = new Regex(@"\<SourceLanguageCode\>(?<SourceLanguageCode>[^\<]*)\<\/SourceLanguageCode\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var mRegSourceLanguageCode = regSourceLanguageCode.Match(fileText);
            if (!mRegSourceLanguageCode.Success) return fileList;
            var sourceLanguageCode = mRegSourceLanguageCode.Groups["SourceLanguageCode"].Value;

            var mRegProjectFiles = regProjectFiles.Match(fileText);
            if (!mRegProjectFiles.Success) 
                return fileList;

            var projectFiles = mRegProjectFiles.Groups["ProjectFiles"].Value;

            var mcRegLanguageFiles = regLanguageFiles.Matches(projectFiles);
            foreach(Match mRegLanguageFiles in mcRegLanguageFiles)
            {

                var languageFiles = mRegLanguageFiles.Groups["LanguageFiles"].Value;


                var mcRegLanguageFile = regLanguageFile.Matches(languageFiles);
                        
                foreach (Match mRegLanguageFile in mcRegLanguageFile)
                {
                    var languageFileElement = mRegLanguageFile.Groups["LanguageFileElement"].Value;
                    var languageFile = mRegLanguageFile.Groups["LanguageFile"].Value;

                    var languageCode = string.Empty;
                    var mRegLanguageCode = regLanguageCode.Match(languageFileElement);
                    if (mRegLanguageCode.Success)
                    {
                        languageCode = mRegLanguageCode.Groups["LanguageCode"].Value;
                    }

                    if (languageCode.Trim() == string.Empty ||
                        string.Compare(languageCode, sourceLanguageCode, StringComparison.OrdinalIgnoreCase) == 0)
                        continue;
                    var mcRegFileVersion = regFileVersion.Matches(languageFile);

                    var fileName = string.Empty;
                    var rootPath = string.Empty;
                    var physicalPath = string.Empty;
                    var fullFilePath = string.Empty;

                    var addedLanguageFile = false;
                    foreach (Match mRegFileVersion in mcRegFileVersion)
                    {
                        fileName = string.Empty;
                        rootPath = Path.GetDirectoryName(filePath);
                        physicalPath = string.Empty;
                        fullFilePath = string.Empty;

                        var fileVersion = mRegFileVersion.Groups["FileVersion"].Value;

                        var mRegFileName = regFileName.Match(fileVersion);
                        if (mRegFileName.Success)
                        {
                            fileName = mRegFileName.Groups["FileName"].Value.Replace("&amp;","&");
                        }

                        var mRegPhysicalPath = regPhysicalPath.Match(fileVersion);
                        if (mRegPhysicalPath.Success)
                        {

                            physicalPath = mRegPhysicalPath.Groups["PhysicalPath"].Value.Replace("&amp;", "&");
                            if (rootPath != null) fullFilePath = Path.Combine(rootPath, physicalPath);

                            if (!File.Exists(fullFilePath) || fileList.Contains(fullFilePath) ||
                                !fullFilePath.ToLower().EndsWith(".sdlxliff")) continue;
                            
                            addedLanguageFile = true;

                            if (isImport)
                            {
                                var docFileName = fullFilePath + ".doc";
                                var rtfFileName = fullFilePath + ".rtf";
                                var ttxFileName = fullFilePath + ".ttx";
                                var docxFileName = fullFilePath + ".docx";

                                var docFileNameFound = File.Exists(docFileName);
                                var rtfFileNameFound = File.Exists(rtfFileName);
                                var ttxFileNameFound = File.Exists(ttxFileName);
                                var docxFileNameFound = File.Exists(docxFileName);

                                var refTranslatedFilesFound = 0;
                                if (docFileNameFound)
                                    refTranslatedFilesFound++;
                                if (rtfFileNameFound)
                                    refTranslatedFilesFound++;
                                if (ttxFileNameFound)
                                    refTranslatedFilesFound++;
                                if (docxFileNameFound)
                                    refTranslatedFilesFound++;


                                switch (refTranslatedFilesFound)
                                {
                                    case 0:
                                        throw new Exception("Unable to automatically add files from the project file; unalbe to locate the translated format for the file:\r\n\r\n" + fullFilePath + "\r\n\r\nNote: expected to find a file with the same name + the translation format extension (.doc, .docx, .ttx, .rtf)");
                                    case 1:
                                        if (docFileNameFound && !fileList.Contains(docFileName))                                                        
                                            fileList.Add(docFileName);
                                        else if (rtfFileNameFound && !fileList.Contains(rtfFileName))                                                        
                                            fileList.Add(rtfFileName);
                                        else if (ttxFileNameFound && !fileList.Contains(ttxFileName))
                                            fileList.Add(ttxFileName);
                                        else if (docxFileNameFound && !fileList.Contains(docxFileName))
                                            fileList.Add(docxFileName);
                                        break;
                                    default:
                                        if (refTranslatedFilesFound > 1)
                                        {
                                            throw new Exception("Unable to automatically add files from the project file because there exists more than one translated format for the file:\r\n\r\n" + fullFilePath + "");
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (!fileList.Contains(fullFilePath))
                                    fileList.Add(fullFilePath);
                            }
                        }
                    }

                    if (!addedLanguageFile && fullFilePath.Trim()!=string.Empty && !File.Exists(fullFilePath))
                        throw new Exception("Unalbe to automatically add files from the project file; unable to locate the file:\r\n\r\n" + fullFilePath);
                }
            }


            return fileList;

        }

        private void sdlXliffReader_Progress(int maximum, int current, int percent, string message)
        {
            #region  |  set progress info  |
            toolStripProgressBar1.Maximum = maximum;
            toolStripProgressBar1.Value = current;

            toolStripStatusLabel_Progress_Percentage.Text = percent + "%";
            toolStripStatusLabel_Message.Text = message;
            toolStripStatusLabel_Status.Text = @"Processsing...";
            Application.DoEvents();
            #endregion
        }

        private void sdlXliffWriter_Progress(int maximum, int current, int percent, string message)
        {
            #region  |  set progress info  |
            toolStripProgressBar1.Maximum = maximum;
            toolStripProgressBar1.Value = current;

            toolStripStatusLabel_Progress_Percentage.Text = percent + "%";
            toolStripStatusLabel_Message.Text = message;
            toolStripStatusLabel_Status.Text = @"Processsing...";
            Application.DoEvents();
            #endregion
        }

        private void converter_Progress(int maximum, int current, int percent, string message)
        {
            #region  |  set progress info  |
            toolStripProgressBar1.Maximum = maximum;
            toolStripProgressBar1.Value = current;

            toolStripStatusLabel_Progress_Percentage.Text = percent + "%";
            toolStripStatusLabel_Message.Text = message;
            toolStripStatusLabel_Status.Text = @"Processsing...";
            Application.DoEvents();
            #endregion
        }

        private void AutoUpdateReportFileLocation()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    {
                        if (listView_export.Items.Count > 0)
                        {
                            textBox_reportDirectory.Text =  Path.GetDirectoryName(listView_export.Items[0].Text); 
                        }
                        
                    }break;

                case 1:
                    {
                        if (listView_import.Items.Count > 0)
                        {
                            textBox_reportDirectory.Text = listView_import.Items[0].Tag.ToString();
                        }
                    } break;
            }

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void toolStripButton_Options_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void listView_export_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                var sdlXliffFiles = new List<FileInfo>();
                foreach (var filePath in fileList)
                {                    
                    if (filePath.ToLower().EndsWith(".sdlxliff"))
                    {
                        sdlXliffFiles.Add(new FileInfo(filePath));
                    }
                    else if (filePath.ToLower().EndsWith(".sdlproj"))
                    {
                        var files = ReadProjectSettingsFile(filePath, false);
                        sdlXliffFiles.AddRange(files.Select(file => new FileInfo(file)));
                    }
                }
                AddFilesToExportListView(sdlXliffFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }


        }

        private void listView_export_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView_export_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteFilesFromExportListView();
        }

        private void button_addSDLXLIFF_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = @"SDL XLIFF file (*.sdlxliff)|*.sdlxliff;",
                Title = @"Select the SDLXLIFF files",
                FilterIndex = 0,
                Multiselect = true,
                RestoreDirectory = true
            };


            if (ofd.ShowDialog() != DialogResult.OK) return;
            var sdlXliffFiles = (from file in ofd.FileNames where file.ToLower().EndsWith(".sdlxliff") select new FileInfo(file)).ToList();
            AddFilesToExportListView(sdlXliffFiles);
        }

        private void button_removeSDLXLIFF_Click(object sender, EventArgs e)
        {
            DeleteFilesFromExportListView();
        }

        private void toolStripButton_Run_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                RunExport();
            else
                RunImport();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                RunExport();
            else
                RunImport();
        }

        private void listView_import_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                var importFiles = new List<FileInfo>();
                foreach (var filePath in fileList)
                {

                    if (filePath.ToLower().EndsWith(".rtf")
                           || filePath.ToLower().EndsWith(".doc")
                           || filePath.ToLower().EndsWith(".docx")
                           || filePath.ToLower().EndsWith(".ttx"))
                    {
                        importFiles.Add(new FileInfo(filePath));
                    }
                    else if (filePath.ToLower().EndsWith(".sdlproj"))
                    {
                        var projectfiles = ReadProjectSettingsFile(filePath, true);
                        importFiles.AddRange(projectfiles.Select(projectFile => new FileInfo(projectFile)));
                    }
                }

                AddFilesToImportListView(importFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void listView_import_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void button_addRTF_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();


            ofd.Filter = @"Bilingual file formats (*.rtf,*.doc,*.docx,*.ttx)|*.rtf;*.doc;*.docx;*.ttx|Rich Text File (*.rtf)|*.rtf|Word 97-2003 Document (*.doc)|*.doc|Word Document (*.docx)|*.docx|TRADOStag Documents (*.ttx)|*.ttx;";
            ofd.Title = @"Select the bilingual files";
            ofd.FilterIndex = 0;

            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var rtfFiles = (from file in ofd.FileNames 
                            where file.ToLower().EndsWith(".rtf") || file.ToLower().EndsWith(".doc") 
                                || file.ToLower().EndsWith(".docx") || file.ToLower().EndsWith(".ttx") 
                            select new FileInfo(file)).ToList();
            AddFilesToImportListView(rtfFiles);
        }

    

        private void button_RemoveRTF_Click(object sender, EventArgs e)
        {
            DeleteFilesFromImportListView();
        }

        private void listView_import_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteFilesFromImportListView();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = new About();
            f.ShowDialog();
        }

        private void comboBox_OuputFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_OuputFormat.SelectedIndex)
            {
                case 0:
                    {
                        checkBox_excludeTags.Visible = false;
                        checkBox_reverseLanguageDirection.Visible = false;

                        checkBox_includeLegacyStructure.Visible = true;
                        checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
                        checkBox_includeLegacyStructure.Enabled = true;
                        
                    } break;
                case 1:
                    {
                        checkBox_excludeTags.Visible = false;
                        checkBox_reverseLanguageDirection.Visible = false;

                        checkBox_includeLegacyStructure.Visible = true;
                        checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
                        checkBox_includeLegacyStructure.Enabled = true;

                        
                    } break;
                case 2:
                    {
                        checkBox_excludeTags.Visible = false;
                        checkBox_reverseLanguageDirection.Visible = false;

                        checkBox_includeLegacyStructure.Visible = true;
                        checkBox_includeLegacyStructure.Checked = true;
                        checkBox_includeLegacyStructure.Enabled = false;
                        
                    } break;
                case 3:
                    {
                        checkBox_excludeTags.Visible = true;
                        checkBox_reverseLanguageDirection.Visible = true;

                        checkBox_includeLegacyStructure.Visible = false;
                        checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
                        checkBox_includeLegacyStructure.Enabled = false;
                        
                    } break;
                default:
                    {
                        checkBox_excludeTags.Visible = false;
                        checkBox_reverseLanguageDirection.Visible = false;

                        checkBox_includeLegacyStructure.Visible = false;
                        checkBox_includeLegacyStructure.Checked = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DefaultSettingsIncludeLegacyStructure;
                        checkBox_includeLegacyStructure.Enabled = false;
                        
                    } break;
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnableRun();
        }

     

        private void button_removeAll_Export_Click(object sender, EventArgs e)
        {
            DeleteAllFilesFromExportListView();
        }

        private void button_removeAll_Import_Click(object sender, EventArgs e)
        {

            DeleteAllFilesFromImportListView();
        }

        private void button_loadProjectExport_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = @"SDL Trados Studio Project Files(*.sdlproj)|*.sdlproj;",
                    Title = @"Select an SDL Trados Studio project file",
                    FilterIndex = 0,
                    Multiselect = true,
                    RestoreDirectory = true
                };


                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                var sdlXliffFiles = (from filePath in openFileDialog.FileNames 
                                     where filePath.ToLower().EndsWith(".sdlproj") 
                                     from file in ReadProjectSettingsFile(filePath, false) 
                                     select new FileInfo(file)).ToList();
                AddFilesToExportListView(sdlXliffFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void button_loadProjectImport_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();


                openFileDialog.Filter = @"SDL Trados Studio Project Files(*.sdlproj)|*.sdlproj;";
                openFileDialog.Title = @"Select an SDL Trados Studio project file";
                openFileDialog.FilterIndex = 0;

                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                var importFiles = (from filePath in openFileDialog.FileNames 
                                   where filePath.ToLower().EndsWith(".sdlproj") 
                                   from projectFile in ReadProjectSettingsFile(filePath, true) 
                                   select new FileInfo(projectFile)).ToList();

                AddFilesToImportListView(importFiles);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void button_browse_reportDirectory_Click(object sender, EventArgs e)
        {
            var sPath = textBox_reportDirectory.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }

            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = sPath,
                ShowNewFolderButton = true,
                Description = @"Select the report directory"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_reportDirectory.Text = folderBrowserDialog.SelectedPath;
            }

            CheckEnableRun();
        }

        private void textBox_reportDirectory_TextChanged(object sender, EventArgs e)
        {
            CheckEnableRun();
        }

        private void textBox_reportFileName_TextChanged(object sender, EventArgs e)
        {
            CheckEnableRun();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                DeleteFilesFromExportListView();
            else
                DeleteFilesFromImportListView();
        }

        private void openFolderContainingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = string.Empty;
            if (tabControl1.SelectedIndex == 0)
            {
                if (listView_export.SelectedItems.Count > 0)
                {
                    path = Path.GetDirectoryName(listView_export.SelectedItems[0].Text);
                }
            }
            else
            {
                if (listView_import.SelectedItems.Count > 0)
                {
                    path = listView_import.SelectedItems[0].Tag.ToString();
                }
            }

            if (path != null && (path.Trim()!=string.Empty && Directory.Exists(path)))
            {
                 System.Diagnostics.Process.Start(
                    "explorer.exe",
                    string.Format("\"{0}\"", path));               
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (listView_export.SelectedItems.Count > 0)
                {
                    removeToolStripMenuItem.Enabled = true;
                    openFolderContainingToolStripMenuItem.Enabled = true;
                }
                else
                {
                    removeToolStripMenuItem.Enabled = false;
                    openFolderContainingToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                if (listView_import.SelectedItems.Count > 0)
                {
                    removeToolStripMenuItem.Enabled = true;
                    openFolderContainingToolStripMenuItem.Enabled = true;
                }
                else
                {
                    removeToolStripMenuItem.Enabled = false;
                    openFolderContainingToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
			Process.Start(@"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3280.legacy-converter");
        }

        private void help1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Process.Start(@"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3280.legacy-converter");
        }
    }
}
