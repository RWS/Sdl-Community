using System;
using System.Windows.Forms;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	[TerminologyProviderWinFormsUI]
	internal class InterpretBankWinFormsUI : ITerminologyProviderWinFormsUI
	{
		public bool SupportsEditing
		{
			get
			{
				return true;
			}
		}

		public string TypeDescription => PluginResources.Plugin_Description;

		public string TypeName => PluginResources.Plugin_Name;

		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
		{
			var provider = InterpretBankProviderFactory.GetInterpretBankProvider();
			return new ITerminologyProvider[] { provider };
		}

		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			return new TerminologyProviderDisplayInfo { Name = "Interpret Bank", };
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri == new Uri(Constants.InterpretBankUri);
		}
	}
}