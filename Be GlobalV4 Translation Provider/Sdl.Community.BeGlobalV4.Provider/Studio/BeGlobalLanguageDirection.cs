using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalLanguageDirection	: ITranslationProviderLanguageDirection
	{
		private readonly BeGlobalTranslationProvider _beGlobalTranslationProvider;
		private readonly BeGlobalTranslationOptions _options;
		private readonly LanguagePair _languageDirection;
		private BeGlobalConnecter _beGlobalConnect;
		private TranslationUnit _inputTu;

		public ITranslationProvider TranslationProvider => _beGlobalTranslationProvider;
		public CultureInfo SourceLanguage { get; }
		public CultureInfo TargetLanguage { get; }
		public bool CanReverseLanguageDirection { get; }

		public BeGlobalLanguageDirection(BeGlobalTranslationProvider beGlobalTranslationProvider,LanguagePair languageDirection)
		{
			_beGlobalTranslationProvider = beGlobalTranslationProvider;
			_languageDirection = languageDirection;
			_options = beGlobalTranslationProvider.Options;
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			var translation = new Segment(_languageDirection.TargetCulture);
			var results = new SearchResults
			{
				SourceSegment = segment.Duplicate()
			};
			if (!_options.ResendDrafts && _inputTu.ConfirmationLevel != ConfirmationLevel.Unspecified)
			{
				translation.Add(PluginResources.TranslationLookupDraftNotResentMessage);
				//later get these strings from resource file
				results.Add(CreateSearchResult(segment, translation));
				return results;
			}
			var newseg = segment.Duplicate();
			if (newseg.HasTags)
			{
				var tagPlacer = new BeGlobalTagPlacer(newseg);
				var translatedText = LookupBeGlobal(tagPlacer.PreparedSourceText);
				translation = tagPlacer.GetTaggedSegment(translatedText);

				results.Add(CreateSearchResult(newseg, translation));
				return results;
			}
			else
			{ 
				var sourcetext = newseg.ToPlain();

				var translatedText = LookupBeGlobal(sourcetext);
				translation.Add(translatedText);

				results.Add(CreateSearchResult(newseg, translation));
				return results;
			}
		}

		public void PrepareTempData(List<PreTranslateSegment> preTranslatesegments)
		{
			var preTranslateHelp = new PreTranslateTempFile();
			var filePath = preTranslateHelp.CreateXmlFile();

			for (int i = 0; i < preTranslatesegments.Count; i++)
			{
				var sourceText = string.Empty;
				var newseg = preTranslatesegments[i].TranslationUnit.SourceSegment.Duplicate();

				if (newseg.HasTags)
				{
					var tagPlacer = new BeGlobalTagPlacer(newseg);
					sourceText = tagPlacer.PreparedSourceText;
				}
				else
				{
					sourceText = newseg.ToPlain(); 
				}
				preTranslateHelp.WriteSegments(filePath,i,sourceText);	 
			}

		}

		private SearchResult CreateSearchResult(Segment segment, Segment translation)
		{ 
			var tu = new TranslationUnit
			{
				SourceSegment = segment.Duplicate(),//this makes the original source segment, with tags, appear in the search window
				TargetSegment = translation
			};
			   
			tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

			const int score = 0; //score to 0...change if needed to support scoring
			tu.Origin = TranslationUnitOrigin.MachineTranslation;
			var searchResult = new SearchResult(tu)
			{
				ScoringResult = new ScoringResult
				{
					BaseScore = score
				}
			};
			tu.ConfirmationLevel = ConfirmationLevel.Draft;

			return searchResult;
		}
	

		private string LookupBeGlobal(string sourcetext)
		{
			if (_beGlobalConnect == null)
			{ 
				_beGlobalConnect = new BeGlobalConnecter(_options.ClientId, _options.ClientSecret, _options.UseClientAuthentication,_options.Model);
			}
			else
			{
				_beGlobalConnect.ClientId = _options.ClientId;
				_beGlobalConnect.ClientSecret = _options.ClientSecret;
				_beGlobalConnect.UseClientAuthentication = _options.UseClientAuthentication;
			}

			var translatedText = _beGlobalConnect.Translate(_languageDirection, sourcetext);
			return translatedText;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			_inputTu = translationUnit;
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			// bug LG-15128 where mask parameters are true for both CM and the actual TU to be updated which cause an unnecessary call for CM segment
			var results = new List<SearchResults>();
			// plugin is called from pre-translate batch task 
			//we receive the data in chunchs of 10 segments
			if (translationUnits.Length > 1)
			{
			   var preTranslateList= new List<PreTranslateSegment>();
				var i = 0;
				foreach (var tu in translationUnits)
				{
					if (mask == null || mask[i])
					{
						var preTranslate = new PreTranslateSegment
						{
							SearchSettings = settings,
							TranslationUnit = tu
						};
						preTranslateList.Add(preTranslate);
						//var result = SearchTranslationUnit(settings, tu);
						//results.Add(result);
					}
					else
					{
						results.Add(null);
					}
					i++;
				}
				PrepareTempData(preTranslateList);
			}
			else
			{
				var i = 0;
				foreach (var tu in translationUnits)
				{
					if (mask == null || mask[i])
					{
						var result = SearchTranslationUnit(settings, tu);
						results.Add(result);
					}
					else
					{
						results.Add(null);
					}
					i++;
				}
			}
			

			
			return results.ToArray();
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits,
			ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

	}
}
