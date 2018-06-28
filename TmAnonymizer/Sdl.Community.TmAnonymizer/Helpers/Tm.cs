using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
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
					var targetText = translationUnit.TargetSegment.ToPlain();
					var sourceContainsPi = pi.ContainsPi(sourceText);
					var targetContainsPi = pi.ContainsPi(targetText);

					if (sourceContainsPi || targetContainsPi)
					{
						var searchResult = new SourceSearchResult
						{
							Id = translationUnit.ResourceId.Guid.ToString(),
							SourceText = sourceText,
							TargetText = targetText,
							TmFilePath = tmPath,
							IsServer = false,
							SegmentNumber = translationUnit.ResourceId.Id.ToString(),
							SelectedWordsDetails = new List<WordDetails>(),
							DeSelectedWordsDetails = new List<WordDetails>(),
							TargetDeSelectedWordsDetails = new List<WordDetails>(),
							TargetSelectedWordsDetails = new List<WordDetails>()

						};
						if (sourceContainsPi)
						{
							searchResult.IsSourceMatch = true;
							searchResult.MatchResult = new MatchResult
							{
								Positions = pi.GetPersonalDataPositions(sourceText)
							};
						}
						if (targetContainsPi)
						{
							searchResult.IsTargetMatch = true;
							searchResult.TargetMatchResult =new MatchResult
							{
								Positions = pi.GetPersonalDataPositions(targetText)
							};
						}
						sourceSearchResult.Add(searchResult);
					}
					
					//if (pi.ContainsPi(sourceText)||pi.ContainsPi(targetText))
					//{
					//	var searchResult = new SourceSearchResult
					//	{
					//		Id = translationUnit.ResourceId.Guid.ToString(),
					//		SourceText = sourceText,
					//		MatchResult = new MatchResult
					//		{
					//			Positions = pi.GetPersonalDataPositions(sourceText)
					//		},
					//		TmFilePath = tmPath,
					//		IsServer = false,
					//		SegmentNumber = translationUnit.ResourceId.Id.ToString(),
					//		SelectedWordsDetails = new List<WordDetails>(),
					//		DeSelectedWordsDetails = new List<WordDetails>(),
					//		IsSourceMatch = true,
					//	};
						
					//	if (pi.ContainsPi(targetText))
					//	{
					//		searchResult.TargetText = targetText;
					//		searchResult.IsTargetMatch = true;
					//		searchResult.TargetMatchResult = new MatchResult
					//		{
					//			Positions = pi.GetPersonalDataPositions(targetText)
					//		};
					//	}
					//	else
					//	{
					//		searchResult.TargetText = targetText;
					//	}
					//	sourceSearchResult.Add(searchResult);
					//}
				}
			});
			return new AnonymizeTranslationMemory
			{
				TmPath = tmPath,
				TranslationUnits = tus.ToList(),
				TranslationUnitDetails = new List<TranslationUnitDetails>()
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
									SelectedWordsDetails =  new List<WordDetails>(),
									DeSelectedWordsDetails = new List<WordDetails>(),
									TargetDeSelectedWordsDetails = new List<WordDetails>(),
									TargetSelectedWordsDetails = new List<WordDetails>(),
									IsSourceMatch = true
									
								};
								var targetText = translationUnit.TargetSegment.ToPlain();
								if (pi.ContainsPi(targetText))
								{
									searchResult.TargetText = targetText;
									searchResult.IsTargetMatch = true;
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
						foreach (var translationUnitDetails in tuToAonymize.TranslationUnitDetails)
						{
							var sourceTranslationElements = translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList();
							var elementsContainsTag =
								sourceTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
							if (elementsContainsTag)
							{
								if (translationUnitDetails.SelectedWordsDetails.Any())
								{
								//	AnonymizeSelectedWordsFromPreview(translationUnitDetails, sourceTranslationElements);
								}
								AnonymizeSegmentsWithTags(translationUnitDetails, true);
							}
							else
							{
								if (translationUnitDetails.SelectedWordsDetails.Any())
								{
									//AnonymizeSelectedWordsFromPreview(translationUnitDetails, sourceTranslationElements);
								}
								AnonymizeSegmentsWithoutTags(translationUnitDetails, true);
							}
							//needs to be uncommented
							languageDirection.UpdateTranslationUnit(translationUnitDetails.TranslationUnit);
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

				foreach (var tuDetails in translationUnitPair.TranslationUnitDetails)
				{
					if (tuDetails.IsSourceMatch)
					{
						var sourceTranslationElements = tuDetails.TranslationUnit.SourceSegment.Elements.ToList();
						var elementsContainsTag = sourceTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
						if (elementsContainsTag)
						{
							//check if there are selected words from the ui
							if (tuDetails.SelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, sourceTranslationElements,true);
							}
							AnonymizeSegmentsWithTags(tuDetails, true);
						}
						else
						{
							if (tuDetails.SelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, sourceTranslationElements,true);
							}
							AnonymizeSegmentsWithoutTags(tuDetails, true);
						}
					}
					if (tuDetails.IsTargetMatch)
					{
						var targetTranslationElements = tuDetails.TranslationUnit.TargetSegment.Elements.ToList();
						var elementsContainsTag = targetTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
						if (elementsContainsTag)
						{
							//check if there are selected words from the ui
							if (tuDetails.TargetSelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, targetTranslationElements,false);
							}
							AnonymizeSegmentsWithTags(tuDetails, false);
						}
						else
						{
							if (tuDetails.TargetSelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, targetTranslationElements,false);
							}
							AnonymizeSegmentsWithoutTags(tuDetails, false);
						}
					}
					tm.LanguageDirection.UpdateTranslationUnit(tuDetails.TranslationUnit);
				}
			}
		}

		/// <summary>
		/// Replace selected words from UI with tags
		/// </summary>
		/// <param name="translationUnitDetails">Translation unit details</param>
		/// <param name="sourceTranslationElements">A List with the elements of segment</param>
		private static void AnonymizeSelectedWordsFromPreview(TranslationUnitDetails translationUnitDetails, List<SegmentElement> sourceTranslationElements,bool isSource)
		{
			if (isSource)
			{
				translationUnitDetails.TranslationUnit.SourceSegment.Elements.Clear();
			}
			else
			{
				translationUnitDetails.TranslationUnit.TargetSegment.Elements.Clear();
			}
			
			foreach (var element in sourceTranslationElements.ToList())
			{
				SelectedWordsFromUiElementVisitor visitor;
				if (isSource)
				{
					visitor = new SelectedWordsFromUiElementVisitor(translationUnitDetails.SelectedWordsDetails);
					element.AcceptSegmentElementVisitor(visitor);
				}
				else
				{
					visitor = new SelectedWordsFromUiElementVisitor(translationUnitDetails.TargetSelectedWordsDetails);
					element.AcceptSegmentElementVisitor(visitor);
				}
					

				//new elements after splited the text for selected words
				var newElements = visitor.SegmentColection;
				if (newElements?.Count > 0)
				{
					foreach (var segment in newElements)
					{
						var text = segment as Text;
						var tag = segment as Tag;
						//add segments back Source Segment
						if (isSource)
						{
							if (text != null)
							{

								translationUnitDetails.TranslationUnit.SourceSegment.Elements.Add(text);
							}
							if (tag != null)
							{
								translationUnitDetails.TranslationUnit.SourceSegment.Elements.Add(tag);
							}
						}
						else
						{
							if (text != null)
							{

								translationUnitDetails.TranslationUnit.TargetSegment.Elements.Add(text);
							}
							if (tag != null)
							{
								translationUnitDetails.TranslationUnit.TargetSegment.Elements.Add(tag);
							}
						}
						
					}
				}
				else
				{
					//add remaining words
					var text = element as Text;
					var tag = element as Tag;
					if (isSource)
					{
						if (text != null)
						{
							translationUnitDetails.TranslationUnit.SourceSegment.Elements.Add(text);
						}
						if (tag != null)
						{
							translationUnitDetails.TranslationUnit.SourceSegment.Elements.Add(tag);
						}
					}
					else
					{
						if (text != null)
						{
							translationUnitDetails.TranslationUnit.TargetSegment.Elements.Add(text);
						}
						if (tag != null)
						{
							translationUnitDetails.TranslationUnit.TargetSegment.Elements.Add(tag);
						}
					}
					
				}
			}
		}

		/// <summary>
		/// Anonymize segments without tags
		/// </summary>
		/// <param name="translationUnitDetails">Translation unit details</param>
		/// <param name="isSource"> Is source parameter</param>
		private static void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails,bool isSource)
		{
			var finalList = new List<SegmentElement>();
			var elementsList = new List<SegmentElement>();
			if (isSource)
			{
				elementsList = translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList();
			}
			else
			{
				elementsList = translationUnitDetails.TranslationUnit.TargetSegment.Elements.ToList();
			}
			foreach (var element in elementsList)
			{
				var visitor = new SegmentElementVisitor(translationUnitDetails.RemovedWordsFromMatches);
				element.AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					foreach (var segment in segmentColection)
					{
						var text = segment as Text;
						var tag = segment as Tag;
						if (text != null)
						{
							finalList.Add(text);
						}
						if (tag != null)
						{
							finalList.Add(tag);
						}
					}
				}
				else
				{
					//add remaining words
					var text = element as Text;
					var tag = element as Tag;
					if(text!=null)
					{
						finalList.Add(text);
					}
					if (tag != null)
					{
						finalList.Add(tag);
					}
				}
			}
			if (isSource)
			{
				//clear initial list
				translationUnitDetails.TranslationUnit.SourceSegment.Elements.Clear();
				//add new elements list to Translation Unit
				translationUnitDetails.TranslationUnit.SourceSegment.Elements = finalList;
			}
			else
			{
				//clear initial list
				translationUnitDetails.TranslationUnit.TargetSegment.Elements.Clear();
				//add new elements list to Translation Unit
				translationUnitDetails.TranslationUnit.TargetSegment.Elements = finalList;
			}
		 
		}

		/// <summary>
		/// Anonymize segments with tags
		/// </summary>
		/// <param name="translationUnitDetails"></param>
		/// <param name="isSource"></param>
		private static void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails,bool isSource)
		{
			var translationUnitElements = new List<SegmentElement>();
			if (isSource)
			{
				translationUnitElements = translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList();
			}
			else
			{
				translationUnitElements = translationUnitDetails.TranslationUnit.TargetSegment.Elements.ToList();
			}
			for (var i = 0; i < translationUnitElements.Count; i++)
			{
				if (isSource)
				{
					if (!translationUnitDetails.TranslationUnit.SourceSegment.Elements[i].GetType().UnderlyingSystemType.Name
						.Equals("Text")) continue;

					var visitor = new SegmentElementVisitor(translationUnitDetails.RemovedWordsFromMatches);
					//check for PI in each element from the list
					translationUnitDetails.TranslationUnit.SourceSegment.Elements[i].AcceptSegmentElementVisitor(visitor);
					var segmentColection = visitor.SegmentColection;

					if (segmentColection?.Count > 0)
					{
						var segmentElements = new List<SegmentElement>();
						//if element contains PI add it to a list of Segment Elements
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
						//remove from the list original element at position
						translationUnitDetails.TranslationUnit.SourceSegment.Elements.RemoveAt(i);
						//to the same position add the new list with elements (Text + Tag)
						translationUnitDetails.TranslationUnit.SourceSegment.Elements.InsertRange(i, segmentElements);

					}
				}
				else
				{
					if (!translationUnitDetails.TranslationUnit.TargetSegment.Elements[i].GetType().UnderlyingSystemType.Name
						.Equals("Text")) continue;

					var visitor = new SegmentElementVisitor(translationUnitDetails.TargetRemovedWordsFromMatches);
					//check for PI in each element from the list
					translationUnitDetails.TranslationUnit.TargetSegment.Elements[i].AcceptSegmentElementVisitor(visitor);
					var segmentColection = visitor.SegmentColection;

					if (segmentColection?.Count > 0)
					{
						var segmentElements = new List<SegmentElement>();
						//if element contains PI add it to a list of Segment Elements
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
						//remove from the list original element at position
						translationUnitDetails.TranslationUnit.TargetSegment.Elements.RemoveAt(i);
						//to the same position add the new list with elements (Text + Tag)
						translationUnitDetails.TranslationUnit.TargetSegment.Elements.InsertRange(i, segmentElements);
					}
				}
			}
		}
	}
}

