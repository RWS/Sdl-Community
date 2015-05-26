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
        private readonly ControlledMtProvidersProvider _controlledMtProvider;


        public ProvidersDialog(ControlledMtProvidersProvider controlledMtProvider)
        {
            InitializeComponent();
            _controlledMtProvider = controlledMtProvider;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IList<ITranslationProviderWinFormsUI> uis = TranslationProviderManager.GetTranslationProviderWinFormsUIs();
            var selectedMtProviders = _controlledMtProvider.GetSelectedMtProvidersUri();
            var defaultMtProvidersUri = _controlledMtProvider.GetDefaultMtProvidersUri();


            foreach (ITranslationProviderWinFormsUI ui in uis)
            {
                //exclude the default MT providers
                if (!CheckIfDefaultMTProvider(ui, defaultMtProvidersUri) && 
                    !ui.SupportsTranslationProviderUri(new Uri(ControlledMtProvidersProvider.ProviderUri)))
                {
                    var isChecked = IsMTProviderChecked(ui, selectedMtProviders);
                    clbProviders.Items.Add(new ProviderItem { Provider = ui }, isChecked);
                }
            }

            clbProviders.SelectedIndex = 0;
        }

        private bool IsMTProviderChecked(ITranslationProviderWinFormsUI ui, IEnumerable<Uri> selectedMtProviders)
        {
            return selectedMtProviders.Any(ui.SupportsTranslationProviderUri);
        }

        private bool CheckIfDefaultMTProvider(ITranslationProviderWinFormsUI ui, IEnumerable<Uri> defaultMtProvidersUri)
        {
            return defaultMtProvidersUri.Any(ui.SupportsTranslationProviderUri);
        }

        public IList<ITranslationProviderWinFormsUI> GetSelectedProviders()
        {
            return (from object item in clbProviders.CheckedItems select item as ProviderItem into pi select pi.Provider).ToList();
        }
    }
}
