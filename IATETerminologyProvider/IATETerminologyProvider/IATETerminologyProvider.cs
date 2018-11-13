using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IATETerminologyProvider
{
	class IATETerminologyProvider : AbstractTerminologyProvider
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
