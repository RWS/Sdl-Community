using InterpretBank.Model;
using InterpretBank.Model.Interface;
using InterpretBank.Service;
using InterpretBank.Service.Interface;
using InterpretBank.Service.Model;
using NSubstitute;

namespace InterpretBankTests
{
	public class GlossaryServiceBuilder
	{
		public GlossaryServiceBuilder()
		{
			ConditionBuilder = new ConditionBuilder();
			SqlBuilder = new SqlBuilder();
			DatabaseConnection = Substitute.For<IDatabaseConnection>();
		}

		private IConditionBuilder ConditionBuilder { get; set; }
		private ISqlBuilder SqlBuilder { get; set; }
		private IDatabaseConnection DatabaseConnection { get; set; }

		public IGlossaryService Build()
		{
			var glossaryService =  new GlossaryService(DatabaseConnection, SqlBuilder, ConditionBuilder);

			ResetFields();

			return glossaryService;
		}

		public GlossaryServiceBuilder WithConditionBuilder(IConditionBuilder conditionBuilder)
		{
			ConditionBuilder = conditionBuilder;
			return this;
		}
		
		public GlossaryServiceBuilder WithDatabaseConnection(IDatabaseConnection connection)
		{
			DatabaseConnection = connection;
			return this;
		}
		
		public GlossaryServiceBuilder WithSqlBuilder(ISqlBuilder sqlBuilder)
		{
			SqlBuilder = sqlBuilder;
			return this;
		}

		private void ResetFields()
		{
			ConditionBuilder = new ConditionBuilder();
			SqlBuilder = new SqlBuilder();
		}
	}
}