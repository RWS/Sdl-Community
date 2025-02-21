using System;
using System.Collections.Generic;
using System.Data;
using Sdl.ProjectAutomation.Core;
using System.Data.SQLite;
using System.IO;
using NLog;
using Sdl.Community.IATETerminologyProvider.Model;


namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class SqliteDatabaseProvider
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly PathInfo _pathInfo;
		private SQLiteConnection _connection;
		private IProject _project;

		public SqliteDatabaseProvider(PathInfo pathInfo)
		{
			_pathInfo = pathInfo;
		}

		public void Connect(IProject project)
		{
			if (project == null)
			{
				return;
			}

			try
			{
				var projectInfo = project.GetProjectInfo();
				var dbFilePath = Path.Combine(_pathInfo.DbaseCacheFullPath, $"{projectInfo.Name}.sqlite");

				if (File.Exists(dbFilePath))
				{
					if (_project?.GetProjectInfo()?.Id == projectInfo.Id)
					{
						if (_connection.State == ConnectionState.Closed)
						{
							_connection.Open();
						}
					}
					else
					{
						OpenExistingDatabase(project, dbFilePath);
					}
				}
				else
				{
					CreateNewDatabase(project, dbFilePath);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}
		}

		public List<SearchCache> Get()
		{
			var searchCaches = new List<SearchCache>();

			var sql = "SELECT * FROM SearchCache";
			var cmdQuery = new SQLiteCommand(sql, _connection);

			var rdrSelect = cmdQuery.ExecuteReader();
			try
			{
				if (rdrSelect.HasRows)
				{
					while (rdrSelect.Read())
					{
						var searchCache =
							new SearchCache
							{
								SourceText = rdrSelect["SourceText"].ToString(),
								TargetLanguage = rdrSelect["TargetLanguage"].ToString(),
								QueryString = rdrSelect["QueryString"].ToString(),
								SearchResultsString = rdrSelect["SearchResultsString"].ToString()
							};

						searchCaches.Add(searchCache);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
				throw;
			}
			finally
			{
				if (rdrSelect != null)
				{
					if (!rdrSelect.IsClosed)
					{
						rdrSelect.Close();
					}
				}
			}

			return searchCaches;
		}

		public SearchCache Get(string sourceText, string targetLanguage, string queryString)
		{
			if (_connection == null)
			{
				return null;
			}
			
			var sql = "SELECT * FROM SearchCache";
			sql += " WHERE";
			sql += " SourceText = @SourceText";
			sql += " AND TargetLanguage = @TargetLanguage";
			sql += " AND QueryString = @QueryString";

			var cmdQuery = new SQLiteCommand(sql, _connection);
			cmdQuery.Parameters.Add(new SQLiteParameter("@SourceText", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@TargetLanguage", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@QueryString", DbType.String));

			cmdQuery.Parameters["@SourceText"].Value = sourceText;
			cmdQuery.Parameters["@TargetLanguage"].Value = targetLanguage;
			cmdQuery.Parameters["@QueryString"].Value = queryString;

			var rdrSelect = cmdQuery.ExecuteReader();
			try
			{
				if (rdrSelect.HasRows)
				{
					rdrSelect.Read();

					var searchCache =
						new SearchCache
						{
							Id = Convert.ToInt32(rdrSelect["Id"]),
							SourceText = sourceText,
							TargetLanguage = targetLanguage,
							QueryString = queryString,
							SearchResultsString = rdrSelect["SearchResultsString"].ToString()
						};

					return searchCache;
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
				throw;
			}
			finally
			{
				if (rdrSelect != null)
				{
					if (!rdrSelect.IsClosed)
					{
						rdrSelect.Close();
					}
				}
			}

			return null;
		}

		public int Insert(SearchCache searchCache)
		{
			if (_connection == null)
			{
				return -1;
			}

			var sql = "INSERT INTO SearchCache";
			sql += " (";
			sql += " SourceText";
			sql += ", TargetLanguage";
			sql += ", QueryString";
			sql += ", SearchResultsString";
			sql += " ) VALUES (";
			sql += " @SourceText";
			sql += ", @TargetLanguage";
			sql += ", @QueryString";
			sql += ", @SearchResultsString";
			sql += " ); SELECT last_insert_rowid();";

			var cmdQuery = new SQLiteCommand(sql, _connection);
			cmdQuery.Parameters.Add(new SQLiteParameter("@SourceText", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@TargetLanguage", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@QueryString", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@SearchResultsString", DbType.String));

			cmdQuery.Parameters["@SourceText"].Value = searchCache.SourceText;
			cmdQuery.Parameters["@TargetLanguage"].Value = searchCache.TargetLanguage;
			cmdQuery.Parameters["@QueryString"].Value = searchCache.QueryString;
			cmdQuery.Parameters["@SearchResultsString"].Value = searchCache.SearchResultsString;

			var id = Convert.ToInt32(cmdQuery.ExecuteScalar());
			return id;
		}

		public void Update(SearchCache searchCache)
		{
			if (_connection == null)
			{
				return;
			}

			var sql = "UPDATE SearchCache";
			sql += " SET";
			sql += " SearchResultsString = @SearchResultsString";
			sql += " WHERE";
			sql += " SourceText = @SourceText";
			sql += " AND TargetLanguage = @TargetLanguage";
			sql += " AND QueryString = @QueryString";

			var cmdQuery = new SQLiteCommand(sql, _connection);
			cmdQuery.Parameters.Add(new SQLiteParameter("@SourceText", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@TargetLanguage", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@QueryString", DbType.String));
			cmdQuery.Parameters.Add(new SQLiteParameter("@SearchResultsString", DbType.String));

			cmdQuery.Parameters["@SourceText"].Value = searchCache.SourceText;
			cmdQuery.Parameters["@TargetLanguage"].Value = searchCache.TargetLanguage;
			cmdQuery.Parameters["@QueryString"].Value = searchCache.QueryString;
			cmdQuery.Parameters["@SearchResultsString"].Value = searchCache.SearchResultsString;

			cmdQuery.ExecuteNonQuery();
		}

		public void RemoveAll()
		{
			if (_connection == null)
			{
				return;
			}
			
			var sql = "DELETE FROM SearchCache";
			var cmdQuery = new SQLiteCommand(sql, _connection);
			cmdQuery.ExecuteNonQuery();
		}

		public bool IsConnected()
		{
			return _connection?.State == ConnectionState.Open;
		}

		public void CloseConnection()
		{
			if (_connection != null)
			{
				try
				{
					if (_connection.State != ConnectionState.Closed)
					{
						_connection.Close();
					}

					_connection.Dispose();
				}
				catch (Exception ex)
				{
					_logger.Error($"{ex.Message}\n{ex.StackTrace}");
				}
			}
		}

		private void CreateNewDatabase(IProject project, string dbFilePath)
		{
			CloseConnection();

			SQLiteConnection.CreateFile(dbFilePath);

			_connection = new SQLiteConnection($"Data Source='{dbFilePath}';Cache=Shared");
			_connection.Open();

			_project = project;

			CreateTable();
			CreateIndex();
		}

		private void OpenExistingDatabase(IProject project, string dbFilePath)
		{
			CloseConnection();
			
			_connection = new SQLiteConnection($"Data Source='{dbFilePath}';Cache=Shared");
			_connection.Open();

			_project = project;
		}

		private void CreateTable()
		{
			var sql = "CREATE TABLE SearchCache("
			          + "Id INTEGER PRIMARY KEY AUTOINCREMENT,"
			          + "SourceText TEXT,"
			          + "TargetLanguage TEXT,"
			          + "QueryString TEXT,"
			          + "SearchResultsString TEXT)";

			var command = new SQLiteCommand(sql, _connection);
			command.ExecuteNonQuery();
		}

		private void CreateIndex()
		{
			var sql = "CREATE INDEX idx_source_language_query"
			          + " ON SearchCache(SourceText, TargetLanguage, QueryString)";

			var command = new SQLiteCommand(sql, _connection);
			command.ExecuteNonQuery();
		}
	}
}
