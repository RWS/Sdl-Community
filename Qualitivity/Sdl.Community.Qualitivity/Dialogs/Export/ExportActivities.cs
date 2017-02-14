using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Sdl.Community.Comparison;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity.Dialogs.Export
{
    public partial class ExportActivities : Form
    {

        public bool Saved { get; set; }
        public List<int> IdsSelected { get; set; }
        public List<int> IdsAll { get; set; }
        

        

        private void button_ok_Click(object sender, EventArgs e)
        {

            var tcc = new TextComparer {Type = TextComparer.ComparisonType.Words};


            #region  |  create lists  |
            var tpas = new List<Activity>();

            
            foreach (var tp in Tracked.TrackingProjects.TrackerProjects)
            {
                foreach (var tpa in tp.Activities)
                {
                    if (radioButton1.Checked)
                    {
                        #region  |  ids_selected  |

                        if (IdsSelected.Contains(tpa.Id))
                            tpas.Add(tpa);
                                               
                        #endregion
                    }
                    else if (radioButton2.Checked)
                    {
                        #region  |  ids_all  |

                        if (IdsAll.Contains(tpa.Id))
                            tpas.Add(tpa);
                       
                        #endregion
                    }
                    else
                    {
                        tpas.Add(tpa);
                    }
                }
            }
           

            var dasDictList = new List<DocumentActivity>();
            foreach(var tpa in tpas)            
                dasDictList.AddRange(Helper.GetDocumentActivityObjects(tpa));
            
            #endregion

            #region  |  create output data  |
            var xmlDasList = new List<XmlDocumentActivity>();
            foreach (var da in dasDictList)
            {
                var foundActivity = false;
                Activity tpa = null;
                foreach (var _tpa in tpas)
                {
                    var da1 = da;
                    if (!_tpa.Activities.Exists(a => a.DocumentId == da1.DocumentId)) continue;
                    foundActivity = true;
                    tpa = _tpa;
                    break;
                }


                if (foundActivity)
                {

                    foreach (var record in da.Records)
                    {

                        var xmlDas = new XmlDocumentActivity
                        {
                            ProjectId = da.ProjectId,
                            ActivityId = da.ProjectActivityId,
                            ActivityName = tpa.Name,
                            RecordId = record.Id.ToString(),
                            DocumentId = da.DocumentId,
                            DocumentName = da.TranslatableDocument.DocumentName,
                            DocumentStartDate = da.Started,
                            DocumentStopDate = da.Stopped,
                            DocumentTotalSeconds = da.TicksActivity/10000000,
                            ParagraphId = record.ParagraphId,
                            SegmentId = record.SegmentId,
                            OriginalConfirmationLevel = record.TranslationOrigins.Original.ConfirmationLevel,
                            OriginalTranslationStatus = record.TranslationOrigins.Original.TranslationStatus,
                            OriginalOriginSystem = record.TranslationOrigins.Original.OriginSystem,
                            OriginalOriginType = record.TranslationOrigins.Original.OriginType,
                            UpdatedConfirmationLevel = record.TranslationOrigins.Updated.ConfirmationLevel,
                            UpdatedTranslationStatus = record.TranslationOrigins.Updated.TranslationStatus,
                            UpdatedOriginSystem = record.TranslationOrigins.Updated.OriginSystem,
                            UpdatedOriginType = record.TranslationOrigins.Updated.OriginType,
                            SourceLang = da.TranslatableDocument.SourceLanguage,
                            TargetLang = da.TranslatableDocument.TargetLanguage,
                            SourceText =
                                Helper.GetCompiledSegmentText(record.ContentSections.SourceSections,
                                    tpa.ComparisonOptions.IncludeTagsInComparison),
                            TargetText =
                                Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections,
                                    tpa.ComparisonOptions.IncludeTagsInComparison),
                            UpdatedText =
                                Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections,
                                    tpa.ComparisonOptions.IncludeTagsInComparison)
                        };



                        if (xmlDas.OriginalTranslationStatus == string.Empty && xmlDas.TargetText.Trim() == string.Empty)                         
                            xmlDas.OriginalTranslationStatus = @"Not Translated";
                         


                        xmlDas.StartDate = record.Started;
                        xmlDas.StopDate = record.Stopped;
                        xmlDas.TotalSeconds = record.TicksElapsed / 10000000;
                        xmlDas.TotalMiliseconds = record.TicksElapsed / 10000;



                        var dld = new EditDistance(record, tpa);

                        xmlDas.WordsSource = record.WordCount;
                        xmlDas.EditDistance = dld.Edits;
                        xmlDas.EditDistanceRelative = dld.EditDistanceRelative;
                        xmlDas.PemPercentage = string.Compare(record.TranslationOrigins.Updated.OriginType, @"interactive", StringComparison.OrdinalIgnoreCase) == 0 ? dld.PemPercentage : 100;

  
                        if (checkBox_includeKeystokeData.Checked)
                            xmlDas.KeyStrokes = record.TargetKeyStrokes;

                        if (record.QualityMetrics != null)
                            xmlDas.QualityMetrics = record.QualityMetrics;



                        #region  |  comments  |
                        xmlDas.CommentsStr = string.Empty;
                        xmlDas.Comments = record.Comments;
                        if (record.Comments != null && record.Comments.Count > 0)
                        {
                            foreach (var comment in record.Comments)
                            {
                                if (comment.Created != null)
                                    xmlDas.CommentsStr += (xmlDas.CommentsStr.Trim() != string.Empty ? "\r\n\r\n" : string.Empty)
                                                           + PluginResources.Created_ + Helper.GetStringFromDateTimeMilli(comment.Created.Value)
                                                           + PluginResources._Severity_ + comment.Severity
                                                           + PluginResources._Author_ + comment.Author
                                                           + PluginResources._Comment_ + comment.Content;
                            }
                        }
                        #endregion

                        xmlDasList.Add(xmlDas);


                    }
                }
            }


            #endregion



            var exportToFile = new ExportToFile();
            if (radioButton_export_to_excel.Checked)
            {
                exportToFile.create_excel_report(textBox_folder_path.Text, tpas, xmlDasList);
            }
            else if (radioButton_export_to_xml.Checked)
            {
                exportToFile.create_xml_report(textBox_folder_path.Text, tpas, xmlDasList);
            }


            if (checkBox_view_reprot_when_complete.Checked)
            {
                Process.Start(textBox_folder_path.Text);
            }

        }


   
        
        public ExportActivities()
        {
            InitializeComponent();
        }

        private void CompanyProfile_Load(object sender, EventArgs e)
        {

            if (IdsSelected.Count > 0)
            {
                radioButton1.Checked = true;
            }
            else if (IdsAll.Count > 0)
            {
                radioButton1.Enabled = false;
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
                radioButton3.Checked = true;
            }

            
            CheckEnableSave();


        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_browse_folder_path_Click(object sender, EventArgs e)
        {
            var saveFileDialog1 = new SaveFileDialog();
            if (radioButton_export_to_excel.Checked)
            {
                saveFileDialog1.Filter = @"Excel Wordkbook (*.xlsx)|*.xlsx";
                saveFileDialog1.Title = PluginResources.Save_an_Excel_report_file;
            }
            else
            {
                saveFileDialog1.Filter = @"XML File (*.xml)|*.xml";
                saveFileDialog1.Title = PluginResources.Click_Save_an_XML_report_file;
            }
            
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                textBox_folder_path.Text = saveFileDialog1.FileName;
            }

            CheckEnableSave();
        }

        private void textBox_folder_path_TextChanged(object sender, EventArgs e)
        {
            CheckEnableSave();
        }

        private void CheckEnableSave()
        {
            var enabled= false;
            if (textBox_folder_path.Text.Trim() != string.Empty)
            {
                var directory = Path.GetDirectoryName(textBox_folder_path.Text);

                if (Directory.Exists(directory))
                {
                    enabled = true;
                }
            }
            button_ok.Enabled = enabled;
        }

    

        private void radioButton_export_to_xml_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_export_to_excel.Checked && !textBox_folder_path.Text.ToLower().EndsWith(".xlsx"))
            {
                if (textBox_folder_path.Text.ToLower().EndsWith(".xml"))
                {
                    var filePath = textBox_folder_path.Text.Substring(0,textBox_folder_path.Text.LastIndexOf(".", StringComparison.Ordinal));
                    textBox_folder_path.Text = filePath + @".xlsx";
                }
                else
                {
                    textBox_folder_path.Text = string.Empty;
                }

            }
            else if (radioButton_export_to_xml.Checked && !textBox_folder_path.Text.ToLower().EndsWith(".xml"))
            {
                if (textBox_folder_path.Text.ToLower().EndsWith(".xlsx"))
                {
                    var filePath = textBox_folder_path.Text.Substring(0,textBox_folder_path.Text.LastIndexOf(".", StringComparison.Ordinal));
                    textBox_folder_path.Text = filePath + @".xml";
                }
                else
                {
                    textBox_folder_path.Text = string.Empty;
                }
            }

        }


    }



























}





