using System.Collections.Generic;
using System.Linq;
using InterpretBank.Model;
using InterpretBank.Model.Interface;
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
			var columns = termEntry.GetColumns();
			columns.Add("RecordCreation");

			var columnValues = termEntry.GetValues();
			columnValues.Add("CURRENT_DATE");

			var insertSqlStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Columns(columns)
				.Insert(columnValues)
				.Build();

			_connection.ExecuteCommand(insertSqlStatement);
		}

		public void CreateGlossary(List<int> languageIndices, List<string> tags, string note)
		{
			//_sqlBuilder
			//	.Table(Tables.GlossaryMetadata)
			//	.Columns()
			//	.Build()
		}

		public void DeleteTerm(string termId)
		{
			var deleteCondition = _conditionBuilder
				.Equals(termId, "ID")
				.Build();
			var deleteSqlStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Where(deleteCondition)
				.Build();

			_connection.ExecuteCommand(deleteSqlStatement);
		}

		public List<IGlossaryEntry> GetGlossaries()
		{
			var sqlStatement = _sqlBuilder
				.Table(Tables.GlossaryMetadata)
				.Build();

			var rows = _connection.ExecuteCommand(sqlStatement);
			var glossaries = ReadEntries<GlossaryMetadataEntry>(rows);

			return glossaries;
		}

		public List<IGlossaryEntry> GetTerms(string searchString, List<int> languages, List<string> glossaryNames, List<string> tags)
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
				.Like(searchString, TermEntry.GetTermColumns(languageIndices, false))
				.Build();

			var sql = _sqlBuilder
				.Columns(TermEntry.GetTermColumns(languageIndices, isRead: true))
				.Table(Tables.GlossaryData)
				.Where(glossaryNamesSelect)
				.Build();

			var rows = _connection.ExecuteCommand(sql);
			var termList = ReadEntries<TermEntry>(rows);

			return termList;
		}

		public void MergeGlossaries(string firstGlossary, string secondGlossary, string subGlossary = null)
		{
			var mergeStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Columns(new List<string> { "Tag1", "Tag2" })
				.Update(new List<string> { firstGlossary, subGlossary })
				.Where($@"Tag1 = ""{secondGlossary}""")
				.Build();

			_connection.ExecuteCommand(mergeStatement);
		}

		public void UpdateTermContent(TermEntry termEntry)
		{
			var updateCondition = _conditionBuilder
				.Equals(termEntry.ID, "ID")
				.Build();
			var updateColumns = TermEntry.GetTermColumns(LanguageIndicesDictionary.Values.ToList());
			var sqlUpdateStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Columns(updateColumns)
				.Update(termEntry.GetValues())
				.Where(updateCondition)
				.Build();

			_connection.ExecuteCommand(sqlUpdateStatement);
		}

		private List<IGlossaryEntry> ReadEntries<T>(List<Dictionary<string, string>> rows)
			where T : IGlossaryEntry, new()
		{
			if (rows is null || rows.Count == 0) return null;
			//var glossaryList = _entryFactory.CreateEntry<T>(rows);

			var glossaryList = new List<IGlossaryEntry>();
			foreach (var row in rows)
			{
				var glossaryEntry = new T();

				foreach (var rowKey in row.Keys)
				{
					glossaryEntry[rowKey] = row[rowKey];
				}

				glossaryList.Add(glossaryEntry);
			}

			return glossaryList;
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