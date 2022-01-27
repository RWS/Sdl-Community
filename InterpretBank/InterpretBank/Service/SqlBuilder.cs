using System.Collections.Generic;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service
{
	public class SqlBuilder : ISqlBuilder
	{
		private List<string> ColumnNames { get; set; }
		private string FromTableName { get; set; }
		private string WhereCondition { get; set; }

		public string Build()
		{
			var select = $"SELECT {string.Join(",", ColumnNames)}";
			var from = $" FROM {FromTableName}";
			var where = WhereCondition is not null ? $" WHERE {WhereCondition}" : null;

			return $"{select}{from}{where}";
		}

		public ISqlBuilder From(string tableName)
		{
			FromTableName = tableName;
			return this;
		}

		public ISqlBuilder Select(List<string> columnNames)
		{
			ColumnNames = columnNames;
			return this;
		}

		public ISqlBuilder Where(string condition)
		{
			WhereCondition = condition;
			return this;
		}
	}
}