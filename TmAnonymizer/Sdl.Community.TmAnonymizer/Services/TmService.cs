using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Extensions;
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
		private readonly object _lockObject = new object();

		public TmService(SettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public TranslationUnit[] LoadTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider, LanguageDirection languageDirectionp)
		{
			if (tmFile != null)
			{
				var languageDirection = tmFile.TmLanguageDirections?.FirstOrDefault(a =>
					Equals(a.Source, languageDirectionp.Source) && Equals(a.Target, languageDirectionp.Target));

				if (languageDirection == null)
				{
					return null;
				}

				lock (_lockObject)
				{
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
							ReadTranslationUnits(context, serverBasedLanguageDirection, languageDirection);
						}
					}
					else
					{
						var translationMemory = new FileBasedTranslationMemory(tmFile.Path);

						if (translationMemory.LanguageDirection.SourceLanguage.Equals(languageDirectionp.Source) &&
							translationMemory.LanguageDirection.TargetLanguage.Equals(languageDirectionp.Target))
						{
							ReadTranslationUnits(context, translationMemory.LanguageDirection, languageDirection);
						}
					}

					return languageDirection.TranslationUnits;
				}
			}

			return null;
		}

		public TranslationUnit[] LoadTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider, List<LanguageDirection> languageDirections)
		{
			var translationUnits = new List<TranslationUnit>();
			foreach (var languageDirection in languageDirections)
			{
				var tus = LoadTranslationUnits(context, tmFile, translationProvider, languageDirection);
				if (tus != null)
				{
					translationUnits.AddRange(tus);
				}
			}

			return translationUnits.ToArray();
		}

		public AnonymizeTranslationMemory FileBaseTmGetTranslationUnits(ProgressDialogContext context,
			TmFile tmFile, PersonalDataParsingService personalDataParsingService,
			out List<SourceSearchResult> sourceSearchResult)
		{
			var translationMemory = new FileBasedTranslationMemory(tmFile.Path);
			var tus = LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = translationMemory.LanguageDirection.SourceLanguage,
					Target = translationMemory.LanguageDirection.TargetLanguage
				});

			sourceSearchResult = new List<SourceSearchResult>();

			decimal iTotal = tmFile.TranslationUnits;
			decimal iCurrent = 0;
			foreach (var translationUnit in tus)
			{
				if (context.CheckCancellationPending())
				{
					break;
				}

				iCurrent++;
				if (iCurrent == 1 || iCurrent % 100 == 0)
				{
					var progress = iCurrent / iTotal * 100;
					context.Report(Convert.ToInt32(progress), "Parsing: " + iCurrent + " of " + iTotal + " Translation Units");
				}

				var sourceText = translationUnit.SourceSegment.ToPlain();
				var targetText = translationUnit.TargetSegment.ToPlain();
				var sourceContainsPi = personalDataParsingService.ContainsPi(sourceText);
				var targetContainsPi = personalDataParsingService.ContainsPi(targetText);

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
							Positions = personalDataParsingService.GetPersonalDataPositions(sourceText)
						};
					}
					if (targetContainsPi)
					{
						searchResult.IsTargetMatch = true;
						searchResult.TargetMatchResult = new MatchResult
						{
							Positions = personalDataParsingService.GetPersonalDataPositions(targetText)
						};
					}

					sourceSearchResult.Add(searchResult);
				}
			}

			return new AnonymizeTranslationMemory
			{
				TmFile = tmFile,
				TranslationUnits = tus.ToList(),
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public AnonymizeTranslationMemory ServerBasedTmGetTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider,
			PersonalDataParsingService personalDataParsingService, out List<SourceSearchResult> sourceSearchResult)
		{
			var allTusForLanguageDirections = new List<TranslationUnit>();
			var searchResults = new List<SourceSearchResult>();

			var translationMemory = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

			var languageDirections = translationMemory.LanguageDirections;

			decimal iTotal = tmFile.TranslationUnits;
			decimal iCurrent = 0;

			foreach (var languageDirection in languageDirections)
			{
				var translationUnits = LoadTranslationUnits(context, tmFile, translationProvider, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (translationUnits != null)
				{
					allTusForLanguageDirections.AddRange(translationUnits);
					foreach (var translationUnit in translationUnits)
					{
						if (context.CheckCancellationPending())
						{
							break;
						}

						iCurrent++;
						if (iCurrent == 1 || iCurrent % 100 == 0)
						{
							var progress = iCurrent / iTotal * 100;
							context.Report(Convert.ToInt32(progress), "Parsing: " + iCurrent + " of " + iTotal + " Translation Units");
						}

						var sourceText = translationUnit.SourceSegment.ToPlain();
						var targetText = translationUnit.TargetSegment.ToPlain();
						var sourceContainsPi = personalDataParsingService.ContainsPi(sourceText);
						var targetContainsPi = personalDataParsingService.ContainsPi(targetText);

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
									Positions = personalDataParsingService.GetPersonalDataPositions(sourceText)
								};
							}
							if (targetContainsPi)
							{
								searchResult.IsTargetMatch = true;
								searchResult.TargetMatchResult = new MatchResult
								{
									Positions = personalDataParsingService.GetPersonalDataPositions(targetText)
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
				TmFile = tmFile,
				TranslationUnits = allTusForLanguageDirections,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public void AnonymizeServerBasedTu(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var translationMemory in anonymizeTranslationMemories)
			{
				var uri = new Uri(translationMemory.TmFile.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false, translationMemory.TmFile.Credentials.UserName, translationMemory.TmFile.Credentials.Password);
				var tm = translationProvider.GetTranslationMemory(translationMemory.TmFile.Path, TranslationMemoryProperties.All);

				var groupsOf = 100;
				var tusGroups = new List<List<TranslationUnit>> { new List<TranslationUnit>(translationMemory.TranslationUnits) };
				if (translationMemory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = translationMemory.TranslationUnits.ChunkBy(groupsOf);
				}

				foreach (var tus in tusGroups)
				{
					iCurrent = iCurrent + tus.Count;
					if (context != null && context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

					foreach (var languageDirection in tm.LanguageDirections)
					{
						var tusToUpdate = new List<TranslationUnit>();
						foreach (var tu in tus)
						{
							if (languageDirection.SourceLanguage.Equals(tu.SourceSegment.Culture) &&
								languageDirection.TargetLanguage.Equals(tu.TargetSegment.Culture))
							{
								tusToUpdate.Add(tu);
							}
						}

						if (tusToUpdate.Count > 0)
						{
							var results = languageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
						}
					}
				}
			}
		}

		public void AnonymizeFileBasedTu(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				var tm = new FileBasedTranslationMemory(anonymizeTranslationMemory.TmFile.Path);

				var groupsOf = 200;
				var tusGroups = new List<List<TranslationUnit>> { new List<TranslationUnit>(anonymizeTranslationMemory.TranslationUnits) };
				if (anonymizeTranslationMemory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = anonymizeTranslationMemory.TranslationUnits.ChunkBy(groupsOf);
				}

				foreach (var tus in tusGroups)
				{
					iCurrent = iCurrent + tus.Count;
					if (context != null && context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

					var results = tm.LanguageDirection.UpdateTranslationUnits(tus.ToArray());
				}
			}
		}

		private void PrepareTranslationUnits(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				foreach (var tuDetails in anonymizeTranslationMemory.TranslationUnitDetails)
				{
					iCurrent++;
					if (context != null && context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress), "Preparing: " + iCurrent + " of " + iTotalUnits + " Translation Units");

					if (tuDetails.IsSourceMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.SourceSegment.Elements.ToList(), true);
					}

					if (tuDetails.IsTargetMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.TargetSegment.Elements.ToList(), false);
					}
				}
			}
		}

		private static void ReadTranslationUnits(ProgressDialogContext context, ITranslationMemoryLanguageDirection languageDirection, TmLanguageDirection tmLanguageDirection)
		{
			decimal iTotalUnits = languageDirection.GetTranslationUnitCount();

			if (iTotalUnits == 0)
			{
				return;
			}

			var tus = new List<TranslationUnit>();

			decimal groups = 1;
			var unitCount = (int)iTotalUnits;
			decimal threshold = 1000;

			if (iTotalUnits > threshold)
			{
				groups = iTotalUnits / threshold;
				unitCount = Convert.ToInt32(iTotalUnits / groups);
			}

			var tmIterator = new RegularIterator
			{
				Forward = true,
				MaxCount = unitCount
			};

			for (var i = 1; i <= groups; i++)
			{
				if (context != null && context.CheckCancellationPending())
				{
					break;
				}

				var iCurrent = i * unitCount;
				var progress = iCurrent / iTotalUnits * 100;
				context?.Report(Convert.ToInt32(progress), "Reading: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				tus.AddRange(languageDirection.GetTranslationUnits(ref tmIterator));
			}

			if (context != null && context.CheckCancellationPending())
			{
				return;
			}

			if (tmIterator.ProcessedTranslationUnits < iTotalUnits)
			{
				tmIterator.MaxCount = (int)iTotalUnits - tmIterator.ProcessedTranslationUnits;
				tus.AddRange(languageDirection.GetTranslationUnits(ref tmIterator));
			}

			if (context != null && context.CheckCancellationPending())
			{
				return;
			}

			tmLanguageDirection.TranslationUnits = tus.ToArray();
			tmLanguageDirection.TranslationUnitsCount = tmLanguageDirection.TranslationUnits.Length;

		}

		private void AnonymizeSegment(TranslationUnitDetails tuDetails, List<SegmentElement> elements, bool isSource)
		{
			var elementsContainsTag = elements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
			if (elementsContainsTag)
			{
				//check if there are selected words from the ui
				if (tuDetails.SelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithTags(tuDetails, isSource);
			}
			else
			{
				if (tuDetails.SelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithoutTags(tuDetails, isSource);
			}
		}
		
		private static void AnonymizeSelectedWordsFromPreview(TranslationUnitDetails translationUnitDetails, IEnumerable<SegmentElement> translationElements, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.SourceSegment, translationElements,
					translationUnitDetails.SelectedWordsDetails);
			}
			else
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.TargetSegment, translationElements,
					translationUnitDetails.TargetSelectedWordsDetails);
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(Segment segment, IEnumerable<SegmentElement> translationElements, List<WordDetails> selectedWords)
		{
			segment.Elements.Clear();

			foreach (var element in translationElements.ToList())
			{
				var visitor = new SelectedWordsFromUiElementVisitor(selectedWords);
				element.AcceptSegmentElementVisitor(visitor);

				//new elements after splited the text for selected words
				var newElements = visitor.SegmentColection;
				if (newElements?.Count > 0)
				{
					foreach (var seg in newElements.Cast<SegmentElement>())
					{
						//add segments back Source Segment
						AddElement(seg, segment.Elements);
					}
				}
				else
				{
					//add remaining words				
					AddElement(element, segment.Elements);
				}
			}
		}

		private void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.SourceSegment, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.TargetSegment, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private void AnonymizeSegmentsWithoutTags(Segment segment, List<WordDetails> removeWords)
		{
			var segmentElements = new List<SegmentElement>();
			
			foreach (var element in segment.Elements.ToList())
			{
				var visitor = new SegmentElementVisitor(removeWords, _settingsService);

				element.AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					foreach (var seg in segmentColection.Cast<SegmentElement>())
					{
						AddElement(seg, segmentElements);
					}
				}
				else
				{
					//add remaining words
					AddElement(element, segmentElements);
				}
			}

			//clear initial list
			segment.Elements.Clear();

			//add new elements list to Translation Unit
			segment.Elements = segmentElements;
		}

		private void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.SourceSegment, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.TargetSegment, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private void AnonymizeSegmentsWithTags(Segment segment, List<WordDetails> removeWords)
		{
			for (var i = 0; i < segment.Elements.Count; i++)
			{
				if (!segment.Elements[i].GetType().UnderlyingSystemType.Name.Equals("Text"))
				{
					return;
				}

				var visitor = new SegmentElementVisitor(removeWords, _settingsService);
				//check for PI in each element from the list
				segment.Elements[i].AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					var segmentElements = new List<SegmentElement>();
					//if element contains PI add it to a list of Segment Elements
					foreach (var seg in segmentColection.Cast<SegmentElement>())
					{
						AddElement(seg, segmentElements);
					}

					//remove from the list original element at position
					segment.Elements.RemoveAt(i);

					//to the same position add the new list with elements (Text + Tag)
					segment.Elements.InsertRange(i, segmentElements);
				}
			}
		}

		private static void AddElement(SegmentElement element, ICollection<SegmentElement> finalList)
		{
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
}

