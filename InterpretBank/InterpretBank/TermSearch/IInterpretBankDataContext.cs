using System.Linq;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.TermSearch;

public interface IInterpretBankDataContext
{
	IQueryable<T> GetRows<T>() where T : class, IInterpretBankTable;
}