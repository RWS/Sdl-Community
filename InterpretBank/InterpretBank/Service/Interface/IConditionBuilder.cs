using System.Collections.Generic;

namespace InterpretBank.Service.Interface
{
	public interface IConditionBuilder
	{
		string Build();

		IConditionBuilder In(string columnName, List<string> glossaryNames, string @operator);

		IConditionBuilder In(string columnName, string sqlSelect, string @operator);

		IConditionBuilder Like(string likeStrings, List<int> languageIndices, string @operator);
	}
}