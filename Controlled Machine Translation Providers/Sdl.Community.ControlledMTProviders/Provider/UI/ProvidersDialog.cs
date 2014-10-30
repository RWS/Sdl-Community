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
    public partial class ProvidersDialog : Form
    {
        private ControlledMTProvidersProvider _controlledMTProvider;


        public ProvidersDialog(ControlledMTProvidersProvider controlledMTProvider)
        {
            InitializeComponent();
            _controlledMTProvider = controlledMTProvider;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IList<ITranslationProviderWinFormsUI> uis = TranslationProviderManager.GetTranslationProviderWinFormsUIs();
            var selectedMTProviders = _controlledMTProvider.GetSelectedMTProvidersUri();
            var defaultMTProvidersUri = _controlledMTProvider.GetDefaultMTProvidersUri();


            foreach (ITranslationProviderWinFormsUI ui in uis)
            {
                //exclude the default MT providers
                if (!CheckIfDefaultMTProvider(ui, defaultMTProvidersUri) && 
                    !ui.SupportsTranslationProviderUri(new Uri(ControlledMTProvidersProvider.ProviderUri)))
                {
                    var isChecked = IsMTProviderChecked(ui, selectedMTProviders);
                    clbProviders.Items.Add(new ProviderItem { Provider = ui }, isChecked);
                }
            }

            clbProviders.SelectedIndex = 0;
        }

        private bool IsMTProviderChecked(ITranslationProviderWinFormsUI ui, IList<Uri> selectedMTProviders)
        {
            return selectedMTProviders.Any(uri => ui.SupportsTranslationProviderUri(uri));
        }

        private bool CheckIfDefaultMTProvider(ITranslationProviderWinFormsUI ui, IList<Uri> defaultMTProvidersUri)
        {
            return defaultMTProvidersUri.Any(uri => ui.SupportsTranslationProviderUri(uri));
        }

        public IList<ITranslationProviderWinFormsUI> GetSelectedProviders()
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
}
