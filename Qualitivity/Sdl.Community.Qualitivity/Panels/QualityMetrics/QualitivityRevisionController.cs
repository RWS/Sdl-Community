using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Panels.QualityMetrics
{
    [ViewPart(
       Id = "QualitivityRevisionController",
       Name = "Quality Metrics",
       Description = "Quality Metrics",
       Icon = "QualitivityRevisionController_Icon"
       )]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Right)]
    public class QualitivityRevisionController : AbstractViewPartController
    {
        QualitivityViewController _controller { get; set; }
        public QualitivityViewController Controller
        {
            get
            {
                return _controller;
            }
            set
            {

                _controller = value;
            }
        }

        internal static List<QualityMetric> QualityMetrics { get; set; }
        
       
        internal static string SelectedSegmentId { get; set; }
        internal static string SelectedParagraphId { get; set; }

        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            Control.Value.comboBox_qm_filter.SelectedIndexChanged -= comboBox_qm_filter_SelectedIndexChanged;
            Control.Value.comboBox_qm_filter.SelectedIndexChanged += comboBox_qm_filter_SelectedIndexChanged;
        }

        

        protected void comboBox_qm_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeQualityMetricsData();
            InitializeQualityQuickInsertMenu();
        }

        public static void InitializeQualityMetricsData(List<QualityMetric> qualityMetrics, bool resetProperties)
        {
            QualityMetrics = qualityMetrics ?? new List<QualityMetric>();
          
            //always work with an initialized reference
            if (qualityMetrics != null && qualityMetrics.Count > 0)
            {
                foreach (var qm in QualityMetrics)
                {
                    //ensure that all updated attributes are set to false on initialization
                    //this will be used to identify what we hand back to the caller from 
                    //the method 'getUpdatedQualityMetricsForCurrentSegment()'
                    if (!resetProperties) continue;
                    qm.Updated = false;
                    qm.Removed = false;
                }
            }

            //initialize the grid
            InitializeQualityMetricsData();
           

            
        }
        public static void SetCurrentSelectedSegmentId(string selectedParagraphId, string selectedSegmentId)
        {
            SelectedSegmentId = selectedSegmentId;
            SelectedParagraphId = selectedParagraphId;
            if (Control.Value.comboBox_qm_filter.SelectedIndex != 1) return;
            InitializeQualityMetricsData();
            InitializeQualityQuickInsertMenu();
        }


        public static List<QualityMetric> GetUpdatedQualityMetricsForCurrentSegment()
        {
            var qualityMetrics = new List<QualityMetric>();

            if (QualityMetrics == null)
                return qualityMetrics;

            foreach (var qualityMetric in QualityMetrics)
            {
                if (qualityMetric.ParagraphId != SelectedParagraphId || qualityMetric.SegmentId != SelectedSegmentId ||
                    (!qualityMetric.Updated && !qualityMetric.Removed)) continue;
                //provide a clone to the caller
                //the caller should decide how to manage their own version control
                qualityMetrics.Add(qualityMetric.Clone() as QualityMetric);

                //reset the status items to false
                qualityMetric.Updated = false;
                qualityMetric.Removed = false;
            }

            return qualityMetrics;
        }
        public static List<QualityMetric> GetQualityMetricsData()
        {                      
            return QualityMetrics;
        }

        public static void CleanQualityMetricsDataContainer()
        {
            QualityMetrics = new List<QualityMetric>();
            Control.Value.dataGridView_qm.Rows.Clear();
            
        }

        private static void InitializeQualityMetricsData()
        {         
            #region  |  update the container  |

            try
            {
                Control.Value.dataGridView_qm.Rows.Clear();

                foreach (var qm in QualityMetrics)
                {
                    var addRow = true;
                    if (Control.Value.comboBox_qm_filter.SelectedIndex == 1)
                    {
                        if ((qm.ParagraphId != SelectedParagraphId) && (qm.SegmentId != SelectedSegmentId))
                            addRow = false;
                    }
                    if (!addRow) continue;
                    var n = Control.Value.dataGridView_qm.Rows.Add();

                    var dataGridViewImageCell = Control.Value.dataGridView_qm.Rows[n].Cells["Column_status"] as DataGridViewImageCell;
                    var img = Control.Value.imageList1.Images["empty"];
                    switch (qm.Status)
                    {
                        case QualityMetric.ItemStatus.Open: img = Control.Value.imageList1.Images["Flag-Blue"]; break;
                        case QualityMetric.ItemStatus.Resolved: img = Control.Value.imageList1.Images["Flag-Green"]; break;
                        default: img = Control.Value.imageList1.Images["Flag-Violet"]; break;
                    }
                    if (dataGridViewImageCell != null)
                    {
                        dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                        dataGridViewImageCell.Value = img;
                        dataGridViewImageCell.ValueType = typeof(Image);
                        dataGridViewImageCell.ToolTipText = string.Empty;
                        switch (qm.Status)
                        {
                            case QualityMetric.ItemStatus.Open: dataGridViewImageCell.ToolTipText = "Open"; break;
                            case QualityMetric.ItemStatus.Resolved: dataGridViewImageCell.ToolTipText = "Resolved"; break;
                            default: dataGridViewImageCell.ToolTipText = "Ignore"; break;
                        }
                    }


                    dataGridViewImageCell = Control.Value.dataGridView_qm.Rows[n].Cells["Column_recorded"] as DataGridViewImageCell;
                    img = qm.RecordId > -1 ? Control.Value.imageList1.Images["Flag-Yellow"] : Control.Value.imageList1.Images["empty"];

                    if (dataGridViewImageCell != null)
                    {
                        dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                        dataGridViewImageCell.Value = img;
                        dataGridViewImageCell.ValueType = typeof(Image);
                        dataGridViewImageCell.ToolTipText = "";
                    }


                    var segmentId = Control.Value.dataGridView_qm.Rows[n].Cells["Column_segment_id"] as DataGridViewTextBoxCell;
                    if (segmentId != null) segmentId.Value = qm.SegmentId;

                    var textBox = Control.Value.dataGridView_qm.Rows[n].Cells["Column_qm"] as DataGridViewTextBoxCell;
                    if (textBox != null) textBox.Value = qm.Name;

                    textBox = Control.Value.dataGridView_qm.Rows[n].Cells["Column_severity"] as DataGridViewTextBoxCell;
                    if (textBox != null) textBox.Value = qm.SeverityName;

                    textBox = Control.Value.dataGridView_qm.Rows[n].Cells["Column_content"] as DataGridViewTextBoxCell;
                    if (textBox != null) textBox.Value = qm.Content;

                    textBox = Control.Value.dataGridView_qm.Rows[n].Cells["Column_comment"] as DataGridViewTextBoxCell;
                    if (textBox != null) textBox.Value = qm.Comment;
         

                    Control.Value.dataGridView_qm.Rows[n].Tag = qm;
                }
                if (Control.Value.dataGridView_qm.Rows.Count > 0)
                {
                    Control.Value.dataGridView_qm.Rows[0].Selected = true;
                    Control.Value.dataGridView_qm_SelectionChanged(null, null);
                }

                Control.Value.dataGridView_qm.Sort(Control.Value.dataGridView_qm.Columns[2], ListSortDirection.Ascending);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

            InitializeQualityQuickInsertMenu();

        }

        public static void InitializeQualityQuickInsertMenu()
        {
            #region  |  initialize quick insert  |

           
            Control.Value.initialize_quick_insert();

            #endregion
        }

        public static void AddNewQualityMetric(QualityMetric qm)
        {
            qm.SegmentId = SelectedSegmentId;
            qm.ParagraphId = SelectedParagraphId;
            qm.Created = DateTime.Now;
            qm.Modified = DateTime.Now;
            Control.Value.AddNewQualityMetric(qm);
        }
        public static void SetCurrentContentQuickInsertSelection(string content)
        {
            Control.Value.textBox_content.Text = content;
        }




        private static readonly Lazy<QualitivityRevisionControl> Control = new Lazy<QualitivityRevisionControl>(() => new QualitivityRevisionControl());   
   

    }
}
