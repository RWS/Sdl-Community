using System.Net;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace GroupshareExcelAddIn
{
    public partial class ThisAddIn
    {
        private void Application_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            Application.DisplayAlerts = true;
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.WorkbookBeforeClose += Application_WorkbookBeforeClose;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
        }

        #endregion VSTO generated code
    }
}