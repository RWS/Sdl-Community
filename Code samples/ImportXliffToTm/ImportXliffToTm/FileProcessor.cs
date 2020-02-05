using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ImportXliffToTm
{
	public class FileProcessor: AbstractBilingualContentProcessor
	{
		private readonly FileBasedTranslationMemory _tm;
		private readonly  string _fileName;

		public FileProcessor(FileBasedTranslationMemory tm,string fileName)
		{
			_tm = tm;
			_fileName = fileName;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }

			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var tu = new TranslationUnit
				{
					SourceSegment = new Sdl.LanguagePlatform.Core.Segment(_tm.LanguageDirection.SourceLanguage),
					TargetSegment = new Sdl.LanguagePlatform.Core.Segment(_tm.LanguageDirection.TargetLanguage),
				};
				tu.SourceSegment.Add(segmentPair.Source.ToString());
				tu.TargetSegment.Add(segmentPair.Target.ToString());
				var fields = _tm.FieldDefinitions;

				foreach (var field in fields)
				{
					if (field.Name.Equals("FileName"))
					{
						var value = field.CreateValue();
						value.Add(_fileName);
						tu.FieldValues.Add(value);
					}
				}

				//var fileNameField = fields.FirstOrDefault(f => f.Name.Equals("FileName"));
				//if (fileNameField != null)
				//{
				//	var value = fileNameField.CreateValue();
				//	value.Add(_fileName);
				//	tu.FieldValues.Add(value);
				//}

				//var containsSegIdField = fields.FirstOrDefault(f => f.Name.Equals("SegmentId"));
				//if (containsSegIdField == null)
				//{
				//	var segmentIdFieldDefinition = new FieldDefinition("SegmentId", FieldValueType.MultipleString);
				//	var segmentIdValue = segmentIdFieldDefinition.CreateValue();
				//	segmentIdValue.Add(segmentPair.Properties.Id.Id);

				//	tu.FieldValues.Add(segmentIdValue);
				//}
				_tm.LanguageDirection.AddTranslationUnit(tu, GetImportSettings());
			}
			_tm.Save();
		}

		private ImportSettings GetImportSettings()
		{
			var settings = new ImportSettings
			{
				CheckMatchingSublanguages = true,
				ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge
			};

			return settings;
		}
	}
}
