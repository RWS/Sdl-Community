using InterpretBank.GlossaryService.DAL.Interface;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace InterpretBank.Extensions
{
    public static class DataContextExtensions
    {
        public static IEnumerable<T> GetTablePendingInserts<T>(this DataContext dataContext)
            where T : class, IInterpretBankTable =>
            dataContext.GetChangeSet().Inserts.OfType<T>();
    }
}