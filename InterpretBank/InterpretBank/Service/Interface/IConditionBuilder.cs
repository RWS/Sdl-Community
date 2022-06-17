﻿using System.Collections.Generic;
using System.Data.SQLite;

namespace InterpretBank.Service.Interface
{
	public interface IConditionBuilder
	{
		ISqlBuilder EndCondition();

		IConditionBuilder Equals(object value, string columnName, string @operator = "AND");

		IConditionBuilder In(string columnName, List<object> values, string @operator = "AND");

		IConditionBuilder In(string columnName, SQLiteCommand sqlSelect, string @operator = "AND");

		IConditionBuilder Like(string likeStrings, List<string> languageIndices, string @operator = "AND");
	}
}