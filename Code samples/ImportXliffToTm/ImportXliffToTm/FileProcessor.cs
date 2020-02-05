using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;
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
					SourceSegment = new Segment(_tm.LanguageDirection.SourceLanguage),
					TargetSegment = new Segment(_tm.LanguageDirection.TargetLanguage)
				};

				tu.SourceSegment.Add(segmentPair.Source.ToString());
				tu.TargetSegment.Add(segmentPair.Target.ToString());
				var fields = _tm.FieldDefinitions;

				var fileNameField = fields.FirstOrDefault(f => f.Name.Equals("FileName"));
				if (fileNameField != null)
				{
					var value = fileNameField.CreateValue();
					value.Add(_fileName);
					tu.FieldValues.Add(value);
				}

				var containsSegIdField = fields.FirstOrDefault(f => f.Name.Equals("SegmentId"));
				if (containsSegIdField == null)
				{
					// Create field in tm
					var segmentIdFieldDefinition = new FieldDefinition("SegmentId", FieldValueType.MultipleString);
					_tm.FieldDefinitions.Add(segmentIdFieldDefinition);
					AddFieldToTu(tu, segmentPair.Properties.Id.Id);
				}
				else
				{
					AddFieldToTu(tu, segmentPair.Properties.Id.Id);
				}
				_tm.LanguageDirection.AddTranslationUnit(tu, GetImportSettings());
			}
			_tm.Save();
		}

		private void AddFieldToTu(TranslationUnit tu,string segmentId)
		{
			var segmentIdFieldDefinition = new FieldDefinition("SegmentId", FieldValueType.MultipleString);
			var segmentIdValue = segmentIdFieldDefinition.CreateValue();
			segmentIdValue.Add(segmentId);
			tu.FieldValues.Add(segmentIdValue);
		}

		private ImportSettings GetImportSettings()
		{
			var settings = new ImportSettings
			{
				CheckMatchingSublanguages = true,
				ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge,
				IncrementUsageCount =  true,
				NewFields = ImportSettings.NewFieldsOption.AddToSetup
			};

			return settings;
		}
	}
}
