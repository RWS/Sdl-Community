using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySampleTerminology
{
    [TerminologyProviderWinFormsUI]
    internal class MyTerminologyProviderWinFormsUI : ITerminologyProviderWinFormsUI
    {
        public bool SupportsEditing
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TypeDescription => "I'm a simple termbase";

        public string TypeName => "Sample Termbase";

        public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog
            {
                Title = "Select list file",
                Filter = "Delimited list files (*.txt)|*.txt"
            };
            dlgOpenFile.ShowDialog();

            var result = new List<ITerminologyProvider>();
            var _termProvider = new MyTerminologyProvider(dlgOpenFile.FileName);
            result.Add(_termProvider);
            return result.ToArray();
        }

        public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
        {
            throw new NotImplementedException();
        }

        public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
        {
			return new TerminologyProviderDisplayInfo()
			{
				Name = "Sample Terminology"
			};
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.ToString().StartsWith("file");
        }
    }
}
