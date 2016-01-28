using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Model
{
    public class TerminologyEntryIndex : DefinitionLanguage, IEntryLanguage
    {
        public IEntry ParentEntry { get; }
        public IList<IEntryField> Fields { get; }
        public IList<IEntryTerm> Terms { get; }
    }
}
