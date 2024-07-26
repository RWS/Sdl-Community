using InterpretBank.GlossaryService.DAL;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace InterpretBank.GlossaryService
{
    public class MyDataContext : DataContext
    {
        public MyDataContext(SQLiteConnection sqLiteConnection) : base(sqLiteConnection)
        {
        }

        public MyDataContext(string dbFilepath) : base(new SQLiteConnection($"Data Source={dbFilepath}"))
        {
            
        }

        public Table<DbGlossaryEntry> GlossaryEntries => GetTable<DbGlossaryEntry>();
    }
}