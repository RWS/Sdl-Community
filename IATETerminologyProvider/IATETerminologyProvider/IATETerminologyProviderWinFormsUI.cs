using IATETerminologyProvider.Service;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IATETerminologyProvider
{
	[TerminologyProviderWinFormsUI]
	public class IATETerminologyProviderWinFormsUI : ITerminologyProviderWinFormsUI
	{
		public string TypeName => PluginResources.IATETerminologyProviderName;
		public string TypeDescription => PluginResources.IATETerminologyProviderDescription;

		public bool SupportsEditing => true;
		
		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
		{
			var result = new List<ITerminologyProvider>();
			try
			{
				var IATETerminologyProvider = new IATETerminologyProvider();

				result.Add(IATETerminologyProvider);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result.ToArray();
		}

		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			return new TerminologyProviderDisplayInfo
			{
				Name = "IATE Terminology Provider",
				TooltipText = "IATE Terminology Provider"
			};
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "IATEglossary";
		}
	}
}
