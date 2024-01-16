using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "TagList")]
public class DbTag : IInterpretBankTable
{
	[Key] [Column(Name = "TagID", IsPrimaryKey = true)] public int Id { get; set; }

	[Column(Name = "TagName")] public string TagName { get; set; }

	public override string ToString()
	{
		return "Tags";
	}
}