using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace ExportToExcel
{
    public partial class ExportToExcelSettingsControl : UserControl,ISettingsAware<GeneratorSettings>
    {
        public GeneratorSettings Settings { get; set; }

        public ExportToExcelSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the initial settings
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            exclusionsUI.SetSettings(Settings);
            exportTypeUI.SetSettings(Settings);
        }
        
        /// <summary>
        /// Updates settings
        /// </summary>
        public void UpdateSettings()
        {
            exclusionsUI.UpdateSettings(Settings);
        }

        /// <summary>
        /// Resets the Ui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            Settings.ResetToDefaults();
            exclusionsUI.UpdateUI(Settings);
        }
    }
}
