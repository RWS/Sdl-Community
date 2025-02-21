using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.GlossaryService.Model;
using InterpretBank.TerminologyService;
using NSubstitute;
using Xunit;
using DbTerm = InterpretBank.GlossaryService.DAL.DbGlossaryEntry;

namespace InterpretBankTests
{
	public class TerminologyServiceTests
	{
		private readonly GlossaryServiceBuilder _glossaryServiceBuilder;

		public TerminologyServiceTests()
		{
			_glossaryServiceBuilder = new GlossaryServiceBuilder();
		}


		[Fact]
		public void GetTerms_Test()
		{
			var filePath = "C:\\Code\\RWS Community\\InterpretBank\\InterpretBankTests\\Resources\\InterpretBankDatabaseV6.db";

			var interpretBankDataContext = new InterpretBankDataContext();
			interpretBankDataContext.Setup(filePath);

			var sut = new TerminologyService(interpretBankDataContext);

			//sut.GetFuzzyTerms("english", "english", "italian");
		}


		[Fact]
		public void CreateDatabase_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.CreateDb(@"C:\Things\Work\Test");

			var createGlossaryDataCommand =
					"CREATE TABLE GlossaryData (ID Int64, Tag1 String, Tag2 String, Term1 String, Comment1a String, Comment1b String, Term2 String, Comment2a String, Comment2b String, Term3 String, Comment3a String, Comment3b String, Term4 String, Comment4a String, Comment4b String, Term5 String, Comment5a String, Comment5b String, Term6 String, Comment6a String, Comment6b String, Term7 String, Comment7a String, Comment7b String, Term8 String, Comment8a String, Comment8b String, Term9 String, Comment9a String, Comment9b String, Term10 String, Comment10a String, Comment10b String, CommentAll String, RecordCreation String, RecordEdit String, RecordValidation String, RecordCreator String, RecordEditor String, Memorization String, Term1index String, Term2index String, Term3index String, Term4index String, Term5index String, Term6index String, Term7index String, Term8index String, Term9index String, Term10index String, TermFullindex String, Tags String, PRIMARY KEY(\"ID\"))";
			var createGlossaryMetadataCommand =
				"CREATE TABLE GlossaryMetadata (ID Int64, Tag1 String, Tag2 String, GlossarySetting String, GlossaryDescription String, GlossaryDataCreation String, GlossaryCreator String, PRIMARY KEY(\"ID\"))";
			var createTagLinkCommand =
				"CREATE TABLE TagLink (TagID Int64, TagName String, GlossaryID Int64, FOREIGN KEY (GlossaryID) REFERENCES GlossaryMetadata(ID), PRIMARY KEY(TagID))";
			var createDatabaseInfoCommand =
				"CREATE TABLE DatabaseInfo (ID Int64, LanguageName1 String, LanguageName2 String, LanguageName3 String, LanguageName4 String, LanguageName5 String, LanguageName6 String, LanguageName7 String, LanguageName8 String, LanguageName9 String, LanguageName10 String, FieldTermLabel String, FieldTermExtraALabel String, FieldTermExtraBLabel String, FieldConferenceInfoLabel String, FieldConferenceGlossaryLabel String, DatabaseVersion String, DatabaseCreation String, DatabaseUser String, Activation String, PRIMARY KEY(\"ID\"))";
			var createTagListCommand =
				"CREATE TABLE TagList (TagID Int64, TagName String, PRIMARY KEY(\"TagID\"))";

