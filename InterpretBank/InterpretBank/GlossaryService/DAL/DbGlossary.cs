using System.Data.Linq.Mapping;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "GlossaryMetadata")]
public class DbGlossary : IInterpretBankTable
{
	[Column(Name = "GlossaryCreator")] public string GlossaryCreator { get; set; }

	[Column(Name = "GlossaryDataCreation")]
	public string GlossaryDataCreation { get; set; }

	[Column(Name = "GlossaryDescription")] public string GlossaryDescription { get; set; }
	[Column(Name = "GlossarySetting")] public string GlossarySetting { get; set; }

	[Column(Name = "ID", IsPrimaryKey = true)]
	public int Id { get; set; }

	[Column(Name = "Tag1")] public string Tag1 { get; set; }

	[Column(Name = "Tag2")] public string Tag2 { get; set; }

	public override string ToString()
	{
		return "Glossaries";
	}
}