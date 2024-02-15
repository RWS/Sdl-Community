using System.Data.Linq.Mapping;
using System.Reflection;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Model;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "GlossaryData")]
public class DbGlossaryEntry : IInterpretBankTable
{
	private static PropertyInfo[] _properties;

	[Column(Name = "Comment10a")] public string Comment10a { get; set; }

	[Column(Name = "Comment10b")] public string Comment10b { get; set; }

	[Column(Name = "Comment1a")] public string Comment1a { get; set; }

	[Column(Name = "Comment1b")] public string Comment1b { get; set; }

	[Column(Name = "Comment2a")] public string Comment2a { get; set; }

	[Column(Name = "Comment2b")] public string Comment2b { get; set; }

	[Column(Name = "Comment3a")] public string Comment3a { get; set; }

	[Column(Name = "Comment3b")] public string Comment3b { get; set; }

	[Column(Name = "Comment4a")] public string Comment4a { get; set; }

	[Column(Name = "Comment4b")] public string Comment4b { get; set; }

	[Column(Name = "Comment5a")] public string Comment5a { get; set; }

	[Column(Name = "Comment5b")] public string Comment5b { get; set; }

	[Column(Name = "Comment6a")] public string Comment6a { get; set; }

	[Column(Name = "Comment6b")] public string Comment6b { get; set; }

	[Column(Name = "Comment7a")] public string Comment7a { get; set; }

	[Column(Name = "Comment7b")] public string Comment7b { get; set; }

	[Column(Name = "Comment8a")] public string Comment8a { get; set; }

	[Column(Name = "Comment8b")] public string Comment8b { get; set; }

	[Column(Name = "Comment9a")] public string Comment9a { get; set; }

	[Column(Name = "Comment9b")] public string Comment9b { get; set; }

	[Column(Name = "CommentAll")] public string CommentAll { get; set; }

	[Column(Name = "ID", IsPrimaryKey = true)] public int Id { get; set; }

	[Column(Name = "Memorization")] public int Memorization { get; set; }

	[Column(Name = "RecordCreation")] public string RecordCreation { get; set; }

	[Column(Name = "RecordCreator")] public string RecordCreator { get; set; }

	[Column(Name = "RecordEdit")] public string RecordEdit { get; set; }

	[Column(Name = "RecordEditor")] public string RecordEditor { get; set; }

	[Column(Name = "RecordValidation")] public string RecordValidation { get; set; }

	[Column(Name = "Tag1")] public string Tag1 { get; set; }

	[Column(Name = "Tag2")] public string Tag2 { get; set; }

	[Column(Name = "Tags")] public string Tags { get; set; }

	[Column(Name = "Term1")] public string Term1 { get; set; }

	[Column(Name = "Term10")] public string Term10 { get; set; }

	[Column(Name = "Term10index")] public string Term10index { get; set; }

	[Column(Name = "Term1index")] public string Term1index { get; set; }

	[Column(Name = "Term2")] public string Term2 { get; set; }

	[Column(Name = "Term2index")] public string Term2index { get; set; }

	[Column(Name = "Term3")] public string Term3 { get; set; }

	[Column(Name = "Term3index")] public string Term3index { get; set; }

	[Column(Name = "Term4")] public string Term4 { get; set; }

	[Column(Name = "Term4index")] public string Term4index { get; set; }

	[Column(Name = "Term5")] public string Term5 { get; set; }

	[Column(Name = "Term5index")] public string Term5index { get; set; }

	[Column(Name = "Term6")] public string Term6 { get; set; }

	[Column(Name = "Term6index")] public string Term6index { get; set; }

	[Column(Name = "Term7")] public string Term7 { get; set; }

	[Column(Name = "Term7index")] public string Term7index { get; set; }

	[Column(Name = "Term8")] public string Term8 { get; set; }

	[Column(Name = "Term8index")] public string Term8index { get; set; }

	[Column(Name = "Term9")] public string Term9 { get; set; }

	[Column(Name = "Term9index")] public string Term9index { get; set; }

	[Column(Name = "TermFullindex")] public string TermFullindex { get; set; }

	private static PropertyInfo[] Properties => _properties ??= typeof(TermEntry).GetProperties();

	public string this[string property]
	{
		get
		{
			var propertyInfo = GetType().GetProperty(property);
			return propertyInfo?.GetValue(this, null)?.ToString();
		}
		set
		{
			var propertyInfo = GetType().GetProperty(property);
			propertyInfo.SetValue(this, value, null);
		}
	}
}