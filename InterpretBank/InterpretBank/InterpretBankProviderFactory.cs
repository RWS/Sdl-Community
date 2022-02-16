using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpretBank
{
    [TerminologyProviderFactory(Id = "My_Terminology_Provider_Id",
                                Name = "My_Terminology_Provider_Name",
                                Description = "My_Terminology_Provider_Description")]
    public class InterpretBankProviderFactory : ITerminologyProviderFactory
    {
        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
        {
            throw new NotImplementedException();
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            throw new NotImplementedException();
        }
    }
}
