using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.TermSearch
{
	public class InterpretBankDataContext : IInterpretBankDataContext
	{
		public InterpretBankDataContext(SQLiteConnection sqLiteConnection)
		{
			SqLiteConnection = sqLiteConnection;
			DataContext = new DataContext(sqLiteConnection);
		}

		private DataContext DataContext { get; set; }
		private SQLiteConnection SqLiteConnection { get; }

		public IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable => DataContext.GetTable<T>();
	}
}