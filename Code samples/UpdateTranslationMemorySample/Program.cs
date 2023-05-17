using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace UpdateTranslationMemorySample
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var translationMemoryPath = @"";
			var tm = new FileBasedTranslationMemory(translationMemoryPath);
			var ri = new RegularIterator();

			var tus = tm.LanguageDirection.GetTranslationUnits(ref ri);

			//AddTu(tm, "A dialog box will open.", "Es öffnet sich ein Dialogfenster.");S
			foreach (var tu in tus)
			{
				tu.ConfirmationLevel = ConfirmationLevel.Draft; //this has to be done for the origin to not be reverted back to TranslationUnitOrigin.TM
				tu.Origin = TranslationUnitOrigin.MachineTranslation;
				tm.LanguageDirection.UpdateTranslationUnit(tu);
			}
			tm.Save();
			
			ri.Reset();
			var x = tm.LanguageDirection.GetTranslationUnits(ref ri);

		}

		private static void AddTu(FileBasedTranslationMemory tm, string source, string target)
		{
			var tu = new TranslationUnit
			{
				SourceSegment = new Segment(tm.LanguageDirection.SourceLanguage),
				TargetSegment = new Segment(tm.LanguageDirection.TargetLanguage)
			};

			tu.SourceSegment.Add(source);
			tu.TargetSegment.Add(target);

			tm.LanguageDirection.AddTranslationUnit(tu, new ImportSettings());
		}
	}
}