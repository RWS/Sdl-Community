using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class Tm
	{
		/// <summary>
		/// Gets TUs which contains PI
		/// </summary>
		/// <param name="tmPath"></param>
		/// <param name="sourceSearchResult">Regex search result</param>
		/// <param name="selectedRules">Selected rules from the grid</param>
		/// <returns>An object which has Tm path and a list of trasnlation units which contains PI</returns>
		public static AnonymizeTranslationMemory FileBaseTmGetTranslationUnits(string tmPath,
			ObservableCollection<SourceSearchResult> sourceSearchResult, List<Rule> selectedRules)
		{
			var tm =
				new FileBasedTranslationMemory(tmPath);
			var unitsCount = tm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);

			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
			
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
							IsServer = false,
							SegmentNumber = translationUnit.ResourceId.Id.ToString(),
							SelectedWordsIndex = new List<int>()
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
			});
			return new AnonymizeTranslationMemory
			{
				TmPath = tmPath,
				TranslationUnits = tus.ToList()
			};
		}
		/// <summary>
		/// Gets server based TUs which contains PI
		/// </summary>
		/// <param name="translationProvider">Translation provider</param>
		/// <param name="tmPath">Translation memory path</param>
		/// <param name="sourceSearchResult">Regex search result</param>
		/// <param name="selectedRules">Selected rules from the grid</param>
		/// <returns>An object which has Tm path and a list of trasnlation units which contains PI</returns>
		public static AnonymizeTranslationMemory ServerBasedTmGetTranslationUnits(TranslationProviderServer translationProvider,string tmPath,
			ObservableCollection<SourceSearchResult> sourceSearchResult, List<Rule> selectedRules)
		{
			var allTusForLanguageDirections = new List<TranslationUnit>();
			System.Windows.Application.Current.Dispatcher.Invoke(delegate
			{
				var translationMemory = translationProvider.GetTranslationMemory(tmPath, TranslationMemoryProperties.All);
				var languageDirections = translationMemory.LanguageDirections;
				var pi = new PersonalInformation(selectedRules);

				foreach (var languageDirection in languageDirections)
				{
					var unitsCount = languageDirection.GetTranslationUnitCount();
					var tmIterator = new RegularIterator(unitsCount);
					var translationUnits = languageDirection.GetTranslationUnits(ref tmIterator);
					if (translationUnits != null)
					{
						allTusForLanguageDirections.AddRange(translationUnits);
						foreach (var translationUnit in translationUnits)
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
									IsServer = true,
									SegmentNumber = translationUnit.ResourceId.Id.ToString(),
									SelectedWordsIndex =  new List<int>(),
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
					}
				}
				
			});
			return new AnonymizeTranslationMemory
			{
				TmPath = tmPath,
				TranslationUnits = allTusForLanguageDirections
			};
		}
		/// <summary>
		/// Anonymize Server Based TU
		/// </summary>
		/// <param name="translationProvider"></param>
		/// <param name="tusToAnonymize">TUs which contains PI</param>
		public static void AnonymizeServerBasedTu(TranslationProviderServer translationProvider,
			List<AnonymizeTranslationMemory> tusToAnonymize)
		{
			try
			{
				foreach (var tuToAonymize in tusToAnonymize)
				{
					var translationMemory =
						translationProvider.GetTranslationMemory(tuToAonymize.TmPath, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;
					foreach (var languageDirection in languageDirections)
					{
						foreach (var translationUnit in tuToAonymize.TranslationUnits)
						{
							var sourceTranslationElements = translationUnit.SourceSegment.Elements.ToList();
							var elementsContainsTag =
								sourceTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
							if (elementsContainsTag)
							{
								AnonymizeSegmentsWithTags(translationUnit, sourceTranslationElements, true);
							}
							else
							{
								AnonymizeSegmentsWithoutTags(translationUnit, sourceTranslationElements, true);
							}
							//needs to be uncommented
							languageDirection.UpdateTranslationUnit(translationUnit);
						}
					}

				}
			}
			catch (Exception exception)
			{
				if (exception.Message.Equals("One or more errors occurred."))
				{
					if (exception.InnerException != null)
					{
						MessageBox.Show(exception.InnerException.Message,
							"", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show(exception.Message,
						"", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

		}
		/// <summary>
		/// Anonymize File Based TU
		/// </summary>
		/// <param name="tusToAnonymize">TUs which contains PI</param>
		public static void AnonymizeFileBasedTu(List<AnonymizeTranslationMemory> tusToAnonymize)
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
						AnonymizeSegmentsWithTags(translationUnit, sourceTranslationElements, true);
					}
					else
					{
						AnonymizeSegmentsWithoutTags(translationUnit, sourceTranslationElements, true);
					}

					translationUnit.SystemFields.CreationUser = "N/A";
					translationUnit.SystemFields.UseUser = "N/A";
					tm.LanguageDirection.UpdateTranslationUnit(translationUnit);
				}
				//	//foreach (FieldValue item in translationUnit.FieldValues)
				//	//{
				//	//	var anonymized = AnonymizeData.EncryptData(item.GetValueString(), "Andrea");
				//	//	item.Clear();
				//	//	item.Add(anonymized);
				//	//}
				//	//var test = translationUnit.DocumentSegmentPair
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

		private static void AnonymizeSegmentsWithTags(TranslationUnit translationUnit,
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

