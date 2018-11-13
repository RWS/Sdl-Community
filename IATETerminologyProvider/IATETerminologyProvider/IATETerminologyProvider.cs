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
		public IATETerminologyProvider()
		{

		}
		public override IDefinition Definition
		{
			get
			{
				return null;
			}
		}

		public override string Description
		{
			get
			{
				return null;

			}
		}

		public override string Name
		{
			get
			{
				return null;

			}
		}

		public override Uri Uri
		{
			get
			{
				return new Uri("https://iate.europa.eu/em-api/entries/_search");
			}
		}

		public override IEntry GetEntry(int id)
		{
			return null;

		}

		public override IEntry GetEntry(int id, IEnumerable<ILanguage> languages)
		{
			return null;

		}

		public override IList<ILanguage> GetLanguages()
		{
			return null;

		}

		public override IList<ISearchResult> Search(string text, ILanguage source, ILanguage destination, int maxResultsCount, SearchMode mode, bool targetRequired)
		{
			return null;

		}
	}
}
