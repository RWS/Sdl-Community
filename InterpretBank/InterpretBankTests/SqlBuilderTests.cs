using System.Collections.Generic;
using InterpretBank.GlossaryService;
using InterpretBank.SqlBuilder;
using Xunit;

namespace InterpretBankTests
{
	public class SqlBuilderTests
	{
		[Fact]
		public void InnerJoin_Test()
		{
			var sqlBuilder = new SqlBuilder();
			var columns = new List<string> { "C1", "C2", "C3" };
			var sqlStatement = sqlBuilder
				.Columns(columns)
				.Table(Tables.GlossaryData)
				.InnerJoin(Tables.GlossaryMetadata, "Tag1", "GlossaryID")
				.Build();

			Assert.Equal("SELECT C1, C2, C3 FROM GlossaryData INNER JOIN GlossaryMetadata ON Tag1=GlossaryID", sqlStatement.CommandText);
		}

		[Fact]
		public void SqlBuild_Test()
		{
			var sqlBuilder = new SqlBuilder();
			var columns = new List<string> { "C1", "C2", "C3" };
			var values = new List<object> { "V1", "V2", "V3" };
			var sqlStatement = sqlBuilder
				.Columns(columns)
				.Table(Tables.GlossaryData)
				.Insert(values)
				.Build();

			Assert.Equal(@"INSERT INTO GlossaryData (C1, C2, C3) VALUES (""@0"", ""@1"", ""@2"")", sqlStatement.CommandText);
		}
	}
}