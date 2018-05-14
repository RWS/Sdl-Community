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

		public static TranslationUnit[] GetTranslationUnits(string tmPath,
			ObservableCollection<SourceSearchResult> sourceSearchResult)
		{
			var tm =
				new FileBasedTranslationMemory(tmPath);
			var tmIterator = new RegularIterator();

			var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			foreach (var translationUnit in tus)
			{
				var sourceText = translationUnit.SourceSegment.ToPlain();
				if (PersonalInformation.ContainsPi(sourceText))
				{
					var searchResult = new SourceSearchResult
					{
						Id = translationUnit.ResourceId.Guid.ToString(),
						SourceText = sourceText,
						MatchResult = new MatchResult
						{
							Positions = PersonalInformation.GetPersonalDataPositions(sourceText)
						},
						TmFilePath = tmPath,
						SegmentNumber = translationUnit.ResourceId.Id.ToString()
					};
					 sourceSearchResult.Add(searchResult);
				}

			}

			return tus;
		}

		public static void OpenTm(List<TmFile> selectedTms, ObservableCollection<SourceSearchResult> sourceSearchResult)
		{
			foreach (var selectedTm in selectedTms)
			{
				var tm =
					new FileBasedTranslationMemory(selectedTm.Path);
				var tmIterator = new RegularIterator();

				var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);

				foreach (var translationUnit in tus)
				{
					foreach (var element in translationUnit.SourceSegment.Elements.ToList())
					{
						var visitor = new SegmentElementVisitor();
						element.AcceptSegmentElementVisitor(visitor);
						var segmentColection = visitor.SegmentColection;
						var matchResult = visitor.MatchResult;

						if (segmentColection?.Count > 0)
						{
							var sourceResult = new SourceSearchResult
							{
								MatchResult = matchResult,
								Id = translationUnit.ResourceId.Guid.ToString(),
								SegmentNumber = translationUnit.ResourceId.Id.ToString(),
								SourceText = matchResult.Text,
								TmFilePath = selectedTm.Path
							};
							sourceSearchResult.Add(sourceResult);
							//translationUnit.SourceSegment.Elements.Clear();
							//foreach (var segment in segmentColection)
							//{
							//	var text = segment as Text;
							//	var tag = segment as Tag;
							//	if (text != null)
							//	{
							//		translationUnit.SourceSegment.Elements.Add(text);
							//	}
							//	if (tag != null)
							//	{
							//		translationUnit.SourceSegment.Elements.Add(tag);
							//	}
							//}
						}
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
					//	tm.LanguageDirection.UpdateTranslationUnit(translationUnit);
					//}
				}

			}

		}
	}
}
