using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class Tm
	{
		public static AnonymizeTranslationMemory GetTranslationUnits(string tmPath,
			ObservableCollection<SourceSearchResult> sourceSearchResult, List<Rule> selectedRules)
		{
			var tm =
				new FileBasedTranslationMemory(tmPath);
			var tmIterator = new RegularIterator();

			var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			var pi = new PersonalInformation(selectedRules);

			foreach (var translationUnit in tus)
			{
				var sourceText = translationUnit.SourceSegment.ToPlain();
				if (pi.ContainsPi(sourceText))
				{
					var searchResult = new SourceSearchResult
					{
						Id = translationUnit.ResourceId.Guid.ToString(),
						SourceText = sourceText,
						MatchResult = new MatchResult
						{
							Positions = pi.GetPersonalDataPositions(sourceText)
						},
						TmFilePath = tmPath,
						SegmentNumber = translationUnit.ResourceId.Id.ToString()
					};
					var targetText = translationUnit.TargetSegment.ToPlain();
					if (pi.ContainsPi(targetText))
					{
						searchResult.TargetText = targetText;
						searchResult.TargetMatchResult = new MatchResult
						{
							Positions = pi.GetPersonalDataPositions(targetText)
						};
					}
					sourceSearchResult.Add(searchResult);
				}
			}
			return new AnonymizeTranslationMemory
			{
				TmPath = tmPath,
				TranslationUnits = tus.ToList()
			};
		}

		public static void AnonymizeTu(List<AnonymizeTranslationMemory> tusToAnonymize)
		{
			foreach (var translationUnitPair in tusToAnonymize)
			{
				var tm = new FileBasedTranslationMemory(translationUnitPair.TmPath);

				foreach (var translationUnit in translationUnitPair.TranslationUnits)
				{
					var sourceTranslationElements = translationUnit.SourceSegment.Elements.ToList();
					var elementsContainsTag = sourceTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));

					if (elementsContainsTag)
					{
						AnunymizeSegmentsWithTags(translationUnit, sourceTranslationElements, true);
					}
					else
					{
						AnonymizeSegmentsWithoutTags(translationUnit, sourceTranslationElements, true);
					}

					tm.LanguageDirection.UpdateTranslationUnit(translationUnit);
				}


				//	//translationUnit.SystemFields.CreationUser =
				//	//	AnonymizeData.EncryptData(translationUnit.SystemFields.CreationUser, "andrea");
				//	//translationUnit.SystemFields.UseUser =
				//	//	AnonymizeData.EncryptData(translationUnit.SystemFields.UseUser, "andrea");


				//	//foreach (FieldValue item in translationUnit.FieldValues)
				//	//{
				//	//	var anonymized = AnonymizeData.EncryptData(item.GetValueString(), "Andrea");
				//	//	item.Clear();
				//	//	item.Add(anonymized);
				//	//}
				//	//var test = translationUnit.DocumentSegmentPair
				//tm.LanguageDirection.UpdateTranslationUnit(translationUnit);
				//}
			}

		}

		private static void AnonymizeSegmentsWithoutTags(TranslationUnit translationUnit,
			List<SegmentElement> sourceTranslationElements, bool isSource)
		{
			foreach (var element in sourceTranslationElements)
			{
				var visitor = new SegmentElementVisitor();
				element.AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
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
					}
				}
			}
		}

		private static void AnunymizeSegmentsWithTags(TranslationUnit translationUnit,
			List<SegmentElement> translationElements, bool isSource)
		{
			for (var i = 0; i < translationElements.Count; i++)
			{
				if (!translationElements[i].GetType().UnderlyingSystemType.Name.Equals("Text")) continue;
				var visitor = new SegmentElementVisitor();
				translationElements[i].AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					if (isSource)
					{
						var segmentElements = new List<SegmentElement>();
						foreach (var segment in segmentColection)
						{
							var text = segment as Text;
							var tag = segment as Tag;
							if (text != null)
							{
								segmentElements.Add(text);
							}
							if (tag != null)
							{
								segmentElements.Add(tag);
							}
						}
						translationUnit.SourceSegment.Elements.RemoveAt(i);
						translationUnit.SourceSegment.Elements.InsertRange(i, segmentElements);
					}
				}
			}
		}
	}
}

