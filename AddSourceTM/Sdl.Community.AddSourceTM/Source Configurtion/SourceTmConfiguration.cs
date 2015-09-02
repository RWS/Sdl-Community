using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.AddSourceTM.Source_Configurtion
{
    public partial class SourceTmConfiguration : Form
    {
        private readonly Uri _providerUri;
        private AddSourceTmConfigurations _addSourceTmConfigurations;
        private bool _isUsed;

        public SourceTmConfiguration(Uri providerUri)
        {
            InitializeComponent();

            _providerUri = providerUri;
            _isUsed = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            var persistance = new Persistance();
            _addSourceTmConfigurations = persistance.Load();

            var addSourceTmConfiguration =
                _addSourceTmConfigurations.Configurations.FirstOrDefault(x => x.ProviderUri == _providerUri);
            if (addSourceTmConfiguration == null)
            {
                addSourceTmConfiguration = _addSourceTmConfigurations.Default;
                addSourceTmConfiguration.ProviderUri = _providerUri;
                _addSourceTmConfigurations.Configurations.Add(addSourceTmConfiguration);
            }
            else
            {
                _isUsed = true;
            }
           
            txtSourceField.Text = addSourceTmConfiguration.TmSourceFieldName;
            chkFullPath.Checked = addSourceTmConfiguration.StoreFullPath;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var persistance = new Persistance();

            var addSourceTmConfiguration = _addSourceTmConfigurations.Configurations.FirstOrDefault(x => x.ProviderUri == _providerUri) ??
                                           _addSourceTmConfigurations.Default;
            if (_isUsed &&
                (addSourceTmConfiguration.StoreFullPath != chkFullPath.Checked ||
                 addSourceTmConfiguration.TmSourceFieldName != txtSourceField.Text))
            {
                var result = MessageBox.Show(
                    "You are about to change the source file configuration for this TM. This will result in creating an additional source file field. Are you sure you want to continue?",
                    "Confirm changes", MessageBoxButtons.OKCancel);

                if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            addSourceTmConfiguration.TmSourceFieldName = string.IsNullOrEmpty(txtSourceField.Text)
                ? "SourceFile"
                : txtSourceField.Text;
            addSourceTmConfiguration.StoreFullPath = chkFullPath.Checked;

            persistance.Save(_addSourceTmConfigurations);
        }
    }
}
