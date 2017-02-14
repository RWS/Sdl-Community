using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.DQF;
using Sdl.Community.Structures.iProperties;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.Structures.QualityMetrics;
using Sdl.Community.Structures.Rates;
using Sdl.Community.Structures.Rates.Base;
using DocumentActivities = Sdl.Community.Structures.Projects.Activities.DocumentActivities;
using LanguageRate = Sdl.Community.Structures.Rates.LanguageRate;
using QualityMetric = Sdl.Community.Structures.QualityMetrics.QualityMetric;

namespace Sdl.Community.TM.Database
{

    public class Query
    {
        public event ProgressChangedHandler ProgressChanged;
        public delegate void ProgressChangedHandler(int maximum, int current, string message);

        private static string GetConnectionString(string databasePath)
        {
            return "Data Source=\"" + databasePath + "\";Version=3;New=False;Compress=True";
        }


        #region  |  InitializeSettings  |



        public bool InitializeSettings(string databasePath
            , List<ViewProperty> viewSettings
            , List<GeneralProperty> backupSettings
            , List<GeneralProperty> generalSettings
            , List<GeneralProperty> trackerSettings)
        {
            var value = false;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    value = initializeSettings(connection, viewSettings, backupSettings, generalSettings, trackerSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private bool initializeSettings(SQLiteConnection connection
            , List<ViewProperty> viewSettings
            , List<GeneralProperty> backupSettings
            , IEnumerable<GeneralProperty> generalSettings
            , IEnumerable<GeneralProperty> trackerSettings)
        {
            initializeViewSettings(connection, "DocumentRecordsView", viewSettings);
            initializeBackupSettings(connection, backupSettings);
            initializeGeneralSettings(connection, generalSettings);
            initializeTrackerSettings(connection, trackerSettings);


            return true;
        }


        #endregion


        #region  |  View Settings  |
        /// <summary>
        /// Get the view settings;  used to manage whether items are visible in view or not!
        /// </summary>
        /// <param name="databasePath">the database local path</param>
        /// <param name="viewName">Optional - if you know the viewName then try to use it, otherwise it will return properties for all views managed here!</param>
        /// <returns>returns a list of ViewProperty</returns>
        public List<ViewProperty> GetViewSettings(string databasePath, string viewName = "")
        {
            List<ViewProperty> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getViewSettings(connection, viewName);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<ViewProperty> getViewSettings(SQLiteConnection connection, string viewName = "")
        {
            var values = new List<ViewProperty>();

            var sqlQuery = "SELECT * FROM ViewSettings";
            if (viewName.Trim() != string.Empty)
                sqlQuery += " WHERE view_name = @view_name";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            if (viewName.Trim() != string.Empty)
            {
                cmdQuery.Parameters.Add(new SQLiteParameter("@view_name", DbType.String));
                cmdQuery.Parameters["@view_name"].Value = viewName;
            }


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new ViewProperty
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            ViewName = rdrSelect["view_name"].ToString(),
                            Name = rdrSelect["name"].ToString(),
                            ValueType = rdrSelect["value_type"].ToString(),
                            Value = rdrSelect["value"].ToString(),
                            Text = rdrSelect["text"].ToString()
                        };

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ViewSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateViewSettingsProperty(string databasePath, string viewName, ViewProperty viewProperty)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createViewSettingsProperty(connection, viewName, viewProperty);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createViewSettingsProperty(SQLiteConnection connection, string viewName, ViewProperty viewProperty)
        {
            var value = -1;

            if (viewName.Trim() == string.Empty)
                throw new Exception("The vew name cannot be null!");

            var sqlQuery = "INSERT INTO ViewSettings";
            sqlQuery += " (";
            sqlQuery += " view_name";
            sqlQuery += ", name";
            sqlQuery += ", value_type";
            sqlQuery += ", value";
            sqlQuery += ", text";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @view_name";
            sqlQuery += ", @name";
            sqlQuery += ", @value_type";
            sqlQuery += ", @value";
            sqlQuery += ", @text";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@view_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));


            cmdQuery.Parameters["@view_name"].Value = viewName;
            cmdQuery.Parameters["@name"].Value = viewProperty.Name;
            cmdQuery.Parameters["@value_type"].Value = viewProperty.ValueType;
            cmdQuery.Parameters["@value"].Value = viewProperty.Value;
            cmdQuery.Parameters["@text"].Value = viewProperty.Text;

            viewProperty.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = viewProperty.Id;

            return value;
        }

        public bool UpdateViewSettingsProperty(string databasePath, string viewName, ViewProperty viewProperty)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = updateViewSettingsProperty(connection, viewName, viewProperty);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool updateViewSettingsProperty(SQLiteConnection connection, string viewName, ViewProperty viewProperty)
        {
            if (viewName.Trim() == string.Empty || viewProperty.Id <= -1)
                throw new Exception("The view name cannot be null!");

            var sqlQuery = "UPDATE ViewSettings";
            sqlQuery += " SET value = @value";
            sqlQuery += ", text = @text";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));

            cmdQuery.Parameters["@id"].Value = viewProperty.Id;
            cmdQuery.Parameters["@value"].Value = viewProperty.Value;
            cmdQuery.Parameters["@text"].Value = viewProperty.Text;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<long> SaveViewSettings(string databasePath, string viewName, List<ViewProperty> viewSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveViewSettings(connection, viewName, viewSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<long> saveViewSettings(SQLiteConnection connection, string viewName, List<ViewProperty> viewSettings)
        {
            var values = new List<long>();
            if (viewName.Trim() == string.Empty)
                throw new Exception("View Name cannot be null!");

            var existingViewProperties = getViewSettings(connection, viewName);

            foreach (var viewSetting in viewSettings)
            {
                if (viewSetting.Id == -1 || !existingViewProperties.Exists(a => { return a.Id == viewSetting.Id; }))
                {
                    values.Add(createViewSettingsProperty(connection, viewName, viewSetting));
                }
                else
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(viewSetting, existingViewProperties.Find(a => a.Id == viewSetting.Id)))
                        updateViewSettingsProperty(connection, viewName, viewSetting);
                    values.Add(viewSetting.Id);
                }
            }


            return values;
        }

