using System.Collections.Generic;
using System.Data.SQLite;
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
		DatabaseInfo
	}

	public class GlossaryService : IGlossaryService
	{
		private readonly IDatabaseConnection _connection;
		private readonly ISqlBuilder _sqlBuilder;

		public GlossaryService(IDatabaseConnection connection, ISqlBuilder sqlBuilder)
		{
			_connection = connection;
			_sqlBuilder = sqlBuilder;
			SetLanguageIndices();
		}

		private Dictionary<string, int> LanguageIndicesDictionary { get; } = new();

		public void Create(IGlossaryEntry entry)
		{
			var columns = entry.GetColumns();
			var columnValues = entry.GetValues();

			var table = entry is TermEntry ? Tables.GlossaryData : Tables.GlossaryMetadata;
			var insertSqlStatement = _sqlBuilder
				.Table(table)
				.Columns(columns)
				.Insert(columnValues)
				.Build();

			_connection.ExecuteCommand(insertSqlStatement);
		}

		public void DeleteTerm(string termId)
		{
			var deleteSqlStatement = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Where()
					.Equals(termId, "ID")
					.EndCondition()
				.Delete()
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

			var joinStatement = new SQLiteCommand();
			if (tags is not null && tags.Count > 0)
			{
				joinStatement = _sqlBuilder
					.Columns(new List<string> { "Tag1" })
					.Table(Tables.GlossaryMetadata)
					.InnerJoin(Tables.TagLink, "GlossaryID", "ID")
					.Where()
						.In("TagName", tags.Cast<object>().ToList())
						.EndCondition()
					.Build();
			}

			var sql = _sqlBuilder
				.Columns(TermEntry.GetColumns(languageIndices, isRead: true))
				.Table(Tables.GlossaryData)
				.Where()
					.In("Tag1", joinStatement)
					.In("Tag1", glossaryNames?.Cast<object>().ToList())
					.Like(searchString, TermEntry.GetColumns(languageIndices, false))
					.EndCondition()
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
				.Update(new List<object> { firstGlossary, subGlossary })
				.Where($@"Tag1 = ""{secondGlossary}""")
				.Build();

			_connection.ExecuteCommand(mergeStatement);
		}

		public void UpdateContent(IGlossaryEntry entry)
		{
			var updateColumns = entry.GetColumns();
			var columnValues = entry.GetValues();

			var table = entry is TermEntry ? Tables.GlossaryData : Tables.GlossaryMetadata;
			var sqlUpdateStatement = _sqlBuilder
				.Table(table)
				.Columns(updateColumns)
				.Update(columnValues)
				.Where()
					.Equals(entry.ID, "ID")
					.EndCondition()
				.Build();

			_connection.ExecuteCommand(sqlUpdateStatement);
		}

		public void DeleteGlossary(string glossaryId)
		{
			var glossaryDeleteCondition = $"ID = {glossaryId}";

			var tagsStatement = _sqlBuilder
				.Table(Tables.GlossaryMetadata)
				.Columns(new() {"Tag1", "Tag2"})
				.Where(glossaryDeleteCondition)
				.Build();

			var tags = _connection.ExecuteCommand(tagsStatement);

			var deleteGlossaryStatement = _sqlBuilder
				.Table(Tables.GlossaryMetadata)
				.Delete()
				.Where(glossaryDeleteCondition)
				.Build();

			var tag2Condition = !string.IsNullOrWhiteSpace(tags[0]["Tag2"]) ? $" AND Tag2 = {tags[0]["Tag2"]}" : null;

			var termsDeleteCondition = $"Tag1 = {tags[0]["Tag1"]}{tag2Condition}";

			var deleteGlossaryTerms = _sqlBuilder
				.Table(Tables.GlossaryData)
				.Where(termsDeleteCondition)
				.Delete()
				.Build();

			_connection.ExecuteCommand(deleteGlossaryStatement);
			_connection.ExecuteCommand(deleteGlossaryTerms);
		}

		private List<IGlossaryEntry> ReadEntries<T>(List<Dictionary<string, string>> rows)
			where T : IGlossaryEntry, new()
		{
			if (rows is null || rows.Count == 0) return null;

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
			var sql = _sqlBuilder
				.Table(Tables.DatabaseInfo)
				.Build();

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