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
				throw new NotImplementedException();
			}
		}

		public string TypeDescription
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string TypeName => PluginResources.Plugin_Name;

		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			throw new NotImplementedException();
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			throw new NotImplementedException();
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			throw new NotImplementedException();
		}
	}
}