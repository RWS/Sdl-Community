using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    [TerminologyProviderFactory(Id = "TerminologyProviderFactoryExcel",Name = "Excel Terminology Provider",Description = "Excel terminology provider factory")]
    public class TerminologyProviderFactoryExcel : ITerminologyProviderFactory
    {
        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return true;
        }

        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri,
            ITerminologyProviderCredentialStore credentials)
        {
            return new TerminologyProviderExcel();
        }
    }
}
