using System.ComponentModel.DataAnnotations.Schema;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.GlossaryService.DAL
{
	[System.Data.Linq.Mapping.Table(Name = "TagLink")]
	public class DbTagLink : IInterpretBankTable
	{
		[ForeignKey("GlossaryID")][System.Data.Linq.Mapping.Column(Name = "GlossaryID")] public int GlossaryId { get; set; }

		[System.Data.Linq.Mapping.Column(Name = "TagID", IsPrimaryKey = true)] public int Id { get; set; }

		[System.Data.Linq.Mapping.Column(Name = "TagName")] public string TagName { get; set; }
	}
}