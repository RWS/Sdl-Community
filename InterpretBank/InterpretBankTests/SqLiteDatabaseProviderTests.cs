using System.Collections.Generic;
using InterpretBank.Service;
using Xunit;

namespace InterpretBankTests
{
	public class SqLiteDatabaseProviderTests
	{
		[Fact]
		public void GetTerms_Test()
		{
			var filepath = @"C:\Users\ealbu\Desktop\Interpret Bank\InterpretBankDatabaseV6.db";
			var sqlProvider = new GlossaryService(filepath);

			var languages = new List<string>
			{
				"English",
				"German"
			};

			var searchString = "englishTerm";
			var data = sqlProvider.GetTerms(searchString, languages, null, null, null);
		}

		[Fact]
		public void LoadIndices()
		{
			var sqlProvider = new GlossaryService(@"C:\Users\ealbu\AppData\Local\InterpretBank6\database\InterpretBankDatabaseV6.db");
			//var data = sqlProvider.GlossaryData;

			var languageIndicesDictionary = new Dictionary<string, int>
			{
				["English"] = 1,
				["German"] = 2,
				["Italian"] = 3,
				["Norwegian"] = 4,
			};
			Assert.Equal(languageIndicesDictionary, sqlProvider.LanguageIndicesDictionary);
		}
	}
}