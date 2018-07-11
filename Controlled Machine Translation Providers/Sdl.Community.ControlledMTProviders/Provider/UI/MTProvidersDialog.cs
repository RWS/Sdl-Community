using Sdl.Community.ControlledMTProviders.Provider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.ControlledMTProviders.Provider.UI
{
    public partial class MTProvidersDialog : Form
    {
        private ControlledMtProvidersProvider _controlledMTProvider;

        public MTProvidersDialog(ControlledMtProvidersProvider controlledMTProvider)
        {
            InitializeComponent();
            _controlledMTProvider = controlledMTProvider;
        }

        public List<ITranslationProviderWinFormsUI> SelectedTranslationProvidersWinformsUI
        {
            get
            {
                List<ITranslationProviderWinFormsUI> mtProviders = new List<ITranslationProviderWinFormsUI>();
                foreach (var item in clbProviders.CheckedItems)
                {
                    ProviderItem pi = item as ProviderItem;
                    mtProviders.Add(pi.Provider);
                }

                return mtProviders;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IList<ITranslationProviderWinFormsUI> uis = TranslationProviderManager.GetTranslationProviderWinFormsUIs();

            var mtProvidersUri = _controlledMTProvider.GetAllMtProvidersUri();
            var selectedProvidersUri = _controlledMTProvider.GetSelectedMtProvidersUri();

            foreach (ITranslationProviderWinFormsUI ui in uis)
            {
                if (mtProvidersUri.Any(x => ui.SupportsTranslationProviderUri(x)))
                {
                    var isChecked = selectedProvidersUri.Any(x => ui.SupportsTranslationProviderUri(x));
                    clbProviders.Items.Add(new ProviderItem { Provider = ui }, isChecked);
                }
            }

            clbProviders.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(clbProviders.CheckedItems.Count == 0)
            {
                MessageBox.Show(this, "Please select a translation provider.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void btnAdditionalMTProviders_Click(object sender, EventArgs e)
        {
            using(ProvidersDialog providersDialog = new ProvidersDialog(_controlledMTProvider))
            {
                DialogResult result = providersDialog.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    var selectedProviders = providersDialog.GetSelectedProviders();

                    foreach (var selectedProvider in selectedProviders)
                    {
                        clbProviders.Items.Add(new ProviderItem { Provider = selectedProvider }, false);
                    }
                }
            }
        }
    }
}
