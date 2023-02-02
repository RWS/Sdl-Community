using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Reflection;
using InterpretBank.GlossaryService.DAL.Interface;
using InterpretBank.GlossaryService.Model;

namespace InterpretBank.GlossaryService.DAL;

[Table(Name = "GlossaryData")]
public class DbTerm : IInterpretBankTable
{
	private static PropertyInfo[] _properties;

	[Column(Name = "Comment2a")] public string Comment10a { get; set; }

	[Column(Name = "Comment2a")] public string Comment10b { get; set; }

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

	[Column(Name = "Comment2a")] public string Comment6b { get; set; }

	[Column(Name = "Comment2a")] public string Comment7a { get; set; }

	[Column(Name = "Comment2a")] public string Comment7b { get; set; }

	[Column(Name = "Comment2a")] public string Comment8a { get; set; }

	[Column(Name = "Comment2a")] public string Comment8b { get; set; }

	[Column(Name = "Comment2a")] public string Comment9a { get; set; }

	[Column(Name = "Comment2a")] public string Comment9b { get; set; }

	[Column(Name = "Comment2a")] public string CommentAll { get; set; }

	[Key] [Column(Name = "ID")] public long Id { get; set; }

	[Column(Name = "Comment2a")] public string Memorization { get; set; }

	[Column(Name = "Comment2a")] public string RecordCreation { get; set; }

	[Column(Name = "Comment2a")] public string RecordCreator { get; set; }

	[Column(Name = "Comment2a")] public string RecordEdit { get; set; }

	[Column(Name = "Comment2a")] public string RecordEditor { get; set; }

	[Column(Name = "Comment2a")] public string RecordValidation { get; set; }

	[Column(Name = "Tag1")] public string Tag1 { get; set; }

	[Column(Name = "Tag2")] public string Tag2 { get; set; }

	[Column(Name = "Comment2a")] public string Tags { get; set; }

	[Column(Name = "Term1")] public string Term1 { get; set; }

	[Column(Name = "Comment2a")] public string Term10 { get; set; }

	[Column(Name = "Comment2a")] public string Term10index { get; set; }

	[Column(Name = "Comment2a")] public string Term1index { get; set; }

	[Column(Name = "Term2")] public string Term2 { get; set; }

	[Column(Name = "Comment2a")] public string Term2index { get; set; }

	[Column(Name = "Term3")] public string Term3 { get; set; }

	[Column(Name = "Comment2a")] public string Term3index { get; set; }

	[Column(Name = "Term4")] public string Term4 { get; set; }

	[Column(Name = "Comment2a")] public string Term4index { get; set; }

	[Column(Name = "Term5")] public string Term5 { get; set; }

	[Column(Name = "Comment2a")] public string Term5index { get; set; }

	[Column(Name = "Term6")] public string Term6 { get; set; }

	[Column(Name = "Comment2a")] public string Term6index { get; set; }

	[Column(Name = "Comment2a")] public string Term7 { get; set; }

	[Column(Name = "Comment2a")] public string Term7index { get; set; }

	[Column(Name = "Comment2a")] public string Term8 { get; set; }

	[Column(Name = "Comment2a")] public string Term8index { get; set; }

	[Column(Name = "Comment2a")] public string Term9 { get; set; }

	[Column(Name = "Comment2a")] public string Term9index { get; set; }

	[Column(Name = "Comment2a")] public string TermFullindex { get; set; }

	private static PropertyInfo[] Properties => _properties ??= typeof(TermEntry).GetProperties();

	public string this[string property, int? index = null]
	{
		get
		{
			var propertyInfo = GetType().GetProperty(property);
			return propertyInfo?.GetValue(this, null)?.ToString();
		}
	}
}