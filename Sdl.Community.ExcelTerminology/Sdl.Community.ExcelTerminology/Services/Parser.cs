using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;

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
            if (string.IsNullOrEmpty(term)) return new List<string>();
            return term
                .Split(_providerSettings.Separator)
                .Select(value => value.Trim())
                .ToList();
        }
    }
}
