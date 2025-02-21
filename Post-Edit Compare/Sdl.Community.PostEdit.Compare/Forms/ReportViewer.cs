using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PostEdit.Compare.Forms
{
    public partial class ReportViewer : Form
    {

        public PanelReportViewer PanelReportViewer;
        public PanelReportViewerNavigation PanelReportViewerNavigation;

        public DeserializeDockContent DeserializeDockContentReportViewerNavigation { get; private set; }

        public string ReportFileFullPath { get; set; }


        private void InitializePanelReportViewerNavigation()
        {


            PanelReportViewer = new PanelReportViewer();

            PanelReportViewerNavigation = new PanelReportViewerNavigation();

            ResetDefaultWindowStatesCompare();

        }

        private void ShowPanelReportViewerPanel(bool resetState)
        {
            try
            {

                if (resetState)
                {
                    if (PanelReportViewer != null && PanelReportViewer.DockPanel != null)
                        PanelReportViewer.Close();

                    PanelReportViewer = new PanelReportViewer();
                    PanelReportViewer.Show(dockPanel_reportViewer, DockState.Document);
                    PanelReportViewer.Focus();
                }
                else if (PanelReportViewer == null || PanelReportViewer.DockPanel == null)
                {
                    PanelReportViewer = new PanelReportViewer();
                    PanelReportViewer.Show(dockPanel_reportViewer, DockState.Document);
                    PanelReportViewer.Focus();
                }
                else
                {
                    PanelReportViewer.Show();
                    PanelReportViewer.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (PanelReportViewer != null && PanelReportViewer.DockPanel != null)
                    PanelReportViewer.Close();
                PanelReportViewer = new PanelReportViewer();
                PanelReportViewer.Show(dockPanel_reportViewer, DockState.Document);

            }


        }

        private void ShowPanelReportViewerNavigationPanel(bool resetState)
        {
            try
            {

                if (resetState)
                {
                    if (PanelReportViewerNavigation != null && PanelReportViewerNavigation.DockPanel != null)
                        PanelReportViewerNavigation.Close();

                    PanelReportViewerNavigation = new PanelReportViewerNavigation();
                    PanelReportViewerNavigation.Show(dockPanel_reportViewer, DockState.DockLeftAutoHide);
                    PanelReportViewerNavigation.Focus();
                }
                else if (PanelReportViewerNavigation == null || PanelReportViewerNavigation.DockPanel == null)
                {
                    PanelReportViewerNavigation = new PanelReportViewerNavigation();
                    PanelReportViewerNavigation.Show(dockPanel_reportViewer, DockState.DockLeftAutoHide);
                    PanelReportViewerNavigation.Focus();
                }
                else
                {
                    PanelReportViewerNavigation.Show();
                    PanelReportViewerNavigation.Focus();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (PanelReportViewerNavigation != null && PanelReportViewerNavigation.DockPanel != null)
                    PanelReportViewerNavigation.Close();
                PanelReportViewerNavigation = new PanelReportViewerNavigation();
                PanelReportViewerNavigation.Show(dockPanel_reportViewer, DockState.DockLeftAutoHide);
                PanelReportViewerNavigation.treeView_navigation.Select();
            }
            finally
            {

                ApplyEventHandlersReportViewerNavigation();

            }
        }

        private void ApplyEventHandlersReportViewerNavigation()
        {
            PanelReportViewerNavigation.treeView_navigation.AfterSelect -= TreeViewNavigationAfterSelect;

            PanelReportViewerNavigation.treeView_navigation.AfterSelect += TreeViewNavigationAfterSelect;
        }

        private void TreeViewNavigationAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (IsLoading) return;
            var key = e.Node.Tag.ToString();

            if (PanelReportViewer.webBrowserReport.Document == null) return;
            var links = PanelReportViewer.webBrowserReport.Document.GetElementsByTagName("a");
            //var element = PanelReportViewer.webBrowserReport.Document.GetElementById("fileId_0_0");


            foreach (HtmlElement link in links)  // this ex is given another SO post 
            {
                if (link.Name.Equals(key))
                {
                    link.ScrollIntoView(true);
                    //link.InvokeMember("Click");
                }
            }
        }

        private void ResetDefaultWindowStatesCompare()
        {
            if (PanelReportViewer != null && PanelReportViewer.DockPanel != null)
                PanelReportViewer.Close();
            ShowPanelReportViewerPanel(true);


            if (PanelReportViewerNavigation != null && PanelReportViewerNavigation.DockPanel != null)
                PanelReportViewerNavigation.Close();
            ShowPanelReportViewerNavigationPanel(true);
        }

        private bool IsLoading { get; set; }

        public Dictionary<string, object> Files { get; set; }
        public ReportViewer(DeserializeDockContent deserializeDockContentReportViewerNavigation)
        {
            DeserializeDockContentReportViewerNavigation = deserializeDockContentReportViewerNavigation;

            InitializeComponent();


            InitializePanelReportViewerNavigation();
            IsLoading = true;

            //if (PanelReportViewer == null)
            //    return;
            //PanelReportViewer.webBrowserReport.DocumentCompleted += WebBrowserReportDocumentCompleted;

        }

        public bool ViewSegmentsWithNoChanges { get; set; }
        public bool ViewSegmentsWithTranslationChanges { get; set; }
        public bool ViewSegmentsWithStatusChanges { get; set; }
        public bool ViewSegmentsWithComments { get; set; }
        public bool ViewFilesWithNoDifferences { get; set; }

        
        private void ReportViewerLoad(object sender, EventArgs e)
        {
            Visible = true;
            Application.DoEvents();

            toolStripButton_viewSegmentsWithNoChanges.CheckState = (ViewSegmentsWithNoChanges ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_viewSegmentsWithTranslationChanges.CheckState = (ViewSegmentsWithTranslationChanges ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_viewSegmentsWithStatusChanges.CheckState = (ViewSegmentsWithStatusChanges ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_viewSegmentsWithComments.CheckState = (ViewSegmentsWithComments ? CheckState.Checked : CheckState.Unchecked);
            toolStripButton_viewFilesWithNoDifferences.CheckState = (ViewFilesWithNoDifferences ? CheckState.Checked : CheckState.Unchecked);

            IsLoading = false;

            //HtmlElementCollection tds = _panel_ReportViewer.webBrowser_report.Document.GetElementsByTagName("tr");
            //foreach (HtmlElement td in tds)  // this ex is given another SO post 
            //{
            //    td.Click += new HtmlElementEventHandler(td_Click);
            //}                     
        }


        private static void WebBrowserReportDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //var isBusy = PanelReportViewer.webBrowserReport.IsBusy;
            //var state = PanelReportViewer.webBrowserReport.ReadyState;
            //Check if page is fully loaded or not
            //if (PanelReportViewer.webBrowserReport.ReadyState != WebBrowserReadyState.Complete)
            //  MessageBox.Show("loaded!");
            //else
            //Action to be taken on page loading completion
            //MessageBox.Show("loadeding!");    
        }

        //void webBrowser_report_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{

        //}
        //void td_Click(object sender, HtmlElementEventArgs e)
        //{
        //    HtmlElement element = (HtmlElement)sender;

        //    //element.SetAttribute("style", "background-color:yellow");
        //    //this.Refresh();
        //    //element.SetAttribute("style", "background-color:yellow;");

        //    //element.Style = "selected";

        //    //MessageBox.Show(element.FirstChild.InnerText);
        //}

        //void webBrowser_report_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{

        //}




        //void Body_MouseDown(Object sender, HtmlElementEventArgs e)
        //{
        //    switch(e.MouseButtonsPressed)
        //    {
        //        case MouseButtons.Left:
        //            {

        //                HtmlElement element = _panel_ReportViewer.webBrowser_report.Document.GetElementFromPoint(e.ClientMousePosition);
        //                if (element != null && "submit".Equals(element.GetAttribute("type"), StringComparison.OrdinalIgnoreCase))
        //                {
        //                }
        //                break;
        //            }
        //    }
        //}



        private void ToolStripButtonSaveAsClick(object sender, EventArgs e)
        {
            PanelReportViewer.webBrowserReport.ShowSaveAsDialog();
        }

        private void CloseToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

    }
}
