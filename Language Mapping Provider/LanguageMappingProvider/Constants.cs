using System;
using System.IO;

namespace LanguageMappingProvider;

internal static class Constants
{
    private const string DatabaseFileName = "{0}data.sqlite3";
    public static string PluginAppDataLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Trados AppStore\Language Mapping Provider";
    public static string DatabaseFilePath = Path.Combine(PluginAppDataLocation, DatabaseFileName);

    public const string SQL_TableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name='languagemapping'";
    public const string SQL_CreateTable = @"CREATE TABLE ""languagemapping"" (""Index"" INTEGER, ""Name"" TEXT, ""Region"" TEXT, ""TradosCode"" TEXT, ""LanguageCode"" TEXT, PRIMARY KEY(""Index"" AUTOINCREMENT))";
    public const string SQL_InsertData = @"INSERT INTO languagemapping (Name, Region, TradosCode, LanguageCode) VALUES (""{0}"", ""{1}"", ""{2}"", ""{3}"")";
    public const string SQL_InsertData_StringBuilder = @"INSERT INTO languagemapping (Name, Region, TradosCode, LanguageCode) VALUES";
    public const string SQL_SelectData = "select * from languagemapping";
    public const string SQL_SelectDefaultData = "select * from defaultdata";
    public const string SQL_UpdateData = "UPDATE languagemapping SET {0} = '{1}' WHERE `Index` = {2}";
    public const string SQL_DropTable = "DROP TABLE languagemapping";

    public const string UndefinedLanguageCode = "n/a";
}