using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

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
            exclusionsUI.UpdateUi(Settings);
        }
    }
}
