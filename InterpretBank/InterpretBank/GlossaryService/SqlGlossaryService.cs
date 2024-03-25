using InterpretBank.GlossaryService.Interface;
using InterpretBank.GlossaryService.Model;
using InterpretBank.SqlBuilder.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace InterpretBank.GlossaryService
{
    public enum Tables
    {
        GlossaryData,
        GlossaryMetadata,
        TagLink,
        DatabaseInfo,
        TagList
    }

    public class SqlGlossaryService : IGlossaryService
    {
        private readonly IDatabaseConnection _connection;
        private readonly ISqlBuilder _sqlBuilder;

        public SqlGlossaryService(IDatabaseConnection connection, ISqlBuilder sqlBuilder)
        {
            _connection = connection;
            _sqlBuilder = sqlBuilder;

            LoadDb();
        }

        private Dictionary<Tables, (List<string>, List<string>)> Columns { get; set; } = new()
        {
            [Tables.DatabaseInfo] =
                (new()
                {
                    "ID",
                    "LanguageName1",
                    "LanguageName2",
                    "LanguageName3",
                    "LanguageName4",
                    "LanguageName5",
                    "LanguageName6",
                    "LanguageName7",
                    "LanguageName8",
                    "LanguageName9",
                    "LanguageName10",
                    "FieldTermLabel",
                    "FieldTermExtraALabel",
                    "FieldTermExtraBLabel",
                    "FieldConferenceInfoLabel",
                    "FieldConferenceGlossaryLabel",
                    "DatabaseVersion",
                    "DatabaseCreation",
                    "DatabaseUser",
                    "Activation"
                }, new()
                {
                    "PRIMARY KEY(\"ID\")"
                }),
            [Tables.GlossaryData] = (new()
            {
                "ID",
                "Tag1",
                "Tag2",
                "Term1",
                "Comment1a",
                "Comment1b",
                "Term2",
                "Comment2a",
                "Comment2b",
                "Term3",
                "Comment3a",
                "Comment3b",
                "Term4",
                "Comment4a",
                "Comment4b",
                "Term5",
                "Comment5a",
                "Comment5b",
                "Term6",
                "Comment6a",
                "Comment6b",
                "Term7",
                "Comment7a",
                "Comment7b",
                "Term8",
                "Comment8a",
                "Comment8b",
                "Term9",
                "Comment9a",
                "Comment9b",
                "Term10",
                "Comment10a",
                "Comment10b",
                "CommentAll",
                "RecordCreation",
                "RecordEdit",
                "RecordValidation",
                "RecordCreator",
                "RecordEditor",
                "Memorization",
                "Term1index",
                "Term2index",
                "Term3index",
                "Term4index",
                "Term5index",
                "Term6index",
                "Term7index",
                "Term8index",
                "Term9index",
                "Term10index",
                "TermFullindex",
                "Tags"
            }, new()
            {
                "PRIMARY KEY(\"ID\")"
            }),
            [Tables.GlossaryMetadata] = (new()
            {
                "ID",
                "Tag1",
                "Tag2",
                "GlossarySetting",
                "GlossaryDescription",
                "GlossaryDataCreation",
                "GlossaryCreator"
            }, new()
            {
                "PRIMARY KEY(\"ID\")"
            }),
            [Tables.TagLink] = (new()
            {
                "TagID",
                "TagName",
                "GlossaryID"
            }, new()
            {
                "FOREIGN KEY (GlossaryID) REFERENCES GlossaryMetadata(ID)",
                "PRIMARY KEY(TagID)",
            }),
            [Tables.TagList] = (new()
            {
                "TagID",
                "TagName"
            }, new()
            {
                "PRIMARY KEY(\"TagID\")"
            })
        };

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

        public void CreateDb(string filePath)
        {
            _connection.CreateDatabaseFile(filePath);
            _connection.LoadDatabase(filePath);

            foreach (Tables table in Enum.GetValues(typeof(Tables)))
            {
                var columnNames = Columns[table];
                var types = new List<DbType>();
                columnNames.Item1.ForEach(cn => { types.Add(cn.Contains("ID") ? DbType.Int64 : DbType.String); });
                var createTablesCommand = _sqlBuilder
                    .Table(table)
                    .Columns(columnNames.Item1)
                    .CreateTable(types, columnNames.Item2)
                    .Build();

                _connection.ExecuteCommand(createTablesCommand);
            }
        }

        public void DeleteGlossary(int glossaryId)
        {
            var glossaryDeleteCondition = $"ID = {glossaryId}";

            var tagsStatement = _sqlBuilder
                .Table(Tables.GlossaryMetadata)
                .Columns(new() { "Tag1", "Tag2" })
                .Where(glossaryDeleteCondition)
                .Build();

            var tags = _connection.ExecuteCommand(tagsStatement);

            var deleteGlossaryStatement = _sqlBuilder
                .Table(Tables.GlossaryMetadata)
                .Delete()
                .Where(glossaryDeleteCondition)
                .Build();

            if (tags.Any())
            {
                var tag2Condition = !string.IsNullOrWhiteSpace(tags[0]["Tag2"]) ? $" AND Tag2 = {tags[0]["Tag2"]}" : null;
                var termsDeleteCondition = $"Tag1 = {tags[0]["Tag1"]}{tag2Condition}";
                var deleteGlossaryTerms = _sqlBuilder
                    .Table(Tables.GlossaryData)
                    .Where(termsDeleteCondition)
                    .Delete()
                    .Build();
                _connection.ExecuteCommand(deleteGlossaryTerms);
            }

            _connection.ExecuteCommand(deleteGlossaryStatement);
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

        public void Dispose() => _connection?.Dispose();

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

        public void LoadDb(string filePath = null)
        {
            if (filePath is not null) _connection.LoadDatabase(filePath);
            if (_connection.IsSet) SetLanguageIndices();
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