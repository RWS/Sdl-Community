using System.Collections.Generic;
using InterpretBank.Service;
using Xunit;

namespace InterpretBankTests
{
	public class SqlBuilderTests
	{
		[Fact]
		public void Select_Test()
		{
			var languageIndices = new List<int> { 1, 2, 3 };
			var sql = "".SelectTermsFromGlossary(languageIndices);

			Assert.Equal(
				@"SELECT Term1, Comment1a, Comment1b, Term2, Comment2a, Comment2b, Term3, Comment3a, Comment3b, ID, Tag1, Tag2, CommentAll FROM GlossaryData",
				sql);
		}

		[Fact]
		public void Test()
		{
			var sqlBuilder = new SqlBuilder();

			var languageIndices = new List<int> { 1, 2, 3 };

			var conditionBuilder = new ConditionBuilder();
			var condition = conditionBuilder
				.Like("english", languageIndices)
				.Build();

			var sql = sqlBuilder
				.Select(GlossaryService.DefaultColumnNamesInGlossaryData, languageIndices)
				.From("GlossaryData")
				.Where(condition)
				.Build();

			Assert.Equal(
				"SELECT ID,Tag1,Tag2,CommentAll,Term1,Comment1a,Comment1b,Term2,Comment2a,Comment2b,Term3,Comment3a,Comment3b " +
				"FROM GlossaryData " +
				"WHERE (" +
				"Term1 like \"%english%\" " +
				"OR Comment1a like \"%english%\" " +
				"OR Comment1b like \"%english%\" " +
				"OR Term2 like \"%english%\" " +
				"OR Comment2a like \"%english%\" " +
				"OR Comment2b like \"%english%\" " +
				"OR Term3 like \"%english%\" " +
				"OR Comment3a like \"%english%\" " +
				"OR Comment3b like \"%english%\")",
				sql);
		}
	}
}