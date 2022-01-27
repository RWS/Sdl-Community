using System.Collections.Generic;
using InterpretBank.Service.Model;

namespace InterpretBank.Service.Interface
{
	public interface ISqlBuilder
	{
		string Build();
		ISqlBuilder Select(List<string> columnNames);
		ISqlBuilder Where(string condition);
		ISqlBuilder From(string tableName);
	}
}