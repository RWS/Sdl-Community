using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PostEdit.Compare.Forms
{
    public partial class FileViewer : Form
    {

        public PanelFileViewer PanelFileViewer;
       
        private DeserializeDockContent deserializeDockContent_FileViewerNavigation;


      

        private void initializePanel_FileViewerNavigation()
        {


            PanelFileViewer = new PanelFileViewer();
            
            resetDefaultWindowStates_Compare();

        }


     


        private void show_Panel_FileViewer_Panel(bool resetState)
        {
            try
            {

                if (resetState)
                {
                    if (PanelFileViewer != null && PanelFileViewer.DockPanel != null)
                        PanelFileViewer.Close();

                    PanelFileViewer = new PanelFileViewer();
                    PanelFileViewer.Show(dockPanel_fileViewer, DockState.Document);
                    PanelFileViewer.Focus();
                }
                else if (PanelFileViewer == null || PanelFileViewer.DockPanel == null)
                {
                    PanelFileViewer = new PanelFileViewer();
                    PanelFileViewer.Show(dockPanel_fileViewer, DockState.Document);
                    PanelFileViewer.Focus();
                }
                else
                {
                    PanelFileViewer.Show();
                    PanelFileViewer.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (PanelFileViewer != null && PanelFileViewer.DockPanel != null)
                    PanelFileViewer.Close();
                PanelFileViewer = new PanelFileViewer();
                PanelFileViewer.Show(dockPanel_fileViewer, DockState.Document);
                
            }
            finally
            {

                applyEventHandlers_FileViewerNavigation();

            }
        }



        private void applyEventHandlers_FileViewerNavigation()
        { 
        }

      


        private void resetDefaultWindowStates_Compare()
        {
            if (PanelFileViewer != null && PanelFileViewer.DockPanel != null)
                PanelFileViewer.Close();
            show_Panel_FileViewer_Panel(true);


        }

       
        bool IsLoading { get; set; }

        bool ViewOriginalSourceColumn { get; set; }
        bool ViewOriginalStatusColumn { get; set; }
        bool ViewOriginalTargetColumn { get; set; }

        bool ViewUpdatedSourceColumn { get; set; }
        bool ViewUpdatedStatusColumn { get; set; }
        bool ViewUpdatedTargetColumn { get; set; }

        bool ViewTargetComparisonColumn { get; set; }

        


        bool ShowEqualSegments { get; set; }
        bool ShowModifiedSegments { get; set; }
        bool ShowSegmentsWithComments { get; set; }


        public FileViewer()
        {
            IsLoading = true;
            InitializeComponent();
            
            initializePanel_FileViewerNavigation();
            


      
        }

        private void FileViewer_Load(object sender, EventArgs e)
        {
            IsLoading = false;

            ShowEqualSegments = false;
            ShowModifiedSegments = true;
            ShowSegmentsWithComments = true;


            ViewOriginalSourceColumn = true;
            ViewOriginalStatusColumn = false;
            ViewOriginalTargetColumn = true;

            ViewUpdatedSourceColumn = false;
            ViewUpdatedStatusColumn = true;
            ViewUpdatedTargetColumn = true;


            ViewTargetComparisonColumn = true;

            panel_listViewMessage.Visible = false;

            

            ResizeDataGridViewColumns();

            Application.DoEvents();






        }

        private void ResizeDataGridViewColumns()
        {
            if (IsLoading) return;
            var iContentColumnsVisible = 0;
            var iStatusColumnsVisible = 0;

            if (ViewOriginalSourceColumn)
                iContentColumnsVisible++;
            else
                dataGridView_main.Columns["Source_original"].Visible = false;

            if (ViewOriginalStatusColumn)
                iStatusColumnsVisible++;
            else
                dataGridView_main.Columns["Status_original"].Visible = false;


            if (ViewOriginalTargetColumn)
                iContentColumnsVisible++;
            else
                dataGridView_main.Columns["Target_original"].Visible = false;

            if (ViewUpdatedSourceColumn)
                iContentColumnsVisible++;
            else
                dataGridView_main.Columns["Source_updated"].Visible = false;


            if (ViewUpdatedStatusColumn)
                iStatusColumnsVisible++;
            else
                dataGridView_main.Columns["Status_updated"].Visible = false;


            if (ViewUpdatedTargetColumn)
                iContentColumnsVisible++;
            else
                dataGridView_main.Columns["Target_updated"].Visible = false;

            if (ViewTargetComparisonColumn)
                iContentColumnsVisible++;
            else
                dataGridView_main.Columns["Target_comparison"].Visible = false;




            decimal totalWidth = dataGridView_main.Width - ((iStatusColumnsVisible * 50) + 10);

            var columnWidth = (totalWidth / iContentColumnsVisible);

            dataGridView_main.Columns["Source_original"].Width = Convert.ToInt32(columnWidth);
            dataGridView_main.Columns["Target_original"].Width = Convert.ToInt32(columnWidth);

            dataGridView_main.Columns["Source_updated"].Width = Convert.ToInt32(columnWidth);
            dataGridView_main.Columns["Target_updated"].Width = Convert.ToInt32(columnWidth);

            dataGridView_main.Columns["Target_comparison"].Width = Convert.ToInt32(columnWidth);


            panel_listViewMessage.Left = (dataGridView_main.Width / 2) - 150;
        }

        private void FileViewer_Resize(object sender, EventArgs e)
        {
            ResizeDataGridViewColumns();
        }

   

       


        

      
    }
}
