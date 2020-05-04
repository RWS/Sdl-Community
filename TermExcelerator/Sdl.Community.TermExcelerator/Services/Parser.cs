using System.Collections.Generic;
using System.Linq;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services.Interfaces;

namespace Sdl.Community.TermExcelerator.Services
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