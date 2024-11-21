using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleTerminology
{
    [TerminologyProviderFactory(Id = "My_Terminology_Provider_Id",
                                Name = "My_Terminology_Provider_Name",
                                Description = "My_Terminology_Provider_Description")]
    public class MyTerminologyProviderFactory : ITerminologyProviderFactory
    {
        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
        {
            MyTerminologyProvider _terminologyProvider = new MyTerminologyProvider(terminologyProviderUri.ToString());
            return _terminologyProvider;
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.ToString().StartsWith("file");
        }
    }
}