        public List<long> InitializeViewSettings(string databasePath, string viewName, List<ViewProperty> viewSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = initializeViewSettings(connection, viewName, viewSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<long> initializeViewSettings(SQLiteConnection connection, string viewName, List<ViewProperty> viewSettings)
        {
            var values = new List<long>();
            if (viewName.Trim() == string.Empty)
                throw new Exception("View Name cannot be null!");

            var viewProperties = getViewSettings(connection, viewName);

            values.AddRange((
                from viewSetting in viewSettings
                where !viewProperties.Exists(a => a.Name == viewSetting.Name)
                select createViewSettingsProperty(connection, viewName, viewSetting)).Select(dummy => (long)dummy));


            return values;
        }

        public bool DeleteViewSettings(string databasePath, string viewName, string propertyName = "")
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteViewSettings(connection, viewName, propertyName);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteViewSettings(SQLiteConnection connection, string viewName, string propertyName = "")
        {
            if (viewName.Trim() == string.Empty)
                throw new Exception("View Name cannot be null!");

            var sqlQuery = "DELETE FROM ViewSettings";
            sqlQuery += " WHERE view_name = @view_name";
            if (propertyName.Trim() != string.Empty)
                sqlQuery += " AND name = @name";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@view_name", DbType.String));
            if (propertyName.Trim() != string.Empty)
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));

            cmdQuery.Parameters["@view_name"].Value = viewName;
            if (propertyName.Trim() != string.Empty)
                cmdQuery.Parameters["@name"].Value = propertyName;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        #endregion
        #region  |  DQF Setttings  |

        public DqfSettings GetDqfSettings(string databasePath)
        {
            DqfSettings values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = GetDqfSettings(connection);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static DqfSettings GetDqfSettings(SQLiteConnection connection)
        {
            var value = new DqfSettings();

            var sqlQuery = "SELECT * FROM DQFSettings";
            var cmdQuery = new SQLiteCommand(sqlQuery, connection);

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    rdrSelect.Read();


                    value.Id = Convert.ToInt32(rdrSelect["id"]);
                    value.UserName = rdrSelect["user_name"].ToString();
                    value.UserEmail = rdrSelect["user_email"].ToString();
                    value.UserKey = rdrSelect["user_key"].ToString();
                    value.TranslatorName = rdrSelect["translator_name"].ToString();
                    value.TranslatorEmail = rdrSelect["translator_email"].ToString();
                    value.TranslatorKey = rdrSelect["translator_key"].ToString();
                    value.EnableReports = Convert.ToBoolean(rdrSelect["enable_reports"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table DQFSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return value;
        }

        public int CreateDqfSettings(string databasePath, DqfSettings dqfSettings)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = CreateDqfSettings(connection, dqfSettings);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int CreateDqfSettings(SQLiteConnection connection, DqfSettings dqfSettings)
        {
            var value = -1;

            var sqlQuery = "INSERT INTO DQFSettings";
            sqlQuery += " (";
            sqlQuery += " user_name";
            sqlQuery += ", user_email";
            sqlQuery += ", user_key";
            sqlQuery += ", translator_name";
            sqlQuery += ", translator_email";
            sqlQuery += ", translator_key";
            sqlQuery += ", enable_reports";

            sqlQuery += " ) VALUES (";
            sqlQuery += " @user_name";
            sqlQuery += ", @user_email";
            sqlQuery += ", @user_key";
            sqlQuery += ", @translator_name";
            sqlQuery += ", @translator_email";
            sqlQuery += ", @translator_key";
            sqlQuery += ", @enable_reports";
            sqlQuery += " ); SELECT last_insert_rowid();";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@enable_reports", DbType.Boolean));

            cmdQuery.Parameters["@user_name"].Value = dqfSettings.UserName;
            cmdQuery.Parameters["@user_email"].Value = dqfSettings.UserEmail;
            cmdQuery.Parameters["@user_key"].Value = dqfSettings.UserKey;
            cmdQuery.Parameters["@translator_name"].Value = dqfSettings.TranslatorName;
            cmdQuery.Parameters["@translator_email"].Value = dqfSettings.TranslatorEmail;
            cmdQuery.Parameters["@translator_key"].Value = dqfSettings.TranslatorKey;
            cmdQuery.Parameters["@enable_reports"].Value = dqfSettings.EnableReports;

            dqfSettings.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = dqfSettings.Id;

            return value;
        }

        public bool UpdateDqfSettings(string databasePath, DqfSettings dqfSettings)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = UpdateDqfSettings(connection, dqfSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool UpdateDqfSettings(SQLiteConnection connection, DqfSettings dqfSettings)
        {
            if (dqfSettings.Id <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "UPDATE DQFSettings";
            sqlQuery += " SET ";
            sqlQuery += " user_name = @user_name";
            sqlQuery += ", user_email = @user_email";
            sqlQuery += ", user_key = @user_key";
            sqlQuery += ", translator_name = @user_name";
            sqlQuery += ", translator_email = @user_email";
            sqlQuery += ", translator_key = @translator_key";
            sqlQuery += ", enable_reports = @enable_reports";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@user_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@translator_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@enable_reports", DbType.Boolean));

            cmdQuery.Parameters["@id"].Value = dqfSettings.Id;
            cmdQuery.Parameters["@user_name"].Value = dqfSettings.UserName;
            cmdQuery.Parameters["@user_email"].Value = dqfSettings.UserEmail;
            cmdQuery.Parameters["@user_key"].Value = dqfSettings.UserKey;
            cmdQuery.Parameters["@translator_name"].Value = dqfSettings.TranslatorName;
            cmdQuery.Parameters["@translator_email"].Value = dqfSettings.TranslatorEmail;
            cmdQuery.Parameters["@translator_key"].Value = dqfSettings.TranslatorKey;
            cmdQuery.Parameters["@enable_reports"].Value = dqfSettings.EnableReports;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public long SaveDqfSettings(string databasePath, DqfSettings dqfSettings)
        {
            long values = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = SaveDqfSettings(connection, dqfSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static long SaveDqfSettings(SQLiteConnection connection, DqfSettings dqfSettings)
        {
            long values = -1;
            var dqfSettingsExisting = GetDqfSettings(connection);


            if (dqfSettings.Id == -1)
            {
                values = CreateDqfSettings(connection, dqfSettings);
            }
            else
            {
                //item *could* be updated
                //if it is 'absolutely' different then add it to the list                            
                if (!Helper.AreObjectsEqual(dqfSettings, dqfSettingsExisting))
                    UpdateDqfSettings(connection, dqfSettings);
                values = dqfSettings.Id;
            }


            return values;
        }

        public bool DeleteDqfSettings(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = DeleteDqfSettings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool DeleteDqfSettings(SQLiteConnection connection, int id)
        {
            if (id <= -1)
                throw new Exception("View User ID cannot be null!");

            var sqlQuery = "DELETE FROM DQFSettings";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion
        #region  |  General Setttings  |

        public List<GeneralProperty> GetGeneralSettings(string databasePath)
        {
            List<GeneralProperty> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getGeneralSettings(connection);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<GeneralProperty> getGeneralSettings(SQLiteConnection connection)
        {
            var values = new List<GeneralProperty>();

            var sqlQuery = "SELECT * FROM GeneralSettings";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new GeneralProperty
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            ValueType = rdrSelect["value_type"].ToString(),
                            Value = rdrSelect["value"].ToString(),
                            Text = rdrSelect["text"].ToString()
                        };


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table GeneralSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateGeneralSettingsProperty(string databasePath, GeneralProperty generalProperty)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createGeneralSettingsProperty(connection, generalProperty);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createGeneralSettingsProperty(SQLiteConnection connection, GeneralProperty generalProperty)
        {
            var value = -1;

            if (generalProperty.Name.Trim() == string.Empty)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "INSERT INTO GeneralSettings";
            sqlQuery += " (";
            sqlQuery += "  name";
            sqlQuery += ", value";
            sqlQuery += ", value_type";
            sqlQuery += ", text";
            sqlQuery += " ) VALUES (";
            sqlQuery += "  @name";
            sqlQuery += ", @value";
            sqlQuery += ", @value_type";
            sqlQuery += ", @text";
            sqlQuery += " ); SELECT last_insert_rowid();";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));


            cmdQuery.Parameters["@name"].Value = generalProperty.Name;
            cmdQuery.Parameters["@value_type"].Value = generalProperty.ValueType;
            cmdQuery.Parameters["@value"].Value = generalProperty.Value;
            cmdQuery.Parameters["@text"].Value = generalProperty.Text;

            generalProperty.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = generalProperty.Id;

            return value;
        }

        public bool UpdateGeneralSettingsProperty(string databasePath, GeneralProperty generalProperty)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = updateGeneralSettingsProperty(connection, generalProperty);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool updateGeneralSettingsProperty(SQLiteConnection connection, GeneralProperty generalProperty)
        {
            if (generalProperty.Name.Trim() == string.Empty || generalProperty.Id <= -1)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "UPDATE GeneralSettings";
            sqlQuery += " SET value = @value";
            sqlQuery += " , text = @text";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));

            cmdQuery.Parameters["@id"].Value = generalProperty.Id;
            cmdQuery.Parameters["@value"].Value = generalProperty.Value;
            cmdQuery.Parameters["@text"].Value = generalProperty.Text;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<long> SaveGeneralSettings(string databasePath, List<GeneralProperty> generalSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveGeneralSettings(connection, generalSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> saveGeneralSettings(SQLiteConnection connection, List<GeneralProperty> generalSettings)
        {
            var values = new List<long>();
            var existingGeneralProperties = getGeneralSettings(connection);

            foreach (var generalSetting in generalSettings)
            {
                if (generalSetting.Id == -1 || !existingGeneralProperties.Exists(a => { return a.Id == generalSetting.Id; }))
                {
                    values.Add(createGeneralSettingsProperty(connection, generalSetting));
                }
                else
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(generalSetting, existingGeneralProperties.Find(a => a.Id == generalSetting.Id)))
                        updateGeneralSettingsProperty(connection, generalSetting);
                    values.Add(generalSetting.Id);
                }
            }


            return values;
        }

        public List<long> InitializeGeneralSettings(string databasePath, List<GeneralProperty> generalSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = initializeGeneralSettings(connection, generalSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> initializeGeneralSettings(SQLiteConnection connection, IEnumerable<GeneralProperty> generalSettings)
        {
            var values = new List<long>();
            var generalProperties = getGeneralSettings(connection);

            values.AddRange((from generalSetting in generalSettings
                             where !generalProperties.Exists(a => a.Name == generalSetting.Name)
                             select createGeneralSettingsProperty(connection, generalSetting))
                .Select(dummy => (long)dummy));


            return values;
        }

        public bool DeleteGeneralSettings(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteGeneralSettings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteGeneralSettings(SQLiteConnection connection, int id)
        {
            if (id <= -1)
                throw new Exception("View Name cannot be null!");

            var sqlQuery = "DELETE FROM GeneralSettings";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion
        #region  |  Tracker Setttings  |

        public List<GeneralProperty> GetTrackerSettings(string databasePath)
        {
            List<GeneralProperty> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getTrackerSettings(connection);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<GeneralProperty> getTrackerSettings(SQLiteConnection connection)
        {
            var values = new List<GeneralProperty>();

            var sqlQuery = "SELECT * FROM TrackingSettings";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new GeneralProperty
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            ValueType = rdrSelect["value_type"].ToString(),
                            Value = rdrSelect["value"].ToString(),
                            Text = rdrSelect["text"].ToString()
                        };


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table TrackerSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateTrackerSettingsProperty(string databasePath, GeneralProperty generalProperty)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createTrackerSettingsProperty(connection, generalProperty);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createTrackerSettingsProperty(SQLiteConnection connection, GeneralProperty generalProperty)
        {
            var value = -1;

            if (generalProperty.Name.Trim() == string.Empty)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "INSERT INTO TrackingSettings";
            sqlQuery += " (";
            sqlQuery += "  name";
            sqlQuery += ", value";
            sqlQuery += ", value_type";
            sqlQuery += ", text";
            sqlQuery += " ) VALUES (";
            sqlQuery += "  @name";
            sqlQuery += ", @value";
            sqlQuery += ", @value_type";
            sqlQuery += ", @text";
            sqlQuery += " ); SELECT last_insert_rowid();";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));


            cmdQuery.Parameters["@name"].Value = generalProperty.Name;
            cmdQuery.Parameters["@value_type"].Value = generalProperty.ValueType;
            cmdQuery.Parameters["@value"].Value = generalProperty.Value;
            cmdQuery.Parameters["@text"].Value = generalProperty.Text;

            generalProperty.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = generalProperty.Id;

            return value;
        }

        public bool UpdateTrackerSettingsProperty(string databasePath, GeneralProperty generalProperty)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = updateTrackerSettingsProperty(connection, generalProperty);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool updateTrackerSettingsProperty(SQLiteConnection connection, GeneralProperty generalProperty)
        {
            if (generalProperty.Name.Trim() == string.Empty || generalProperty.Id <= -1)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "UPDATE TrackingSettings";
            sqlQuery += " SET value = @value";
            sqlQuery += " , text = @text";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));

            cmdQuery.Parameters["@id"].Value = generalProperty.Id;
            cmdQuery.Parameters["@value"].Value = generalProperty.Value;
            cmdQuery.Parameters["@text"].Value = generalProperty.Text;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<long> SaveTrackerSettings(string databasePath, List<GeneralProperty> trackerSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveTrackerSettings(connection, trackerSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> saveTrackerSettings(SQLiteConnection connection, List<GeneralProperty> trackerSettings)
        {
            var values = new List<long>();
            var existingTrackerProperties = getTrackerSettings(connection);

            foreach (var trackerSetting in trackerSettings)
            {
                if (trackerSetting.Id == -1 || !existingTrackerProperties.Exists(a => { return a.Id == trackerSetting.Id; }))
                {
                    values.Add(createTrackerSettingsProperty(connection, trackerSetting));
                }
                else
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(trackerSetting, existingTrackerProperties.Find(a => a.Id == trackerSetting.Id)))
                        updateTrackerSettingsProperty(connection, trackerSetting);
                    values.Add(trackerSetting.Id);
                }
            }


            return values;
        }

        public List<long> InitializeTrackerSettings(string databasePath, List<GeneralProperty> trackerSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = initializeTrackerSettings(connection, trackerSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> initializeTrackerSettings(SQLiteConnection connection, IEnumerable<GeneralProperty> trackerSettings)
        {
            var values = new List<long>();
            var trackerProperties = getTrackerSettings(connection);

            values.AddRange((from trackerSetting in trackerSettings
                             where !trackerProperties.Exists(a => a.Name == trackerSetting.Name)
                             select createTrackerSettingsProperty(connection, trackerSetting))
                .Select(dummy => (long)dummy));


            return values;
        }

        public bool DeleteTrackerSettings(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteTrackerSettings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteTrackerSettings(SQLiteConnection connection, int id)
        {
            if (id <= -1)
                throw new Exception("View Name cannot be null!");

            var sqlQuery = "DELETE FROM TrackingSettings";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion
        #region  |  Backup Setttings  |

        public List<GeneralProperty> GetBackupSettings(string databasePath)
        {
            List<GeneralProperty> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getBackupSettings(connection);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<GeneralProperty> getBackupSettings(SQLiteConnection connection)
        {
            var values = new List<GeneralProperty>();

            var sqlQuery = "SELECT * FROM BackupSettings";
            var cmdQuery = new SQLiteCommand(sqlQuery, connection);

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new GeneralProperty
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            ValueType = rdrSelect["value_type"].ToString(),
                            Value = rdrSelect["value"].ToString(),
                            Text = rdrSelect["text"].ToString()
                        };

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table BackupSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateBackupSettingsProperty(string databasePath, GeneralProperty backupProperty)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createBackupSettingsProperty(connection, backupProperty);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createBackupSettingsProperty(SQLiteConnection connection, GeneralProperty backupProperty)
        {
            var value = -1;

            if (backupProperty.Name.Trim() == string.Empty)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "INSERT INTO BackupSettings";
            sqlQuery += " (";
            sqlQuery += " name";
            sqlQuery += ", value_type";
            sqlQuery += ", value";
            sqlQuery += ", text";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @name";
            sqlQuery += ", @value_type";
            sqlQuery += ", @value";
            sqlQuery += ", @text";
            sqlQuery += " ); SELECT last_insert_rowid();";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));


            cmdQuery.Parameters["@name"].Value = backupProperty.Name;
            cmdQuery.Parameters["@value_type"].Value = backupProperty.ValueType;
            cmdQuery.Parameters["@value"].Value = backupProperty.Value;
            cmdQuery.Parameters["@text"].Value = backupProperty.Text;

            backupProperty.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = backupProperty.Id;

            return value;
        }

        public bool UpdateBackupSettingsProperty(string databasePath, GeneralProperty backupProperty)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = updateBackupSettingsProperty(connection, backupProperty);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool updateBackupSettingsProperty(SQLiteConnection connection, GeneralProperty backupProperty)
        {
            if (backupProperty.Name.Trim() == string.Empty || backupProperty.Id <= -1)
                throw new Exception("The property name cannot be null!");

            var sqlQuery = "UPDATE BackupSettings";
            sqlQuery += " SET value = @value";
            sqlQuery += " , text = @text";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text", DbType.String));


            cmdQuery.Parameters["@id"].Value = backupProperty.Id;
            cmdQuery.Parameters["@value"].Value = backupProperty.Value;
            cmdQuery.Parameters["@text"].Value = backupProperty.Text;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<long> SaveBackupSettings(string databasePath, List<GeneralProperty> backupSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveBackupSettings(connection, backupSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<long> saveBackupSettings(SQLiteConnection connection, List<GeneralProperty> backupSettings)
        {
            var values = new List<long>();
            var exisingBackupProperties = getBackupSettings(connection);

            foreach (var backupSetting in backupSettings)
            {
                if (backupSetting.Id == -1 | !exisingBackupProperties.Exists(a => a.Id == backupSetting.Id))
                {
                    values.Add(createBackupSettingsProperty(connection, backupSetting));
                }
                else
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(backupSetting, exisingBackupProperties.Find(a => a.Id == backupSetting.Id)))
                        updateBackupSettingsProperty(connection, backupSetting);
                    values.Add(backupSetting.Id);
                }

            }


            return values;
        }

        public List<long> InitializeBackupSettings(string databasePath, List<GeneralProperty> backupSettings)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = initializeBackupSettings(connection, backupSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> initializeBackupSettings(SQLiteConnection connection, List<GeneralProperty> backupSettings)
        {
            var values = new List<long>();
            var backupProperties = getBackupSettings(connection);

            values.AddRange(backupSettings.Where(
                    backupSetting => !backupProperties.Exists(a => a.Name == backupSetting.Name))
                .Select(backupSetting => createBackupSettingsProperty(connection, backupSetting)).Select(dummy => (long)dummy));


            return values;
        }

        public bool DeleteBackupSettings(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteBackupSettings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteBackupSettings(SQLiteConnection connection, int id)
        {
            if (id <= -1)
                throw new Exception("View Name cannot be null!");

            var sqlQuery = "DELETE FROM BackupSettings";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion

        #region  |  User Profiles  |

        public List<UserProfile> GetUserProfiles(string databasePath, int? id)
        {
            List<UserProfile> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getUserProfiles(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<UserProfile> getUserProfiles(SQLiteConnection connection, int? id)
        {
            var values = new List<UserProfile>();

            var sqlQuery = "SELECT * FROM UserProfiles";
            if (id.HasValue)
                sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            if (id.HasValue)
            {
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id.Value;
            }


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new UserProfile
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString().Trim(),
                            UserName = rdrSelect["userName"].ToString().Trim(),
                            Street = rdrSelect["street"].ToString(),
                            City = rdrSelect["city"].ToString(),
                            State = rdrSelect["state"].ToString(),
                            Zip = rdrSelect["zip"].ToString(),
                            Country = rdrSelect["country"].ToString(),
                            TaxCode = rdrSelect["tax_code"].ToString(),
                            VatCode = rdrSelect["vat_code"].ToString(),
                            Email = rdrSelect["email"].ToString(),
                            Web = rdrSelect["web"].ToString(),
                            Phone = rdrSelect["phone"].ToString(),
                            Mobile = rdrSelect["mobile"].ToString(),
                            Fax = rdrSelect["fax"].ToString(),
                            Note = rdrSelect["note"].ToString()
                        };

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table UserProfiles!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateUserProfile(string databasePath, UserProfile userProfile)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createUserProfile(connection, userProfile);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createUserProfile(SQLiteConnection connection, UserProfile userProfile)
        {
            var value = -1;
            if (userProfile.Name.Trim() == string.Empty)
                throw new Exception("The name cannot be null!");

            var sqlQuery = "INSERT INTO UserProfiles";
            sqlQuery += " (";
            sqlQuery += " name";
            sqlQuery += ", userName";
            sqlQuery += ", street";
            sqlQuery += ", city";
            sqlQuery += ", state";
            sqlQuery += ", zip";
            sqlQuery += ", country";
            sqlQuery += ", tax_code";
            sqlQuery += ", vat_code";
            sqlQuery += ", email";
            sqlQuery += ", web";
            sqlQuery += ", phone";
            sqlQuery += ", mobile";
            sqlQuery += ", fax";
            sqlQuery += ", note";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @name";
            sqlQuery += ", @userName";
            sqlQuery += ", @street";
            sqlQuery += ", @city";
            sqlQuery += ", @state";
            sqlQuery += ", @zip";
            sqlQuery += ", @country";
            sqlQuery += ", @tax_code";
            sqlQuery += ", @vat_code";
            sqlQuery += ", @email";
            sqlQuery += ", @web";
            sqlQuery += ", @phone";
            sqlQuery += ", @mobile";
            sqlQuery += ", @fax";
            sqlQuery += ", @note";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@userName", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@street", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@city", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@state", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@zip", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@country", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@tax_code", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@vat_code", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@web", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@phone", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@mobile", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@fax", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@note", DbType.String));

            cmdQuery.Parameters["@name"].Value = userProfile.Name == "" ? " " : userProfile.Name;
            cmdQuery.Parameters["@userName"].Value = userProfile.UserName;
            cmdQuery.Parameters["@street"].Value = userProfile.Street;
            cmdQuery.Parameters["@city"].Value = userProfile.City;
            cmdQuery.Parameters["@state"].Value = userProfile.State;
            cmdQuery.Parameters["@zip"].Value = userProfile.Zip;
            cmdQuery.Parameters["@country"].Value = userProfile.Country;
            cmdQuery.Parameters["@tax_code"].Value = userProfile.TaxCode;
            cmdQuery.Parameters["@vat_code"].Value = userProfile.VatCode;
            cmdQuery.Parameters["@email"].Value = userProfile.Email;
            cmdQuery.Parameters["@web"].Value = userProfile.Web;
            cmdQuery.Parameters["@phone"].Value = userProfile.Phone;
            cmdQuery.Parameters["@mobile"].Value = userProfile.Mobile;
            cmdQuery.Parameters["@fax"].Value = userProfile.Fax;
            cmdQuery.Parameters["@note"].Value = userProfile.Note;

            userProfile.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = userProfile.Id;

            return value;
        }

        public bool UpdateUserProfile(string databasePath, UserProfile userProfile)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = updateUserProfile(connection, userProfile);
                }
                finally
                {
                    connection.Close();
                }
            }


            return success;
        }
        private static bool updateUserProfile(SQLiteConnection connection, UserProfile userProfile)
        {
            if (userProfile.Id <= -1)
                throw new Exception("The id cannot be null!");
            if (userProfile.Name.Trim() == string.Empty)
                throw new Exception("The name cannot be null!");

            var sqlQuery = "UPDATE UserProfiles";
            sqlQuery += " SET";
            sqlQuery += " name = @name";
            sqlQuery += ", userName = @userName";
            sqlQuery += ", street = @street";
            sqlQuery += ", city = @city";
            sqlQuery += ", state = @state";
            sqlQuery += ", zip = @zip";
            sqlQuery += ", country = @country";
            sqlQuery += ", tax_code = @tax_code";
            sqlQuery += ", vat_code = @vat_code";
            sqlQuery += ", email = @email";
            sqlQuery += ", web = @web";
            sqlQuery += ", phone = @phone";
            sqlQuery += ", mobile = @mobile";
            sqlQuery += ", fax = @fax";
            sqlQuery += ", note = @note";
            sqlQuery += " WHERE id = @id";



            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@userName", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@street", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@city", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@state", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@zip", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@country", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@tax_code", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@vat_code", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@email", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@web", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@phone", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@mobile", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@fax", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@note", DbType.String));

            cmdQuery.Parameters["@id"].Value = userProfile.Id;
            cmdQuery.Parameters["@name"].Value = userProfile.Name == "" ? " " : userProfile.Name;
            cmdQuery.Parameters["@userName"].Value = userProfile.UserName;
            cmdQuery.Parameters["@street"].Value = userProfile.Street;
            cmdQuery.Parameters["@city"].Value = userProfile.City;
            cmdQuery.Parameters["@state"].Value = userProfile.State;
            cmdQuery.Parameters["@zip"].Value = userProfile.Zip;
            cmdQuery.Parameters["@country"].Value = userProfile.Country;
            cmdQuery.Parameters["@tax_code"].Value = userProfile.TaxCode;
            cmdQuery.Parameters["@vat_code"].Value = userProfile.VatCode;
            cmdQuery.Parameters["@email"].Value = userProfile.Email;
            cmdQuery.Parameters["@web"].Value = userProfile.Web;
            cmdQuery.Parameters["@phone"].Value = userProfile.Phone;
            cmdQuery.Parameters["@mobile"].Value = userProfile.Mobile;
            cmdQuery.Parameters["@fax"].Value = userProfile.Fax;
            cmdQuery.Parameters["@note"].Value = userProfile.Note;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<long> SaveUserProfiles(string databasePath, List<UserProfile> userProfiles)
        {
            List<long> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveUserProfiles(connection, userProfiles);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<long> saveUserProfiles(SQLiteConnection connection, List<UserProfile> userProfiles)
        {
            var values = new List<long>();
            var existingUserProfiles = getUserProfiles(connection, null);

            foreach (var userProfile in userProfiles)
            {
                if (userProfile.Id == -1 || !existingUserProfiles.Exists(a => { return a.Id == userProfile.Id; }))
                {
                    if (userProfile.Id == -1 && userProfile.Name.Trim() == string.Empty)
                        userProfile.Name = "[company name]"; //set a default name here to avoid and exception message

                    values.Add(createUserProfile(connection, userProfile));
                }
                else
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(userProfile, existingUserProfiles.Find(a => a.Id == userProfile.Id)))
                        updateUserProfile(connection, userProfile);

                    values.Add(userProfile.Id);
                }
            }


            return values;
        }

        public bool DeleteUserProfile(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteUserProfile(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteUserProfile(SQLiteConnection connection, int id)
        {
            if (id <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "DELETE FROM UserProfiles";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        #endregion


        #region  |  Company Profiles  |

        public List<CompanyProfile> GetCompanyProfiles(string databasePath, int? id)
        {
            var values = new List<CompanyProfile>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getCompanyProfiles(connection, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<CompanyProfile> getCompanyProfiles(SQLiteConnection connection, int? id)
        {
            var values = new List<CompanyProfile>();

            var sqlQuery = "SELECT * FROM CompanyProfiles";
            if (id.HasValue)
                sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            if (id.HasValue)
            {
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id.Value;
            }


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new CompanyProfile();

                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.Name = rdrSelect["name"].ToString();
                        value.ContactName = rdrSelect["contact_name"].ToString();
                        value.Street = rdrSelect["street"].ToString();
                        value.City = rdrSelect["city"].ToString();
                        value.State = rdrSelect["state"].ToString();
                        value.Zip = rdrSelect["zip"].ToString();
                        value.Country = rdrSelect["country"].ToString();
                        value.TaxCode = rdrSelect["tax_code"].ToString();
                        value.VatCode = rdrSelect["vat_code"].ToString();
                        value.Email = rdrSelect["email"].ToString();
                        value.Web = rdrSelect["web"].ToString();
                        value.Phone = rdrSelect["phone"].ToString();
                        value.Mobile = rdrSelect["mobile"].ToString();
                        value.Fax = rdrSelect["fax"].ToString();
                        value.Note = rdrSelect["note"].ToString();
                        value.MetricGroup = new QualityMetricGroup();
                        var qualityMetricGroupId = rdrSelect["quality_metric_group_id"].ToString();
                        if (qualityMetricGroupId == string.Empty)
                            qualityMetricGroupId = "-1";
                        value.MetricGroup.Id = Convert.ToInt32(qualityMetricGroupId);

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table CompanyProfiles!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            foreach (var value in values)
            {
                var comparerSettings = getComparerSettings(connection, value.Id);
                if (comparerSettings.Count > 0)
                    value.ComparerOptions = comparerSettings[0];

                var companyProfileRates = getCompanyProfileRates(connection, value.Id);
                if (companyProfileRates.Count > 0)
                    value.ProfileRate = companyProfileRates[0];

                var qmgs = getQualityMetricGroupSettings(connection, value.MetricGroup.Id);
                if (qmgs.Count > 0)
                    value.MetricGroup = qmgs[0];
                else
                    value.MetricGroup = new QualityMetricGroup();
            }

            return values;
        }

        public int CreateCompanyProfile(string databasePath, CompanyProfile companyProfile)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createCompanyProfile(connection, companyProfile);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createCompanyProfile(SQLiteConnection connection, CompanyProfile companyProfile)
        {
            var value = -1;

            try
            {
                if (companyProfile.Name.Trim() == string.Empty)
                    throw new Exception("The name cannot be null!");

                var sqlQuery = "INSERT INTO CompanyProfiles";
                sqlQuery += " (";
                sqlQuery += " name";
                sqlQuery += ", contact_name";
                sqlQuery += ", street";
                sqlQuery += ", city";
                sqlQuery += ", state";
                sqlQuery += ", zip";
                sqlQuery += ", country";
                sqlQuery += ", tax_code";
                sqlQuery += ", vat_code";
                sqlQuery += ", email";
                sqlQuery += ", web";
                sqlQuery += ", phone";
                sqlQuery += ", mobile";
                sqlQuery += ", fax";
                sqlQuery += ", note";
                sqlQuery += ", quality_metric_group_id";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @name";
                sqlQuery += ", @contact_name";
                sqlQuery += ", @street";
                sqlQuery += ", @city";
                sqlQuery += ", @state";
                sqlQuery += ", @zip";
                sqlQuery += ", @country";
                sqlQuery += ", @tax_code";
                sqlQuery += ", @vat_code";
                sqlQuery += ", @email";
                sqlQuery += ", @web";
                sqlQuery += ", @phone";
                sqlQuery += ", @mobile";
                sqlQuery += ", @fax";
                sqlQuery += ", @note";
                sqlQuery += ", @quality_metric_group_id";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@contact_name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@street", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@city", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@state", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@zip", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@country", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@tax_code", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@vat_code", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@email", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@web", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@phone", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@mobile", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@fax", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@note", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@quality_metric_group_id", DbType.Int32));

                cmdQuery.Parameters["@name"].Value = companyProfile.Name;
                cmdQuery.Parameters["@contact_name"].Value = companyProfile.ContactName;
                cmdQuery.Parameters["@street"].Value = companyProfile.Street;
                cmdQuery.Parameters["@city"].Value = companyProfile.City;
                cmdQuery.Parameters["@state"].Value = companyProfile.State;
                cmdQuery.Parameters["@zip"].Value = companyProfile.Zip;
                cmdQuery.Parameters["@country"].Value = companyProfile.Country;
                cmdQuery.Parameters["@tax_code"].Value = companyProfile.TaxCode;
                cmdQuery.Parameters["@vat_code"].Value = companyProfile.VatCode;
                cmdQuery.Parameters["@email"].Value = companyProfile.Email;
                cmdQuery.Parameters["@web"].Value = companyProfile.Web;
                cmdQuery.Parameters["@phone"].Value = companyProfile.Phone;
                cmdQuery.Parameters["@mobile"].Value = companyProfile.Mobile;
                cmdQuery.Parameters["@fax"].Value = companyProfile.Fax;
                cmdQuery.Parameters["@note"].Value = companyProfile.Note;
                cmdQuery.Parameters["@quality_metric_group_id"].Value = companyProfile.MetricGroup.Id;

                companyProfile.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = companyProfile.Id;

                #region  |  create  CompanyProfileRate  |


                companyProfile.ProfileRate.CompanyProfileId = value;
                companyProfile.ProfileRate.Id = createCompanyProfileRate(connection, companyProfile.ProfileRate);

                #endregion

                #region  |  create  createComparerSettings  |

                companyProfile.ComparerOptions.CompanyProfileId = value;
                companyProfile.ComparerOptions.StyleNewTag.CompanyProfileId = value;
                companyProfile.ComparerOptions.StyleNewText.CompanyProfileId = value;
                companyProfile.ComparerOptions.StyleRemovedTag.CompanyProfileId = value;
                companyProfile.ComparerOptions.StyleRemovedText.CompanyProfileId = value;


                companyProfile.ComparerOptions.Id = createComparerSettings(connection, companyProfile.ComparerOptions);

                #endregion


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateCompanyProfile(string databasePath, CompanyProfile companyProfile)
        {
            var success = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        success = updateCompanyProfile(connection, companyProfile);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return success;
        }
        private bool updateCompanyProfile(SQLiteConnection connection, CompanyProfile companyProfile)
        {
            var success = false;

            try
            {
                if (companyProfile.Id <= -1)
                    throw new Exception("The id cannot be null! - company profile");
                if (companyProfile.Name.Trim() == string.Empty)
                    throw new Exception("The name cannot be null! - company profile");

                var sqlQuery = "UPDATE CompanyProfiles";
                sqlQuery += " SET";
                sqlQuery += " name = @name";
                sqlQuery += ", contact_name = @contact_name";
                sqlQuery += ", street = @street";
                sqlQuery += ", city = @city";
                sqlQuery += ", state = @state";
                sqlQuery += ", zip = @zip";
                sqlQuery += ", country = @country";
                sqlQuery += ", tax_code = @tax_code";
                sqlQuery += ", vat_code = @vat_code";
                sqlQuery += ", email = @email";
                sqlQuery += ", web = @web";
                sqlQuery += ", phone = @phone";
                sqlQuery += ", mobile = @mobile";
                sqlQuery += ", fax = @fax";
                sqlQuery += ", note = @note";
                sqlQuery += ", quality_metric_group_id = @quality_metric_group_id";
                sqlQuery += " WHERE id = @id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@contact_name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@street", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@city", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@state", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@zip", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@country", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@tax_code", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@vat_code", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@email", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@web", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@phone", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@mobile", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@fax", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@note", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@quality_metric_group_id", DbType.Int32));

                cmdQuery.Parameters["@id"].Value = companyProfile.Id;
                cmdQuery.Parameters["@name"].Value = companyProfile.Name;
                cmdQuery.Parameters["@contact_name"].Value = companyProfile.ContactName;
                cmdQuery.Parameters["@street"].Value = companyProfile.Street;
                cmdQuery.Parameters["@city"].Value = companyProfile.City;
                cmdQuery.Parameters["@state"].Value = companyProfile.State;
                cmdQuery.Parameters["@zip"].Value = companyProfile.Zip;
                cmdQuery.Parameters["@country"].Value = companyProfile.Country;
                cmdQuery.Parameters["@tax_code"].Value = companyProfile.TaxCode;
                cmdQuery.Parameters["@vat_code"].Value = companyProfile.VatCode;
                cmdQuery.Parameters["@email"].Value = companyProfile.Email;
                cmdQuery.Parameters["@web"].Value = companyProfile.Web;
                cmdQuery.Parameters["@phone"].Value = companyProfile.Phone;
                cmdQuery.Parameters["@mobile"].Value = companyProfile.Mobile;
                cmdQuery.Parameters["@fax"].Value = companyProfile.Fax;
                cmdQuery.Parameters["@note"].Value = companyProfile.Note;
                cmdQuery.Parameters["@quality_metric_group_id"].Value = companyProfile.MetricGroup.Id;

                cmdQuery.ExecuteNonQuery();
                success = true;


                //update the childeren classes
                companyProfile.ProfileRate.CompanyProfileId = companyProfile.Id;
                companyProfile.ProfileRate.Id = saveCompanyProfileRate(connection, companyProfile.ProfileRate);
                companyProfile.ComparerOptions.CompanyProfileId = companyProfile.Id;
                companyProfile.ComparerOptions.Id = saveComparerSettings(connection, companyProfile.ComparerOptions);


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        public List<long> SaveCompanyProfiles(string databasePath, List<CompanyProfile> companyProfiles)
        {
            var values = new List<long>();
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        values = saveCompanyProfiles(connection, companyProfiles);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private List<long> saveCompanyProfiles(SQLiteConnection connection, List<CompanyProfile> companyProfiles)
        {
            var values = new List<long>();
            try
            {
                var existingCompanyProfiles = getCompanyProfiles(connection, null);

                foreach (var companyProfile in companyProfiles)
                {
                    if (companyProfile.Id == -1 || !existingCompanyProfiles.Exists(a => { return a.Id == companyProfile.Id; }))
                    {
                        values.Add(createCompanyProfile(connection, companyProfile));
                    }
                    else
                    {
                        //if it already exists then leave it as it is
                        //it's too deep to compare the class at this level; this check will be done at a lower level
                        var success = updateCompanyProfile(connection, companyProfile);
                        values.Add(companyProfile.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return values;
        }

        public bool DeleteCompanyProfile(string databasePath, int id)
        {
            var success = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        success = deleteCompanyProfile(connection, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
        private static bool deleteCompanyProfile(SQLiteConnection connection, int id)
        {
            var success = false;

            try
            {
                if (id <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM CompanyProfiles";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id;

                cmdQuery.ExecuteNonQuery();
                success = true;

                //PH: 01-05-2015 - include a check for the return variable; figure out some way to handle this better
                //delete the childern classes
                success = deleteCompanyProfileRate(connection, id);
                success = deleteComparerSettings(connection, id);
                success = deleteComparerDifferencesFormatting(connection, id);


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        #endregion
        #region  |  Company Profile Rates  |

        public List<CompanyProfileRate> GetCompanyProfileRates(string databasePath, int companyProfileId)
        {
            var values = new List<CompanyProfileRate>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getCompanyProfileRates(connection, companyProfileId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<CompanyProfileRate> getCompanyProfileRates(SQLiteConnection connection, int companyProfileId)
        {
            var values = new List<CompanyProfileRate>();

            var sqlQuery = "SELECT * FROM CompanyProfileRates";
            sqlQuery += " WHERE company_profile_id = @company_profile_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;



            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new CompanyProfileRate
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]),
                            LanguageRateAutoAdd =
                                rdrSelect["language_rate_auto_add"].ToString() != string.Empty &&
                                Convert.ToBoolean(rdrSelect["language_rate_auto_add"]),
                            LanguageRateId = Convert.ToInt32(rdrSelect["language_rate_id"]),
                            HourlyRateAutoAdd =
                                rdrSelect["hourly_rate_auto_add"].ToString() != string.Empty &&
                                Convert.ToBoolean(rdrSelect["hourly_rate_auto_add"]),
                            HourlyRateCurrency = rdrSelect["hourly_rate_currency"].ToString(),
                            HourlyRateRate = Convert.ToInt32(rdrSelect["hourly_rate_rate"])
                        };
                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table CompanyProfileRates!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateCompanyProfileRate(string databasePath, CompanyProfileRate companyProfileRate)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createCompanyProfileRate(connection, companyProfileRate);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createCompanyProfileRate(SQLiteConnection connection, CompanyProfileRate companyProfileRate)
        {
            var value = -1;


            if (companyProfileRate.CompanyProfileId <= -1)
                throw new Exception("The company profile id cannot be null!");

            var sqlQuery = "INSERT INTO CompanyProfileRates";
            sqlQuery += " (";
            sqlQuery += " company_profile_id";
            sqlQuery += ", language_rate_auto_add";
            sqlQuery += ", language_rate_id";
            sqlQuery += ", hourly_rate_auto_add";
            sqlQuery += ", hourly_rate_currency";
            sqlQuery += ", hourly_rate_rate";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @company_profile_id";
            sqlQuery += ", @language_rate_auto_add";
            sqlQuery += ", @language_rate_id";
            sqlQuery += ", @hourly_rate_auto_add";
            sqlQuery += ", @hourly_rate_currency";
            sqlQuery += ", @hourly_rate_rate";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_auto_add", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_auto_add", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_rate", DbType.Double));

            cmdQuery.Parameters["@company_profile_id"].Value = companyProfileRate.CompanyProfileId;
            cmdQuery.Parameters["@language_rate_auto_add"].Value = companyProfileRate.LanguageRateAutoAdd;
            cmdQuery.Parameters["@language_rate_id"].Value = companyProfileRate.LanguageRateId;
            cmdQuery.Parameters["@hourly_rate_auto_add"].Value = companyProfileRate.HourlyRateAutoAdd;
            cmdQuery.Parameters["@hourly_rate_currency"].Value = companyProfileRate.HourlyRateCurrency;
            cmdQuery.Parameters["@hourly_rate_rate"].Value = companyProfileRate.HourlyRateRate;


            companyProfileRate.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = companyProfileRate.Id;



            return value;
        }

        public bool UpdateCompanyProfileRate(string databasePath, CompanyProfileRate companyProfileRate)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateCompanyProfileRate(connection, companyProfileRate);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateCompanyProfileRate(SQLiteConnection connection, CompanyProfileRate companyProfileRate)
        {
            var value = false;

            try
            {
                if (companyProfileRate.CompanyProfileId <= -1)
                    throw new Exception("The company profile id cannot be null!  - company profile rates");

                var sqlQuery = "UPDATE CompanyProfileRates";
                sqlQuery += " SET";
                sqlQuery += " language_rate_auto_add = @language_rate_auto_add";
                sqlQuery += ", language_rate_id = @language_rate_id";
                sqlQuery += ", hourly_rate_auto_add = @hourly_rate_auto_add";
                sqlQuery += ", hourly_rate_currency = @hourly_rate_currency";
                sqlQuery += ", hourly_rate_rate = @hourly_rate_rate";
                sqlQuery += " WHERE company_profile_id = @company_profile_id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_auto_add", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_auto_add", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_currency", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_rate", DbType.Double));

                cmdQuery.Parameters["@company_profile_id"].Value = companyProfileRate.CompanyProfileId;
                cmdQuery.Parameters["@language_rate_auto_add"].Value = companyProfileRate.LanguageRateAutoAdd;
                cmdQuery.Parameters["@language_rate_id"].Value = companyProfileRate.LanguageRateId;
                cmdQuery.Parameters["@hourly_rate_auto_add"].Value = companyProfileRate.HourlyRateAutoAdd;
                cmdQuery.Parameters["@hourly_rate_currency"].Value = companyProfileRate.HourlyRateCurrency;
                cmdQuery.Parameters["@hourly_rate_rate"].Value = companyProfileRate.HourlyRateRate;


                cmdQuery.ExecuteNonQuery();
                value = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public int SaveCompanyProfileRate(string databasePath, CompanyProfileRate companyProfileRate)
        {
            var value = -1;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        value = saveCompanyProfileRate(connection, companyProfileRate);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static int saveCompanyProfileRate(SQLiteConnection connection, CompanyProfileRate companyProfileRate)
        {
            var value = -1;

            var companyProfileRates = getCompanyProfileRates(connection, companyProfileRate.CompanyProfileId);


            var exists = false;
            var isDifferent = false;

            if (companyProfileRates.Count > 0)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(companyProfileRate, companyProfileRates.Find(a => a.Id == companyProfileRate.Id)))
                    isDifferent = true;
            }


            if (!exists && companyProfileRate.Id == -1)
            {
                value = createCompanyProfileRate(connection, companyProfileRate);
            }
            else if (isDifferent)
            {
                updateCompanyProfileRate(connection, companyProfileRate);
                value = companyProfileRate.Id;
            }




            return value;
        }

        public bool DeleteCompanyProfileRate(string databasePath, int companyProfileId)
        {
            var success = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        success = deleteCompanyProfileRate(connection, companyProfileId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
        private static bool deleteCompanyProfileRate(SQLiteConnection connection, int companyProfileId)
        {

            if (companyProfileId <= -1)
                throw new Exception("The company_profile_id cannot be null!");

            var sqlQuery = "DELETE FROM CompanyProfileRates";
            sqlQuery += " WHERE company_profile_id = @company_profile_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;

            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion
        #region  |  Company Profile Comparer Settings  |

        public List<ComparerSettings> GetComparerSettings(string databasePath, int companyProfileId)
        {
            List<ComparerSettings> values;

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getComparerSettings(connection, companyProfileId);
                }
                finally
                {
                    connection.Close();
                }
            }


            return values;
        }
        private static List<ComparerSettings> getComparerSettings(SQLiteConnection connection, int companyProfileId)
        {
            var values = new List<ComparerSettings>();

            if (companyProfileId <= -1)
                throw new Exception(" company profile id cannot be null!");

            var sqlQuery = "SELECT * FROM CompanyProfileComparerSettings";
            sqlQuery += " WHERE company_profile_id = @company_profile_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new ComparerSettings();

                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]);
                        value.ComparisonType = Convert.ToInt32(rdrSelect["comparison_type"]);
                        value.ConsolidateChanges = Convert.ToBoolean(rdrSelect["consolidate_changes"]);
                        value.IncludeTagsInComparison = Convert.ToBoolean(rdrSelect["include_tags_in_comparison"]);
                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table CompanyProfileComparerSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            //get the formatting styles
            foreach (var value in values)
            {
                var styles = getComparerDifferencesFormattings(connection, value.Id, null);
                foreach (var style in styles)
                {
                    switch (style.Name)
                    {
                        case DifferencesFormatting.StyleName.NewText: value.StyleNewText = style; break;
                        case DifferencesFormatting.StyleName.RemovedText: value.StyleRemovedText = style; break;
                        case DifferencesFormatting.StyleName.NewTag: value.StyleNewTag = style; break;
                        case DifferencesFormatting.StyleName.RemovedTag: value.StyleRemovedTag = style; break;
                    }
                }
            }


            return values;
        }

        public int CreateComparerSettings(string databasePath, ComparerSettings comparerSettings)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createComparerSettings(connection, comparerSettings);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createComparerSettings(SQLiteConnection connection, ComparerSettings comparerSettings)
        {
            var value = -1;

            try
            {
                if (comparerSettings.CompanyProfileId <= -1)
                    throw new Exception("The company profile id cannot be null!");

                var sqlQuery = "INSERT INTO CompanyProfileComparerSettings";
                sqlQuery += " (";
                sqlQuery += " company_profile_id";
                sqlQuery += ", comparison_type";
                sqlQuery += ", consolidate_changes";
                sqlQuery += ", include_tags_in_comparison";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @company_profile_id";
                sqlQuery += ", @comparison_type";
                sqlQuery += ", @consolidate_changes";
                sqlQuery += ", @include_tags_in_comparison";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@comparison_type", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@consolidate_changes", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@include_tags_in_comparison", DbType.Boolean));

                cmdQuery.Parameters["@company_profile_id"].Value = comparerSettings.CompanyProfileId;
                cmdQuery.Parameters["@comparison_type"].Value = comparerSettings.ComparisonType;
                cmdQuery.Parameters["@consolidate_changes"].Value = comparerSettings.ConsolidateChanges;
                cmdQuery.Parameters["@include_tags_in_comparison"].Value = comparerSettings.IncludeTagsInComparison;

                comparerSettings.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = comparerSettings.Id;

                #region  |  create the styles  |
                comparerSettings.StyleNewTag.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleNewText.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleRemovedTag.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleRemovedText.CompanyProfileId = comparerSettings.CompanyProfileId;


                comparerSettings.StyleNewTag.Id = createComparerDifferencesFormatting(connection, comparerSettings.StyleNewTag);
                comparerSettings.StyleNewText.Id = createComparerDifferencesFormatting(connection, comparerSettings.StyleNewText);
                comparerSettings.StyleRemovedTag.Id = createComparerDifferencesFormatting(connection, comparerSettings.StyleRemovedTag);
                comparerSettings.StyleRemovedText.Id = createComparerDifferencesFormatting(connection, comparerSettings.StyleRemovedText);

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateComparerSettings(string databasePath, ComparerSettings comparerSettings)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateComparerSettings(connection, comparerSettings);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateComparerSettings(SQLiteConnection connection, ComparerSettings comparerSettings)
        {
            var value = false;

            try
            {
                if (comparerSettings.CompanyProfileId <= -1)
                    throw new Exception("The company profile id cannot be null!  - company comparer settings");
                if (comparerSettings.Id <= -1)
                    throw new Exception("The comparer setings id cannot be null!  - company comparer settings");

                var sqlQuery = "UPDATE CompanyProfileComparerSettings";
                sqlQuery += " SET";
                sqlQuery += " company_profile_id = @company_profile_id";
                sqlQuery += ", comparison_type = @comparison_type";
                sqlQuery += ", consolidate_changes = @consolidate_changes";
                sqlQuery += ", include_tags_in_comparison = @include_tags_in_comparison";
                sqlQuery += " WHERE";
                sqlQuery += " id = @id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@comparison_type", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@consolidate_changes", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@include_tags_in_comparison", DbType.Boolean));


                cmdQuery.Parameters["@id"].Value = comparerSettings.Id;
                cmdQuery.Parameters["@company_profile_id"].Value = comparerSettings.CompanyProfileId;
                cmdQuery.Parameters["@comparison_type"].Value = comparerSettings.ComparisonType;
                cmdQuery.Parameters["@consolidate_changes"].Value = comparerSettings.ConsolidateChanges;
                cmdQuery.Parameters["@include_tags_in_comparison"].Value = comparerSettings.IncludeTagsInComparison;

                cmdQuery.ExecuteNonQuery();
                value = true;

                #region  |  udpate the styles  |

                comparerSettings.StyleNewTag.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleNewText.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleRemovedTag.CompanyProfileId = comparerSettings.CompanyProfileId;
                comparerSettings.StyleRemovedText.CompanyProfileId = comparerSettings.CompanyProfileId;


                comparerSettings.StyleNewTag.Id = saveComparerDifferencesFormatting(connection, comparerSettings.StyleNewTag);
                comparerSettings.StyleNewText.Id = saveComparerDifferencesFormatting(connection, comparerSettings.StyleNewText);
                comparerSettings.StyleRemovedTag.Id = saveComparerDifferencesFormatting(connection, comparerSettings.StyleRemovedTag);
                comparerSettings.StyleRemovedText.Id = saveComparerDifferencesFormatting(connection, comparerSettings.StyleRemovedText);

                #endregion



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public int SaveComparerSettings(string databasePath, ComparerSettings comparerSettings)
        {
            var value = -1;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        value = saveComparerSettings(connection, comparerSettings);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static int saveComparerSettings(SQLiteConnection connection, ComparerSettings comparerSettings)
        {
            var value = comparerSettings.Id;
            try
            {
                var existingComparerSettings = getComparerSettings(connection, comparerSettings.CompanyProfileId);


                var exists = false;
                var isDifferent = false;

                if (existingComparerSettings.Count > 0)
                {
                    exists = true;
                    if (!Helper.AreObjectsEqual(comparerSettings, existingComparerSettings.Find(a => a.Id == comparerSettings.Id)))
                        isDifferent = true;
                }


                if (!exists && comparerSettings.Id == -1)
                {
                    value = createComparerSettings(connection, comparerSettings);
                }
                else if (isDifferent)
                {
                    var success = updateComparerSettings(connection, comparerSettings);
                    value = comparerSettings.Id;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }

        public bool DeleteComparerSettings(string databasePath, int companyProfileId)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteComparerSettings(connection, companyProfileId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteComparerSettings(SQLiteConnection connection, int companyProfileId)
        {
            var success = false;

            try
            {
                if (companyProfileId <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM CompanyProfileComparerSettings";
                sqlQuery += " WHERE company_profile_id = @company_profile_id ";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;

                cmdQuery.ExecuteNonQuery();
                success = true;


                if (success)
                    success = deleteComparerDifferencesFormatting(connection, companyProfileId);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion
        #region  |  Company Profile Comparer Setting Styles  |

        public List<DifferencesFormatting> GetComparerDifferencesFormattings(string databasePath, int companyProfileId, int? id)
        {
            List<DifferencesFormatting> values;

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getComparerDifferencesFormattings(connection, companyProfileId, id);
                }
                finally
                {
                    connection.Close();
                }
            }


            return values;
        }
        private static List<DifferencesFormatting> getComparerDifferencesFormattings(SQLiteConnection connection, int companyProfileId, int? id)
        {
            var values = new List<DifferencesFormatting>();

            if (companyProfileId <= -1)
                throw new Exception(" comparer id cannot be null!");
            var sqlQuery = "SELECT * FROM CompanyProfileComparerSettingStyles";
            sqlQuery += " WHERE company_profile_id = @company_profile_id";
            if (id != null)
                sqlQuery += " AND id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));


            cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;
            if (id != null & id.HasValue)
                cmdQuery.Parameters["@id"].Value = id;



            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new DifferencesFormatting
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]),
                            Name = (DifferencesFormatting.StyleName)Enum.Parse(
                                typeof(DifferencesFormatting.StyleName), rdrSelect["style_name"].ToString(), true),
                            StyleBold = rdrSelect["style_bold"].ToString(),
                            StyleItalic = rdrSelect["style_italic"].ToString(),
                            StyleStrikethrough = rdrSelect["style_strikethrough"].ToString(),
                            StyleUnderline = rdrSelect["style_underline"].ToString(),
                            TextPosition = rdrSelect["text_position"].ToString(),
                            FontSpecifyColor = Convert.ToBoolean(rdrSelect["font_specify_color"]),
                            FontColor = rdrSelect["font_color"].ToString(),
                            FontSpecifyBackroundColor = Convert.ToBoolean(rdrSelect["font_specify_backround_color"]),
                            FontBackroundColor = rdrSelect["font_backround_color"].ToString()
                        };


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table CompanyProfileComparerSettingStyles!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateComparerDifferencesFormatting(string databasePath, DifferencesFormatting differencesFormatting)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createComparerDifferencesFormatting(connection, differencesFormatting);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createComparerDifferencesFormatting(SQLiteConnection connection, DifferencesFormatting differencesFormatting)
        {
            var value = -1;

            try
            {
                if (differencesFormatting.CompanyProfileId <= -1)
                    throw new Exception("The compan profile id cannot be null!");

                var sqlQuery = "INSERT INTO CompanyProfileComparerSettingStyles";
                sqlQuery += " (";
                sqlQuery += " company_profile_id";
                sqlQuery += ", style_name";
                sqlQuery += ", style_bold";
                sqlQuery += ", style_italic";
                sqlQuery += ", style_strikethrough";
                sqlQuery += ", style_underline";
                sqlQuery += ", text_position";
                sqlQuery += ", font_specify_color";
                sqlQuery += ", font_color";
                sqlQuery += ", font_specify_backround_color";
                sqlQuery += ", font_backround_color";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @company_profile_id";
                sqlQuery += ", @style_name";
                sqlQuery += ", @style_bold";
                sqlQuery += ", @style_italic";
                sqlQuery += ", @style_strikethrough";
                sqlQuery += ", @style_underline";
                sqlQuery += ", @text_position";
                sqlQuery += ", @font_specify_color";
                sqlQuery += ", @font_color";
                sqlQuery += ", @font_specify_backround_color";
                sqlQuery += ", @font_backround_color";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@style_name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@style_bold", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@style_italic", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@style_strikethrough", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@style_underline", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@text_position", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@font_specify_color", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@font_color", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@font_specify_backround_color", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@font_backround_color", DbType.String));


                cmdQuery.Parameters["@company_profile_id"].Value = differencesFormatting.CompanyProfileId;
                cmdQuery.Parameters["@style_name"].Value = differencesFormatting.Name.ToString();
                cmdQuery.Parameters["@style_bold"].Value = differencesFormatting.StyleBold;
                cmdQuery.Parameters["@style_italic"].Value = differencesFormatting.StyleItalic;
                cmdQuery.Parameters["@style_strikethrough"].Value = differencesFormatting.StyleStrikethrough;
                cmdQuery.Parameters["@style_underline"].Value = differencesFormatting.StyleUnderline;
                cmdQuery.Parameters["@text_position"].Value = differencesFormatting.TextPosition;
                cmdQuery.Parameters["@font_specify_color"].Value = differencesFormatting.FontSpecifyColor;
                cmdQuery.Parameters["@font_color"].Value = differencesFormatting.FontColor;
                cmdQuery.Parameters["@font_specify_backround_color"].Value = differencesFormatting.FontSpecifyBackroundColor;
                cmdQuery.Parameters["@font_backround_color"].Value = differencesFormatting.FontBackroundColor;


                differencesFormatting.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = differencesFormatting.Id;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateComparerDifferencesFormatting(string databasePath, DifferencesFormatting differencesFormatting)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = UdpateComparerDifferencesFormatting(connection, differencesFormatting);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool UdpateComparerDifferencesFormatting(SQLiteConnection connection, DifferencesFormatting differencesFormatting)
        {

            if (differencesFormatting.CompanyProfileId <= -1 || differencesFormatting.Id <= -1)
                throw new Exception("The comparer id cannot be null! - company comparer styles");

            var sqlQuery = "UPDATE CompanyProfileComparerSettingStyles";
            sqlQuery += " SET";
            sqlQuery += "  company_profile_id = @company_profile_id ";
            sqlQuery += ", style_name = @style_name";
            sqlQuery += ", style_bold = @style_bold";
            sqlQuery += ", style_italic = @style_italic";
            sqlQuery += ", style_strikethrough = @style_strikethrough";
            sqlQuery += ", style_underline = @style_underline";
            sqlQuery += ", text_position = @text_position";
            sqlQuery += ", font_specify_color = @font_specify_color";
            sqlQuery += ", font_color = @font_color";
            sqlQuery += ", font_specify_backround_color = @font_specify_backround_color";
            sqlQuery += ", font_backround_color = @font_backround_color";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@style_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@style_bold", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@style_italic", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@style_strikethrough", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@style_underline", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@text_position", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@font_specify_color", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@font_color", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@font_specify_backround_color", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@font_backround_color", DbType.String));

            cmdQuery.Parameters["@id"].Value = differencesFormatting.Id;
            cmdQuery.Parameters["@company_profile_id"].Value = differencesFormatting.CompanyProfileId;
            cmdQuery.Parameters["@style_name"].Value = differencesFormatting.Name.ToString();
            cmdQuery.Parameters["@style_bold"].Value = differencesFormatting.StyleBold;
            cmdQuery.Parameters["@style_italic"].Value = differencesFormatting.StyleItalic;
            cmdQuery.Parameters["@style_strikethrough"].Value = differencesFormatting.StyleStrikethrough;
            cmdQuery.Parameters["@style_underline"].Value = differencesFormatting.StyleUnderline;
            cmdQuery.Parameters["@text_position"].Value = differencesFormatting.TextPosition;
            cmdQuery.Parameters["@font_specify_color"].Value = differencesFormatting.FontSpecifyColor;
            cmdQuery.Parameters["@font_color"].Value = differencesFormatting.FontColor;
            cmdQuery.Parameters["@font_specify_backround_color"].Value = differencesFormatting.FontSpecifyBackroundColor;
            cmdQuery.Parameters["@font_backround_color"].Value = differencesFormatting.FontBackroundColor;


            cmdQuery.ExecuteNonQuery();


            return true;
        }

        public int SaveComparerDifferencesFormatting(string databasePath, DifferencesFormatting differencesFormatting)
        {
            var value = -1;

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    value = saveComparerDifferencesFormatting(connection, differencesFormatting);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int saveComparerDifferencesFormatting(SQLiteConnection connection, DifferencesFormatting differencesFormatting)
        {
            var value = differencesFormatting.Id;

            var differencesFormattings = getComparerDifferencesFormattings(connection, differencesFormatting.CompanyProfileId, null);


            var exists = false;
            var isDifferent = false;

            if (differencesFormattings.Count > 0)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(differencesFormatting, differencesFormattings.Find(a => a.Id == differencesFormatting.Id)))
                    isDifferent = true;
            }


            if (!exists && differencesFormatting.Id == -1)
            {
                differencesFormatting.Id = createComparerDifferencesFormatting(connection, differencesFormatting);
            }
            else if (isDifferent)
            {
                UdpateComparerDifferencesFormatting(connection, differencesFormatting);
                value = differencesFormatting.Id;
            }




            return value;
        }

        public bool DeleteComparerDifferencesFormatting(string databasePath, int companyProfileId)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteComparerDifferencesFormatting(connection, companyProfileId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteComparerDifferencesFormatting(SQLiteConnection connection, int companyProfileId)
        {
            var success = false;

            try
            {
                if (companyProfileId <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM CompanyProfileComparerSettingStyles";
                sqlQuery += " WHERE company_profile_id = @company_profile_id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters["@company_profile_id"].Value = companyProfileId;

                cmdQuery.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion


        #region  |  Quality Metric Groups  |

        public List<QualityMetricGroup> GetQualityMetricGroupSettings(string databasePath, int? id)
        {
            List<QualityMetricGroup> values;

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getQualityMetricGroupSettings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }


            return values;
        }
        private static List<QualityMetricGroup> getQualityMetricGroupSettings(SQLiteConnection connection, int? id)
        {
            var values = new List<QualityMetricGroup>();

            var sqlQuery = "SELECT * FROM QualityMetricGroups";
            if (id != null)
                sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            if (id != null)
            {
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id.Value;
            }


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new QualityMetricGroup
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            IsDefault = Convert.ToBoolean(rdrSelect["is_default"]),
                            Description = rdrSelect["description"].ToString(),
                            MaxSeverityValue = Convert.ToInt32(rdrSelect["max_severity_value"]),
                            MaxSeverityInValue = Convert.ToInt32(rdrSelect["max_severity_in_value"]),
                            MaxSeverityInType = rdrSelect["max_severity_in_type"].ToString()
                        };

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table QualityMetricGroups!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            //get the associated severity instances
            foreach (var qmg in values)
            {
                qmg.Metrics = getQualityMetricSettings(connection, qmg.Id, null);
                qmg.Severities = getQualityMetricSeveritySettings(connection, qmg.Id, null);
            }


            return values;
        }

        public int CreateQualityMetricGroupSetting(string databasePath, QualityMetricGroup qualityMetricGroup)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createQualityMetricGroupSetting(connection, qualityMetricGroup);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createQualityMetricGroupSetting(SQLiteConnection connection, QualityMetricGroup qualityMetricGroup)
        {
            var value = -1;

            try
            {
                if (qualityMetricGroup.Name.Trim() == string.Empty)
                    throw new Exception("The quality metric Group name cannot be null!");

                var sqlQuery = "INSERT INTO QualityMetricGroups";
                sqlQuery += " (";
                sqlQuery += " name";
                sqlQuery += ", description";
                sqlQuery += ", max_severity_value";
                sqlQuery += ", max_severity_in_value";
                sqlQuery += ", max_severity_in_type";
                sqlQuery += ", is_default";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @name";
                sqlQuery += ", @description";
                sqlQuery += ", @max_severity_value";
                sqlQuery += ", @max_severity_in_value";
                sqlQuery += ", @max_severity_in_type";
                sqlQuery += ", @is_default";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_value", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_value", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_type", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@is_default", DbType.Boolean));


                cmdQuery.Parameters["@name"].Value = qualityMetricGroup.Name;
                cmdQuery.Parameters["@description"].Value = qualityMetricGroup.Description;
                cmdQuery.Parameters["@max_severity_value"].Value = qualityMetricGroup.MaxSeverityValue;
                cmdQuery.Parameters["@max_severity_in_value"].Value = qualityMetricGroup.MaxSeverityInValue;
                cmdQuery.Parameters["@max_severity_in_type"].Value = qualityMetricGroup.MaxSeverityInType;
                cmdQuery.Parameters["@is_default"].Value = qualityMetricGroup.IsDefault;

                qualityMetricGroup.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = qualityMetricGroup.Id;

                foreach (var severity in qualityMetricGroup.Severities)
                {
                    severity.GroupId = qualityMetricGroup.Id;
                    severity.Id = createQualityMetricSeveritySetting(connection, severity);
                }

                foreach (var metric in qualityMetricGroup.Metrics)
                {
                    metric.GroupId = qualityMetricGroup.Id;
                    metric.Id = createQualityMetricSetting(connection, metric);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateQualityMetricGroupSetting(string databasePath, QualityMetricGroup qualityMetricGroup)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateQualityMetricGroupSetting(connection, qualityMetricGroup);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private bool updateQualityMetricGroupSetting(SQLiteConnection connection, QualityMetricGroup qualityMetricGroup)
        {
            var value = false;

            try
            {
                if (qualityMetricGroup.Id <= -1)
                    throw new Exception("The quality metric id cannot be null!");

                var sqlQuery = "UPDATE QualityMetricGroups";
                sqlQuery += " SET";
                sqlQuery += " name = @name";
                sqlQuery += ", description = @description";
                sqlQuery += ", max_severity_value = @max_severity_value";
                sqlQuery += ", max_severity_in_value = @max_severity_in_value";
                sqlQuery += ", max_severity_in_type = @max_severity_in_type";
                sqlQuery += ", is_default = @is_default";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_value", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_value", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_type", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@is_default", DbType.Boolean));

                cmdQuery.Parameters["@id"].Value = qualityMetricGroup.Id;
                cmdQuery.Parameters["@name"].Value = qualityMetricGroup.Name;
                cmdQuery.Parameters["@description"].Value = qualityMetricGroup.Description;
                cmdQuery.Parameters["@max_severity_value"].Value = qualityMetricGroup.MaxSeverityValue;
                cmdQuery.Parameters["@max_severity_in_value"].Value = qualityMetricGroup.MaxSeverityInValue;
                cmdQuery.Parameters["@max_severity_in_type"].Value = qualityMetricGroup.MaxSeverityInType;
                cmdQuery.Parameters["@is_default"].Value = qualityMetricGroup.IsDefault;

                cmdQuery.ExecuteNonQuery();
                value = true;


                foreach (var severity in qualityMetricGroup.Severities)
                    severity.GroupId = qualityMetricGroup.Id;

                foreach (var metric in qualityMetricGroup.Metrics)
                    metric.GroupId = qualityMetricGroup.Id;


                saveQualityMetricSeveritySettings(connection, qualityMetricGroup.Severities);
                saveQualityMetricSettings(connection, qualityMetricGroup.Metrics);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public List<int> SaveQualityMetricGroupSettings(string databasePath, List<QualityMetricGroup> qualityMetricGroups)
        {
            var values = new List<int>();
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        values = saveQualityMetricGroupSettings(connection, qualityMetricGroups);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private List<int> saveQualityMetricGroupSettings(SQLiteConnection connection, List<QualityMetricGroup> qualityMetricGroups)
        {
            var values = new List<int>();
            try
            {
                //PH 30-04-2015;
                //need to figure out what was added, updated & deleted here...      
                #region  |  find out what was added, updated & deleted   |

                var itemsList = new List<ItemReflection>();
                List<QualityMetricGroup> existingItems = existingItems = getQualityMetricGroupSettings(connection, null);


                //find out what is new or updated
                foreach (var metric in qualityMetricGroups)
                {


                    if (metric.Id > -1 && existingItems.Exists(a => a.Id == metric.Id))
                    {
                        //item *could* be updated
                        //if it is 'absolutely' different then add it to the list                            
                        if (!Helper.AreObjectsEqual(metric, existingItems.Find(a => a.Id == metric.Id)))
                            itemsList.Add(new ItemReflection<QualityMetricGroup>(metric
                            , ItemReflection<QualityMetricGroup>.State.updated));
                    }
                    else
                    {
                        //item *is* new
                        itemsList.Add(new ItemReflection<QualityMetricGroup>(metric
                            , ItemReflection<QualityMetricGroup>.State.created));
                    }
                }

                //find out what was deleted
                foreach (var existingItem in existingItems)
                {
                    if (!qualityMetricGroups.Exists(a => a.Id == existingItem.Id))
                    {
                        //item "is" deleted
                        itemsList.Add(new ItemReflection<QualityMetricGroup>(existingItem
                           , ItemReflection<QualityMetricGroup>.State.deleted));
                    }
                }

                #endregion

                foreach (var tr in itemsList)
                {
                    var c = tr as ItemReflection<QualityMetricGroup>;

                    var metric = c.item;
                    switch (c.itemState)
                    {
                        case ItemReflection<QualityMetricGroup>.State.created:
                            {
                                values.Add(createQualityMetricGroupSetting(connection, metric));
                                break;
                            }
                        case ItemReflection<QualityMetricGroup>.State.updated:
                            {
                                updateQualityMetricGroupSetting(connection, metric);
                                values.Add(metric.Id);
                                break;
                            }
                        case ItemReflection<QualityMetricGroup>.State.deleted:
                            {
                                deleteQualityMetricGroupSetting(connection, metric.Id);
                                values.Add(-1);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return values;
        }

        public bool DeleteQualityMetricGroupSetting(string databasePath, int id)
        {
            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteQualityMetricGroupSetting(connection, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteQualityMetricGroupSetting(SQLiteConnection connection, int id)
        {
            var success = false;

            try
            {
                if (id <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM QualityMetricGroups";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id;

                cmdQuery.ExecuteNonQuery();
                success = true;


                deleteQualityMetricSetting(connection, id, null);
                deleteQualityMetricSeveritySetting(connection, id, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion
        #region  |  Quality Metrics  |

        public List<QualityMetric> GetQualityMetricSettings(string databasePath, int groupId, int? id)
        {
            var values = new List<QualityMetric>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getQualityMetricSettings(connection, groupId, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<QualityMetric> getQualityMetricSettings(SQLiteConnection connection, int groupId, int? id)
        {
            var values = new List<QualityMetric>();

            var sqlQuery = "SELECT * FROM QualityMetrics";
            sqlQuery += " WHERE group_id = @group_id";
            if (id != null)
                sqlQuery += " AND id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@group_id"].Value = groupId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id.Value;


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new QualityMetric
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            GroupId = Convert.ToInt32(rdrSelect["group_id"]),
                            Name = rdrSelect["name"].ToString(),
                            Description = rdrSelect["description"].ToString(),
                            Modifed = Helper.DateTimeFromSQLite(rdrSelect["modified"].ToString())
                        };

                        value.MetricSeverity = new Severity(Convert.ToInt32(rdrSelect["severity_id"]), value.GroupId);
                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table QualityMetrics!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            //get the associated severity instances
            foreach (var qm in values)
            {
                if (qm.MetricSeverity.Id <= -1) continue;
                var severities = getQualityMetricSeveritySettings(connection, qm.GroupId, qm.MetricSeverity.Id);
                if (severities.Count > 0)
                    qm.MetricSeverity = severities[0];
            }


            return values;
        }

        public int CreateQualityMetricSetting(string databasePath, QualityMetric qualityMetric)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createQualityMetricSetting(connection, qualityMetric);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createQualityMetricSetting(SQLiteConnection connection, QualityMetric qualityMetric)
        {
            var value = -1;

            try
            {
                if (qualityMetric.Name.Trim() == string.Empty)
                    throw new Exception("The quality metric name cannot be null!");

                var sqlQuery = "INSERT INTO QualityMetrics";
                sqlQuery += " (";
                sqlQuery += " group_id";
                sqlQuery += ", name";
                sqlQuery += ", description";
                sqlQuery += ", modified";
                sqlQuery += ", severity_id";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @group_id";
                sqlQuery += ", @name";
                sqlQuery += ", @description";
                sqlQuery += ", @modified";
                sqlQuery += ", @severity_id";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@modified", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@severity_id", DbType.Int32));

                if (qualityMetric.Modifed == null)
                    qualityMetric.Modifed = DateTime.Now;

                cmdQuery.Parameters["@group_id"].Value = qualityMetric.GroupId;
                cmdQuery.Parameters["@name"].Value = qualityMetric.Name;
                cmdQuery.Parameters["@description"].Value = qualityMetric.Description;
                cmdQuery.Parameters["@modified"].Value = Helper.DateTimeToSQLite(qualityMetric.Modifed);
                cmdQuery.Parameters["@severity_id"].Value = qualityMetric.MetricSeverity.Id;

                qualityMetric.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = qualityMetric.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateQualityMetricSetting(string databasePath, QualityMetric qualityMetric)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateQualityMetricSetting(connection, qualityMetric);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateQualityMetricSetting(SQLiteConnection connection, QualityMetric qualityMetric)
        {


            if (qualityMetric.Id <= -1)
                throw new Exception("The quality metric id cannot be null!");

            var sqlQuery = "UPDATE QualityMetrics";
            sqlQuery += " SET";
            sqlQuery += " group_id = @group_id";
            sqlQuery += ", name = @name";
            sqlQuery += ", description = @description";
            sqlQuery += ", modified = @modified";
            sqlQuery += ", severity_id = @severity_id";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@modified", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@severity_id", DbType.Int32));

            if (qualityMetric.Modifed == null)
                qualityMetric.Modifed = DateTime.Now;

            cmdQuery.Parameters["@id"].Value = qualityMetric.Id;
            cmdQuery.Parameters["@group_id"].Value = qualityMetric.GroupId;
            cmdQuery.Parameters["@name"].Value = qualityMetric.Name;
            cmdQuery.Parameters["@description"].Value = qualityMetric.Description;
            cmdQuery.Parameters["@modified"].Value = Helper.DateTimeToSQLite(qualityMetric.Modifed);
            cmdQuery.Parameters["@severity_id"].Value = qualityMetric.MetricSeverity.Id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public List<int> SaveQualityMetricSettings(string databasePath, List<QualityMetric> qualityMetrics)
        {
            List<int> values;


            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveQualityMetricSettings(connection, qualityMetrics);
                }
                finally
                {
                    connection.Close();
                }
            }


            return values;
        }
        private static List<int> saveQualityMetricSettings(SQLiteConnection connection, List<QualityMetric> qualityMetrics)
        {
            var values = new List<int>();

            var itemsList = new List<ItemReflection>();

            if (qualityMetrics.Count <= 0) return values;
            //PH 30-04-2015;
            //need to figure out what was added, updated & deleted here...      
            #region  |  find out what was added, updated & deleted   |

            var existingItems = getQualityMetricSettings(connection, qualityMetrics[0].GroupId, null);

            //find out what is new or updated
            foreach (var metric in qualityMetrics)
            {
                if (existingItems.Exists(a => a.Id == metric.Id))
                {
                    //item *could* be updated
                    //if it is 'absolutely' different then add it to the list                            
                    if (!Helper.AreObjectsEqual(metric, existingItems.Find(a => a.Id == metric.Id)))
                        itemsList.Add(new ItemReflection<QualityMetric>(metric
                            , ItemReflection<QualityMetric>.State.updated));
                }
                else
                {
                    //item *is* new
                    itemsList.Add(new ItemReflection<QualityMetric>(metric
                        , ItemReflection<QualityMetric>.State.created));
                }
            }

            //find out what was deleted
            itemsList.AddRange((from existingItem in existingItems
                                where !qualityMetrics.Exists(a => a.Id == existingItem.Id)
                                select new ItemReflection<QualityMetric>(existingItem, ItemReflection<QualityMetric>.State.deleted))
                .Cast<ItemReflection>());

            #endregion

            foreach (var tr in itemsList)
            {
                var c = tr as ItemReflection<QualityMetric>;

                var metric = c.item;
                switch (c.itemState)
                {
                    case ItemReflection<QualityMetric>.State.created:
                        {
                            values.Add(createQualityMetricSetting(connection, metric));
                            break;
                        }
                    case ItemReflection<QualityMetric>.State.updated:
                        {
                            updateQualityMetricSetting(connection, metric);
                            values.Add(metric.Id);
                            break;
                        }
                    case ItemReflection<QualityMetric>.State.deleted:
                        {
                            deleteQualityMetricSetting(connection, metric.GroupId, metric.Id);
                            values.Add(-1);
                            break;
                        }
                }
            }


            return values;
        }

        public bool DeleteQualityMetricSetting(string databasePath, int groupId, int? id)
        {
            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteQualityMetricSetting(connection, groupId, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteQualityMetricSetting(SQLiteConnection connection, int groupId, int? id)
        {

            if (id <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "DELETE FROM QualityMetrics";
            sqlQuery += " WHERE group_id = @group_id";
            if (id != null)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@group_id"].Value = groupId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id.Value;

            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion
        #region  |  Quality Metrics Severity  |

        public List<Severity> GetQualityMetricSeveritySettings(string databasePath, int groupId, int? id)
        {
            var values = new List<Severity>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getQualityMetricSeveritySettings(connection, groupId, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<Severity> getQualityMetricSeveritySettings(SQLiteConnection connection, int groupId, int? id)
        {
            var values = new List<Severity>();

            var sqlQuery = "SELECT * FROM QualityMetricSeverities";
            sqlQuery += " WHERE group_id = @group_id";
            if (id != null)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@group_id"].Value = groupId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id.Value;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new Severity
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            GroupId = Convert.ToInt32(rdrSelect["group_id"]),
                            Name = rdrSelect["name"].ToString(),
                            Value = Convert.ToInt32(rdrSelect["value"])
                        };

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table QualityMetricSeverities!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateQualityMetricSeveritySetting(string databasePath, Severity severity)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createQualityMetricSeveritySetting(connection, severity);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createQualityMetricSeveritySetting(SQLiteConnection connection, Severity severity)
        {
            var value = -1;

            try
            {
                if (severity.Name.Trim() == string.Empty)
                    throw new Exception("The severity name cannot be null!");

                var sqlQuery = "INSERT INTO QualityMetricSeverities";
                sqlQuery += " (";
                sqlQuery += " group_id";
                sqlQuery += ", name";
                sqlQuery += ", value";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @group_id";
                sqlQuery += ", @name";
                sqlQuery += ", @value";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.Int32));

                cmdQuery.Parameters["@group_id"].Value = severity.GroupId;
                cmdQuery.Parameters["@name"].Value = severity.Name;
                cmdQuery.Parameters["@value"].Value = severity.Value;

                severity.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = severity.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateQualityMetricSeveritySetting(string databasePath, Severity severity)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateQualityMetricSeveritySetting(connection, severity);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateQualityMetricSeveritySetting(SQLiteConnection connection, Severity severity)
        {
            var value = false;

            try
            {
                if (severity.Name.Trim() == string.Empty || severity.Id <= -1)
                    throw new Exception("The severity id cannot be null!");

                var sqlQuery = "UPDATE QualityMetricSeverities";
                sqlQuery += " SET";
                sqlQuery += " name = @name";
                sqlQuery += ", group_id = @group_id";
                sqlQuery += ", value = @value";
                sqlQuery += " WHERE id = @id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@value", DbType.Int32));

                cmdQuery.Parameters["@id"].Value = severity.Id;
                cmdQuery.Parameters["@group_id"].Value = severity.GroupId;
                cmdQuery.Parameters["@name"].Value = severity.Name;
                cmdQuery.Parameters["@value"].Value = severity.Value;

                cmdQuery.ExecuteNonQuery();
                value = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public List<int> SaveQualityMetricSeveritySettings(string databasePath, List<Severity> metricSeverities)
        {
            var values = new List<int>();
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        values = saveQualityMetricSeveritySettings(connection, metricSeverities);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<int> saveQualityMetricSeveritySettings(SQLiteConnection connection, List<Severity> metricSeverities)
        {
            var values = new List<int>();
            try
            {
                //PH 30-04-2015;
                //need to figure out what was added, updated & deleted here...      
                #region  |  find out what was added, updated & deleted   |

                var itemsList = new List<ItemReflection>();

                var existingItems = getQualityMetricSeveritySettings(connection, metricSeverities[0].GroupId, null);

                //find out what is new or updated
                foreach (var severity in metricSeverities)
                {
                    if (existingItems.Exists(a => a.Id == severity.Id))
                    {
                        //item *could* be updated
                        //if it is 'absolutely' different then add it to the list                            
                        if (!Helper.AreObjectsEqual(severity, existingItems.Find(a => a.Id == severity.Id)))
                            itemsList.Add(new ItemReflection<Severity>(severity
                            , ItemReflection<Severity>.State.updated));
                    }
                    else
                    {
                        //item *is* new
                        itemsList.Add(new ItemReflection<Severity>(severity
                            , ItemReflection<Severity>.State.created));
                    }
                }

                //find out what was deleted
                foreach (var existingItem in existingItems)
                {
                    if (!metricSeverities.Exists(a => a.Id == existingItem.Id))
                    {
                        //item "is" deleted
                        itemsList.Add(new ItemReflection<Severity>(existingItem
                           , ItemReflection<Severity>.State.deleted));
                    }
                }

                #endregion

                foreach (var tr in itemsList)
                {
                    var c = tr as ItemReflection<Severity>;

                    var severity = c.item;
                    switch (c.itemState)
                    {
                        case ItemReflection<Severity>.State.created:
                            {
                                values.Add(createQualityMetricSeveritySetting(connection, severity));
                                break;
                            }
                        case ItemReflection<Severity>.State.updated:
                            {
                                updateQualityMetricSeveritySetting(connection, severity);
                                values.Add(severity.Id);
                                break;
                            }
                        case ItemReflection<Severity>.State.deleted:
                            {
                                deleteQualityMetricSeveritySetting(connection, severity.GroupId, severity.Id);
                                values.Add(-1);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return values;
        }

        public bool DeleteQualityMetricSeveritySetting(string databasePath, int groupId, int? id)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteQualityMetricSeveritySetting(connection, groupId, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteQualityMetricSeveritySetting(SQLiteConnection connection, int groupId, int? id)
        {
            var success = false;

            try
            {
                if (id <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM QualityMetricSeverities";
                sqlQuery += " WHERE group_id = @group_id";
                if (id != null && id.HasValue)
                    sqlQuery += " AND id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@group_id", DbType.Int32));
                if (id != null && id.HasValue)
                    cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

                cmdQuery.Parameters["@group_id"].Value = groupId;
                if (id != null && id.HasValue)
                    cmdQuery.Parameters["@id"].Value = id.Value;

                cmdQuery.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion


        #region  |  LanguageRateGroup  |

        public List<LanguageRateGroup> GetLanguageRateGroupSetttings(string databasePath, int? id)
        {
            List<LanguageRateGroup> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getLanguageRateGroupSetttings(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<LanguageRateGroup> getLanguageRateGroupSetttings(SQLiteConnection connection, int? id)
        {
            var values = new List<LanguageRateGroup>();

            if (id <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "SELECT * FROM LanguageRateGroups";
            if (id != null)
                sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            if (id != null)
            {
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id;
            }

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new LanguageRateGroup
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            Description = rdrSelect["description"].ToString(),
                            Currency = rdrSelect["currency"].ToString()
                        };



                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table LanguageRateGroups!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            foreach (var value in values)
            {
                value.GroupLanguages = getLanguageRateGroupLanguages(connection, value.Id);
                value.LanguageRates = getLanguageRatesSettings(connection, value.Id);

                //get the default analysis band
                var lrgab = getLanguageRateGroupAnalysisBands(connection, value.Id);
                if (lrgab.Count > 0)
                    value.DefaultAnalysisBand = lrgab[0];
            }


            return values;
        }

        public int CreateLanguageRateGroup(string databasePath, LanguageRateGroup languageRateGroup)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createLanguageRateGroup(connection, languageRateGroup);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private int createLanguageRateGroup(SQLiteConnection connection, LanguageRateGroup languageRateGroup)
        {
            var value = -1;

            try
            {
                if (languageRateGroup.Name.Trim() == string.Empty)
                    throw new Exception("The language rate name cannot be null!");

                var sqlQuery = "INSERT INTO LanguageRateGroups";
                sqlQuery += " (";
                sqlQuery += " name";
                sqlQuery += ", description";
                sqlQuery += ", currency";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @name";
                sqlQuery += ", @description";
                sqlQuery += ", @currency";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@currency", DbType.String));


                cmdQuery.Parameters["@name"].Value = languageRateGroup.Name;
                cmdQuery.Parameters["@description"].Value = languageRateGroup.Description;
                cmdQuery.Parameters["@currency"].Value = languageRateGroup.Currency;

                languageRateGroup.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = languageRateGroup.Id;




                languageRateGroup.DefaultAnalysisBand.LanguageRateId = value;
                languageRateGroup.DefaultAnalysisBand.Id = createLanguageRateGroupAnalysisBand(connection, languageRateGroup.DefaultAnalysisBand);

                foreach (var lrgl in languageRateGroup.GroupLanguages)
                {
                    lrgl.LanguageRateId = value;
                    lrgl.Id = createLanguageRateGroupLanguage(connection, lrgl);
                }


                foreach (var lr in languageRateGroup.LanguageRates)
                {

                    lr.LanguageRateId = value;
                    lr.Id = createLanguageRateSettings(connection, lr);

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateLanguageRateGroup(string databasePath, LanguageRateGroup languageRateGroup)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateLanguageRateGroup(connection, languageRateGroup);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private bool updateLanguageRateGroup(SQLiteConnection connection, LanguageRateGroup languageRateGroup)
        {
            var value = false;

            try
            {
                if (languageRateGroup.Id <= -1)
                    throw new Exception("The language rate id cannot be null!");

                var sqlQuery = "UPDATE LanguageRateGroups";
                sqlQuery += " SET";
                sqlQuery += " name = @name";
                sqlQuery += ", description = @description";
                sqlQuery += ", currency = @currency";
                sqlQuery += " WHERE id = @id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@currency", DbType.String));


                cmdQuery.Parameters["@id"].Value = languageRateGroup.Id;
                cmdQuery.Parameters["@name"].Value = languageRateGroup.Name;
                cmdQuery.Parameters["@description"].Value = languageRateGroup.Description;
                cmdQuery.Parameters["@currency"].Value = languageRateGroup.Currency;

                cmdQuery.ExecuteNonQuery();
                value = true;

                //PH: 30-4-2015
                //ensure that the parent id is assigned correctly to the child elements before
                //attempting to update the table structure
                languageRateGroup.DefaultAnalysisBand.LanguageRateId = languageRateGroup.Id;

                foreach (var lrgl in languageRateGroup.GroupLanguages)
                    lrgl.LanguageRateId = languageRateGroup.Id;

                foreach (var lr in languageRateGroup.LanguageRates)
                    lr.LanguageRateId = languageRateGroup.Id;

                //the save operations will decide if something is new or has been updated || deleted
                saveLanguageRateGroupLanguage(connection, languageRateGroup.GroupLanguages);
                saveLanguageRateGroupAnalysisBand(connection, languageRateGroup.DefaultAnalysisBand);
                saveLanguageRatesSettings(connection, languageRateGroup.LanguageRates);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public List<int> SaveLanguageRateGroups(string databasePath, List<LanguageRateGroup> languageRateGroups)
        {
            var values = new List<int>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        values = saveLanguageRateGroups(connection, languageRateGroups);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private List<int> saveLanguageRateGroups(SQLiteConnection connection, List<LanguageRateGroup> languageRateGroups)
        {
            var values = new List<int>();
            try
            {
                var sqlQuery = "SELECT * FROM LanguageRateGroups";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));


                foreach (var languageRateGroup in languageRateGroups)
                {
                    #region  |  check if already exists  |
                    var exists = false;

                    if (languageRateGroup.Id > -1)
                    {
                        cmdQuery.Parameters["@id"].Value = languageRateGroup.Id;
                        var rdrSelect = cmdQuery.ExecuteReader();
                        try
                        {
                            if (rdrSelect.HasRows)
                            {
                                exists = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error querying the table LanguageRateGroups!  " + ex.Message);
                        }
                        finally
                        {
                            if (rdrSelect != null)
                                if (!rdrSelect.IsClosed)
                                    rdrSelect.Close();
                        }
                    }
                    #endregion

                    if (!exists || languageRateGroup.Id == -1)
                    {
                        values.Add(createLanguageRateGroup(connection, languageRateGroup));
                    }
                    else
                    {
                        var success = updateLanguageRateGroup(connection, languageRateGroup);
                        values.Add(languageRateGroup.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return values;
        }

        public bool DeleteLanguageRateGroup(string databasePath, int id)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteLanguageRateGroup(connection, id);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private bool deleteLanguageRateGroup(SQLiteConnection connection, int id)
        {
            var success = false;
            try
            {
                if (id <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM LanguageRateGroups";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id;

                cmdQuery.ExecuteNonQuery();
                success = true;

                if (success)
                {
                    success = deleteLanguageRateGroupLanguage(connection, id);
                    success = deleteLanguageRateGroupAnalysisBand(connection, id);
                    success = deleteLanguageRateSettings(connection, id, null);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion
        #region  |  LanguageRateGroupLanguages  |

        public List<LanguageRateGroupLanguage> GetLanguageRateGroupLanguages(string databasePath, int languageRateId)
        {
            var values = new List<LanguageRateGroupLanguage>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getLanguageRateGroupLanguages(connection, languageRateId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<LanguageRateGroupLanguage> getLanguageRateGroupLanguages(SQLiteConnection connection, int languageRateId)
        {
            var values = new List<LanguageRateGroupLanguage>();

            if (languageRateId <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "SELECT * FROM LanguageRateGroupLanguages";
            sqlQuery += " WHERE language_rate_id = @language_rate_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new LanguageRateGroupLanguage();

                        //value.
                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.LanguageRateId = Convert.ToInt32(rdrSelect["language_rate_id"]);
                        value.LanguageIdCi = rdrSelect["language_id_ci"].ToString();
                        value.Type = (LanguageRateGroupLanguage.LanguageType)Enum.Parse(
                            typeof(LanguageRateGroupLanguage.LanguageType), rdrSelect["type"].ToString(), true);

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table LanguageRateGroupLanguages!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateLanguageRateGroupLanguage(string databasePath, LanguageRateGroupLanguage languageRateGroupLanguage)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createLanguageRateGroupLanguage(connection, languageRateGroupLanguage);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createLanguageRateGroupLanguage(SQLiteConnection connection, LanguageRateGroupLanguage languageRateGroupLanguage)
        {
            var value = -1;

            try
            {
                if (languageRateGroupLanguage.LanguageRateId <= -1)
                    throw new Exception("The language rate id cannot be null!");

                var sqlQuery = "INSERT INTO LanguageRateGroupLanguages";
                sqlQuery += " (";
                sqlQuery += " language_rate_id";
                sqlQuery += ", language_id_ci";
                sqlQuery += ", type";
                sqlQuery += " ) VALUES (";
                sqlQuery += " @language_rate_id";
                sqlQuery += ", @language_id_ci";
                sqlQuery += ", @type";
                sqlQuery += " ); SELECT last_insert_rowid();";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_id_ci", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@type", DbType.String));


                cmdQuery.Parameters["@language_rate_id"].Value = languageRateGroupLanguage.LanguageRateId;
                cmdQuery.Parameters["@language_id_ci"].Value = languageRateGroupLanguage.LanguageIdCi;
                cmdQuery.Parameters["@type"].Value = languageRateGroupLanguage.Type;


                languageRateGroupLanguage.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                value = languageRateGroupLanguage.Id;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public bool UpdateLanguageRateGroupLanguage(string databasePath, LanguageRateGroupLanguage languageRateGroupLanguage)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateLanguageRateGroupLanguage(connection, languageRateGroupLanguage);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateLanguageRateGroupLanguage(SQLiteConnection connection, LanguageRateGroupLanguage languageRateGroupLanguage)
        {
            var value = false;

            try
            {
                if (languageRateGroupLanguage.LanguageRateId <= -1 || languageRateGroupLanguage.Id <= -1)
                    throw new Exception("The language rate id cannot be null!");

                var sqlQuery = "UPDATE LanguageRateGroupLanguages";
                sqlQuery += " SET";
                sqlQuery += " language_rate_id = @language_rate_id";
                sqlQuery += ", language_id_ci = @language_id_ci";
                sqlQuery += ", type = @type";
                sqlQuery += " WHERE id = @id";


                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_id_ci", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@type", DbType.String));

                cmdQuery.Parameters["@id"].Value = languageRateGroupLanguage.Id;
                cmdQuery.Parameters["@language_rate_id"].Value = languageRateGroupLanguage.LanguageRateId;
                cmdQuery.Parameters["@language_id_ci"].Value = languageRateGroupLanguage.LanguageIdCi;
                cmdQuery.Parameters["@type"].Value = languageRateGroupLanguage.Type;


                cmdQuery.ExecuteNonQuery();
                value = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }

        public List<int> SaveLanguageRateGroupLanguage(string databasePath, List<LanguageRateGroupLanguage> languageRateGroupLanguages)
        {
            var values = new List<int>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        values = saveLanguageRateGroupLanguage(connection, languageRateGroupLanguages);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private List<int> saveLanguageRateGroupLanguage(SQLiteConnection connection, List<LanguageRateGroupLanguage> languageRateGroupLanguages)
        {
            var values = new List<int>();
            try
            {
                //PH 30-04-2015;
                //need to figure out what was added, updated & deleted here... 
                #region  |  find out what was added, updated & deleted   |

                var itemsList = new List<ItemReflection>();

                //get a list of the parent grouping id
                var itemParentIds = new List<int>();
                foreach (var languageRateGroupLanguage in languageRateGroupLanguages)
                {
                    if (!itemParentIds.Contains(languageRateGroupLanguage.LanguageRateId))
                        itemParentIds.Add(languageRateGroupLanguage.LanguageRateId);
                }

                //get the list of items in the database that have the parent grouping id (i.e. language_rate_id)
                foreach (var itemParentId in itemParentIds)
                {
                    var existingItems = getLanguageRateGroupLanguages(connection, itemParentId);

                    //find out what was is new or updated
                    foreach (var languageRateGroupLanguage in languageRateGroupLanguages)
                    {
                        if (existingItems.Exists(a => a.Id == languageRateGroupLanguage.Id))
                        {
                            //item *could* be updated
                            //if it is 'absolutely' different then add it to the list                            
                            if (!Helper.AreObjectsEqual(languageRateGroupLanguage, existingItems.Find(a => a.Id == languageRateGroupLanguage.Id)))
                                itemsList.Add(new ItemReflection<LanguageRateGroupLanguage>(languageRateGroupLanguage
                                    , ItemReflection<LanguageRateGroupLanguage>.State.updated));
                        }
                        else
                        {
                            //item *is* new
                            itemsList.Add(new ItemReflection<LanguageRateGroupLanguage>(languageRateGroupLanguage
                                , ItemReflection<LanguageRateGroupLanguage>.State.created));
                        }
                    }

                    //find out what was deleted
                    foreach (var existingItem in existingItems)
                    {
                        if (!languageRateGroupLanguages.Exists(a => a.Id == existingItem.Id))
                        {
                            //item "is" deleted
                            itemsList.Add(new ItemReflection<LanguageRateGroupLanguage>(existingItem
                                 , ItemReflection<LanguageRateGroupLanguage>.State.deleted));
                        }
                    }
                }


                #endregion

                foreach (var tr in itemsList)
                {
                    var c = tr as ItemReflection<LanguageRateGroupLanguage>;
                    var languageRateGroupLanguage = c.item;
                    switch (c.itemState)
                    {
                        case ItemReflection<LanguageRateGroupLanguage>.State.created:
                            {
                                values.Add(createLanguageRateGroupLanguage(connection, languageRateGroupLanguage));
                                break;
                            }
                        case ItemReflection<LanguageRateGroupLanguage>.State.updated:
                            {
                                updateLanguageRateGroupLanguage(connection, languageRateGroupLanguage);
                                values.Add(languageRateGroupLanguage.Id);
                                break;
                            }
                        case ItemReflection<LanguageRateGroupLanguage>.State.deleted:
                            {
                                deleteLanguageRateGroupLanguage(connection, languageRateGroupLanguage.Id);
                                values.Add(-1);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return values;
        }

        public bool DeleteLanguageRateGroupLanguage(string databasePath, int languageRateId)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteLanguageRateGroupLanguage(connection, languageRateId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteLanguageRateGroupLanguage(SQLiteConnection connection, int languageRateId)
        {
            var success = false;

            try
            {
                if (languageRateId <= -1)
                    throw new Exception("The id cannot be null!");

                var sqlQuery = "DELETE FROM LanguageRateGroupLanguages";
                sqlQuery += " WHERE language_rate_id = @language_rate_id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
                cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;

                cmdQuery.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }


        #endregion
        #region  |  LanguageRateGroupAnalysisBand  |


        public List<LanguageRateGroupAnalysisBand> GetLanguageRateGroupAnalysisBands(string databasePath, int languageRateId)
        {
            var values = new List<LanguageRateGroupAnalysisBand>();
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        values = getLanguageRateGroupAnalysisBands(connection, languageRateId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return values;
        }
        private static List<LanguageRateGroupAnalysisBand> getLanguageRateGroupAnalysisBands(SQLiteConnection connection, int languageRateId)
        {
            var values = new List<LanguageRateGroupAnalysisBand>();

            if (languageRateId <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "SELECT * FROM LanguageRateGroupAnalysisBand";
            sqlQuery += " WHERE language_rate_id = @language_rate_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new LanguageRateGroupAnalysisBand
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            LanguageRateId = Convert.ToInt32(rdrSelect["language_rate_id"]),
                            PercentPm = Convert.ToInt32(rdrSelect["percent_pm"]),
                            PercentCm = Convert.ToInt32(rdrSelect["percent_cm"]),
                            PercentRep = Convert.ToInt32(rdrSelect["percent_rep"]),
                            Percent100 = Convert.ToInt32(rdrSelect["percent_100"]),
                            Percent95 = Convert.ToInt32(rdrSelect["percent_95"]),
                            Percent85 = Convert.ToInt32(rdrSelect["percent_85"]),
                            Percent75 = Convert.ToInt32(rdrSelect["percent_75"]),
                            Percent50 = Convert.ToInt32(rdrSelect["percent_50"]),
                            PercentNew = Convert.ToInt32(rdrSelect["percent_new"])
                        };



                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table LanguageRateGroupAnalysisBand!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            return values;
        }

        public int CreateLanguageRateGroupAnalysisBand(string databasePath, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {
            var value = -1;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = createLanguageRateGroupAnalysisBand(connection, languageRateGroupAnalysisBand);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static int createLanguageRateGroupAnalysisBand(SQLiteConnection connection, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {
            var value = -1;

            if (languageRateGroupAnalysisBand.LanguageRateId <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "INSERT INTO LanguageRateGroupAnalysisBand";
            sqlQuery += " (";
            sqlQuery += " language_rate_id";
            sqlQuery += ", percent_pm";
            sqlQuery += ", percent_cm";
            sqlQuery += ", percent_rep";
            sqlQuery += ", percent_100";
            sqlQuery += ", percent_95";
            sqlQuery += ", percent_85";
            sqlQuery += ", percent_75";
            sqlQuery += ", percent_50";
            sqlQuery += ", percent_new";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @language_rate_id";
            sqlQuery += ", @percent_pm";
            sqlQuery += ", @percent_cm";
            sqlQuery += ", @percent_rep";
            sqlQuery += ", @percent_100";
            sqlQuery += ", @percent_95";
            sqlQuery += ", @percent_85";
            sqlQuery += ", @percent_75";
            sqlQuery += ", @percent_50";
            sqlQuery += ", @percent_new";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_pm", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_cm", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_rep", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_100", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_95", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_85", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_75", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_50", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_new", DbType.Int32));


            cmdQuery.Parameters["@language_rate_id"].Value = languageRateGroupAnalysisBand.LanguageRateId;
            cmdQuery.Parameters["@percent_pm"].Value = languageRateGroupAnalysisBand.PercentPm;
            cmdQuery.Parameters["@percent_cm"].Value = languageRateGroupAnalysisBand.PercentCm;
            cmdQuery.Parameters["@percent_rep"].Value = languageRateGroupAnalysisBand.PercentRep;
            cmdQuery.Parameters["@percent_100"].Value = languageRateGroupAnalysisBand.Percent100;
            cmdQuery.Parameters["@percent_95"].Value = languageRateGroupAnalysisBand.Percent95;
            cmdQuery.Parameters["@percent_85"].Value = languageRateGroupAnalysisBand.Percent85;
            cmdQuery.Parameters["@percent_75"].Value = languageRateGroupAnalysisBand.Percent75;
            cmdQuery.Parameters["@percent_50"].Value = languageRateGroupAnalysisBand.Percent50;
            cmdQuery.Parameters["@percent_new"].Value = languageRateGroupAnalysisBand.PercentNew;


            languageRateGroupAnalysisBand.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = languageRateGroupAnalysisBand.Id;



            return value;
        }

        public bool UpdateLanguageRateGroupAnalysisBand(string databasePath, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {
            var value = false;
            try
            {

                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = updateLanguageRateGroupAnalysisBand(connection, languageRateGroupAnalysisBand);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return value;
        }
        private static bool updateLanguageRateGroupAnalysisBand(SQLiteConnection connection, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {

            if (languageRateGroupAnalysisBand.LanguageRateId <= -1 || languageRateGroupAnalysisBand.Id <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "UPDATE LanguageRateGroupAnalysisBand";
            sqlQuery += " SET";
            sqlQuery += " language_rate_id = @language_rate_id";
            sqlQuery += ", percent_pm = @percent_pm";
            sqlQuery += ", percent_cm = @percent_cm";
            sqlQuery += ", percent_rep = @percent_rep";
            sqlQuery += ", percent_100 = @percent_100";
            sqlQuery += ", percent_95 = @percent_95";
            sqlQuery += ", percent_85 = @percent_85";
            sqlQuery += ", percent_75 = @percent_75";
            sqlQuery += ", percent_50 = @percent_50";
            sqlQuery += ", percent_new = @percent_new";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_pm", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_cm", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_rep", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_100", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_95", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_85", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_75", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_50", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@percent_new", DbType.Int32));


            cmdQuery.Parameters["@id"].Value = languageRateGroupAnalysisBand.Id;
            cmdQuery.Parameters["@language_rate_id"].Value = languageRateGroupAnalysisBand.LanguageRateId;
            cmdQuery.Parameters["@percent_pm"].Value = languageRateGroupAnalysisBand.PercentPm;
            cmdQuery.Parameters["@percent_cm"].Value = languageRateGroupAnalysisBand.PercentCm;
            cmdQuery.Parameters["@percent_rep"].Value = languageRateGroupAnalysisBand.PercentRep;
            cmdQuery.Parameters["@percent_100"].Value = languageRateGroupAnalysisBand.Percent100;
            cmdQuery.Parameters["@percent_95"].Value = languageRateGroupAnalysisBand.Percent95;
            cmdQuery.Parameters["@percent_85"].Value = languageRateGroupAnalysisBand.Percent85;
            cmdQuery.Parameters["@percent_75"].Value = languageRateGroupAnalysisBand.Percent75;
            cmdQuery.Parameters["@percent_50"].Value = languageRateGroupAnalysisBand.Percent50;
            cmdQuery.Parameters["@percent_new"].Value = languageRateGroupAnalysisBand.PercentNew;


            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public int SaveLanguageRateGroupAnalysisBand(string databasePath, string viewName, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {
            var value = -1;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();

                    try
                    {
                        value = saveLanguageRateGroupAnalysisBand(connection, languageRateGroupAnalysisBand);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static int saveLanguageRateGroupAnalysisBand(SQLiteConnection connection, LanguageRateGroupAnalysisBand languageRateGroupAnalysisBand)
        {
            var value = languageRateGroupAnalysisBand.Id;


            var languageRateGroupAnalysisBands = getLanguageRateGroupAnalysisBands(connection, languageRateGroupAnalysisBand.LanguageRateId);


            var exists = false;
            var isDifferent = false;

            if (languageRateGroupAnalysisBands.Count > 0)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(languageRateGroupAnalysisBand, languageRateGroupAnalysisBands.Find(a => a.Id == languageRateGroupAnalysisBand.Id)))
                    isDifferent = true;
            }


            if (!exists && languageRateGroupAnalysisBand.Id == -1)
            {
                value = createLanguageRateGroupAnalysisBand(connection, languageRateGroupAnalysisBand);
            }
            else if (isDifferent)
            {
                updateLanguageRateGroupAnalysisBand(connection, languageRateGroupAnalysisBand);
                value = languageRateGroupAnalysisBand.Id;
            }




            return value;
        }

        public bool DeleteLanguageRateGroupAnalysisBand(string databasePath, int languageRateId)
        {


            var value = false;
            try
            {
                using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
                {
                    connection.Open();
                    try
                    {
                        value = deleteLanguageRateGroupAnalysisBand(connection, languageRateId);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return value;
        }
        private static bool deleteLanguageRateGroupAnalysisBand(SQLiteConnection connection, int languageRateId)
        {

            if (languageRateId <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "DELETE FROM LanguageRateGroupAnalysisBand";
            sqlQuery += " WHERE language_rate_id = @language_rate_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;

            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion
        #region  |  LanguageRate  |

        public List<LanguageRate> GetLanguageRatesSettings(string databasePath, int languageRateId)
        {
            List<LanguageRate> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getLanguageRatesSettings(connection, languageRateId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<LanguageRate> getLanguageRatesSettings(SQLiteConnection connection, int languageRateId)
        {
            var values = new List<LanguageRate>();

            if (languageRateId <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "SELECT * FROM LanguageRates";
            sqlQuery += " WHERE language_rate_id = @language_rate_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new LanguageRate
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            LanguageRateId = Convert.ToInt32(rdrSelect["language_rate_id"]),
                            SourceLanguage = rdrSelect["source_language"].ToString(),
                            TargetLanguage = rdrSelect["target_language"].ToString(),
                            RndType = (RoundType)Enum.Parse(
                                typeof(RoundType), rdrSelect["round_type"].ToString(), true),
                            BaseRate = Convert.ToDecimal(rdrSelect["base_rate"]),
                            RatePm = Convert.ToDecimal(rdrSelect["rate_pm"]),
                            RateCm = Convert.ToDecimal(rdrSelect["rate_cm"]),
                            RateRep = Convert.ToDecimal(rdrSelect["rate_rep"]),
                            Rate100 = Convert.ToDecimal(rdrSelect["rate_100"]),
                            Rate95 = Convert.ToDecimal(rdrSelect["rate_95"]),
                            Rate85 = Convert.ToDecimal(rdrSelect["rate_85"]),
                            Rate75 = Convert.ToDecimal(rdrSelect["rate_75"]),
                            Rate50 = Convert.ToDecimal(rdrSelect["rate_50"]),
                            RateNew = Convert.ToDecimal(rdrSelect["rate_new"])
                        };




                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table LanguageRates!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateLanguageRateSettings(string databasePath, LanguageRate languageRate)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createLanguageRateSettings(connection, languageRate);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createLanguageRateSettings(SQLiteConnection connection, LanguageRate languageRate)
        {
            var value = -1;
            if (languageRate.LanguageRateId <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "INSERT INTO LanguageRates";
            sqlQuery += " (";
            sqlQuery += " language_rate_id";

            sqlQuery += ", round_type";
            sqlQuery += ", base_rate";
            sqlQuery += ", source_language";
            sqlQuery += ", target_language";

            sqlQuery += ", rate_pm";
            sqlQuery += ", rate_cm";
            sqlQuery += ", rate_rep";
            sqlQuery += ", rate_100";
            sqlQuery += ", rate_95";
            sqlQuery += ", rate_85";
            sqlQuery += ", rate_75";
            sqlQuery += ", rate_50";
            sqlQuery += ", rate_new";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @language_rate_id";

            sqlQuery += ", @round_type";
            sqlQuery += ", @base_rate";
            sqlQuery += ", @source_language";
            sqlQuery += ", @target_language";

            sqlQuery += ", @rate_pm";
            sqlQuery += ", @rate_cm";
            sqlQuery += ", @rate_rep";
            sqlQuery += ", @rate_100";
            sqlQuery += ", @rate_95";
            sqlQuery += ", @rate_85";
            sqlQuery += ", @rate_75";
            sqlQuery += ", @rate_50";
            sqlQuery += ", @rate_new";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@round_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@base_rate", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_pm", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_cm", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_rep", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_100", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_95", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_85", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_75", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_50", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_new", DbType.Decimal));


            cmdQuery.Parameters["@language_rate_id"].Value = languageRate.LanguageRateId;
            cmdQuery.Parameters["@round_type"].Value = languageRate.RndType.ToString();
            cmdQuery.Parameters["@base_rate"].Value = languageRate.BaseRate;
            cmdQuery.Parameters["@source_language"].Value = languageRate.SourceLanguage;
            cmdQuery.Parameters["@target_language"].Value = languageRate.TargetLanguage;
            cmdQuery.Parameters["@rate_pm"].Value = languageRate.RatePm;
            cmdQuery.Parameters["@rate_cm"].Value = languageRate.RateCm;
            cmdQuery.Parameters["@rate_rep"].Value = languageRate.RateRep;
            cmdQuery.Parameters["@rate_100"].Value = languageRate.Rate100;
            cmdQuery.Parameters["@rate_95"].Value = languageRate.Rate95;
            cmdQuery.Parameters["@rate_85"].Value = languageRate.Rate85;
            cmdQuery.Parameters["@rate_75"].Value = languageRate.Rate75;
            cmdQuery.Parameters["@rate_50"].Value = languageRate.Rate50;
            cmdQuery.Parameters["@rate_new"].Value = languageRate.RateNew;


            languageRate.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = languageRate.Id;

            return value;
        }

        public bool UpdateLanguageRateSettings(string databasePath, LanguageRate languageRate)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateLanguageRateSettings(connection, languageRate);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateLanguageRateSettings(SQLiteConnection connection, LanguageRate languageRate)
        {


            if (languageRate.LanguageRateId <= -1 || languageRate.Id <= -1)
                throw new Exception("The language rate id cannot be null!");

            var sqlQuery = "UPDATE LanguageRates";
            sqlQuery += " SET";
            sqlQuery += " language_rate_id = @language_rate_id";

            sqlQuery += ", round_type = @round_type";
            sqlQuery += ", base_rate = @base_rate";
            sqlQuery += ", source_language = @source_language";
            sqlQuery += ", target_language = @target_language";

            sqlQuery += ", rate_pm = @rate_pm";
            sqlQuery += ", rate_cm = @rate_cm";
            sqlQuery += ", rate_rep = @rate_rep";
            sqlQuery += ", rate_100 = @rate_100";
            sqlQuery += ", rate_95 = @rate_95";
            sqlQuery += ", rate_85 = @rate_85";
            sqlQuery += ", rate_75 = @rate_75";
            sqlQuery += ", rate_50 = @rate_50";
            sqlQuery += ", rate_new = @rate_new";
            sqlQuery += " WHERE id = @id";



            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@round_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@base_rate", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_pm", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_cm", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_rep", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_100", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_95", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_85", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_75", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_50", DbType.Decimal));
            cmdQuery.Parameters.Add(new SQLiteParameter("@rate_new", DbType.Decimal));


            cmdQuery.Parameters["@id"].Value = languageRate.Id;
            cmdQuery.Parameters["@language_rate_id"].Value = languageRate.LanguageRateId;
            cmdQuery.Parameters["@round_type"].Value = languageRate.RndType.ToString();
            cmdQuery.Parameters["@base_rate"].Value = languageRate.BaseRate;
            cmdQuery.Parameters["@source_language"].Value = languageRate.SourceLanguage;
            cmdQuery.Parameters["@target_language"].Value = languageRate.TargetLanguage;
            cmdQuery.Parameters["@rate_pm"].Value = languageRate.RatePm;
            cmdQuery.Parameters["@rate_cm"].Value = languageRate.RateCm;
            cmdQuery.Parameters["@rate_rep"].Value = languageRate.RateRep;
            cmdQuery.Parameters["@rate_100"].Value = languageRate.Rate100;
            cmdQuery.Parameters["@rate_95"].Value = languageRate.Rate95;
            cmdQuery.Parameters["@rate_85"].Value = languageRate.Rate85;
            cmdQuery.Parameters["@rate_75"].Value = languageRate.Rate75;
            cmdQuery.Parameters["@rate_50"].Value = languageRate.Rate50;
            cmdQuery.Parameters["@rate_new"].Value = languageRate.RateNew;


            cmdQuery.ExecuteNonQuery();


            return true;
        }

        public List<int> SaveLanguageRatesSettings(string databasePath, List<LanguageRate> languageRates)
        {
            List<int> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    values = saveLanguageRatesSettings(connection, languageRates);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<int> saveLanguageRatesSettings(SQLiteConnection connection, List<LanguageRate> languageRates)
        {
            var values = new List<int>();
            //PH 30-04-2015;
            //need to figure out what was added, updated & deleted here...
            //this will open some local memory but it is easier to handle this here as opposed to
            //depending on remembering to handle this from the GUI level :-); obviously this can only be
            //done when we know that there are a limited amount of items in the database associated with the
            //parent id.
            //created an abstract generic class to help with the reflection becuase I want to resue 
            //for the other methods
            #region  |  find out what was added, updated & deleted   |

            var itemsList = new List<ItemReflection>();

            //get a list of the parent grouping id (i.e. language_rate_id)  
            var itemParentIds = new List<int>();
            foreach (var languageRate in languageRates)
            {
                if (!itemParentIds.Contains(languageRate.LanguageRateId))
                    itemParentIds.Add(languageRate.LanguageRateId);
            }

            //get the list of items in the database that have the parent grouping id (i.e. language_rate_id)
            foreach (var itemParentId in itemParentIds)
            {

                var existingItems = getLanguageRatesSettings(connection, itemParentId);


                //find out what is new or updated
                foreach (var languageRate in languageRates)
                {
                    if (existingItems.Exists(a => a.Id == languageRate.Id))
                    {
                        //item *could* be updated
                        //if it is 'absolutely' different then add it to the list                            
                        if (!Helper.AreObjectsEqual(languageRate, existingItems.Find(a => a.Id == languageRate.Id)))
                            itemsList.Add(new ItemReflection<LanguageRate>(languageRate
                                , ItemReflection<LanguageRate>.State.updated));
                    }
                    else
                    {
                        //item *is* new
                        itemsList.Add(new ItemReflection<LanguageRate>(languageRate
                            , ItemReflection<LanguageRate>.State.created));
                    }
                }

                //find out what was deleted
                itemsList.AddRange((from existingItem in existingItems
                                    where !languageRates.Exists(a => a.Id == existingItem.Id)
                                    select new ItemReflection<LanguageRate>(existingItem,
                                        ItemReflection<LanguageRate>.State.deleted)).Cast<ItemReflection>());
            }


            #endregion

            foreach (var tr in itemsList)
            {
                var c = tr as ItemReflection<LanguageRate>;

                if (c == null)
                    continue;
                var languageRate = c.item;
                switch (c.itemState)
                {
                    case ItemReflection<LanguageRate>.State.created:
                        {
                            values.Add(createLanguageRateSettings(connection, languageRate));
                            break;
                        }
                    case ItemReflection<LanguageRate>.State.updated:
                        {
                            updateLanguageRateSettings(connection, languageRate);
                            values.Add(languageRate.Id);
                            break;
                        }
                    case ItemReflection<LanguageRate>.State.deleted:
                        {
                            deleteLanguageRateSettings(connection, languageRate.LanguageRateId, languageRate.Id);
                            values.Add(-1);
                            break;
                        }
                }
            }


            return values;
        }

        public bool DeleteLanguageRateSettings(string databasePath, int languageRateId, int? id)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteLanguageRateSettings(connection, languageRateId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteLanguageRateSettings(SQLiteConnection connection, int languageRateId, int? id)
        {

            if (languageRateId <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "DELETE FROM LanguageRates";
            sqlQuery += " WHERE language_rate_id = @language_rate_id";
            if (id.HasValue)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            if (id.HasValue)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@language_rate_id"].Value = languageRateId;
            if (id.HasValue)
                cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion


        #region  |  Projects |

        public List<Project> GetProjects(string databasePath, string projectStatusFilter = "", string projectNameFilter = "", string activityStatusFilter = "", string activityNameFilter = "")
        {
            List<Project> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getProjects(connection, projectStatusFilter, projectNameFilter, activityStatusFilter, activityNameFilter);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<Project> getProjects(SQLiteConnection connection, string projectStatusFilter = "", string projectNameFilter = "", string activityStatusFilter = "", string activityNameFilter = "")
        {
            var values = new List<Project>();



            var sqlQuery = "SELECT * FROM Projects";
            if (projectStatusFilter.Trim() != string.Empty || projectNameFilter.Trim() != string.Empty)
            {
                sqlQuery += " WHERE";

                var sqlQueryFields = string.Empty;

                if (projectStatusFilter.Trim() != string.Empty)
                    sqlQueryFields += " project_status = '" + projectStatusFilter.Trim() + "' ";

                if (projectNameFilter.Trim() != string.Empty)
                    sqlQueryFields += (sqlQueryFields != string.Empty ? " AND" : string.Empty)
                        + " name LIKE '%" + projectNameFilter.Trim() + "%'";

                sqlQuery += sqlQueryFields;
            }


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);


            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new Project
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            Description = rdrSelect["description"].ToString(),
                            Path = rdrSelect["path"].ToString(),
                            SourceLanguage = rdrSelect["source_language"].ToString(),
                            ProjectStatus = rdrSelect["project_status"].ToString(),
                            StudioProjectId = rdrSelect["studio_project_id"].ToString(),
                            StudioProjectName = rdrSelect["studio_project_name"].ToString(),
                            StudioProjectPath = rdrSelect["studio_project_path"].ToString(),
                            CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]),
                            Created = Helper.DateTimeFromSQLite(rdrSelect["created"].ToString()),
                            Started = Helper.DateTimeFromSQLite(rdrSelect["started"].ToString()),
                            Completed = Helper.DateTimeFromSQLite(rdrSelect["completed"].ToString()),
                            Due = Helper.DateTimeFromSQLite(rdrSelect["due"].ToString())
                        };


                        values.Add(value);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table Projects!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            //get activities
            foreach (var value in values)
            {
                value.Activities = getActivities(connection, value.Id, null);
                value.DqfProjects = GetDqfProjects(connection, value.Id);
            }

            return values;
        }

        public Project GetProject(string databasePath, int id)
        {
            Project value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = getProject(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private Project getProject(SQLiteConnection connection, int id)
        {
            var value = new Project();


            if (id <= -1)
                throw new Exception("The project id cannot be null!");

            var sqlQuery = "SELECT * FROM Projects";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {

                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.Name = rdrSelect["name"].ToString();
                        value.Description = rdrSelect["description"].ToString();

                        value.Path = rdrSelect["path"].ToString();
                        value.SourceLanguage = rdrSelect["source_language"].ToString();
                        value.ProjectStatus = rdrSelect["project_status"].ToString();

                        value.StudioProjectId = rdrSelect["studio_project_id"].ToString();
                        value.StudioProjectName = rdrSelect["studio_project_name"].ToString();
                        value.StudioProjectPath = rdrSelect["studio_project_path"].ToString();

                        value.CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]);

                        value.Created = Helper.DateTimeFromSQLite(rdrSelect["created"].ToString());
                        value.Started = Helper.DateTimeFromSQLite(rdrSelect["started"].ToString());
                        value.Completed = Helper.DateTimeFromSQLite(rdrSelect["completed"].ToString());
                        value.Due = Helper.DateTimeFromSQLite(rdrSelect["due"].ToString());



                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table Projects!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            value.Activities = getActivities(connection, value.Id, null);


            return value;
        }

        public int CreateProject(string databasePath, Project project)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createProject(connection, project);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private int createProject(SQLiteConnection connection, Project project)
        {
            var value = -1;
            if (project.StudioProjectId.Trim() == string.Empty)
                throw new Exception("The studio project id cannot be null!");

            var sqlQuery = "INSERT INTO Projects";
            sqlQuery += " (";
            sqlQuery += " name";
            sqlQuery += ", description";
            sqlQuery += ", path";
            sqlQuery += ", source_language";
            sqlQuery += ", project_status";
            sqlQuery += ", studio_project_id";
            sqlQuery += ", studio_project_name";
            sqlQuery += ", studio_project_path";
            sqlQuery += ", company_profile_id";
            sqlQuery += ", created";
            sqlQuery += ", started";
            sqlQuery += ", completed";
            sqlQuery += ", due";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @name";
            sqlQuery += ", @description";
            sqlQuery += ", @path";
            sqlQuery += ", @source_language";
            sqlQuery += ", @project_status";
            sqlQuery += ", @studio_project_id";
            sqlQuery += ", @studio_project_name";
            sqlQuery += ", @studio_project_path";
            sqlQuery += ", @company_profile_id";
            sqlQuery += ", @created";
            sqlQuery += ", @started";
            sqlQuery += ", @completed";
            sqlQuery += ", @due";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_status", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@created", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@completed", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@due", DbType.String));


            cmdQuery.Parameters["@name"].Value = project.Name;
            cmdQuery.Parameters["@description"].Value = project.Description;
            cmdQuery.Parameters["@path"].Value = project.Path;
            cmdQuery.Parameters["@source_language"].Value = project.SourceLanguage;
            cmdQuery.Parameters["@project_status"].Value = project.ProjectStatus;
            cmdQuery.Parameters["@studio_project_id"].Value = project.StudioProjectId;
            cmdQuery.Parameters["@studio_project_name"].Value = project.StudioProjectName;
            cmdQuery.Parameters["@studio_project_path"].Value = project.StudioProjectPath;
            cmdQuery.Parameters["@company_profile_id"].Value = project.CompanyProfileId;
            cmdQuery.Parameters["@created"].Value = Helper.DateTimeToSQLite(project.Created);
            cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(project.Started);
            cmdQuery.Parameters["@completed"].Value = Helper.DateTimeToSQLite(project.Completed);
            cmdQuery.Parameters["@due"].Value = Helper.DateTimeToSQLite(project.Due);


            project.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = project.Id;


            //create child classes
            foreach (var activity in project.Activities)
            {
                activity.ProjectId = project.Id;
                activity.Id = createActivity(connection, activity);
            }

            return value;
        }

        public bool UpdateProject(string databasePath, Project project)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateProject(connection, project);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private bool updateProject(SQLiteConnection connection, Project project)
        {
            if (project.StudioProjectId.Trim() == string.Empty)
                throw new Exception("The studio project id cannot be null!");

            var sqlQuery = "UPDATE Projects";
            sqlQuery += " SET";
            sqlQuery += " name = @name";
            sqlQuery += ", description = @description";
            sqlQuery += ", path = @path";
            sqlQuery += ", source_language = @source_language";
            sqlQuery += ", project_status = @project_status";
            sqlQuery += ", studio_project_id = @studio_project_id";
            sqlQuery += ", studio_project_name = @studio_project_name";
            sqlQuery += ", studio_project_path = @studio_project_path";
            sqlQuery += ", company_profile_id = @company_profile_id";
            sqlQuery += ", created = @created";
            sqlQuery += ", started = @started";
            sqlQuery += ", completed =@completed";
            sqlQuery += ", due = @due";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_status", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@created", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@completed", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@due", DbType.String));


            cmdQuery.Parameters["@id"].Value = project.Id;
            cmdQuery.Parameters["@name"].Value = project.Name;
            cmdQuery.Parameters["@description"].Value = project.Description;
            cmdQuery.Parameters["@path"].Value = project.Path;
            cmdQuery.Parameters["@source_language"].Value = project.SourceLanguage;
            cmdQuery.Parameters["@project_status"].Value = project.ProjectStatus;
            cmdQuery.Parameters["@studio_project_id"].Value = project.StudioProjectId;
            cmdQuery.Parameters["@studio_project_name"].Value = project.StudioProjectName;
            cmdQuery.Parameters["@studio_project_path"].Value = project.StudioProjectPath;
            cmdQuery.Parameters["@company_profile_id"].Value = project.CompanyProfileId;
            cmdQuery.Parameters["@created"].Value = Helper.DateTimeToSQLite(project.Created);
            cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(project.Started);
            cmdQuery.Parameters["@completed"].Value = Helper.DateTimeToSQLite(project.Completed);
            cmdQuery.Parameters["@due"].Value = Helper.DateTimeToSQLite(project.Due);

            cmdQuery.ExecuteNonQuery();
            var value = true;

            //create child classes
            foreach (var activity in project.Activities)
            {
                activity.ProjectId = project.Id;
                value = updateActivity(connection, activity);
            }

            return value;
        }

        public bool DeleteProject(string databasePath, string databasePathProject, int id)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteProject(connection, databasePathProject, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private bool deleteProject(SQLiteConnection connection, string databasePathProject, int id)
        {
            var success = false;

            if (id <= -1)
                throw new Exception("The project id cannot be null!");



            var project = getProject(connection, id);

            if (project != null)
            {


                success = DeleteDqfProject(connection, id, null);


                var sqlQuery = "DELETE FROM Projects";
                sqlQuery += " WHERE id = @id";

                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters["@id"].Value = id;

                cmdQuery.ExecuteNonQuery();
                success = true;


                foreach (var activity in project.Activities)
                {
                    if (activity.ProjectId == id)
                        success = deleteActivity(connection, databasePathProject, activity.Id, true);
                }
            }

            return success;
        }


        #endregion



        #region  |  Project.Activity.Reports |



        public ActivityReports GetActivityReports(string databasePath, int projectActivityId)
        {
            ActivityReports values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getActivityReports(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static ActivityReports getActivityReports(SQLiteConnection connection, int projectActivityId)
        {
            var value = new ActivityReports();

            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivityReports";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {

                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]);
                        value.ReportOverview = rdrSelect["report_overview"].ToString();
                        value.ReportMetrics = rdrSelect["report_metrics"].ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivityReports!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            return value;
        }

        public int CreateActivityReports(string databasePath, ActivityReports activityReports)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createActivityReports(connection, activityReports);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createActivityReports(SQLiteConnection connection, ActivityReports activityReports)
        {
            var value = -1;
            if (activityReports.ProjectActivityId <= -1)
                throw new Exception("The activity id cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivityReports";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";
            sqlQuery += ", report_overview";
            sqlQuery += ", report_metrics";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";
            sqlQuery += ", @report_overview";
            sqlQuery += ", @report_metrics";
            sqlQuery += " ); SELECT last_insert_rowid();";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@report_overview", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@report_metrics", DbType.String));


            cmdQuery.Parameters["@project_activity_id"].Value = activityReports.ProjectActivityId;
            cmdQuery.Parameters["@report_overview"].Value = activityReports.ReportOverview;
            cmdQuery.Parameters["@report_metrics"].Value = activityReports.ReportMetrics;


            activityReports.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = activityReports.Id;

            return value;
        }

        public bool UpdateActivityReports(string databasePath, ActivityReports activityReports)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateActivityReports(connection, activityReports);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateActivityReports(SQLiteConnection connection, ActivityReports activityReports)
        {

            if (activityReports.Id <= -1)
                throw new Exception("The  id cannot be null!");

            var sqlQuery = "UPDATE ProjectActivityReports";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", report_overview = @report_overview";
            sqlQuery += ", report_metrics = @report_metrics";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@report_overview", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@report_metrics", DbType.String));


            cmdQuery.Parameters["@id"].Value = activityReports.Id;
            cmdQuery.Parameters["@project_activity_id"].Value = activityReports.ProjectActivityId;
            cmdQuery.Parameters["@report_overview"].Value = activityReports.ReportOverview;
            cmdQuery.Parameters["@report_metrics"].Value = activityReports.ReportMetrics;


            cmdQuery.ExecuteNonQuery();


            return true;
        }

        public int SaveActivityReports(string databasePath, ActivityReports activityReports)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    value = saveActivityReports(connection, activityReports);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static int saveActivityReports(SQLiteConnection connection, ActivityReports activityReports)
        {
            var value = -1;
            var activityReportsExisting = getActivityReports(connection, activityReports.ProjectActivityId);


            var exists = false;
            var isDifferent = false;

            if (activityReportsExisting.Id > -1)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(activityReportsExisting, activityReports))
                    isDifferent = true;
            }


            if (!exists && activityReports.Id == -1)
            {
                value = createActivityReports(connection, activityReports);
            }
            else if (isDifferent)
            {
                updateActivityReports(connection, activityReports);
                value = activityReports.Id;
            }



            return value;
        }

        public bool DeleteActivityReports(string databasePath, int projectActivityId)
        {

            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteActivityReports(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteActivityReports(SQLiteConnection connection, int projectActivityId)
        {

            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");


            var sqlQuery = "DELETE FROM ProjectActivityReports";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion


        #region  |  Project.Activities |

        public List<Activity> GetActivities(string databasePath, int projectId, int? id, string activityStatusFilter = "", string activityNameFilter = "")
        {
            List<Activity> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getActivities(connection, projectId, id, activityStatusFilter, activityNameFilter);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<Activity> getActivities(SQLiteConnection connection, int projectId, int? id, string activityStatusFilter = "", string activityNameFilter = "")
        {
            var values = new List<Activity>();

            if (projectId <= -1)
                throw new Exception("The project id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivities";
            sqlQuery += " WHERE project_id = @project_id";
            if (id != null)
                sqlQuery += " AND id = @id";

            if (activityStatusFilter.Trim() != string.Empty || activityNameFilter.Trim() != string.Empty)
            {
                if (activityStatusFilter.Trim() != string.Empty)
                    sqlQuery += " AND activity_status = '" + activityStatusFilter.Trim() + "' ";

                if (activityNameFilter.Trim() != string.Empty)
                    sqlQuery += " AND name LIKE '%" + activityNameFilter.Trim() + "%'";
            }


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@project_id"].Value = projectId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new Activity
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            Name = rdrSelect["name"].ToString(),
                            Description = rdrSelect["description"].ToString(),
                            ActivityStatus = (Activity.Status)Enum.Parse(
                                typeof(Activity.Status), rdrSelect["activity_status"].ToString(), true),
                            Billable = Convert.ToBoolean(rdrSelect["billable"]),
                            ProjectId = Convert.ToInt32(rdrSelect["project_id"]),
                            CompanyProfileId = Convert.ToInt32(rdrSelect["company_profile_id"]),
                            Started = Helper.DateTimeFromSQLite(rdrSelect["started"].ToString()),
                            Stopped = Helper.DateTimeFromSQLite(rdrSelect["stopped"].ToString()),
                            LanguageRateChecked = Convert.ToBoolean(rdrSelect["language_rate_checked"]),
                            HourlyRateChecked = Convert.ToBoolean(rdrSelect["hourly_rate_checked"]),
                            CustomRateChecked =
                            rdrSelect["custom_rate_checked"].ToString() != string.Empty && Convert.ToBoolean(rdrSelect["custom_rate_checked"])
                        };


                        values.Add(value);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivities!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }



            foreach (var value in values)
            {
                value.Activities = getDocumentActivities(connection, value.Id);
                value.DocumentActivityRates = getActivityRates(connection, value.Id);
                value.ComparisonOptions = getComparisonSettings(connection, value.Id);
                value.MetricReportSettings = getActivityMetricReportSettings(connection, value.Id);

            }



            return values;
        }

        public int CreateActivity(string databasePath, Activity activity)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createActivity(connection, activity);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private int createActivity(SQLiteConnection connection, Activity activity)
        {
            var value = -1;
            if (activity.Name.Trim() == string.Empty)
                throw new Exception("The activity name cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivities";
            sqlQuery += " (";
            sqlQuery += " name";
            sqlQuery += ", description";
            sqlQuery += ", activity_status";
            sqlQuery += ", billable";
            sqlQuery += ", project_id";
            sqlQuery += ", company_profile_id";
            sqlQuery += ", started";
            sqlQuery += ", stopped";
            sqlQuery += ", language_rate_checked";
            sqlQuery += ", hourly_rate_checked";
            sqlQuery += ", custom_rate_checked";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @name";
            sqlQuery += ", @description";
            sqlQuery += ", @activity_status";
            sqlQuery += ", @billable";
            sqlQuery += ", @project_id";
            sqlQuery += ", @company_profile_id";
            sqlQuery += ", @started";
            sqlQuery += ", @stopped";
            sqlQuery += ", @language_rate_checked";
            sqlQuery += ", @hourly_rate_checked";
            sqlQuery += ", @custom_rate_checked";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@activity_status", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@billable", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_checked", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_checked", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_checked", DbType.Boolean));


            cmdQuery.Parameters["@name"].Value = activity.Name;
            cmdQuery.Parameters["@description"].Value = activity.Description;
            cmdQuery.Parameters["@activity_status"].Value = activity.ActivityStatus.ToString();
            cmdQuery.Parameters["@billable"].Value = activity.Billable;
            cmdQuery.Parameters["@project_id"].Value = activity.ProjectId;
            cmdQuery.Parameters["@company_profile_id"].Value = activity.CompanyProfileId;
            cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(activity.Started);
            cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(activity.Stopped);
            cmdQuery.Parameters["@language_rate_checked"].Value = activity.LanguageRateChecked;
            cmdQuery.Parameters["@hourly_rate_checked"].Value = activity.HourlyRateChecked;
            cmdQuery.Parameters["@custom_rate_checked"].Value = activity.CustomRateChecked;


            activity.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = activity.Id;


            #region  |  create DocumentActivities  |
            foreach (var documentActivity in activity.Activities)
            {
                documentActivity.ProjectActivityId = activity.Id;
                documentActivity.Id = createDocumentActivity(connection, documentActivity);
            }
            #endregion

            #region  |  create comparison settings  |
            activity.ComparisonOptions.ProjectActivityId = activity.Id;
            activity.Id = createComparisonSettings(connection, activity.ComparisonOptions);
            #endregion

            #region  |  create activity rates  |

            activity.DocumentActivityRates.ProjectActivityId = activity.Id;
            activity.DocumentActivityRates.Id = createActivityRates(connection, activity.DocumentActivityRates);

            #endregion


            activity.MetricReportSettings.ProjectActivityId = activity.Id;
            activity.MetricReportSettings.Id = createActivityMetricReportSettings(connection, activity.MetricReportSettings);

            return value;
        }

        public bool UpdateActivity(string databasePath, Activity activity)
        {
            var value = false;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateActivity(connection, activity);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private bool updateActivity(SQLiteConnection connection, Activity activity)
        {
            try
            {
                if (activity.Id <= -1)
                    throw new Exception("The activity id cannot be null!");

                var sqlQuery = "UPDATE ProjectActivities";
                sqlQuery += " SET";
                sqlQuery += " name = @name";
                sqlQuery += ", description = @description";
                sqlQuery += ", activity_status = @activity_status";
                sqlQuery += ", billable = @billable";
                sqlQuery += ", project_id = @project_id";
                sqlQuery += ", company_profile_id = @company_profile_id";
                sqlQuery += ", started = @started";
                sqlQuery += ", stopped = @stopped";
                sqlQuery += ", language_rate_checked = @language_rate_checked";
                sqlQuery += ", hourly_rate_checked = @hourly_rate_checked";
                sqlQuery += ", custom_rate_checked = @custom_rate_checked";
                sqlQuery += " WHERE";
                sqlQuery += " id = @id";



                var cmdQuery = new SQLiteCommand(sqlQuery, connection);
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@description", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@activity_status", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@billable", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@company_profile_id", DbType.Int32));
                cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
                cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_checked", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_checked", DbType.Boolean));
                cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_checked", DbType.Boolean));

                cmdQuery.Parameters["@id"].Value = activity.Id;
                cmdQuery.Parameters["@name"].Value = activity.Name;
                cmdQuery.Parameters["@description"].Value = activity.Description;
                cmdQuery.Parameters["@activity_status"].Value = activity.ActivityStatus.ToString();
                cmdQuery.Parameters["@billable"].Value = activity.Billable;
                cmdQuery.Parameters["@project_id"].Value = activity.ProjectId;
                cmdQuery.Parameters["@company_profile_id"].Value = activity.CompanyProfileId;
                cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(activity.Started);
                cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(activity.Stopped);
                cmdQuery.Parameters["@language_rate_checked"].Value = activity.LanguageRateChecked;
                cmdQuery.Parameters["@hourly_rate_checked"].Value = activity.HourlyRateChecked;
                cmdQuery.Parameters["@custom_rate_checked"].Value = activity.CustomRateChecked;


                cmdQuery.ExecuteNonQuery();

                #region  |  create DocumentActivities  |

                foreach (var documentActivity in activity.Activities)
                {
                    documentActivity.ProjectActivityId = activity.Id;
                    updateDocumentActivity(connection, documentActivity);
                }

                #endregion


                #region  |  create comparison settings  |

                activity.ComparisonOptions.ProjectActivityId = activity.Id;
                updateComparisonSettings(connection, activity.ComparisonOptions);

                #endregion


                #region  |  create activity rates  |

                activity.DocumentActivityRates.ProjectActivityId = activity.Id;
                updateActivityRates(connection, activity.DocumentActivityRates);

                #endregion


                activity.MetricReportSettings.ProjectActivityId = activity.Id;
                saveActivityMetricReportSettings(connection, activity.MetricReportSettings);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public bool DeleteActivity(string databasePath, string databasePathProject, int id, bool deleteDocumentActivity)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteActivity(connection, databasePathProject, id, deleteDocumentActivity);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private bool deleteActivity(SQLiteConnection connection, string databasePathProject, int id, bool deleteDocumentActivity)
        {
            if (id <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "DELETE FROM ProjectActivities";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            //remove the child classes
            var success = deleteComparisonSettings(connection, id);
            success = deleteActivityRates(connection, id, null);
            success = deleteDocumentActivities(connection, databasePathProject, id, deleteDocumentActivity);
            success = deleteActivityMetricReportSettings(connection, id);

            success = deleteActivityReports(connection, id);

            return success;
        }


        #endregion

        #region  |  Project.Activity.DocumentActivities  |

        public List<DocumentActivities> GetDocumentActivities(string databasePath, int projectActivityId)
        {
            List<DocumentActivities> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getDocumentActivities(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<DocumentActivities> getDocumentActivities(SQLiteConnection connection, int projectActivityId)
        {
            var values = new List<DocumentActivities>();

            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivityDocumentActivities";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new DocumentActivities
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]),
                            DocumentActivityTicks = Convert.ToInt64(rdrSelect["document_activity_ticks"]),
                            DocumentRecordsTicks = Convert.ToInt64(rdrSelect["document_activity_ticks"]),
                            DocumentId = rdrSelect["document_id"].ToString(),
                            DocumentActivityIds = new List<int>()
                        };




                        var documentActivityIds = rdrSelect["document_activity_ids"].ToString();
                        if (documentActivityIds.Length > 0)
                        {
                            var strArray = documentActivityIds.Split(';');
                            foreach (var item in strArray)
                            {
                                if (item.Trim() == string.Empty) continue;
                                try
                                {
                                    var iItem = Convert.ToInt32(item);

                                    if (iItem > -1 && !value.DocumentActivityIds.Contains(iItem))
                                        value.DocumentActivityIds.Add(iItem);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivityDocumentActivities!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            //get the document object for reference
            foreach (var value in values)
                value.TranslatableDocument = getDocument(connection, value.DocumentId);

            return values;
        }

        public int CreateDocumentActivity(string databasePath, DocumentActivities documentActivities)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createDocumentActivity(connection, documentActivities);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createDocumentActivity(SQLiteConnection connection, DocumentActivities documentActivities)
        {
            var value = -1;
            if (documentActivities.ProjectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivityDocumentActivities";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";
            sqlQuery += ", document_activity_ticks";
            sqlQuery += ", document_records_ticks";
            sqlQuery += ", document_id";
            sqlQuery += ", document_activity_ids";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";
            sqlQuery += ", @document_activity_ticks";
            sqlQuery += ", @document_records_ticks";
            sqlQuery += ", @document_id";
            sqlQuery += ", @document_activity_ids";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_ticks", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_records_ticks", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_ids", DbType.String));


            cmdQuery.Parameters["@project_activity_id"].Value = documentActivities.ProjectActivityId;
            cmdQuery.Parameters["@document_activity_ticks"].Value = documentActivities.DocumentActivityTicks;
            cmdQuery.Parameters["@document_records_ticks"].Value = documentActivities.DocumentRecordsTicks;
            cmdQuery.Parameters["@document_id"].Value = documentActivities.DocumentId;

            var strDocumentActivityIds = documentActivities.DocumentActivityIds.Aggregate(string.Empty, (current, id) => current + ((current.Trim() != string.Empty ? ";" : string.Empty) + id));

            cmdQuery.Parameters["@document_activity_ids"].Value = strDocumentActivityIds;


            documentActivities.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = documentActivities.Id;

            return value;
        }

        public bool UpdateDocumentActivity(string databasePath, DocumentActivities documentActivities)
        {
            var value = false;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateDocumentActivity(connection, documentActivities);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateDocumentActivity(SQLiteConnection connection, DocumentActivities documentActivities)
        {
            if (documentActivities.ProjectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "UPDATE ProjectActivityDocumentActivities";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", document_activity_ticks = @document_activity_ticks";
            sqlQuery += ", document_records_ticks = @document_records_ticks";
            sqlQuery += ", document_id = @document_id";
            sqlQuery += ", document_activity_ids = @document_activity_ids";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_ticks", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_records_ticks", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_ids", DbType.String));

            cmdQuery.Parameters["@id"].Value = documentActivities.Id;
            cmdQuery.Parameters["@project_activity_id"].Value = documentActivities.ProjectActivityId;
            cmdQuery.Parameters["@document_activity_ticks"].Value = documentActivities.DocumentActivityTicks;
            cmdQuery.Parameters["@document_records_ticks"].Value = documentActivities.DocumentRecordsTicks;
            cmdQuery.Parameters["@document_id"].Value = documentActivities.DocumentId;

            var strDocumentActivityIds = documentActivities.DocumentActivityIds.Aggregate(string.Empty, (current, id) => current + ((current.Trim() != string.Empty ? ";" : string.Empty) + id));

            cmdQuery.Parameters["@document_activity_ids"].Value = strDocumentActivityIds;


            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public bool DeleteDocumentActivities(string databasePath, string databasePathProject, int projectActivityId, bool deleteDocumentActivity)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteDocumentActivities(connection, databasePathProject, projectActivityId, deleteDocumentActivity);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteDocumentActivities(SQLiteConnection connection, string databasePathProject, int projectActivityId, bool _deleteDocumentActivity)
        {
            var success = false;

            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "DELETE FROM ProjectActivityDocumentActivities";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            cmdQuery.ExecuteNonQuery();
            success = true;

            //PH 2015-05-09
            //this option to clean up the sub levels associated with the activity are not removed
            //when the activities are being merged at a higher level
            //later the parent ID will be updated.  This is quicker than removing all the records
            //and then later reinserting *everything* associated to the newly merged activity.
            if (!_deleteDocumentActivity)
                return true;

            using (var connectionProject = new SQLiteConnection(GetConnectionString(databasePathProject)))
            {
                connectionProject.Open();
                try
                {

                    var ids = new List<int>();
                    #region  |  if only project activity id is present  |

                    var sqlQuery1 = "SELECT * FROM DocumentActivities";
                    sqlQuery1 += " WHERE project_activity_id = @project_activity_id";

                    var cmdQuery1 = new SQLiteCommand(sqlQuery1, connectionProject);

                    cmdQuery1.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
                    cmdQuery1.Parameters["@project_activity_id"].Value = projectActivityId;

                    var rdrSelect = cmdQuery1.ExecuteReader();
                    try
                    {
                        if (rdrSelect.HasRows)
                        {
                            while (rdrSelect.Read())
                            {
                                ids.Add(Convert.ToInt32(rdrSelect["id"]));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error querying the table DocumentActivities!  " + ex.Message);
                    }
                    finally
                    {
                        if (rdrSelect != null)
                            if (!rdrSelect.IsClosed)
                                rdrSelect.Close();
                    }

                    #endregion

                    foreach (var id in ids)
                        success = deleteDocumentActivity(connectionProject, projectActivityId, id);

                }
                finally
                {
                    connectionProject.Close();
                }
            }

            return success;
        }


        #endregion
        #region  |  Project.Activity.ComparisonSettings  |

        public ComparisonSettings GetComparisonSettings(string databasePath, int projectActivityId)
        {
            ComparisonSettings values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getComparisonSettings(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static ComparisonSettings getComparisonSettings(SQLiteConnection connection, int projectActivityId)
        {
            var value = new ComparisonSettings();

            if (projectActivityId <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivityComparisonSettings";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));

            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    rdrSelect.Read();


                    value.Id = Convert.ToInt32(rdrSelect["id"]);
                    value.ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]);

                    value.ComparisonType = Convert.ToInt32(rdrSelect["comparison_type"]);
                    value.ConsolidateChanges = Convert.ToBoolean(rdrSelect["consolidate_changes"]);
                    value.IncludeTagsInComparison = Convert.ToBoolean(rdrSelect["include_tags_in_comparison"]);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivityComparisonSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }




            return value;
        }

        public int CreateComparisonSettings(string databasePath, ComparisonSettings comparisonSettings)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createComparisonSettings(connection, comparisonSettings);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createComparisonSettings(SQLiteConnection connection, ComparisonSettings comparisonSettings)
        {
            var value = -1;
            if (comparisonSettings.ProjectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivityComparisonSettings";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";
            sqlQuery += ", comparison_type";
            sqlQuery += ", consolidate_changes";
            sqlQuery += ", include_tags_in_comparison";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";
            sqlQuery += ", @comparison_type";
            sqlQuery += ", @consolidate_changes";
            sqlQuery += ", @include_tags_in_comparison";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@comparison_type", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@consolidate_changes", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@include_tags_in_comparison", DbType.Boolean));


            cmdQuery.Parameters["@project_activity_id"].Value = comparisonSettings.ProjectActivityId;
            cmdQuery.Parameters["@comparison_type"].Value = comparisonSettings.ComparisonType;
            cmdQuery.Parameters["@consolidate_changes"].Value = comparisonSettings.ConsolidateChanges;
            cmdQuery.Parameters["@include_tags_in_comparison"].Value = comparisonSettings.IncludeTagsInComparison;


            comparisonSettings.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = comparisonSettings.Id;

            return value;
        }

        public bool UpdateComparisonSettings(string databasePath, ComparisonSettings comparisonSettings)
        {
            var value = false;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateComparisonSettings(connection, comparisonSettings);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateComparisonSettings(SQLiteConnection connection, ComparisonSettings comparisonSettings)
        {
            if (comparisonSettings.ProjectActivityId <= -1 || comparisonSettings.Id <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "UPDATE ProjectActivityComparisonSettings";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", comparison_type = @comparison_type";
            sqlQuery += ", consolidate_changes = @consolidate_changes";
            sqlQuery += ", include_tags_in_comparison = @include_tags_in_comparison";
            sqlQuery += " WHERE id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@comparison_type", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@consolidate_changes", DbType.Boolean));
            cmdQuery.Parameters.Add(new SQLiteParameter("@include_tags_in_comparison", DbType.Boolean));


            cmdQuery.Parameters["@id"].Value = comparisonSettings.Id;
            cmdQuery.Parameters["@project_activity_id"].Value = comparisonSettings.ProjectActivityId;
            cmdQuery.Parameters["@comparison_type"].Value = comparisonSettings.ComparisonType;
            cmdQuery.Parameters["@consolidate_changes"].Value = comparisonSettings.ConsolidateChanges;
            cmdQuery.Parameters["@include_tags_in_comparison"].Value = comparisonSettings.IncludeTagsInComparison;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public bool DeleteComparisonSettings(string databasePath, int projectActivityId)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteComparisonSettings(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteComparisonSettings(SQLiteConnection connection, int projectActivityId)
        {
            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "DELETE FROM ProjectActivityComparisonSettings";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion
        #region  |  Project.Activity.MetricReportSettings  |


        public QualityMetricReportSettings GetActivityMetricReportSettings(string databasePath, int projectActivityId)
        {
            QualityMetricReportSettings values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getActivityMetricReportSettings(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static QualityMetricReportSettings getActivityMetricReportSettings(SQLiteConnection connection, int projectActivityId)
        {
            var values = new QualityMetricReportSettings();


            if (projectActivityId <= -1)
                throw new Exception(" company project_activity_id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivityMetricReportSettings";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    rdrSelect.Read();


                    values.Id = Convert.ToInt32(rdrSelect["id"]);
                    values.MetricGroupName = rdrSelect["metric_group_name"].ToString();
                    values.ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]);
                    values.MaxSeverityValue = Convert.ToInt32(rdrSelect["max_severity_value"]);
                    values.MaxSeverityInValue = Convert.ToInt32(rdrSelect["max_severity_in_value"]);
                    values.MaxSeverityInType = rdrSelect["max_severity_in_type"].ToString();


                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivityMetricReportSettings!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            return values;
        }

        public int CreateActivityMetricReportSettings(string databasePath, QualityMetricReportSettings qualityMetricReportSettings)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createActivityMetricReportSettings(connection, qualityMetricReportSettings);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createActivityMetricReportSettings(SQLiteConnection connection, QualityMetricReportSettings qualityMetricReportSettings)
        {
            var value = -1;

            if (qualityMetricReportSettings.ProjectActivityId <= -1)
                throw new Exception("The project_activity_id cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivityMetricReportSettings";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";
            sqlQuery += ", metric_group_name";
            sqlQuery += ", max_severity_value";
            sqlQuery += ", max_severity_in_value";
            sqlQuery += ", max_severity_in_type";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";
            sqlQuery += ", @metric_group_name";
            sqlQuery += ", @max_severity_value";
            sqlQuery += ", @max_severity_in_value";
            sqlQuery += ", @max_severity_in_type";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@metric_group_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_value", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_value", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_type", DbType.String));


            cmdQuery.Parameters["@project_activity_id"].Value = qualityMetricReportSettings.ProjectActivityId;
            cmdQuery.Parameters["@metric_group_name"].Value = qualityMetricReportSettings.MetricGroupName;
            cmdQuery.Parameters["@max_severity_value"].Value = qualityMetricReportSettings.MaxSeverityValue;
            cmdQuery.Parameters["@max_severity_in_value"].Value = qualityMetricReportSettings.MaxSeverityInValue;
            cmdQuery.Parameters["@max_severity_in_type"].Value = qualityMetricReportSettings.MaxSeverityInType;


            qualityMetricReportSettings.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = qualityMetricReportSettings.Id;

            return value;
        }

        public bool UpdateActivityMetricReportSettings(string databasePath, QualityMetricReportSettings qualityMetricReportSettings)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateActivityMetricReportSettings(connection, qualityMetricReportSettings);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateActivityMetricReportSettings(SQLiteConnection connection, QualityMetricReportSettings qualityMetricReportSettings)
        {
            if (qualityMetricReportSettings.ProjectActivityId <= -1)
                throw new Exception("The project_activity_id cannot be null!");

            var sqlQuery = "UPDATE ProjectActivityMetricReportSettings";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", metric_group_name = @metric_group_name";
            sqlQuery += ", max_severity_value = @max_severity_value";
            sqlQuery += ", max_severity_in_value = @max_severity_in_value";
            sqlQuery += ", max_severity_in_type = @max_severity_in_type";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@metric_group_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_value", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_value", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@max_severity_in_type", DbType.String));

            cmdQuery.Parameters["@id"].Value = qualityMetricReportSettings.Id;
            cmdQuery.Parameters["@project_activity_id"].Value = qualityMetricReportSettings.ProjectActivityId;
            cmdQuery.Parameters["@metric_group_name"].Value = qualityMetricReportSettings.MetricGroupName;
            cmdQuery.Parameters["@max_severity_value"].Value = qualityMetricReportSettings.MaxSeverityValue;
            cmdQuery.Parameters["@max_severity_in_value"].Value = qualityMetricReportSettings.MaxSeverityInValue;
            cmdQuery.Parameters["@max_severity_in_type"].Value = qualityMetricReportSettings.MaxSeverityInType;


            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public int SaveActivityMetricReportSettings(string databasePath, QualityMetricReportSettings qualityMetricReportSettings)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    value = saveActivityMetricReportSettings(connection, qualityMetricReportSettings);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private int saveActivityMetricReportSettings(SQLiteConnection connection, QualityMetricReportSettings qualityMetricReportSettings)
        {
            var value = -1;
            var qualityMetricAssessmentExisting = getActivityMetricReportSettings(connection, qualityMetricReportSettings.ProjectActivityId);


            var exists = false;
            var isDifferent = false;

            if (qualityMetricAssessmentExisting.Id > -1)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(qualityMetricAssessmentExisting, qualityMetricReportSettings))
                    isDifferent = true;
            }


            if (!exists && qualityMetricReportSettings.Id == -1)
            {
                value = createActivityMetricReportSettings(connection, qualityMetricReportSettings);
            }
            else if (isDifferent)
            {
                updateActivityMetricReportSettings(connection, qualityMetricReportSettings);
                value = qualityMetricReportSettings.Id;
            }


            return value;
        }

        public bool DeleteActivityMetricReportSettings(string databasePath, int projectActivityId)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteActivityMetricReportSettings(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteActivityMetricReportSettings(SQLiteConnection connection, int projectActivityId)
        {
            if (projectActivityId <= -1)
                throw new Exception("The project_activity_id cannot be null!");

            var sqlQuery = "DELETE FROM ProjectActivityMetricReportSettings";
            sqlQuery += " WHERE project_activity_id = @project_activity_id ";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion
        #region  |  Project.Activity.ActivityRates  |

        public ActivityRates GetActivityRates(string databasePath, int projectActivityId)
        {
            ActivityRates value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = getActivityRates(connection, projectActivityId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private ActivityRates getActivityRates(SQLiteConnection connection, int projectActivityId)
        {
            var value = new ActivityRates();

            if (projectActivityId <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "SELECT * FROM ProjectActivityRates";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));

            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    rdrSelect.Read();

                    value.Id = Convert.ToInt32(rdrSelect["id"]);
                    value.ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]);

                    value.LanguageRateId = Convert.ToInt32(rdrSelect["language_rate_id"]);
                    value.LanguageRateName = rdrSelect["language_rate_name"].ToString();
                    value.LanguageRateDescription = rdrSelect["language_rate_description"].ToString();
                    value.LanguageRateCurrency = rdrSelect["language_rate_currency"].ToString();
                    value.LanguageRateTotal = Convert.ToDouble(rdrSelect["language_rate_total"]);

                    value.HourlyRateName = rdrSelect["hourly_rate_name"].ToString();
                    value.HourlyRateDescription = rdrSelect["hourly_rate_description"].ToString();
                    value.HourlyRateRate = Convert.ToDouble(rdrSelect["hourly_rate_rate"]);
                    value.HourlyRateQuantity = Convert.ToDouble(rdrSelect["hourly_rate_quantity"]);
                    value.HourlyRateCurrency = rdrSelect["hourly_rate_currency"].ToString();
                    value.HourlyRateTotal = Convert.ToDouble(rdrSelect["hourly_rate_total"]);

                    value.LanguageRates = DeserializeList<Sdl.Community.Structures.Projects.Activities.LanguageRate>(rdrSelect["xml_language_rates"].ToString());
                    #region  |  assign object parent ids  |
                    foreach (var lr in value.LanguageRates)
                    {
                        lr.ProjectActivityRateId = value.Id;
                        lr.ProjectActivityId = value.ProjectActivityId;
                    }
                    #endregion



                    value.CustomRateName = rdrSelect["custom_rate_name"].ToString();
                    value.CustomRateDescription = rdrSelect["custom_rate_description"].ToString();
                    value.CustomRateCurrency = rdrSelect["custom_rate_currency"].ToString();
                    value.CustomRateTotal = Convert.ToDouble(rdrSelect["custom_rate_total"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table ProjectActivityRates!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }



            return value;
        }

        public int CreateActivityRates(string databasePath, ActivityRates activityRates)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createActivityRates(connection, activityRates);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private int createActivityRates(SQLiteConnection connection, ActivityRates activityRates)
        {
            var value = -1;
            if (activityRates.ProjectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "INSERT INTO ProjectActivityRates";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";

            sqlQuery += ", language_rate_id";
            sqlQuery += ", language_rate_name";
            sqlQuery += ", language_rate_description";
            sqlQuery += ", language_rate_currency";
            sqlQuery += ", language_rate_total";
            sqlQuery += ", xml_language_rates";

            sqlQuery += ", hourly_rate_name";
            sqlQuery += ", hourly_rate_description";
            sqlQuery += ", hourly_rate_rate";
            sqlQuery += ", hourly_rate_quantity";
            sqlQuery += ", hourly_rate_currency";
            sqlQuery += ", hourly_rate_total";

            sqlQuery += ", custom_rate_name";
            sqlQuery += ", custom_rate_description";
            sqlQuery += ", custom_rate_currency";
            sqlQuery += ", custom_rate_total";

            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";

            sqlQuery += ", @language_rate_id";
            sqlQuery += ", @language_rate_name";
            sqlQuery += ", @language_rate_description";
            sqlQuery += ", @language_rate_currency";
            sqlQuery += ", @language_rate_total";
            sqlQuery += ", @xml_language_rates";

            sqlQuery += ", @hourly_rate_name";
            sqlQuery += ", @hourly_rate_description";
            sqlQuery += ", @hourly_rate_rate";
            sqlQuery += ", @hourly_rate_quantity";
            sqlQuery += ", @hourly_rate_currency";
            sqlQuery += ", @hourly_rate_total";

            sqlQuery += ", @custom_rate_name";
            sqlQuery += ", @custom_rate_description";
            sqlQuery += ", @custom_rate_currency";
            sqlQuery += ", @custom_rate_total";

            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_total", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_language_rates", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_rate", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_quantity", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_total", DbType.Double));


            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_total", DbType.Double));




            cmdQuery.Parameters["@project_activity_id"].Value = activityRates.ProjectActivityId;

            cmdQuery.Parameters["@language_rate_id"].Value = activityRates.LanguageRateId;
            cmdQuery.Parameters["@language_rate_name"].Value = activityRates.LanguageRateName;
            cmdQuery.Parameters["@language_rate_description"].Value = activityRates.LanguageRateDescription;
            cmdQuery.Parameters["@language_rate_currency"].Value = activityRates.LanguageRateCurrency;
            cmdQuery.Parameters["@language_rate_total"].Value = activityRates.LanguageRateTotal;
            cmdQuery.Parameters["@xml_language_rates"].Value = SerializeList(activityRates.LanguageRates);

            cmdQuery.Parameters["@hourly_rate_name"].Value = activityRates.HourlyRateName;
            cmdQuery.Parameters["@hourly_rate_description"].Value = activityRates.HourlyRateDescription;
            cmdQuery.Parameters["@hourly_rate_rate"].Value = activityRates.HourlyRateRate;
            cmdQuery.Parameters["@hourly_rate_quantity"].Value = activityRates.HourlyRateQuantity;
            cmdQuery.Parameters["@hourly_rate_currency"].Value = activityRates.HourlyRateCurrency;
            cmdQuery.Parameters["@hourly_rate_total"].Value = activityRates.HourlyRateTotal;

            cmdQuery.Parameters["@custom_rate_name"].Value = activityRates.CustomRateName;
            cmdQuery.Parameters["@custom_rate_description"].Value = activityRates.CustomRateDescription;
            cmdQuery.Parameters["@custom_rate_currency"].Value = activityRates.CustomRateCurrency;
            cmdQuery.Parameters["@custom_rate_total"].Value = activityRates.CustomRateTotal;

            activityRates.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = activityRates.Id;

            return value;
        }

        public bool UpdateActivityRates(string databasePath, ActivityRates activityRates)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateActivityRates(connection, activityRates);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private bool updateActivityRates(SQLiteConnection connection, ActivityRates activityRates)
        {
            if (activityRates.Id <= -1)
                throw new Exception("The activity rate id cannot be null!");

            var sqlQuery = "UPDATE ProjectActivityRates";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", language_rate_id = @language_rate_id";
            sqlQuery += ", language_rate_name = @language_rate_name";
            sqlQuery += ", language_rate_description = @language_rate_description";
            sqlQuery += ", language_rate_currency = @language_rate_currency";
            sqlQuery += ", language_rate_total = @language_rate_total";
            sqlQuery += ", xml_language_rates = @xml_language_rates";

            sqlQuery += ", hourly_rate_name = @hourly_rate_name";
            sqlQuery += ", hourly_rate_description = @hourly_rate_description";
            sqlQuery += ", hourly_rate_rate = @hourly_rate_rate";
            sqlQuery += ", hourly_rate_quantity = @hourly_rate_quantity";
            sqlQuery += ", hourly_rate_currency = @hourly_rate_currency";
            sqlQuery += ", hourly_rate_total = @hourly_rate_total";

            sqlQuery += ", custom_rate_name = @custom_rate_name";
            sqlQuery += ", custom_rate_description = @custom_rate_description";
            sqlQuery += ", custom_rate_currency = @custom_rate_currency";
            sqlQuery += ", custom_rate_total = @custom_rate_total";

            sqlQuery += "  WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@language_rate_total", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_language_rates", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_rate", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_quantity", DbType.Double));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@hourly_rate_total", DbType.Double));


            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_description", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_currency", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@custom_rate_total", DbType.Double));


            cmdQuery.Parameters["@id"].Value = activityRates.Id;

            cmdQuery.Parameters["@project_activity_id"].Value = activityRates.ProjectActivityId;

            cmdQuery.Parameters["@language_rate_id"].Value = activityRates.LanguageRateId;
            cmdQuery.Parameters["@language_rate_name"].Value = activityRates.LanguageRateName;
            cmdQuery.Parameters["@language_rate_description"].Value = activityRates.LanguageRateDescription;
            cmdQuery.Parameters["@language_rate_currency"].Value = activityRates.LanguageRateCurrency;
            cmdQuery.Parameters["@language_rate_total"].Value = activityRates.LanguageRateTotal;
            cmdQuery.Parameters["@xml_language_rates"].Value = SerializeList(activityRates.LanguageRates);

            cmdQuery.Parameters["@hourly_rate_name"].Value = activityRates.HourlyRateName;
            cmdQuery.Parameters["@hourly_rate_description"].Value = activityRates.HourlyRateDescription;
            cmdQuery.Parameters["@hourly_rate_rate"].Value = activityRates.HourlyRateRate;
            cmdQuery.Parameters["@hourly_rate_quantity"].Value = activityRates.HourlyRateQuantity;
            cmdQuery.Parameters["@hourly_rate_currency"].Value = activityRates.HourlyRateCurrency;
            cmdQuery.Parameters["@hourly_rate_total"].Value = activityRates.HourlyRateTotal;

            cmdQuery.Parameters["@custom_rate_name"].Value = activityRates.CustomRateName;
            cmdQuery.Parameters["@custom_rate_description"].Value = activityRates.CustomRateDescription;
            cmdQuery.Parameters["@custom_rate_currency"].Value = activityRates.CustomRateCurrency;
            cmdQuery.Parameters["@custom_rate_total"].Value = activityRates.CustomRateTotal;

            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public bool DeleteActivityRates(string databasePath, int projectActivityId, int? id)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteActivityRates(connection, projectActivityId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteActivityRates(SQLiteConnection connection, int projectActivityId, int? id)
        {
            if (projectActivityId <= -1)
                throw new Exception("The project activity id cannot be null!");

            var sqlQuery = "DELETE FROM ProjectActivityRates";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";
            if (id.HasValue)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            if (id.HasValue)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;
            if (id.HasValue)
                cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion


        #region  |  Project.DQF.DQFProjects  |

        public List<DqfProject> GetDqfProjects(string databasePath, int projectId)
        {
            List<DqfProject> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = GetDqfProjects(connection, projectId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<DqfProject> GetDqfProjects(SQLiteConnection connection, int projectId)
        {
            var values = new List<DqfProject>();

            if (projectId <= -1)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "SELECT * FROM TausDQFProjects";
            sqlQuery += " WHERE project_id = @project_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters["@project_id"].Value = projectId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new DqfProject
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            ProjectId = Convert.ToInt32(rdrSelect["project_id"]),
                            ProjectIdStudio = rdrSelect["project_id_studio"].ToString(),
                            DqfPmanagerKey = rdrSelect["dqf_pmanager_key"].ToString(),
                            DqfProjectKey = rdrSelect["dqf_project_key"].ToString(),
                            DqfProjectId = Convert.ToInt32(rdrSelect["dqf_project_id"]),
                            Name = rdrSelect["name"].ToString(),
                            Created = Helper.DateTimeFromSQLite(rdrSelect["created"].ToString()),
                            SourceLanguage = rdrSelect["source_language"].ToString(),
                            Process = Convert.ToInt32(rdrSelect["process"]),
                            ContentType = Convert.ToInt32(rdrSelect["content_type"]),
                            Industry = Convert.ToInt32(rdrSelect["industry"]),
                            QualityLevel = Convert.ToInt32(rdrSelect["quality_level"]),
                            Imported = Convert.ToBoolean(rdrSelect["imported"])
                        };


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table TausDQFProjects!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            foreach (var value in values)
            {
                value.DqfTasks = GetDqfProjectTasks(connection, value.Id);
            }


            return values;
        }

        public int CreateDqfProject(string databasePath, DqfProject dqfProject)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createDqfProject(connection, dqfProject);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createDqfProject(SQLiteConnection connection, DqfProject dqfProject)
        {
            var value = -1;
            if (dqfProject.Name.Trim() == string.Empty)
                throw new Exception("The project name cannot be null!");


            var sqlQuery = "INSERT INTO TausDQFProjects";
            sqlQuery += " (";
            sqlQuery += " project_id";
            sqlQuery += ", project_id_studio";
            sqlQuery += ", dqf_pmanager_key";
            sqlQuery += ", dqf_project_key";
            sqlQuery += ", dqf_project_id";
            sqlQuery += ", name";
            sqlQuery += ", created";
            sqlQuery += ", source_language";
            sqlQuery += ", process";
            sqlQuery += ", content_type";
            sqlQuery += ", industry";
            sqlQuery += ", quality_level";
            sqlQuery += ", imported";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_id";
            sqlQuery += ", @project_id_studio";
            sqlQuery += ", @dqf_pmanager_key";
            sqlQuery += ", @dqf_project_key";
            sqlQuery += ", @dqf_project_id";
            sqlQuery += ", @name";
            sqlQuery += ", @created";
            sqlQuery += ", @source_language";
            sqlQuery += ", @process";
            sqlQuery += ", @content_type";
            sqlQuery += ", @industry";
            sqlQuery += ", @quality_level";
            sqlQuery += ", @imported";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id_studio", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_pmanager_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_id", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@created", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@process", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@content_type", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@industry", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@quality_level", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@imported", DbType.Boolean));

            cmdQuery.Parameters["@project_id"].Value = dqfProject.ProjectId;
            cmdQuery.Parameters["@project_id_studio"].Value = dqfProject.ProjectIdStudio;
            cmdQuery.Parameters["@dqf_pmanager_key"].Value = dqfProject.DqfPmanagerKey;
            cmdQuery.Parameters["@dqf_project_key"].Value = dqfProject.DqfProjectKey;
            cmdQuery.Parameters["@dqf_project_id"].Value = dqfProject.DqfProjectId;

            cmdQuery.Parameters["@name"].Value = dqfProject.Name;
            cmdQuery.Parameters["@created"].Value = Helper.DateTimeToSQLite(dqfProject.Created);
            cmdQuery.Parameters["@source_language"].Value = dqfProject.SourceLanguage;

            cmdQuery.Parameters["@process"].Value = dqfProject.Process;
            cmdQuery.Parameters["@content_type"].Value = dqfProject.ContentType;
            cmdQuery.Parameters["@industry"].Value = dqfProject.Industry;
            cmdQuery.Parameters["@quality_level"].Value = dqfProject.QualityLevel;

            cmdQuery.Parameters["@imported"].Value = dqfProject.Imported;


            dqfProject.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = dqfProject.Id;

            return value;
        }

        public bool UpdateDqfProject(string databasePath, DqfProject dqfProject)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateDqfProject(connection, dqfProject);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateDqfProject(SQLiteConnection connection, DqfProject dqfProject)
        {
            if (dqfProject.Id == -1)
                throw new Exception("The id cannot be null!");


            var sqlQuery = "UPDATE TausDQFProjects";
            sqlQuery += " SET";
            sqlQuery += " project_id = @project_id";
            sqlQuery += ", project_id_studio = @project_id_studio";
            sqlQuery += ", dqf_pmanager_key = @dqf_pmanager_key";
            sqlQuery += ", dqf_project_key = @dqf_project_key";
            sqlQuery += ", dqf_project_id = @dqf_project_id";

            sqlQuery += ", name = @name";
            sqlQuery += ", created = @created";
            sqlQuery += ", source_language = @source_language";

            sqlQuery += ", process = @process";
            sqlQuery += ", content_type = @content_type";
            sqlQuery += ", industry = @industry";
            sqlQuery += ", quality_level = @quality_level";
            sqlQuery += ", imported = @imported";

            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id_studio", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_pmanager_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_id", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@created", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@process", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@content_type", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@industry", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@quality_level", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@imported", DbType.Boolean));


            cmdQuery.Parameters["@id"].Value = dqfProject.Id;
            cmdQuery.Parameters["@project_id"].Value = dqfProject.ProjectId;
            cmdQuery.Parameters["@project_id_studio"].Value = dqfProject.ProjectIdStudio;
            cmdQuery.Parameters["@dqf_pmanager_key"].Value = dqfProject.DqfPmanagerKey;
            cmdQuery.Parameters["@dqf_project_key"].Value = dqfProject.DqfProjectKey;
            cmdQuery.Parameters["@dqf_project_id"].Value = dqfProject.DqfProjectId;

            cmdQuery.Parameters["@name"].Value = dqfProject.Name;
            cmdQuery.Parameters["@created"].Value = Helper.DateTimeToSQLite(dqfProject.Created);
            cmdQuery.Parameters["@source_language"].Value = dqfProject.SourceLanguage;

            cmdQuery.Parameters["@process"].Value = dqfProject.Process;
            cmdQuery.Parameters["@content_type"].Value = dqfProject.ContentType;
            cmdQuery.Parameters["@industry"].Value = dqfProject.Industry;
            cmdQuery.Parameters["@quality_level"].Value = dqfProject.QualityLevel;

            cmdQuery.Parameters["@imported"].Value = dqfProject.Imported;


            cmdQuery.ExecuteNonQuery();

            return true;
        }


        public bool DeleteDqfProject(string databasePath, int projectId, int? id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = DeleteDqfProject(connection, projectId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool DeleteDqfProject(SQLiteConnection connection, int projectId, int? id)
        {
            if (projectId <= -1)
                throw new Exception("The id cannot be null!");

            if (id != null)
                DeleteDqfProjectTask(connection, id.Value, null);



            var sqlQuery = "DELETE FROM TausDQFProjects";
            sqlQuery += " WHERE project_id = @project_id";
            if (id != null)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@project_id"].Value = projectId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id;


            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion
        #region  |  Project.DQF.DQFProjectTasks  |

        public List<DqfProjectTask> GetDqfProjectTasks(string databasePath, int tableTausdqfprojectsId)
        {
            List<DqfProjectTask> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = GetDqfProjectTasks(connection, tableTausdqfprojectsId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private static List<DqfProjectTask> GetDqfProjectTasks(SQLiteConnection connection, int tableTausdqfprojectsId)
        {
            var values = new List<DqfProjectTask>();

            if (tableTausdqfprojectsId <= -1)
                throw new Exception("The  id cannot be null!");

            var sqlQuery = "SELECT * FROM TausDQFProjectTasks";
            sqlQuery += " WHERE table_tausdqfprojects_id = @table_tausdqfprojects_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@table_tausdqfprojects_id", DbType.Int32));
            cmdQuery.Parameters["@table_tausdqfprojects_id"].Value = tableTausdqfprojectsId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new DqfProjectTask();
                        value.Id = Convert.ToInt32(rdrSelect["id"]);
                        value.TableTausdqfprojectsId = Convert.ToInt32(rdrSelect["table_tausdqfprojects_id"]);
                        value.ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]);
                        value.DocumentId = rdrSelect["document_id"].ToString();
                        value.DocumentName = rdrSelect["document_name"].ToString();

                        value.DqfTranslatorKey = rdrSelect["dqf_translator_key"].ToString();
                        value.DqfProjectKey = rdrSelect["dqf_project_key"].ToString();
                        value.DqfProjectId = Convert.ToInt32(rdrSelect["dqf_project_id"]);
                        value.DqfTaskId = Convert.ToInt32(rdrSelect["dqf_task_id"]);

                        value.Uploaded = Helper.DateTimeFromSQLite(rdrSelect["uploaded"].ToString());
                        value.TargetLanguage = rdrSelect["target_language"].ToString();

                        value.CatTool = Convert.ToInt32(rdrSelect["cat_tool"]);
                        value.TotalSegments = Convert.ToInt32(rdrSelect["total_segments"]);


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table TausDQFProjectTasks!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return values;
        }

        public int CreateDqfProjectTask(string databasePath, DqfProjectTask dqfProjectTask)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = CreateDqfProjectTask(connection, dqfProjectTask);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int CreateDqfProjectTask(SQLiteConnection connection, DqfProjectTask dqfProjectTask)
        {
            var value = -1;
            if (dqfProjectTask.DocumentName.Trim() == string.Empty)
                throw new Exception("The document name cannot be null!");


            var sqlQuery = "INSERT INTO TausDQFProjectTasks";
            sqlQuery += " (";
            sqlQuery += " table_tausdqfprojects_id";
            sqlQuery += " ,project_activity_id";
            sqlQuery += ", document_id";
            sqlQuery += ", document_name";
            sqlQuery += ", dqf_translator_key";
            sqlQuery += ", dqf_project_key";
            sqlQuery += ", dqf_project_id";
            sqlQuery += ", dqf_task_id";

            sqlQuery += ", uploaded";
            sqlQuery += ", target_language";

            sqlQuery += ", cat_tool";
            sqlQuery += ", total_segments";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @table_tausdqfprojects_id";
            sqlQuery += ", @project_activity_id";
            sqlQuery += ", @document_id";
            sqlQuery += ", @document_name";
            sqlQuery += ", @dqf_translator_key";
            sqlQuery += ", @dqf_project_key";
            sqlQuery += ", @dqf_project_id";
            sqlQuery += ", @dqf_task_id";

            sqlQuery += ", @uploaded";
            sqlQuery += ", @target_language";

            sqlQuery += ", @cat_tool";
            sqlQuery += ", @total_segments";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@table_tausdqfprojects_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_name", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_translator_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_task_id", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@uploaded", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@cat_tool", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@total_segments", DbType.Int32));


            cmdQuery.Parameters["@table_tausdqfprojects_id"].Value = dqfProjectTask.TableTausdqfprojectsId;
            cmdQuery.Parameters["@project_activity_id"].Value = dqfProjectTask.ProjectActivityId;
            cmdQuery.Parameters["@document_id"].Value = dqfProjectTask.DocumentId;
            cmdQuery.Parameters["@document_name"].Value = dqfProjectTask.DocumentName;

            cmdQuery.Parameters["@dqf_translator_key"].Value = dqfProjectTask.DqfTranslatorKey;
            cmdQuery.Parameters["@dqf_project_key"].Value = dqfProjectTask.DqfProjectKey;
            cmdQuery.Parameters["@dqf_project_id"].Value = dqfProjectTask.DqfProjectId;
            cmdQuery.Parameters["@dqf_task_id"].Value = dqfProjectTask.DqfTaskId;

            cmdQuery.Parameters["@uploaded"].Value = Helper.DateTimeToSQLite(dqfProjectTask.Uploaded);
            cmdQuery.Parameters["@target_language"].Value = dqfProjectTask.TargetLanguage;

            cmdQuery.Parameters["@cat_tool"].Value = dqfProjectTask.CatTool;
            cmdQuery.Parameters["@total_segments"].Value = dqfProjectTask.TotalSegments;


            dqfProjectTask.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = dqfProjectTask.Id;

            return value;
        }

        public bool UpdateDqfProjectTask(string databasePath, DqfProjectTask dqfProjectTask)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = UpdateDqfProjectTask(connection, dqfProjectTask);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool UpdateDqfProjectTask(SQLiteConnection connection, DqfProjectTask dqfProjectTask)
        {
            if (dqfProjectTask.Id == -1)
                throw new Exception("The id cannot be null!");


            var sqlQuery = "UPDATE TausDQFProjectTasks";
            sqlQuery += " SET";
            sqlQuery += " table_tausdqfprojects_id = @table_tausdqfprojects_id";
            sqlQuery += ", project_activity_id = @project_activity_id";
            sqlQuery += ", document_id = @document_id";
            sqlQuery += ", document_name = @document_name";

            sqlQuery += ", dqf_translator_key = @dqf_translator_key";
            sqlQuery += ", dqf_project_key = @dqf_project_key";
            sqlQuery += ", dqf_project_id = @dqf_project_id";
            sqlQuery += ", dqf_task_id = @dqf_task_id";


            sqlQuery += ", uploaded = @uploaded";
            sqlQuery += ", target_language = @target_language";

            sqlQuery += ", cat_tool = @cat_tool";
            sqlQuery += ", total_segments = @total_segments";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@table_tausdqfprojects_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_name", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_translator_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_key", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@dqf_task_id", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@uploaded", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));

            cmdQuery.Parameters.Add(new SQLiteParameter("@cat_tool", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@total_segments", DbType.Int32));


            cmdQuery.Parameters["@id"].Value = dqfProjectTask.Id;
            cmdQuery.Parameters["@table_tausdqfprojects_id"].Value = dqfProjectTask.TableTausdqfprojectsId;
            cmdQuery.Parameters["@project_activity_id"].Value = dqfProjectTask.ProjectActivityId;
            cmdQuery.Parameters["@document_id"].Value = dqfProjectTask.DocumentId;
            cmdQuery.Parameters["@document_name"].Value = dqfProjectTask.DocumentName;

            cmdQuery.Parameters["@dqf_translator_key"].Value = dqfProjectTask.DqfTranslatorKey;
            cmdQuery.Parameters["@dqf_project_key"].Value = dqfProjectTask.DqfProjectKey;
            cmdQuery.Parameters["@dqf_project_id"].Value = dqfProjectTask.DqfProjectId;
            cmdQuery.Parameters["@dqf_task_id"].Value = dqfProjectTask.DqfTaskId;

            cmdQuery.Parameters["@uploaded"].Value = Helper.DateTimeToSQLite(dqfProjectTask.Uploaded);
            cmdQuery.Parameters["@target_language"].Value = dqfProjectTask.TargetLanguage;

            cmdQuery.Parameters["@cat_tool"].Value = dqfProjectTask.CatTool;
            cmdQuery.Parameters["@total_segments"].Value = dqfProjectTask.TotalSegments;


            cmdQuery.ExecuteNonQuery();

            return true;
        }


        public bool DeleteDqfProjectTask(string databasePath, int tableTausdqfprojectsId, int? id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = DeleteDqfProjectTask(connection, tableTausdqfprojectsId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool DeleteDqfProjectTask(SQLiteConnection connection, int tableTausdqfprojectsId, int? id)
        {
            if (tableTausdqfprojectsId <= -1)
                throw new Exception("The id cannot be null!");


            var sqlQuery = "DELETE FROM TausDQFProjectTasks";
            sqlQuery += " WHERE table_tausdqfprojects_id = @table_tausdqfprojects_id";
            if (id != null)
                sqlQuery += " AND id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@table_tausdqfprojects_id", DbType.Int32));
            if (id != null)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@table_tausdqfprojects_id"].Value = tableTausdqfprojectsId;
            if (id != null)
                cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();


            return true;
        }


        #endregion


        #region  |  Document  |


        public Document GetDocument(string databasePath, string documentId)
        {
            Document value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = getDocument(connection, documentId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static Document getDocument(SQLiteConnection connection, string documentId)
        {
            var value = new Document();

            var sqlQuery = "SELECT * FROM Documents";
            sqlQuery += " WHERE document_id = @document_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));

            cmdQuery.Parameters["@document_id"].Value = documentId;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    rdrSelect.Read();


                    value.Id = Convert.ToInt32(rdrSelect["id"]);
                    value.DocumentId = rdrSelect["document_id"].ToString();
                    value.DocumentName = rdrSelect["document_name"].ToString();
                    value.DocumentPath = rdrSelect["document_path"].ToString();
                    value.StudioProjectId = rdrSelect["studio_project_id"].ToString();

                    value.SourceLanguage = rdrSelect["source_language"].ToString();
                    value.TargetLanguage = rdrSelect["target_language"].ToString();



                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table Documents!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            return value;
        }

        public int CreateDocument(string databasePath, Document document)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createDocument(connection, document);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static int createDocument(SQLiteConnection connection, Document document)
        {
            var value = -1;
            if (document.DocumentId.Trim() == string.Empty)
                throw new Exception("The document id cannot be null!");

            var sqlQuery = "INSERT INTO Documents";
            sqlQuery += " (";
            sqlQuery += " document_id";
            sqlQuery += ", document_name";
            sqlQuery += ", document_path";
            sqlQuery += ", studio_project_id";
            sqlQuery += ", source_language";
            sqlQuery += ", target_language";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @document_id";
            sqlQuery += ", @document_name";
            sqlQuery += ", @document_path";
            sqlQuery += ", @studio_project_id";
            sqlQuery += ", @source_language";
            sqlQuery += ", @target_language";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));


            cmdQuery.Parameters["@document_id"].Value = document.DocumentId;
            cmdQuery.Parameters["@document_name"].Value = document.DocumentName;
            cmdQuery.Parameters["@document_path"].Value = document.DocumentPath;
            cmdQuery.Parameters["@studio_project_id"].Value = document.StudioProjectId;
            cmdQuery.Parameters["@source_language"].Value = document.SourceLanguage;
            cmdQuery.Parameters["@target_language"].Value = document.TargetLanguage;


            document.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = document.Id;

            return value;
        }

        public bool UpdateDocument(string databasePath, Document document)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateDocument(connection, document);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private static bool updateDocument(SQLiteConnection connection, Document document)
        {
            if (document.DocumentId.Trim() == string.Empty)
                throw new Exception("The document id cannot be null!");

            var sqlQuery = "UPDATE Documents";
            sqlQuery += " SET";
            sqlQuery += " document_name = @document_name";
            sqlQuery += ", document_path = @document_path";
            sqlQuery += ", studio_project_id = @studio_project_id";
            sqlQuery += ", source_language = @source_language";
            sqlQuery += ", target_language = @target_language";
            sqlQuery += " WHERE";
            sqlQuery += " document_id = @document_id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_name", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_path", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@studio_project_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@source_language", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@target_language", DbType.String));


            cmdQuery.Parameters["@document_id"].Value = document.DocumentId;
            cmdQuery.Parameters["@document_name"].Value = document.DocumentName;
            cmdQuery.Parameters["@document_path"].Value = document.DocumentPath;
            cmdQuery.Parameters["@studio_project_id"].Value = document.StudioProjectId;
            cmdQuery.Parameters["@source_language"].Value = document.SourceLanguage;
            cmdQuery.Parameters["@target_language"].Value = document.TargetLanguage;


            cmdQuery.ExecuteNonQuery();

            return true;
        }

        public int SaveDocument(string databasePath, Document document)
        {
            var value = -1;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();

                try
                {
                    value = saveDocument(connection, document);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static int saveDocument(SQLiteConnection connection, Document document)
        {
            var value = document.Id;
            var existingDocument = getDocument(connection, document.DocumentId);

            var exists = false;
            var isDifferent = false;

            if (existingDocument.Id > -1)
            {
                exists = true;
                if (!Helper.AreObjectsEqual(existingDocument, document))
                    isDifferent = true;
            }


            if (!exists && existingDocument.Id == -1)
            {
                value = createDocument(connection, document);
            }
            else if (isDifferent)
            {
                updateDocument(connection, document);
                value = document.Id;
            }


            return value;
        }

        public bool DeleteDocument(string databasePath, string documentId)
        {


            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteDocument(connection, documentId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteDocument(SQLiteConnection connection, string documentId)
        {
            if (documentId.Trim() != string.Empty)
                throw new Exception("The id cannot be null!");

            var sqlQuery = "DELETE FROM Documents";
            sqlQuery += " WHERE document_id = @document_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.Int32));
            cmdQuery.Parameters["@document_id"].Value = documentId;

            cmdQuery.ExecuteNonQuery();

            return true;
        }


        #endregion


        #region  |  Document.Activity  |

        public List<DocumentActivity> GetDocumentActivities(string databasePath, string databasePathProjects, int projectActivityId, int? id)
        {
            List<DocumentActivity> values;
            if (!File.Exists(databasePath))
                Helper.InitializeDatabasesFirst("Sdl.Community.TM.Database.New.Project.sqlite", databasePath);


            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getDocumentActivities(connection, databasePathProjects, projectActivityId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<DocumentActivity> getDocumentActivities(SQLiteConnection connection, string databasePathProjects, int projectActivityId, int? id)
        {
            var values = new List<DocumentActivity>();

            var sqlQuery = "SELECT * FROM DocumentActivities";
            sqlQuery += " WHERE project_activity_id = @project_activity_id";
            if (id.HasValue)
                sqlQuery += " AND id = @id";
            sqlQuery += " ORDER BY started";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            if (id.HasValue)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@project_activity_id"].Value = projectActivityId;
            if (id.HasValue)
                cmdQuery.Parameters["@id"].Value = id;




            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {
                        var value = new DocumentActivity
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            ProjectActivityId = Convert.ToInt32(rdrSelect["project_activity_id"]),
                            ProjectId = Convert.ToInt32(rdrSelect["project_id"]),
                            DocumentId = rdrSelect["document_id"].ToString(),
                            DocumentActivityType = rdrSelect["document_activity_type"].ToString(),
                            Started = Helper.DateTimeFromSQLite(rdrSelect["started"].ToString()),
                            Stopped = Helper.DateTimeFromSQLite(rdrSelect["stopped"].ToString()),
                            TicksActivity = Convert.ToInt64(rdrSelect["ticks_activity"]),
                            TicksRecords = Convert.ToInt64(rdrSelect["ticks_records"]),
                            WordCount = Convert.ToInt32(rdrSelect["word_count"]),

                        };


                        value.DocumentStateCounters =
                            Deserialize<DocumentStateCounters>(rdrSelect["xml_activity_state_counters"].ToString());
                        #region  |  assign object parent ids  |
                        foreach (var sci in value.DocumentStateCounters.ConfirmationStatuses)
                        {
                            sci.DocumentActivityId = value.Id;
                        }
                        foreach (var sci in value.DocumentStateCounters.TranslationMatchTypes)
                        {
                            sci.DocumentActivityId = value.Id;
                        }
                        #endregion


                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table DocumentActivities!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            foreach (var value in values)
                value.Records = getRecords(connection, value.Id, null);





            //we will now need to get the document object form the projects database
            using (var connectionProjects = new SQLiteConnection(GetConnectionString(databasePathProjects)))
            {
                connectionProjects.Open();
                try
                {
                    foreach (var value in values)
                    {
                        value.TranslatableDocument = getDocument(connectionProjects, value.DocumentId);
                    }
                }
                finally
                {
                    connectionProjects.Close();
                }
            }






            return values;
        }

        public int CreateDocumentActivity(string databasePath, DocumentActivity documentActivity)
        {
            var value = -1;
            if (!File.Exists(databasePath))
                Helper.InitializeDatabasesFirst("Sdl.Community.TM.Database.New.Project.sqlite", databasePath);


            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = createDocumentActivity(connection, documentActivity);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private int createDocumentActivity(SQLiteConnection connection, DocumentActivity documentActivity)
        {
            var value = -1;
            if (documentActivity.DocumentId.Trim() == string.Empty)
                throw new Exception("The document id cannot be null!");

            var sqlQuery = "INSERT INTO DocumentActivities";
            sqlQuery += " (";
            sqlQuery += " project_activity_id";
            sqlQuery += ", project_id";
            sqlQuery += ", document_id";
            sqlQuery += ", document_activity_type";
            sqlQuery += ", started";
            sqlQuery += ", stopped";
            sqlQuery += ", ticks_activity";
            sqlQuery += ", ticks_records";
            sqlQuery += ", word_count";
            sqlQuery += ", xml_activity_state_counters";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @project_activity_id";
            sqlQuery += ", @project_id";
            sqlQuery += ", @document_id";
            sqlQuery += ", @document_activity_type";
            sqlQuery += ", @started";
            sqlQuery += ", @stopped";
            sqlQuery += ", @ticks_activity";
            sqlQuery += ", @ticks_records";
            sqlQuery += ", @word_count";
            sqlQuery += ", @xml_activity_state_counters";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_activity", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_records", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@word_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_activity_state_counters", DbType.String));


            cmdQuery.Parameters["@project_activity_id"].Value = documentActivity.ProjectActivityId;
            cmdQuery.Parameters["@project_id"].Value = documentActivity.ProjectId;
            cmdQuery.Parameters["@document_id"].Value = documentActivity.DocumentId;
            cmdQuery.Parameters["@document_activity_type"].Value = documentActivity.DocumentActivityType;
            cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(documentActivity.Started);
            cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(documentActivity.Stopped);
            cmdQuery.Parameters["@ticks_activity"].Value = documentActivity.TicksActivity;
            cmdQuery.Parameters["@ticks_records"].Value = documentActivity.TicksRecords;
            cmdQuery.Parameters["@word_count"].Value = documentActivity.WordCount;
            cmdQuery.Parameters["@xml_activity_state_counters"].Value = Serialize(documentActivity.DocumentStateCounters);


            documentActivity.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
            value = documentActivity.Id;

            #region  |  create records  |

            foreach (var record in documentActivity.Records)
                record.DocumentActivityId = documentActivity.Id;

            CreateRecords(connection, documentActivity.Records);

            #endregion

            return value;
        }

        public bool UpdateDocumentActivity(string databasePath, DocumentActivity documentActivity)
        {
            bool value;
            if (!File.Exists(databasePath))
                Helper.InitializeDatabasesFirst("Sdl.Community.TM.Database.New.Project.sqlite", databasePath);

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = updateDocumentActivity(connection, documentActivity);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private bool updateDocumentActivity(SQLiteConnection connection, DocumentActivity documentActivity)
        {

            if (documentActivity.Id <= -1)
                throw new Exception("The document id cannot be null!");

            var sqlQuery = "UPDATE DocumentActivities";
            sqlQuery += " SET";
            sqlQuery += " project_activity_id = @project_activity_id";
            sqlQuery += ", project_id = @project_id";
            sqlQuery += ", document_id = @document_id";
            sqlQuery += ", document_activity_type = @document_activity_type";
            sqlQuery += ", started = @started";
            sqlQuery += ", stopped = @stopped";
            sqlQuery += ", ticks_activity = @ticks_activity";
            sqlQuery += ", ticks_records = @ticks_records";
            sqlQuery += ", word_count = @word_count";
            sqlQuery += ", xml_activity_state_counters = @xml_activity_state_counters";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@project_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_type", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_activity", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_records", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@word_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_activity_state_counters", DbType.String));

            cmdQuery.Parameters["@id"].Value = documentActivity.Id;
            cmdQuery.Parameters["@project_activity_id"].Value = documentActivity.ProjectActivityId;
            cmdQuery.Parameters["@project_id"].Value = documentActivity.ProjectId;
            cmdQuery.Parameters["@document_id"].Value = documentActivity.DocumentId;
            cmdQuery.Parameters["@document_activity_type"].Value = documentActivity.DocumentActivityType;
            cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(documentActivity.Started);
            cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(documentActivity.Stopped);
            cmdQuery.Parameters["@ticks_activity"].Value = documentActivity.TicksActivity;
            cmdQuery.Parameters["@ticks_records"].Value = documentActivity.TicksRecords;
            cmdQuery.Parameters["@word_count"].Value = documentActivity.WordCount;
            cmdQuery.Parameters["@xml_activity_state_counters"].Value = Serialize(documentActivity.DocumentStateCounters);


            cmdQuery.ExecuteNonQuery();

            foreach (var record in documentActivity.Records)
                record.DocumentActivityId = documentActivity.Id;

            var value = UpdateRecords(connection, documentActivity.Records);


            return value;
        }

        public bool DeleteDocumentActivity(string databasePath, int projectActivityId, int? id)
        {


            bool value;
            if (!File.Exists(databasePath))
                Helper.InitializeDatabasesFirst("Sdl.Community.TM.Database.New.Project.sqlite", databasePath);

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = deleteDocumentActivity(connection, projectActivityId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return value;
        }
        private static bool deleteDocumentActivity(SQLiteConnection connection, int projectActivityId, int? id)
        {
            var success = false;

            if (id <= -1)
                throw new Exception("The id cannot be null!");

            var ids = new List<int>();
            if (id == null)
            {
                #region  |  if only project activity id is present  |

                var sqlQuery1 = "SELECT * FROM DocumentActivities";
                sqlQuery1 += " WHERE project_activity_id = @project_activity_id";
                var cmdQuery1 = new SQLiteCommand(sqlQuery1, connection);
                cmdQuery1.Parameters.Add(new SQLiteParameter("@project_activity_id", DbType.Int32));
                cmdQuery1.Parameters["@project_activity_id"].Value = projectActivityId;

                var rdrSelect = cmdQuery1.ExecuteReader();
                try
                {
                    if (rdrSelect.HasRows)
                    {
                        while (rdrSelect.Read())
                        {
                            ids.Add(Convert.ToInt32(rdrSelect["id"]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error querying the table DocumentActivities!  " + ex.Message);
                }
                finally
                {
                    if (rdrSelect != null)
                        if (!rdrSelect.IsClosed)
                            rdrSelect.Close();
                }

                #endregion
            }
            else
            {
                ids.Add(id.Value);
            }


            var sqlQuery = "DELETE FROM DocumentActivities";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            foreach (var documentId in ids)
            {
                cmdQuery.Parameters["@id"].Value = documentId;

                cmdQuery.ExecuteNonQuery();

                success = DeleteRecords(connection, documentId);
            }

            return success;
        }

        #endregion


        #region  |  Document.Activity.Record  |

        public List<Record> GetRecords(string databasePath, int documentActivityId, int? id)
        {
            List<Record> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getRecords(connection, documentActivityId, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<Record> getRecords(SQLiteConnection connection, int documentActivityId, int? id)
        {
            var values = new List<Record>();

            var sqlQuery = "SELECT * FROM DocumentActivityRecords";
            sqlQuery += " WHERE document_activity_id = @document_activity_id";
            if (id.HasValue)
                sqlQuery += " AND id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_id", DbType.Int32));
            if (id.HasValue)
                cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));

            cmdQuery.Parameters["@document_activity_id"].Value = documentActivityId;
            if (id.HasValue)
                cmdQuery.Parameters["@id"].Value = id;

            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    var dbSupport = 0; // first release;
                    if (rdrSelect.FieldCount > 15)
                        dbSupport = 1; // updated on the 2016-11-04; manage properties for char, tag & placeable counts

                    while (rdrSelect.Read())
                    {
                        var value = new Record
                        {
                            Id = Convert.ToInt32(rdrSelect["id"]),
                            DocumentActivityId = Convert.ToInt32(rdrSelect["document_activity_id"]),
                            ParagraphId = rdrSelect["paragraph_id"].ToString(),
                            SegmentId = rdrSelect["segment_id"].ToString(),
                            WordCount = Convert.ToInt32(rdrSelect["word_count"]),
                            CharsCount = dbSupport > 0 ? Convert.ToInt32(rdrSelect["char_count"]) : 0,
                            TagsCount = dbSupport > 0 ? Convert.ToInt32(rdrSelect["tag_count"]) : 0,
                            PlaceablesCount = dbSupport > 0 ? Convert.ToInt32(rdrSelect["placeable_count"]) : 0,
                            Started = Helper.DateTimeFromSQLite(rdrSelect["started"].ToString()),
                            Stopped = Helper.DateTimeFromSQLite(rdrSelect["stopped"].ToString()),
                            TicksElapsed = Convert.ToInt64(rdrSelect["ticks_elapsed"]),
                        };


                        value.TranslationOrigins =
                            Deserialize<TranslationOrigins>(rdrSelect["xml_translation_origins"].ToString());
                        #region  |  assign object parent ids  |
                        value.TranslationOrigins.Original.RecordId = value.Id;
                        value.TranslationOrigins.Original.DocumentActivityId = value.DocumentActivityId;
                        value.TranslationOrigins.Updated.RecordId = value.Id;
                        value.TranslationOrigins.Updated.DocumentActivityId = value.DocumentActivityId;
                        value.TranslationOrigins.UpdatedPrevious.RecordId = value.Id;
                        value.TranslationOrigins.UpdatedPrevious.DocumentActivityId = value.DocumentActivityId;
                        #endregion

                        value.ContentSections = Deserialize<ContentSections>(rdrSelect["xml_content_sections"].ToString());
                        #region  |  assign object parent ids  |
                        foreach (var cs in value.ContentSections.SourceSections)
                        {
                            cs.RecordId = value.Id;
                            cs.DocumentActivityId = value.DocumentActivityId;

                            if (cs.HasRevision)
                            {
                                cs.RevisionMarker.ContentSectionId = cs.Id;
                                cs.RevisionMarker.RecordId = cs.RecordId;
                                cs.RevisionMarker.DocumentActivityId = cs.DocumentActivityId;
                            }
                        }
                        foreach (var cs in value.ContentSections.TargetOriginalSections)
                        {
                            cs.RecordId = value.Id;
                            cs.DocumentActivityId = value.DocumentActivityId;

                            if (cs.HasRevision)
                            {
                                cs.RevisionMarker.ContentSectionId = cs.Id;
                                cs.RevisionMarker.RecordId = cs.RecordId;
                                cs.RevisionMarker.DocumentActivityId = cs.DocumentActivityId;
                            }
                        }
                        foreach (var cs in value.ContentSections.TargetUpdatedSections)
                        {
                            cs.RecordId = value.Id;
                            cs.DocumentActivityId = value.DocumentActivityId;

                            if (cs.HasRevision)
                            {
                                cs.RevisionMarker.ContentSectionId = cs.Id;
                                cs.RevisionMarker.RecordId = cs.RecordId;
                                cs.RevisionMarker.DocumentActivityId = cs.DocumentActivityId;
                            }
                        }
                        #endregion

                        value.TargetKeyStrokes = DeserializeList<KeyStroke>(rdrSelect["xml_key_strokes"].ToString());
                        #region  |  assign object parent ids  |
                        foreach (var ks in value.TargetKeyStrokes)
                        {
                            ks.RecordId = value.Id;
                            ks.DocumentActivityId = value.DocumentActivityId;
                        }
                        #endregion

                        value.QualityMetrics = DeserializeList<Sdl.Community.Structures.Documents.Records.QualityMetric>(rdrSelect["xml_quality_metrics"].ToString());
                        #region  |  assign object parent ids  |
                        foreach (var qm in value.QualityMetrics)
                        {
                            qm.RecordId = value.Id;
                            qm.DocumentActivityId = value.DocumentActivityId;
                        }
                        #endregion

                        value.Comments = DeserializeList<Comment>(rdrSelect["xml_comments"].ToString());
                        #region  |  assign object parent ids  |
                        foreach (var cm in value.Comments)
                        {
                            cm.RecordId = value.Id;
                            cm.DocumentActivityId = value.DocumentActivityId;
                        }
                        #endregion

                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table DocumentActivityRecords!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }



            return values;
        }

        public List<int> CreateRecord(string databasePath, List<Record> records)
        {
            List<int> values;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = CreateRecords(connection, records);
                }
                finally
                {
                    connection.Close();
                }
            }


            return values;
        }
        private List<int> CreateRecords(SQLiteConnection connection, IReadOnlyCollection<Record> records)
        {
            VerifyDocumentActivityRecordsSupportLevel(connection, "DocumentActivityRecords");

            var values = new List<int>();
            var sqlQuery = "INSERT INTO DocumentActivityRecords";
            sqlQuery += " (";
            sqlQuery += " document_activity_id";
            sqlQuery += ", paragraph_id";
            sqlQuery += ", segment_id";
            sqlQuery += ", word_count";

            sqlQuery += ", char_count";
            sqlQuery += ", tag_count";
            sqlQuery += ", placeable_count";

            sqlQuery += ", started";
            sqlQuery += ", stopped";
            sqlQuery += ", ticks_elapsed";
            sqlQuery += ", xml_translation_origins";
            sqlQuery += ", xml_content_sections";
            sqlQuery += ", xml_key_strokes";
            sqlQuery += ", xml_quality_metrics";
            sqlQuery += ", xml_comments";
            sqlQuery += " ) VALUES (";
            sqlQuery += " @document_activity_id";
            sqlQuery += ", @paragraph_id";
            sqlQuery += ", @segment_id";
            sqlQuery += ", @word_count";

            sqlQuery += ", @char_count";
            sqlQuery += ", @tag_count";
            sqlQuery += ", @placeable_count";

            sqlQuery += ", @started";
            sqlQuery += ", @stopped";
            sqlQuery += ", @ticks_elapsed";
            sqlQuery += ", @xml_translation_origins";
            sqlQuery += ", @xml_content_sections";
            sqlQuery += ", @xml_key_strokes";
            sqlQuery += ", @xml_quality_metrics";
            sqlQuery += ", @xml_comments";
            sqlQuery += " ); SELECT last_insert_rowid();";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@paragraph_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@segment_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@word_count", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@char_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@tag_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@placeable_count", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_elapsed", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_translation_origins", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_content_sections", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_key_strokes", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_quality_metrics", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_comments", DbType.String));



            var index = 0;
            foreach (var record in records)
            {
                index++;

                if (ProgressChanged != null)
                    ProgressChanged(records.Count, index, string.Format("Inserting {0} of {1} records", index, records.Count));


                if (record.DocumentActivityId <= -1)
                    throw new Exception("The activity id cannot be null!");

                cmdQuery.Parameters["@document_activity_id"].Value = record.DocumentActivityId;
                cmdQuery.Parameters["@paragraph_id"].Value = record.ParagraphId;
                cmdQuery.Parameters["@segment_id"].Value = record.SegmentId;
                cmdQuery.Parameters["@word_count"].Value = record.WordCount;

                cmdQuery.Parameters["@char_count"].Value = record.CharsCount;
                cmdQuery.Parameters["@tag_count"].Value = record.TagsCount;
                cmdQuery.Parameters["@placeable_count"].Value = record.PlaceablesCount;

                cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(record.Started);
                cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(record.Stopped);
                cmdQuery.Parameters["@ticks_elapsed"].Value = record.TicksElapsed;
                cmdQuery.Parameters["@xml_translation_origins"].Value = Serialize(record.TranslationOrigins);
                cmdQuery.Parameters["@xml_content_sections"].Value = Serialize(record.ContentSections);
                cmdQuery.Parameters["@xml_key_strokes"].Value = SerializeList(record.TargetKeyStrokes);
                cmdQuery.Parameters["@xml_quality_metrics"].Value = SerializeList(record.QualityMetrics);
                cmdQuery.Parameters["@xml_comments"].Value = SerializeList(record.Comments);

                record.Id = Convert.ToInt32(cmdQuery.ExecuteScalar());
                values.Add(record.Id);

            }

            return values;
        }

        public bool UpdateRecord(string databasePath, List<Record> records)
        {
            bool value;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    value = UpdateRecords(connection, records);
                }
                finally
                {
                    connection.Close();
                }
            }


            return value;
        }
        private bool UpdateRecords(SQLiteConnection connection, IReadOnlyCollection<Record> records)
        {
            VerifyDocumentActivityRecordsSupportLevel(connection, "DocumentActivityRecords");

            var sqlQuery = "UPDATE DocumentActivityRecords";
            sqlQuery += " SET";
            sqlQuery += " document_activity_id = @document_activity_id";
            sqlQuery += ", paragraph_id = @paragraph_id";
            sqlQuery += ", segment_id = @segment_id";
            sqlQuery += ", word_count = @word_count";

            sqlQuery += ", char_count = @char_count";
            sqlQuery += ", tag_count = @tag_count";
            sqlQuery += ", placeable_count = @placeable_count";

            sqlQuery += ", started = @started";
            sqlQuery += ", stopped = @stopped";
            sqlQuery += ", ticks_elapsed = @ticks_elapsed";
            sqlQuery += ", xml_translation_origins = @xml_translation_origins";
            sqlQuery += ", xml_content_sections = @xml_content_sections";
            sqlQuery += ", xml_key_strokes = @xml_key_strokes";
            sqlQuery += ", xml_quality_metrics = @xml_quality_metrics";
            sqlQuery += ", xml_comments = @xml_comments";
            sqlQuery += " WHERE";
            sqlQuery += " id = @id";


            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_id", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@paragraph_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@segment_id", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@word_count", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@char_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@tag_count", DbType.Int32));
            cmdQuery.Parameters.Add(new SQLiteParameter("@placeable_count", DbType.Int32));

            cmdQuery.Parameters.Add(new SQLiteParameter("@started", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@stopped", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@ticks_elapsed", DbType.Int64));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_translation_origins", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_content_sections", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_key_strokes", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_quality_metrics", DbType.String));
            cmdQuery.Parameters.Add(new SQLiteParameter("@xml_comments", DbType.String));

            var index = 0;
            foreach (var record in records)
            {
                index++;

                if (ProgressChanged != null)
                    ProgressChanged(records.Count, index, string.Format("Updating {0} of {1} records", index, records.Count));

                if (record.Id <= -1)
                    throw new Exception("The activity id cannot be null!");

                cmdQuery.Parameters["@id"].Value = record.Id;
                cmdQuery.Parameters["@document_activity_id"].Value = record.DocumentActivityId;
                cmdQuery.Parameters["@paragraph_id"].Value = record.ParagraphId;
                cmdQuery.Parameters["@segment_id"].Value = record.SegmentId;
                cmdQuery.Parameters["@word_count"].Value = record.WordCount;

                cmdQuery.Parameters["@char_count"].Value = record.CharsCount;
                cmdQuery.Parameters["@tag_count"].Value = record.TagsCount;
                cmdQuery.Parameters["@placeable_count"].Value = record.PlaceablesCount;

                cmdQuery.Parameters["@started"].Value = Helper.DateTimeToSQLite(record.Started);
                cmdQuery.Parameters["@stopped"].Value = Helper.DateTimeToSQLite(record.Stopped);
                cmdQuery.Parameters["@ticks_elapsed"].Value = record.TicksElapsed;
                cmdQuery.Parameters["@xml_translation_origins"].Value = Serialize(record.TranslationOrigins);
                cmdQuery.Parameters["@xml_content_sections"].Value = Serialize(record.ContentSections);
                cmdQuery.Parameters["@xml_key_strokes"].Value = SerializeList(record.TargetKeyStrokes);
                cmdQuery.Parameters["@xml_quality_metrics"].Value = SerializeList(record.QualityMetrics);
                cmdQuery.Parameters["@xml_comments"].Value = SerializeList(record.Comments);

                cmdQuery.ExecuteNonQuery();


            }

            return true;
        }

        public bool DeleteRecord(string databasePath, int id)
        {
            bool success;
            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    success = deleteRecord(connection, id);
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
        private static bool deleteRecord(SQLiteConnection connection, int id)
        {


            if (id <= -1)
                throw new Exception("The id cannot be null!");


            var sqlQuery = "DELETE FROM DocumentActivityRecords";
            sqlQuery += " WHERE id = @id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@id", DbType.Int32));
            cmdQuery.Parameters["@id"].Value = id;

            cmdQuery.ExecuteNonQuery();

            return true;
        }
        private static bool DeleteRecords(SQLiteConnection connection, int documentActivityId)
        {

            if (documentActivityId <= -1)
                throw new Exception("The id cannot be null!");


            var sqlQuery = "DELETE FROM DocumentActivityRecords";
            sqlQuery += " WHERE document_activity_id = @document_activity_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_activity_id", DbType.Int32));
            cmdQuery.Parameters["@document_activity_id"].Value = documentActivityId;

            cmdQuery.ExecuteNonQuery();



            return true;
        }


        private static void VerifyDocumentActivityRecordsSupportLevel(SQLiteConnection connection, string tableName)
        {
            var dbSupportLevel = 0; // first release;

            var sqlQuery = "PRAGMA table_info('" + tableName + "')";
            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {
                if (rdrSelect.HasRows)
                {
                    var columns = 0;
                    while (rdrSelect.Read())
                        columns++;

                    if (columns >= 16)
                        dbSupportLevel = 1;// updated on the 2016-11-04; manage properties for char, tag & placeable counts
                    if (columns > 13 && columns < 16)
                        throw new Exception("Incorrect number of columns for '" + tableName + "'");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying table '" + tableName + "' " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }

            if (dbSupportLevel != 0) 
                return;

            sqlQuery = " ALTER TABLE '" + tableName + "' ADD COLUMN char_count INTEGER NOT NULL DEFAULT 0;";
            cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.ExecuteNonQuery();

            sqlQuery = " ALTER TABLE '" + tableName + "' ADD COLUMN tag_count INTEGER NOT NULL DEFAULT 0;";
            cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.ExecuteNonQuery();

            sqlQuery = " ALTER TABLE '" + tableName + "' ADD COLUMN placeable_count INTEGER NOT NULL DEFAULT 0;";
            cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.ExecuteNonQuery();
        }


        #endregion
        #region  |  Document.Activity.Record.Metric  |

        public List<Sdl.Community.Structures.Documents.Records.QualityMetric> GetAllQualityMetrics(string databasePath, string documentId)
        {
            List<Sdl.Community.Structures.Documents.Records.QualityMetric> values;
            if (!File.Exists(databasePath))
                Helper.InitializeDatabasesFirst("Sdl.Community.TM.Database.New.Project.sqlite", databasePath);

            using (var connection = new SQLiteConnection(GetConnectionString(databasePath)))
            {
                connection.Open();
                try
                {
                    values = getAllQualityMetrics(connection, documentId);
                }
                finally
                {
                    connection.Close();
                }
            }

            return values;
        }
        private List<Sdl.Community.Structures.Documents.Records.QualityMetric> getAllQualityMetrics(SQLiteConnection connection, string documentId)
        {
            var values = new List<Sdl.Community.Structures.Documents.Records.QualityMetric>();

            var valuesTmp = new List<Sdl.Community.Structures.Documents.Records.QualityMetric>();

            var sqlQuery = "SELECT t1.*";
            sqlQuery += " FROM DocumentActivityRecords t1";
            sqlQuery += " INNER JOIN DocumentActivities t2";
            sqlQuery += " ON t2.id = t1.document_activity_id";
            sqlQuery += " WHERE t2.document_id =  @document_id";

            var cmdQuery = new SQLiteCommand(sqlQuery, connection);
            cmdQuery.Parameters.Add(new SQLiteParameter("@document_id", DbType.String));
            cmdQuery.Parameters["@document_id"].Value = documentId;



            var rdrSelect = cmdQuery.ExecuteReader();
            try
            {

                if (rdrSelect.HasRows)
                {
                    while (rdrSelect.Read())
                    {


                        var id = Convert.ToInt32(rdrSelect["id"]);
                        var documentActivityId = Convert.ToInt32(rdrSelect["document_activity_id"]);

                        valuesTmp.AddRange(DeserializeList<Sdl.Community.Structures.Documents.Records.QualityMetric>(rdrSelect["xml_quality_metrics"].ToString()));
                        foreach (var qm in valuesTmp)
                        {
                            qm.RecordId = id;
                            qm.DocumentActivityId = documentActivityId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying the table DocumentActivityRecords!  " + ex.Message);
            }
            finally
            {
                if (rdrSelect != null)
                    if (!rdrSelect.IsClosed)
                        rdrSelect.Close();
            }


            valuesTmp = valuesTmp.OrderByDescending(a => a.Modified.Value).ToList();
            var guids = new List<string>();
            foreach (var qm in valuesTmp)
            {
                if (guids.Contains(qm.Guid)) continue;
                guids.Add(qm.Guid);
                values.Add(qm);
            }



            return values;
        }

        #endregion



        #region  |  serialization  |

        public string SerializeList<T>(List<T> obj)
        {
            string result;
            using (var stream = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(List<T>));
                s.Serialize(XmlWriter.Create(stream), obj);
                stream.Flush();

                stream.Position = 0;
                var sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }
        public List<T> DeserializeList<T>(string obj)
        {
            var sr = new StringReader(GetNormalizedStructure<T>(new StringBuilder(obj)));
            var s = new XmlSerializer(typeof(List<T>));
            var o = s.Deserialize(XmlReader.Create(sr));
            var result = o as List<T>;
            return result;
        }

        public string Serialize<T>(T obj)
        {
            string result;
            using (var stream = new MemoryStream())
            {
                var s = new XmlSerializer(typeof(T));
                s.Serialize(XmlWriter.Create(stream), obj);
                stream.Flush();

                stream.Position = 0;
                var sr = new StreamReader(stream);
                result = sr.ReadToEnd();
            }
            return result;
        }
        public T Deserialize<T>(string obj)
        {
            var sr = new StringReader(GetNormalizedStructure<T>(new StringBuilder(obj)));
            var s = new XmlSerializer(typeof(T));
            var o = s.Deserialize(XmlReader.Create(sr));
            var result = (T)o;
            return result;
        }

        #endregion

        public static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            var buffer = new byte[CHUNK_SIZE];
            long fieldOffset = 0;
            using (var stream = new MemoryStream())
            {
                long bytesRead;
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
        public static byte[] GetImageFromFile(string filePath)
        {
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var br = new BinaryReader(fs);

            var photo = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();

            return photo;
        }


        #region  |  serialization backward compatibility  |


        /// <summary>
        /// Provides ah-hoc backward compatibility after migration 
        /// to the standard naming conventions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        /// <returns></returns>
        private static string GetNormalizedStructure<T>(StringBuilder sb)
        {
            if (typeof(T) == typeof(TranslationOrigins))
            {
                if (!sb.ToString().Contains("<original>")) return sb.ToString();
                NormalizeTagContent(sb, "original", "Original");
                NormalizeTagContent(sb, "updated", "Updated");
                NormalizeTagContent(sb, "updatedPrevious", "UpdatedPrevious");
                NormalizeTagContent(sb, "id", "Id");
                NormalizeTagContent(sb, "language_type", "LangType");

                NormalizeEnumName(sb, "LangType", "original", "Original");
                NormalizeEnumName(sb, "LangType", "updated", "Updated");
                NormalizeEnumName(sb, "LangType", "updatedPrevious", "UpdatedPrevious");
                NormalizeEnumName(sb, "LangType", "none", "None");
            }
            else if (typeof(T) == typeof(ContentSections))
            {
                if (!sb.ToString().Contains("<sourceSections>")) return sb.ToString();
                NormalizeTagContent(sb, "sourceSections", "SourceSections");
                NormalizeTagContent(sb, "targetOriginalSections", "TargetOriginalSections");
                NormalizeTagContent(sb, "targetUpdatedSections", "TargetUpdatedSections");
                NormalizeTagContent(sb, "id", "Id");
                NormalizeTagContent(sb, "content_type", "CntType");
                NormalizeTagContent(sb, "language_type", "LangType");
                NormalizeTagContent(sb, "id_ref", "IdRef");
                NormalizeTagContent(sb, "content", "Content");
                NormalizeTagContent(sb, "has_revision", "HasRevision");
                NormalizeTagContent(sb, "revision_marker", "RevisionMarker");
                NormalizeTagContent(sb, "revision_type", "RevType");
                NormalizeTagContent(sb, "author", "Author");

                NormalizeEnumName(sb, "LangType", "source", "Source");
                NormalizeEnumName(sb, "LangType", "target", "Target");
                NormalizeEnumName(sb, "LangType", "targetUpdated", "TargetUpdated");
            }
            else if (typeof(T) == typeof(DocumentStateCounters))
            {
                if (!sb.ToString().Contains("<translation_match_types>")) return sb.ToString();
                NormalizeTagContent(sb, "translation_match_types", "TranslationMatchTypes");
                NormalizeTagContent(sb, "confirmation_statuses", "ConfirmationStatuses");
                NormalizeTagContent(sb, "name", "Name");
                NormalizeTagContent(sb, "value", "Value");
            }
            else if (typeof(T) == typeof(KeyStroke))
            {
                if (!sb.ToString().Contains("<origin_type>")) return sb.ToString();
                NormalizeTagContent(sb, "Id", "Id");
                NormalizeTagContent(sb, "origin_type", "OriginType");
                NormalizeTagContent(sb, "origin_system", "OriginSystem");
                NormalizeTagContent(sb, "match", "Match");
                NormalizeTagContent(sb, "selection", "Selection");
                NormalizeTagContent(sb, "key", "Key");
                NormalizeTagContent(sb, "text", "Text");
                NormalizeTagContent(sb, "ctrl", "Ctrl");
                NormalizeTagContent(sb, "alt", "Alt");
                NormalizeTagContent(sb, "shift", "Shift");
            }
            else if (typeof(T) == typeof(Comment))
            {
                if (!sb.ToString().Contains("<author>")) return sb.ToString();
                NormalizeTagContent(sb, "id", "Id");
                NormalizeTagContent(sb, "author", "Author");
                NormalizeTagContent(sb, "severity", "Severity");
                NormalizeTagContent(sb, "content", "Content");
                NormalizeTagContent(sb, "version", "Version");

            }
            else if (typeof(T) == typeof(Sdl.Community.Structures.Projects.Activities.LanguageRate))
            {
                if (!sb.ToString().Contains("<source_language>")) return sb.ToString();
                NormalizeTagContent(sb, "id", "Id");
                NormalizeTagContent(sb, "source_language", "SourceLanguage");
                NormalizeTagContent(sb, "target_language", "TargetLanguage");
                NormalizeTagContent(sb, "round_type", "RndType");
                NormalizeTagContent(sb, "base_rate", "BaseRate");
                NormalizeTagContent(sb, "rate_pm", "RatePm");
                NormalizeTagContent(sb, "rate_cm", "RateCm");
                NormalizeTagContent(sb, "rate_rep", "RateRep");
                NormalizeTagContent(sb, "rate_100", "Rate100");
                NormalizeTagContent(sb, "rate_95", "Rate95");
                NormalizeTagContent(sb, "rate_85", "Rate85");
                NormalizeTagContent(sb, "rate_75", "Rate75");
                NormalizeTagContent(sb, "rate_50", "Rate50");
                NormalizeTagContent(sb, "rate_new", "RateNew");


                NormalizeEnumName(sb, "RndType", "roundup", "Roundup");
                NormalizeEnumName(sb, "RndType", "rounddown", "Rounddown");
                NormalizeEnumName(sb, "RndType", "round", "Round");
            }
            else if (typeof(T) == typeof(Sdl.Community.Structures.Documents.Records.QualityMetric))
            {
                if (!sb.ToString().Contains("<severity_name>")) return sb.ToString();
                NormalizeTagContent(sb, "guid", "Guid");
                NormalizeTagContent(sb, "id", "Id");
                NormalizeTagContent(sb, "status", "Status");
                NormalizeTagContent(sb, "name", "Name");
                NormalizeTagContent(sb, "severity_name", "SeverityName");
                NormalizeTagContent(sb, "severity_value", "SeverityValue");
                NormalizeTagContent(sb, "content", "Content");
                NormalizeTagContent(sb, "comment", "Comment");
                NormalizeTagContent(sb, "user_name", "UserName");
                NormalizeTagContent(sb, "document_id", "DocumentId");
                NormalizeTagContent(sb, "segment_id", "SegmentId");
                NormalizeTagContent(sb, "paragraph_id", "ParagraphId");
            }

            return sb.ToString();
        }


        private static void NormalizeTagContent(StringBuilder sb, string nameIn, string nameOut)
        {
            sb.Replace("<" + nameIn + ">", "<" + nameOut + ">");
            sb.Replace("</" + nameIn + ">", "</" + nameOut + ">");
            sb.Replace("<" + nameIn + " />", "<" + nameOut + " />");

            // opening tag with attributes?
            sb.Replace("<" + nameIn + " ", "<" + nameOut + " ");
        }
        private static void NormalizeEnumName(StringBuilder sb, string enumName, string nameIn, string nameOut)
        {
            sb.Replace("<" + enumName + ">" + nameIn + "</" + enumName + ">", "<" + enumName + ">" + nameOut + "</" + enumName + ">");
        }



        #endregion




    }
}