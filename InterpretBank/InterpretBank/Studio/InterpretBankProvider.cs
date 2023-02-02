using System;
using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	internal class InterpretBankProvider : AbstractTerminologyProvider
	{
		public override IDefinition Definition
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Description
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Uri Uri
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override IEntry GetEntry(int id)
		{
			throw new NotImplementedException();
		}

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			throw new NotImplementedException();
		}

		public override IList<ILanguage> GetLanguages()
		{
			throw new NotImplementedException();
		}

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			throw new NotImplementedException();
		}
	}
}