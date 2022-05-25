﻿using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public class TerminologyEntryIndex : DefinitionLanguage, IEntryLanguage
    {
        public IEntry ParentEntry { get; }
        public IList<IEntryField> Fields { get; }
        public IList<IEntryTerm> Terms { get; }
    }
}
