using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class Parser: IParser
    {
        private readonly ProviderSettings _providerSettings;
        public Parser(ProviderSettings providerSettings)
        {
            _providerSettings = providerSettings;
        }

        public IList<string> Parse(string term)
        {
            return term
                .Split(_providerSettings.Separator)
                .Select(value => value.Trim())
                .ToList();
        }
    }
}
