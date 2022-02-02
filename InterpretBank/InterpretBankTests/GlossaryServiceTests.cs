using System.Collections.Generic;
using InterpretBank.Model;
using InterpretBank.Service;
using InterpretBank.Service.Interface;
using InterpretBank.Service.Model;
using NSubstitute;
using Xunit;

namespace InterpretBankTests
{
	public class GlossaryServiceTests
	{
		[Fact]
		public void RealDB_Test()
		{
			var filepath = @"../../Resources/InterpretBankDatabaseV6.db";
			var glossaryService = new GlossaryService(new DatabaseConnection(filepath), new SqlBuilder(),
				new ConditionBuilder());

			var languages = new List<int> {1, 2};
			glossaryService.GetTerms(null, languages, null,
				new List<string> { "AnotherTagOne" });
		}
		
		[Fact]
		public void UpdateTerm_Test()
		{
			var connectionMock = Substitute.For<IDatabaseConnection>();
			var glossaryService = new GlossaryService(connectionMock, new SqlBuilder(), new ConditionBuilder());

			glossaryService.UpdateTerm(new TermEntry());

			connectionMock
				.Received(2)
				.ExecuteCommand(Arg.Any<string>());
		}
		
		[Fact]
		public void AddTerm_Test()
		{
			var connectionMock = Substitute.For<IDatabaseConnection>();
			var sqlBuilderMock = Substitute.For<ISqlBuilder>();
			var conditionBuilderMock = Substitute.For<IConditionBuilder>();
			var glossaryService = new GlossaryService(connectionMock, sqlBuilderMock, conditionBuilderMock);

			glossaryService.AddTerm(new TermEntry());

			connectionMock
				.Received(2)
				.ExecuteCommand(Arg.Any<string>());
		}

		[Fact]
		public void GetTerms_Test()
		{
			//Arrange
			var sqlBuilder = new SqlBuilder();
			var conditionBuilder = new ConditionBuilder();
			var connectionMock = Substitute.For<IDatabaseConnection>();
			var glossaryService = new GlossaryService(connectionMock, sqlBuilder, conditionBuilder);
			var languages = new List<int> { 1, 2 };

			//Act
			_ = glossaryService.GetTerms(null, languages, null,
				new List<string> { "AnotherTagOne" });

			//Assert
			connectionMock
				.Received(2)
				.ExecuteCommand(Arg.Any<string>());
		}
	}
}