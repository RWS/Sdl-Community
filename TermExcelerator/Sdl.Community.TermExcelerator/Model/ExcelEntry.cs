﻿using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public class ExcelEntry: IExcelEntry
    {
        public int Id { get; set; }
        public IList<IEntryField> Fields { get; set; }
        public IList<IEntryTransaction> Transactions { get; set; }
        public IList<IEntryLanguage> Languages { get; set; }
        public string SearchText { get; set; }
        public bool IsDirty { get; set; } 
    }
}