using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using InterpretBank.Model;
using InterpretBank.Service.Interface;
using InterpretBank.Service.Model;

namespace InterpretBank.Service
{
	public class GlossaryService : IGlossaryService
	{
		private readonly SQLiteConnection _connection;
		public Dictionary<string, int> LanguageIndicesDictionary { get; } = new();

		public static List<string> DefaultColumnNamesInGlossaryData = new()
		{
			"ID",
			"Tag1",
			"Tag2",
			"CommentAll",
		};

		public GlossaryService(string filepath)
		{
			_connection = new SQLiteConnection($"Data Source='{filepath}';Cache=Shared");

			LoadLanguageIndices();
		}

		private void LoadLanguageIndices()
		{
			_connection.Open();

			var sql = "SELECT * FROM DatabaseInfo";
			var cmdQuery = new SQLiteCommand(sql, _connection);
			var rdrSelect = cmdQuery.ExecuteReader();

			rdrSelect.Read();
			for (var i = 1; i <= 10; i++)
			{
				var current = rdrSelect[$"LanguageName{i}"];
				if (current.ToString() == "undef") continue;
				LanguageIndicesDictionary[current.ToString()] = i;
			}

			_connection.Close();
		}

		public List<Term> GetTerms()
		{
			return GetTerms(null, null, null, null);
		}

		public List<Term> GetTerms(string searchString, List<string> languages, List<string> glossaryNames, List<string> tags)
		{
			_connection.Open();

			var languageIndices = languages is null || languages.Count < 1
				? LanguageIndicesDictionary.Values.ToList()
				: languages.Select(language => LanguageIndicesDictionary[language]).ToList();

			var sqlBuilder = new SqlBuilder();
			var conditionBuilder = new ConditionBuilder();

			var condition = conditionBuilder
				.Like(searchString, languageIndices)
				.Build();

			//TODO: Create separate method for getting the dynamically named rows
			var sql = sqlBuilder
				.Select(GetColumnNames(languageIndices))
				.From("GlossaryData")
				.Where(condition)
				.Build();

			var cmdQuery = new SQLiteCommand(sql, _connection);
			var rdrSelect = cmdQuery.ExecuteReader();

			var termList = new List<Term>();
			while (rdrSelect.Read())
			{
				var languageEquivalents = new List<LanguageEquivalent>();
				foreach (var index in languageIndices)
					languageEquivalents.Add(new LanguageEquivalent
					{
						Text = rdrSelect[$"Term{index}"].ToString(),
						CommentA = rdrSelect[$"Comment{index}a"].ToString(),
						CommentB = rdrSelect[$"Comment{index}b"].ToString()
					});

				var term = new Term
				{
					Tag1 = rdrSelect["Tag1"].ToString(),
					Tag2 = rdrSelect["Tag2"].ToString(),
					CommentAll = rdrSelect["CommentAll"].ToString(),
					LanguageEquivalents = languageEquivalents
				};

				termList.Add(term);
			}

			_connection.Close();

			return termList;
		}

		private static List<string> GetColumnNames(List<int> languageIndices)
		{
			var indexedColumns = new List<string>(languageIndices.Count * 3);
			foreach (var index in languageIndices)
			{
				indexedColumns.Add($"Term{index}");
				indexedColumns.Add($"Comment{index}a");
				indexedColumns.Add($"Comment{index}b");
			}
			var otherColumns = new List<string> { "ID", "Tag1", "Tag2", "CommentAll" };
			var columnNames = indexedColumns.Concat(otherColumns).ToList();
			return columnNames;
		}
	}

	public enum Tables
	{
		GlossaryData,
	}
}