using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace InterpretBank.SqlBuilder.Interface
{
	public interface ISqlBuilder
	{
		SQLiteCommand Build();

		ISqlBuilder Columns(List<string> columnNames);

		ISqlBuilder CreateTable(List<DbType> types, List<string> constraints);

		ISqlBuilder Delete();

		ISqlBuilder InnerJoin<T>(T tableName, string firstId, string secondId);

		ISqlBuilder Insert(List<object> values);

		ISqlBuilder Table<T>(T tableName);

		ISqlBuilder Update(List<object> values);

		ISqlBuilder Where(string condition);

		IConditionBuilder Where();
	}
}