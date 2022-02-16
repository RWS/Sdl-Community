using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace InterpretBank.Service.Interface
{
	public interface ISqlBuilder
	{
		ISqlBuilder Columns(List<string> columnNames);
		ISqlBuilder InnerJoin<T>(T tableName, string firstId, string secondId);
		ISqlBuilder Insert(List<object> values);

		ISqlBuilder Table<T>(T tableName);

		ISqlBuilder Where(string condition);
		
		IConditionBuilder Where();
		ISqlBuilder Update(List<object> values);
		ISqlBuilder Delete();
		
		SQLiteCommand Build();
		ISqlBuilder CreateTable(List<DbType> types, List<string> constraints);
	}
}