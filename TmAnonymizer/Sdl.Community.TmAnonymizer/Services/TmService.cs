using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class TmService
	{
		private readonly SettingsService _settingsService;

		public TmService(SettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public TranslationUnit[] LoadTranslationUnits(TmFile tmFile, TranslationProviderServer translationProvider, LanguageDirection languageDirectionp)
		{
			if (tmFile != null)
			{
				var languageDirection = tmFile.TmLanguageDirections?.FirstOrDefault(a => Equals(a.Source, languageDirectionp.Source) && Equals(a.Target, languageDirectionp.Target));

				if (languageDirection == null)
				{
					return null;
				}

				if (languageDirection.TranslationUnits != null)
				{
					return languageDirection.TranslationUnits;
				}

				if (tmFile.IsServerTm)
				{
					var translationMemory = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

					var serverBasedLanguageDirection = translationMemory.LanguageDirections.FirstOrDefault(a =>
						a.SourceLanguage.Equals(languageDirectionp.Source) && a.TargetLanguage.Equals(languageDirectionp.Target));

					if (serverBasedLanguageDirection != null)
					{
						var unitsCount = serverBasedLanguageDirection.GetTranslationUnitCount();
						var tmIterator = new RegularIterator(unitsCount);

						languageDirection.TranslationUnits = serverBasedLanguageDirection.GetTranslationUnits(ref tmIterator);
						languageDirection.TranslationUnitsCount = languageDirection.TranslationUnits.Length;
					}
				}
				else
				{
					var translationMemory = new FileBasedTranslationMemory(tmFile.Path);

					if (translationMemory.LanguageDirection.SourceLanguage.Equals(languageDirectionp.Source) &&
						translationMemory.LanguageDirection.TargetLanguage.Equals(languageDirectionp.Target))
					{
						var unitsCount = translationMemory.LanguageDirection.GetTranslationUnitCount();
						var tmIterator = new RegularIterator(unitsCount);

						languageDirection.TranslationUnits = translationMemory.LanguageDirection.GetTranslationUnits(ref tmIterator);
						languageDirection.TranslationUnitsCount = languageDirection.TranslationUnits.Length;
					}
				}

				return languageDirection.TranslationUnits;
			}

			return null;
		}

		public TranslationUnit[] LoadTranslationUnits(TmFile tmFile, TranslationProviderServer translationProvider, List<LanguageDirection> languageDirections)
		{
			var translationUnits = new List<TranslationUnit>();
			foreach (var languageDirection in languageDirections)
			{
				var tus = LoadTranslationUnits(tmFile, translationProvider, languageDirection);
				if (tus != null)
				{
					translationUnits.AddRange(tus);
				}
			}

			return translationUnits.ToArray();
		}


		public AnonymizeTranslationMemory FileBaseTmGetTranslationUnits(TmFile tmFile,
			List<Rule> selectedRules, out List<SourceSearchResult> sourceSearchResult)
		{
			var translationMemory = new FileBasedTranslationMemory(tmFile.Path);
			var tus = LoadTranslationUnits(tmFile, null,
				new LanguageDirection
				{
					Source = translationMemory.LanguageDirection.SourceLanguage,
					Target = translationMemory.LanguageDirection.TargetLanguage
				});

			sourceSearchResult = new List<SourceSearchResult>();

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
						TmFilePath = tmFile.Path,
						IconFilePath = "../Resources/TranslationMemory.ico",
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
				TmPath = tmFile.Path,
				TranslationUnits = tus.ToList(),
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public AnonymizeTranslationMemory ServerBasedTmGetTranslationUnits(TmFile tmFile, TranslationProviderServer translationProvider,
			List<Rule> selectedRules, out List<SourceSearchResult> sourceSearchResult)
		{
			var allTusForLanguageDirections = new List<TranslationUnit>();
			var searchResults = new List<SourceSearchResult>();

			var translationMemory = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

			var languageDirections = translationMemory.LanguageDirections;
			var pi = new PersonalInformation(selectedRules);

			foreach (var languageDirection in languageDirections)
			{
				var translationUnits = LoadTranslationUnits(tmFile, translationProvider, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (translationUnits != null)
				{
					allTusForLanguageDirections.AddRange(translationUnits);
					foreach (var translationUnit in translationUnits)
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
								TmFilePath = tmFile.Path,
								IsServer = true,
								IconFilePath = "../Resources/ServerBasedTranslationMemory.ico",
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
								searchResult.TargetMatchResult = new MatchResult
								{
									Positions = pi.GetPersonalDataPositions(targetText)
								};
							}

							searchResults.Add(searchResult);
						}
					}
				}
			}

			sourceSearchResult = searchResults;

			return new AnonymizeTranslationMemory
			{
				TmPath = tmFile.Path,
				TranslationUnits = allTusForLanguageDirections,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public void AnonymizeServerBasedTu(TranslationProviderServer translationProvider, List<AnonymizeTranslationMemory> tusToAnonymize)
		{
			try
			{
				foreach (var tuToAonymize in tusToAnonymize)
				{
					var translationMemory = translationProvider.GetTranslationMemory(tuToAonymize.TmPath, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;

					foreach (var languageDirection in languageDirections)
					{
						foreach (var translationUnitDetails in tuToAonymize.TranslationUnitDetails)
						{
							if (translationUnitDetails.IsSourceMatch)
							{
								var sourceTranslationElements = translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList();
								var elementsContainsTag = sourceTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
								if (elementsContainsTag)
								{
									if (translationUnitDetails.SelectedWordsDetails.Any())
									{
										AnonymizeSelectedWordsFromPreview(translationUnitDetails, sourceTranslationElements, true);
									}
									AnonymizeSegmentsWithTags(translationUnitDetails, true);
								}
								else
								{
									if (translationUnitDetails.SelectedWordsDetails.Any())
									{
										AnonymizeSelectedWordsFromPreview(translationUnitDetails, sourceTranslationElements, true);
									}
									AnonymizeSegmentsWithoutTags(translationUnitDetails, true);
								}
							}

							if (translationUnitDetails.IsTargetMatch)
							{
								var targetTranslationElements = translationUnitDetails.TranslationUnit.TargetSegment.Elements.ToList();
								var elementsContainsTag =
									targetTranslationElements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
								if (elementsContainsTag)
								{
									if (translationUnitDetails.TargetSelectedWordsDetails.Any())
									{
										AnonymizeSelectedWordsFromPreview(translationUnitDetails, targetTranslationElements, false);
									}
									AnonymizeSegmentsWithTags(translationUnitDetails, false);
								}
								else
								{
									if (translationUnitDetails.TargetSelectedWordsDetails.Any())
									{
										AnonymizeSelectedWordsFromPreview(translationUnitDetails, targetTranslationElements, false);
									}
									AnonymizeSegmentsWithoutTags(translationUnitDetails, false);
								}
							}
							
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
						MessageBox.Show(exception.InnerException.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show(exception.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

		}

		public void AnonymizeFileBasedTu(List<AnonymizeTranslationMemory> tusToAnonymize)
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
								AnonymizeSelectedWordsFromPreview(tuDetails, sourceTranslationElements, true);
							}
							AnonymizeSegmentsWithTags(tuDetails, true);
						}
						else
						{
							if (tuDetails.SelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, sourceTranslationElements, true);
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
								AnonymizeSelectedWordsFromPreview(tuDetails, targetTranslationElements, false);
							}
							AnonymizeSegmentsWithTags(tuDetails, false);
						}
						else
						{
							if (tuDetails.TargetSelectedWordsDetails.Any())
							{
								AnonymizeSelectedWordsFromPreview(tuDetails, targetTranslationElements, false);
							}
							AnonymizeSegmentsWithoutTags(tuDetails, false);
						}
					}
					tm.LanguageDirection.UpdateTranslationUnit(tuDetails.TranslationUnit);
				}
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(TranslationUnitDetails translationUnitDetails, List<SegmentElement> sourceTranslationElements, bool isSource)
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

		private void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			var finalList = new List<SegmentElement>();

			var elementsList = isSource
				? translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList()
				: translationUnitDetails.TranslationUnit.TargetSegment.Elements.ToList();

			foreach (var element in elementsList)
			{
				var visitor = isSource
					? new SegmentElementVisitor(translationUnitDetails.RemovedWordsFromMatches, _settingsService)
					: new SegmentElementVisitor(translationUnitDetails.TargetRemovedWordsFromMatches, _settingsService);

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

		private void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			var translationUnitElements = isSource
				? translationUnitDetails.TranslationUnit.SourceSegment.Elements.ToList()
				: translationUnitDetails.TranslationUnit.TargetSegment.Elements.ToList();

			for (var i = 0; i < translationUnitElements.Count; i++)
			{
				if (isSource)
				{
					if (!translationUnitDetails.TranslationUnit.SourceSegment.Elements[i].GetType().UnderlyingSystemType.Name
						.Equals("Text")) continue;

					var visitor = new SegmentElementVisitor(translationUnitDetails.RemovedWordsFromMatches, _settingsService);
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

					var visitor = new SegmentElementVisitor(translationUnitDetails.TargetRemovedWordsFromMatches, _settingsService);
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

