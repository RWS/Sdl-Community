using System.Collections.Generic;
using System.Linq;
using InterpretBank.Model;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service
{
	public enum Tables
	{
		GlossaryData,
		GlossaryMetadata,
		TagLink,
	}

	public class GlossaryService : IGlossaryService
	{
		private readonly IConditionBuilder _conditionBuilder;
		private readonly IDatabaseConnection _connection;
		private readonly ISqlBuilder _sqlBuilder;

		public GlossaryService(IDatabaseConnection connection, ISqlBuilder sqlBuilder, IConditionBuilder conditionBuilder)
		{
			_connection = connection;

			_sqlBuilder = sqlBuilder;
			_conditionBuilder = conditionBuilder;

			SetLanguageIndices();
		}

		private Dictionary<string, int> LanguageIndicesDictionary { get; } = new();

		public void AddTerm(TermEntry termEntry)
		{
			var columns = GetTermColumns(termEntry);
			columns.Add("RecordCreation");

			var columnValues = GetTermValues(termEntry);
			columnValues.Add("CURRENT_DATE");

			var insertSqlStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Columns(columns)
				.Insert(columnValues)
				.Build();

			_connection.ExecuteCommand(insertSqlStatement);
		}

		public List<TermEntry> GetTerms(string searchString, List<int> languages, List<string> glossaryNames, List<string> tags)
		{
			var languageIndices = languages is null || languages.Count < 1
				? LanguageIndicesDictionary.Values.ToList()
				: languages;

			if (tags is not null && tags.Count > 0)
			{
				var tagCondition = _conditionBuilder
					.In("TagName", tags)
					.Build();
				var joinStatement = _sqlBuilder
					.Columns(new List<string> { "Tag1" })
					.Table(Tables.GlossaryMetadata)
					.InnerJoin(Tables.TagLink, "GlossaryID", "ID")
					.Where(tagCondition)
					.Build();
				_conditionBuilder
					.In("Tag1", joinStatement);
			}

			var glossaryNamesSelect = _conditionBuilder
				.In("Tag1", glossaryNames)
				.Like(searchString, GetTermColumns(languageIndices, false))
				.Build();

			var sql = _sqlBuilder
				.Columns(GetTermColumns(languageIndices))
				.Table(Tables.GlossaryData)
				.Where(glossaryNamesSelect)
				.Build();

			var rows = _connection.ExecuteCommand(sql);
			var termList = ReadTermEntries(rows, languageIndices);

			return termList;
		}

		public void UpdateTerm(TermEntry termEntry)
		{
			var updateCondition = _conditionBuilder
				.Equals(termEntry.Id, "ID")
				.Build();
			var sqlUpdateStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Columns(GetTermColumns(LanguageIndicesDictionary.Values.ToList()))
				.Update(GetTermValues(termEntry))
				.Where(updateCondition)
				.Build();

			_connection.ExecuteCommand(sqlUpdateStatement);
		}

		private static List<string> GetTermColumns(List<int> languageIndices, bool withLocation = true)
		{
			var columns = new List<string> { "CommentAll" };

			if (withLocation)
			{
				columns.Insert(0, "Tag2");
				columns.Insert(0, "Tag1");
				columns.Insert(0, "ID");
			}

			foreach (var le in languageIndices)
			{
				columns.AddRange(LanguageEquivalent.GetColumns(le));
			}

			return columns;
		}

		private static List<TermEntry> ReadTermEntries(List<Dictionary<string, string>> rows, List<int> languageIndices)
		{
			var termList = new List<TermEntry>();

			if (rows is null || rows.Count == 0) return termList;

			foreach (var row in rows)
			{
				var term = new TermEntry
				{
					Id = row["ID"],
					Tag1 = row["Tag1"],
					Tag2 = row["Tag2"],
					CommentAll = row["CommentAll"]
				};

				foreach (var index in languageIndices)
					term.Add(new LanguageEquivalent
					{
						LanguageIndex = index,
						Term = row[$"Term{index}"],
						CommentA = row[$"Comment{index}a"],
						CommentB = row[$"Comment{index}b"]
					});

				termList.Add(term);
			}

			return termList;
		}

		private List<string> GetTermColumns(TermEntry term)
		{
			var columns = new List<string> { "Tag1", "Tag2", "CommentAll" };
			foreach (var le in term.LanguageEquivalents)
			{
				columns.AddRange(le.GetColumns());
			}

			return columns;
		}

		private List<string> GetTermValues(TermEntry term)
		{
			var values = new List<string> { term.Id, term.Tag1, term.Tag2, term.CommentAll };
			foreach (var le in term.LanguageEquivalents)
			{
				values.AddRange(le.GetValues());
			}

			return values;
		}

		private void SetLanguageIndices()
		{
			var sql = "SELECT * FROM DatabaseInfo";
			var rows = _connection.ExecuteCommand(sql);

			if (rows is null || rows.Count == 0) return;

			foreach (var row in rows)
			{
				for (var i = 1; i <= 10; i++)
				{
					var current = row[$"LanguageName{i}"];
					if (current == "undef") continue;
					LanguageIndicesDictionary[current] = i;
				}
			}
		}
	}
}