using System.Data.Linq.Mapping;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "TagList")]
public class DbTag : IInterpretBankTable
{
	[Column(Name = "TagID")] public int TagId { get; set; }

	[Column(Name = "TagName")] public string TagName { get; set; }

	public override string ToString()
	{
		return "Tags";
	}
}