using System.Collections.Generic;
using InterpretBank.Service.Model;
using Xunit;

namespace InterpretBankTests
{
	public class ConditionBuilderTests
	{
		private readonly ConditionBuilder _conditionBuilder;

		public ConditionBuilderTests()
		{
			_conditionBuilder = new ConditionBuilder();
		}

		[Fact]
		public void Equals_MultipleValues_Test()
		{
			var comparisonValues = new List<string> { "TestValue1", "TestValue2" };
			var sqlStatement = _conditionBuilder
				.Equals(comparisonValues, "TestColumn")
				.Build();

			var expectedString = @"(TestColumn = ""TestValue1"" OR TestColumn = ""TestValue2"")";
			Assert.Equal(expectedString, sqlStatement);
		}

		[Fact]
		public void Equals_SingleValue_Test()
		{
			var sqlStatement = _conditionBuilder
				.Equals("TestValue", "TestColumn")
				.Build();

			Assert.Equal(@"(TestColumn = ""TestValue"")", sqlStatement);
		}

		[Fact]
		public void Like_Test()
		{
			var columns = new List<string> { "C1", "C2" };
			var sqlStatement = _conditionBuilder
				.Like("TestSearch", columns)
				.Build();

			var expectedString = @"(C1 like ""%TestSearch%"" OR C2 like ""%TestSearch%"")";
			Assert.Equal(expectedString, sqlStatement);
		}
	}
}