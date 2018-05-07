using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	class Tm
	{
		public void OpenTm()
		{
			var tm =
				new FileBasedTranslationMemory(@"C:\Users\aghisa\Desktop\emails2.sdltm");
			//var tm =
			//	new FileBasedTranslationMemory(@"C:\Users\aghisa\Desktop\AnonymizeTM.sdltm");
			var tmIterator = new RegularIterator(10);

			var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);

			foreach (var translationUnit in tus)
			{
				foreach (var element in translationUnit.SourceSegment.Elements.ToList())
				{
					var visitor = new SegmentElementVisitor();
					element.AcceptSegmentElementVisitor(visitor);
					var segmentColection = visitor.SegmentColection;
					if (segmentColection.Count > 0)
					{
						translationUnit.SourceSegment.Elements.Clear();
						foreach (var segment in segmentColection)
						{
							var text = segment as Text;
							var tag = segment as Tag;
							if (text != null)
							{
								translationUnit.SourceSegment.Elements.Add(text);
							}
							if (tag != null)
							{
								translationUnit.SourceSegment.Elements.Add(tag);
							}
							//translationUnit.SourceSegment.Elements.Add(element);
						}

					}
				}
				//translationUnit.SystemFields.CreationUser =
				//	AnonymizeData.EncryptData(translationUnit.SystemFields.CreationUser, "andrea");
				//translationUnit.SystemFields.UseUser =
				//	AnonymizeData.EncryptData(translationUnit.SystemFields.UseUser, "andrea");


				//foreach (FieldValue item in translationUnit.FieldValues)
				//{
				//	var anonymized = AnonymizeData.EncryptData(item.GetValueString(), "Andrea");
				//	item.Clear();
				//	item.Add(anonymized);
				//}
				//var test = translationUnit.DocumentSegmentPair
				tm.LanguageDirection.UpdateTranslationUnit(translationUnit);
			}
		}
	
		}
	
}
