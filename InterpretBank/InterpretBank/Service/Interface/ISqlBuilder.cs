using System.Collections.Generic;

namespace InterpretBank.Service.Interface
{
	public interface ISqlBuilder
	{
		string Build();

		ISqlBuilder Columns(List<string> columnNames);
		ISqlBuilder InnerJoin<T>(T tableName, string firstId, string secondId);
		ISqlBuilder Insert(List<string> values);

		ISqlBuilder Table<T>(T tableName);

		ISqlBuilder Where(string condition);
		ISqlBuilder Update(List<string> values);
	}
}