			connectionMock
				.ReceivedWithAnyArgs()
				.CreateDatabaseFile(default);
			connectionMock
				.ReceivedWithAnyArgs()
				.LoadDatabase(default);

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == createGlossaryDataCommand));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == createGlossaryMetadataCommand));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == createTagLinkCommand));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == createDatabaseInfoCommand));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == createTagListCommand));
		}

		[Fact]
		public void CreateGlossary_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.Create(new GlossaryMetadataEntry());

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText ==
															@"INSERT INTO GlossaryMetadata (GlossaryCreator, GlossaryDataCreation, GlossaryDescription, GlossarySetting, Tag1, Tag2) VALUES (""@0"", ""@1"", ""@2"", ""@3"", ""@4"", ""@5"")"));
		}

		[Fact]
		public void CreateTerm_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.Create(new TermEntry());

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText ==
					@"INSERT INTO GlossaryData (CommentAll, RecordCreation, Tag1, Tag2) VALUES (""@0"", ""@1"", ""@2"", ""@3"")"));
		}

		[Fact]
		public void DeleteGlossary_Test()
		{
			var connectionMock = GetConnectionMock();

			connectionMock
				.ExecuteSelectCommand(default)
				.ReturnsForAnyArgs
				(
					null,
					new List<Dictionary<string, string>>
					{
						new() {["Tag1"] = "TestGlossary", ["Tag2"] = "TestSubGlossary"}
					},
					null,
					null
				);

			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.DeleteGlossary(5);

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s =>
					s.CommandText == "SELECT Tag1, Tag2 FROM GlossaryMetadata WHERE ID = 5"));

			connectionMock
				.Received()
				.ExecuteSelectCommand(
					Arg.Is<SQLiteCommand>(s =>
						s.CommandText ==
						@"DELETE FROM GlossaryData WHERE Tag1 = TestGlossary AND Tag2 = TestSubGlossary"));
		}

		[Fact]
		public void DeleteTerm_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.DeleteTerm("5");

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == @"DELETE FROM GlossaryData WHERE (ID = @0)"));
		}

		[Fact]
		public void GetGlossaries_RealDB_Test()
		{
			var filepath = @"../../Resources/InterpretBankDatabaseV6.db";
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(new DatabaseConnection(filepath))
				.Build();

			var termList = glossaryService.GetGlossaries();

			Assert.Equal(12, termList.Count);
			Assert.Equal(typeof(GlossaryMetadataEntry), termList[0].GetType());
		}

		[Fact]
		public void GetTerms_RealDB_Test()
		{
			var filepath = @"../../Resources/InterpretBankDatabaseV6.db";
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(new DatabaseConnection(filepath))
				.Build();

			var languages = new List<int> { 1, 2 };
			var termList = glossaryService.GetTerms(null, languages, null,
				new List<string> { "AnotherTagOne" });

			Assert.Single(termList);
			Assert.Equal(2, ((TermEntry)termList[0]).LanguageEquivalents.Count);
			Assert.Equal(typeof(TermEntry), termList[0].GetType());
		}

		//[Fact]
		//public void GetTerms_Test()
		//{
		//	//Arrange
		//	var connectionMock = GetConnectionMock();
		//	var glossaryService = _glossaryServiceBuilder
		//		.WithDatabaseConnection(connectionMock)
		//		.Build();
		//	var languages = new List<int> { 1, 2 };
		//	var expectedSqlStatement =
		//		"SELECT ID, Tag1, Tag2, CommentAll, Term1, Comment1a, Comment1b, Term2, Comment2a, Comment2b FROM GlossaryData WHERE Tag1 IN (SELECT Tag1 FROM GlossaryMetadata INNER JOIN TagLink ON GlossaryID=ID WHERE TagName IN (@0))";

		//	var sqlData = new List<Dictionary<string, string>>
		//	{
		//		new()
		//		{
		//			["ID"] = "2",
		//			["Tag1"] =  null,
		//			["Tag2"] = null,
		//			["Term1"] = null,
		//			["Term2"] = null,
		//			["CommentAll"] = null,
		//			["Comment1a"] = null,
		//			["Comment2a"] = null,
		//			["Comment1b"] = null,
		//			["Comment2b"] = null
		//		}
		//	};

		//	connectionMock
		//		.ExecuteCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == expectedSqlStatement))
		//		.Returns(sqlData);

		//	//Act
		//	var termList = glossaryService.GetTerms(null, languages, null,
		//		new List<string> { "AnotherTagOne" });

		//	//Assert
		//	connectionMock
		//		.Received()
		//		.ExecuteCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));

		//	connectionMock
		//		.Received()
		//		.ExecuteCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == expectedSqlStatement));

		//	Assert.Single(termList);
		//	Assert.Equal(typeof(TermEntry), termList[0].GetType());
		//}

		[Fact]
		public void MergeGlossaries_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.MergeGlossaries("Glossary", "ToBeMergedGlossary");

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));
			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s =>
					s.CommandText ==
					@"UPDATE GlossaryData SET Tag1 = ""@0"", Tag2 = ""@1"" WHERE Tag1 = ""ToBeMergedGlossary"""));
		}

		[Fact]
		public void UpdateGlossaryMetadata_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.UpdateContent(new GlossaryMetadataEntry
			{
				ID = 2,
				Tag1 = "Glossary"
			});

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s =>
					s.CommandText ==
					@"UPDATE GlossaryMetadata SET GlossaryCreator = ""@0"", GlossaryDataCreation = ""@1"", GlossaryDescription = ""@2"", GlossarySetting = ""@3"", Tag1 = ""@4"", Tag2 = ""@5"" WHERE (ID = @6)"));
		}

		[Fact]
		public void UpdateTermContent_Test()
		{
			var connectionMock = GetConnectionMock();
			var glossaryService = _glossaryServiceBuilder
				.WithDatabaseConnection(connectionMock)
				.Build();

			glossaryService.UpdateContent(new TermEntry
			{
				ID = 2,
				Tag1 = "Glossary"
			});

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s => s.CommandText == "SELECT * FROM DatabaseInfo"));

			connectionMock
				.Received()
				.ExecuteSelectCommand(Arg.Is<SQLiteCommand>(s =>
					s.CommandText ==
					@"UPDATE GlossaryData SET CommentAll = ""@0"", RecordCreation = ""@1"", Tag1 = ""@2"", Tag2 = ""@3"" WHERE (ID = @4)"));
		}

		private static IDatabaseConnection GetConnectionMock()
		{
			var databaseConnection = Substitute.For<IDatabaseConnection>();
			databaseConnection.IsSet.Returns(true);
			return databaseConnection;
		}
	}
}