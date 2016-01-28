using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace ExportToExcel
{
    public partial class ExportTypeUI : UserControl
    {
        private GeneratorSettings _settings;

        public ExportTypeUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Calls Automatic task method for data binding
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettings(GeneratorSettings settings)
        {
            _settings = settings;

            SettingsBinder.DataBindSetting<string>(tb_Prefix, "Text", _settings,
                nameof(_settings.FileNamePrefix));
            SettingsBinder.DataBindSetting<decimal>(n_ColumnWidth, "Value", _settings,
                nameof(_settings.ColumnWidth));
            SettingsBinder.DataBindSetting<bool>(cb_ExtractComments, "Checked", _settings,
                nameof(_settings.ExtractComments));
        }

        private void cb_ExtractComments_CheckedChanged(object sender, EventArgs e)
        {
        }
      
    }


}
