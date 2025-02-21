using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using InterpretBank.GlossaryService.DAL.Interface;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "DatabaseInfo")]
public class DatabaseInfo : IInterpretBankTable
{
	[Column(Name = "Activation")] public string Activation { get; set; }

	[Column(Name = "DatabaseCreation")] public string DatabaseCreation { get; set; }

	[Column(Name = "DatabaseUser")] public string DatabaseUser { get; set; }

	[Column(Name = "DatabaseVersion")] public string DatabaseVersion { get; set; }

	[Column(Name = "FieldConferenceGlossaryLabel")]
	public string FieldConferenceGlossaryLabel { get; set; }

	[Column(Name = "FieldConferenceInfoLabel")]
	public string FieldConferenceInfoLabel { get; set; }

	[Column(Name = "FieldTermExtraALabel")]
	public string FieldTermExtraALabel { get; set; }

	[Column(Name = "FieldTermExtraBLabel")]
	public string FieldTermExtraBLabel { get; set; }

	[Column(Name = "FieldTermLabel")] public string FieldTermLabel { get; set; }

	[Column(IsPrimaryKey = true, Name = "ID")] public int Id { get; set; }

	[Column(Name = "LanguageName1")] public string LanguageName1 { get; set; }

	[Column(Name = "LanguageName10")] public string LanguageName10 { get; set; }

	[Column(Name = "LanguageName2")] public string LanguageName2 { get; set; }

	[Column(Name = "LanguageName3")] public string LanguageName3 { get; set; }

	[Column(Name = "LanguageName4")] public string LanguageName4 { get; set; }

	[Column(Name = "LanguageName5")] public string LanguageName5 { get; set; }

	[Column(Name = "LanguageName6")] public string LanguageName6 { get; set; }

	[Column(Name = "LanguageName7")] public string LanguageName7 { get; set; }

	[Column(Name = "LanguageName8")] public string LanguageName8 { get; set; }

	[Column(Name = "LanguageName9")] public string LanguageName9 { get; set; }
}