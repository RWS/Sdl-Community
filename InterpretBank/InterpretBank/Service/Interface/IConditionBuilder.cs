using System.Collections.Generic;

namespace InterpretBank.Service.Interface
{
	public interface IConditionBuilder
	{
		ISqlBuilder EndCondition();
		IConditionBuilder Equals(List<string> values, string columnName, string @operator = "AND");

		IConditionBuilder Equals(string value, string columnName, string @operator = "AND");

		IConditionBuilder In(string columnName, List<string> glossaryNames, string @operator = "AND");

		IConditionBuilder In(string columnName, string sqlSelect, string @operator = "AND");

		bool IsEmpty();

		IConditionBuilder Like(string likeStrings, List<string> languageIndices, string @operator = "AND");
	}
}