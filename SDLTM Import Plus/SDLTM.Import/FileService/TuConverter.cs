using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace SDLTM.Import.FileService
{
	public class TuConverter
	{
		private const string SidKey = "sdl:sid";

		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, bool includeTrackChanges = false)
		{
			var settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = false,
				StripTags = false,
				ExcludeTagsInLockedContentText = false,
				AcceptTrackChanges = true
			};

			return BuildLinguaSegment(culture, segment, settings);
		}

		/// <summary>
		/// Builds a language platform segment from a filter framework segment. 
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="segment"></param>
		/// <param name="stripTags"></param>
		/// <param name="excludeTagsInLockedContentText"></param>
		/// <param name="acceptTrackChanges"></param>
		/// <param name="hasTrackChanges">an out parameter to show if this segment has track changes</param>
		/// <param name="includeTrackChanges"> </param>
		/// <returns></returns>
		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment,
			bool stripTags, bool excludeTagsInLockedContentText,
			bool acceptTrackChanges, out bool hasTrackChanges, bool includeTrackChanges = false)
		{
			var settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = includeTrackChanges,
				StripTags = stripTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges
			};

			return BuildLinguaSegmentInternal(culture, segment, settings, out hasTrackChanges, out _, out _);
		}


		/// <summary>
		/// Builds a language platform segment from a filter framework segment. 
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="segment"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, LinguaTuBuilderSettings settings)
		{
			return BuildLinguaSegmentInternal(culture, segment, settings, out _, out _, out _);
		}

		/// <summary>
		/// Converts bilingual segment to lingua and returns text elements mapping between the 2 models
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="segment"></param>
		/// <param name="settings"></param>
		/// <param name="textAssociations"></param>
		/// <param name="tagAssociations"></param>
		/// <returns></returns>
		public static Segment BuildLinguaSegment(CultureInfo culture, ISegment segment, LinguaTuBuilderSettings settings,
			out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations, out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations)
		{
			return BuildLinguaSegmentInternal(culture, segment, settings, out _, out tagAssociations, out textAssociations);
		}

		private static Segment BuildLinguaSegmentInternal(CultureInfo culture, IAbstractMarkupDataContainer segment, LinguaTuBuilderSettings settings, out bool hasTrackChanges,
			out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations,
			out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations)
		{
			var result = new Segment(culture);

			hasTrackChanges = AppendToLinguaSegment(segment, result, settings, out tagAssociations, out textAssociations);

			if (!settings.DoNotTrim)
				result.Trim();
			result.MergeAdjacentTextRuns();

			return result;
		}


		/// <summary>
		/// Builds a language platform segment from a filter framework segment. 
		/// </summary>
		/// <param name="culture">The culture of the segment</param>
		/// <param name="segment">The filter framework segment</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result
		/// will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text
		/// will be excluded.</param>
		/// <param name="includeTrackChanges"> </param>
		/// <returns>A new language platform segment which corresponds to <paramref name="segment"/></returns>
		public static Segment BuildLinguaSegment(CultureInfo culture,
			ISegment segment,
			bool stripTags,
			bool excludeTagsInLockedContentText, bool includeTrackChanges = false)
		{
			return BuildLinguaSegment(culture, segment, stripTags, excludeTagsInLockedContentText, true, out _, includeTrackChanges);
		}

		/// <summary>
		/// Searches for the most sigifnicant structure context information in an <see cref="Sdl.FileTypeSupport.Framework.NativeApi.IParagraphUnitProperties"/> object.
		/// </summary>
		/// <param name="paragraphProperties">The paragraph unit's properties</param>
		/// <returns>The most significant structure context name, or <c>null</c> if none is present</returns>
		public static string GetMostSignificantStructureContext(IParagraphUnitProperties paragraphProperties)
		{
			return paragraphProperties?.Contexts?.Contexts != null && paragraphProperties.Contexts.Contexts.Count > 0
				? (from context in paragraphProperties.Contexts.Contexts
				   where context.Purpose == ContextPurpose.Match && context.ContextType != SidKey
				   select context.ContextType).FirstOrDefault()
				: null;
		}

		/// <summary>
		/// Searches for SID value in an <see cref="Sdl.FileTypeSupport.Framework.NativeApi.IParagraphUnitProperties"/> object.
		/// </summary>
		/// <param name="paragraphProperties">The paragraph unit's properties</param>
		/// <returns>The most significant structure context name, or <c>null</c> if none is present</returns>
		public static string GeSIDContext(IParagraphUnitProperties paragraphProperties)
		{
			return paragraphProperties?.Contexts?.Contexts != null && paragraphProperties.Contexts.Contexts.Count > 0
				? (from context in paragraphProperties.Contexts.Contexts
				   where context.Purpose == ContextPurpose.Match && context.ContextType == SidKey
				   select context.Description).FirstOrDefault()
				: null;
		}
		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="documentProperties">Document properties for the document which the segment pair belongs to </param>
		/// <param name="fileProperties">File properties for file which the segment pair belongs to </param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result
		/// will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text
		/// will be excluded.</param>
		/// <param name="acceptTrackChanges"></param>
		/// <param name="hasSourceTrackChanges"></param>
		/// <param name="includeTrackChanges">If true, result will contain revision tags </param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or 
		/// <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			var tu = BuildLinguaTranslationUnitInternal(
				lp, sp, paragraphProperties,
				stripTags, excludeTagsInLockedContentText, acceptTrackChanges, false, out hasSourceTrackChanges, includeTrackChanges);

			SetTranslationUnitDocInfo(tu, documentProperties, fileProperties, sp);
			return tu;
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result
		/// will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text
		/// will be excluded.</param>
		/// <param name="acceptTrackChanges"></param>
		/// <param name="hasSourceTrackChanges"></param>
		/// <param name="includeTrackChanges">If true, result will contain revision tags </param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or 
		/// <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags,
				excludeTagsInLockedContentText, acceptTrackChanges, false, out hasSourceTrackChanges, includeTrackChanges);
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="documentProperties">document properties for the document which the segment belongs to </param>	    
		/// <param name="fileProperties">file properties for the file which the segment belongs to </param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result
		/// will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text
		/// will be excluded.</param>
		/// <param name="acceptTrackChanges"></param>
		/// <param name="hasSourceTrackChanges"></param>
		/// <param name="alignTags">If true, an attempt is made to align cross-segment tags</param>
		/// <param name="includeTrackChanges">If true, result will contain revision tags </param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or 
		/// <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges,
			bool alignTags, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			var tu = BuildLinguaTranslationUnitInternal(
				lp, sp, paragraphProperties, stripTags,
				excludeTagsInLockedContentText, acceptTrackChanges, alignTags, out hasSourceTrackChanges, includeTrackChanges);

			SetTranslationUnitDocInfo(tu, documentProperties, fileProperties, sp);
			return tu;
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result
		/// will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text
		/// will be excluded.</param>
		/// <param name="acceptTrackChanges"></param>
		/// <param name="hasSourceTrackChanges"></param>
		/// <param name="alignTags">If true, an attempt is made to align cross-segment tags</param>
		/// <param name="includeTrackChanges">If true, result will contain revision tags </param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or 
		/// <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges,
			bool alignTags, out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, stripTags,
				excludeTagsInLockedContentText, acceptTrackChanges, alignTags, out hasSourceTrackChanges, includeTrackChanges);
		}


		//public static List<Token> TokenizeSegment(Segment segment)
		//{
		//	var setup = new TokenizerSetup
		//	{
		//		Culture = segment.Culture,
		//		BuiltinRecognizers = BuiltinRecognizers.RecognizeNone
		//	};

		//	var tokenizer = new Tokenizer(setup);
		//	return tokenizer.Tokenize(segment);
		//}

		internal static void OverwriteTargetTagIdsFromSource(List<KeyValuePair<Tag, IAbstractMarkupData>> sourceTags,
															List<KeyValuePair<Tag, IAbstractMarkupData>> targetTags)
		{
			var sourceLcList = sourceTags.Where(x => x.Key.Type == TagType.LockedContent).Select(x => x.Key).ToArray();
			var targetLcList = targetTags.Where(x => x.Key.Type == TagType.LockedContent).Select(x => x.Key).ToArray();
			var usedIds = new List<string>();

			foreach (var targetLc in targetLcList)
			{
				//lookup in source
				var sourceLcMatch = sourceLcList.FirstOrDefault(x => x.TextEquivalent == targetLc.TextEquivalent && !usedIds.Contains(x.TagID));

				if (sourceLcMatch == null) continue;
				targetLc.TagID = sourceLcMatch.TagID;
				usedIds.Add(sourceLcMatch.TagID);
			}
		}

		private static TranslationUnit BuildLinguaTranslationUnitInternal(LanguagePair lp,
			ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool acceptTrackChanges,
			bool alignTags,
			out bool hasSourceTrackChanges, bool includeTrackChanges = false)
		{
			hasSourceTrackChanges = false;
			if (lp == null)
				throw new ArgumentNullException(nameof(lp));
			if (sp == null)
				throw new ArgumentNullException(nameof(sp));

			List<KeyValuePair<Tag, IAbstractMarkupData>> targetTagAssociations = null;

			var settings = new LinguaTuBuilderSettings
			{
				IncludeTrackChanges = includeTrackChanges,
				StripTags = stripTags,
				ExcludeTagsInLockedContentText = excludeTagsInLockedContentText,
				AcceptTrackChanges = acceptTrackChanges
			};

			var s = BuildLinguaSegmentInternal(lp.SourceCulture, sp.Source, settings, out hasSourceTrackChanges, out var sourceTagAssociations, out _);

			if (s.IsEmpty)
				return null;

			Segment t = null;
			if (sp.Target != null && lp.TargetCulture != null)
			{
				t = BuildLinguaSegmentInternal(lp.TargetCulture, sp.Target, settings, out _, out targetTagAssociations, out _);
			}

			//ensure matching LockContent will have same tagIds in target as in source so they are properly aligned
			if (targetTagAssociations != null)
				OverwriteTargetTagIdsFromSource(sourceTagAssociations, targetTagAssociations);

			if (alignTags && sourceTagAssociations != null && targetTagAssociations != null
				&& sourceTagAssociations.Count > 0 && targetTagAssociations.Count > 0)
			{
				AlignTags(sourceTagAssociations, targetTagAssociations);
			}

			var tu = new TranslationUnit
			{
				SourceSegment = s,
				TargetSegment = t,
				Origin = TranslationUnitOrigin.Unknown
			};

			if (sp.Properties != null)
			{
				tu.ConfirmationLevel = sp.Properties.ConfirmationLevel;

				if (sp.Properties.TranslationOrigin?.MetaData != null && sp.Properties.TranslationOrigin.MetaData.Any())
				{
					foreach (var metaData in sp.Properties.TranslationOrigin.MetaData)
					{
						switch (metaData.Key)
						{
							case TMTranslationOriginMetaData.CreationDate:
								tu.SystemFields.CreationDate = TryParseDateTimeWithFallback(metaData.Value);
								break;

							case TMTranslationOriginMetaData.CreatedBy:
								tu.SystemFields.CreationUser = metaData.Value;
								break;

							case TMTranslationOriginMetaData.LastModifiedDate:
								tu.SystemFields.ChangeDate = TryParseDateTimeWithFallback(metaData.Value);
								break;

							case TMTranslationOriginMetaData.LastModifiedUserId:
								tu.SystemFields.ChangeUser = metaData.Value;
								break;
						}
					}
				}
				else
				{
					tu.SystemFields.CreationDate = tu.SystemFields.ChangeDate = DateTime.Now;
				}
			}

			if (t != null && sp.Properties.TranslationOrigin != null && !string.IsNullOrEmpty(sp.Properties.TranslationOrigin.OriginType))
			{
				switch (sp.Properties.TranslationOrigin.OriginType)
				{
					case DefaultTranslationOrigin.DocumentMatch:
						tu.Origin = TranslationUnitOrigin.ContextTM;
						break;

					case DefaultTranslationOrigin.AutomatedAlignment:
						tu.Origin = TranslationUnitOrigin.Alignment;
						break;

					case DefaultTranslationOrigin.MachineTranslation when sp.Properties.ConfirmationLevel == ConfirmationLevel.Translated:
					case DefaultTranslationOrigin.TranslationMemory:
						tu.Origin = TranslationUnitOrigin.TM;
						break;

					case DefaultTranslationOrigin.MachineTranslation:
						tu.Origin = TranslationUnitOrigin.MachineTranslation;
						break;

					default:
						tu.Origin = TranslationUnitOrigin.Unknown;
						break;
				}
			}

			if (paragraphProperties == null)
			{
				paragraphProperties = sp.Source.ParentParagraphUnit?.Properties;
			}

			if (paragraphProperties == null) return tu;
			var structureContext = GetMostSignificantStructureContext(paragraphProperties);
			if (!string.IsNullOrEmpty(structureContext))
			{
				tu.StructureContexts = new[] { structureContext };
			}

			var sidContext = GeSIDContext(paragraphProperties);
			if (!string.IsNullOrEmpty(sidContext))
			{
				tu.IdContexts.Add(sidContext);
			}

			return tu;
		}

		/// <summary>
		/// Tryes to parse the given string using invariant culture and if fails tries with current culture.
		/// </summary>
		private static DateTime TryParseDateTimeWithFallback(string metaValue)
		{
			if (DateTime.TryParse(metaValue, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
				return date;

			return DateTime.TryParse(metaValue, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out date) ?
				date : DateTime.MinValue;
		}

		private static void AlignTags(List<KeyValuePair<Tag, IAbstractMarkupData>> sourceTags,
			List<KeyValuePair<Tag, IAbstractMarkupData>> targetTags)
		{
			if (sourceTags == null || targetTags == null)
				return;

			var maxAlignmentAnchor = sourceTags.Max(t => t.Key.AlignmentAnchor);
			foreach (var t in targetTags)
			{
				maxAlignmentAnchor = Math.Max(maxAlignmentAnchor, t.Key.AlignmentAnchor);
			}

			// NOTE in each collection, keys are non-null, but values may be (e.g. for locked
			//  content) if no BCM tag is associated with the tag.

			foreach (var srcTag in sourceTags)
			{
				if (srcTag.Key.AlignmentAnchor != 0 || !IsAlignableTag(srcTag))
					continue;

				foreach (var tgtTag in targetTags)
				{
					if (tgtTag.Key.AlignmentAnchor != 0
						|| !IsAlignableTag(tgtTag)
						|| srcTag.Key.Type != tgtTag.Key.Type)
						continue;

					var alignable = false;

					switch (srcTag.Key.Type)
					{
						case TagType.LockedContent:
						case TagType.TextPlaceholder:
							alignable = string.Equals(srcTag.Key.TextEquivalent, tgtTag.Key.TextEquivalent);
							break;
						case TagType.Start:
						case TagType.Standalone:
							alignable = AreAlignable(srcTag, tgtTag);
							break;
					}

					if (!alignable) continue;
					++maxAlignmentAnchor;
					srcTag.Key.AlignmentAnchor = maxAlignmentAnchor;
					tgtTag.Key.AlignmentAnchor = maxAlignmentAnchor;
					break; // stop iteration through target tags, continue with next source tag
				}
			}
		}

		private static bool IsAlignableTag(KeyValuePair<Tag, IAbstractMarkupData> tag)
		{
			var t = tag.Key;
			return t != null && t.Type != TagType.End;
		}

		/// <summary>
		/// For a paired tag, determines whether the tags can be aligned.
		/// </summary>
		/// <param name="leftTag"></param>
		/// <param name="rightTag"></param>
		/// <returns></returns>
		private static bool AreAlignable(KeyValuePair<Tag, IAbstractMarkupData> leftTag,
			KeyValuePair<Tag, IAbstractMarkupData> rightTag)
		{
			var eqTagId = string.Equals(leftTag.Key.TagID, rightTag.Key.TagID);
			if (eqTagId)
				return true;

			if (leftTag.Value != null || rightTag.Value != null)
				return false;

			switch (leftTag.Value)
			{
				case IAbstractTag leftTagValue when rightTag.Value is IAbstractTag rightTagValue:
					{

						if (leftTag.Value == null || rightTag.Value == null
												  || leftTagValue.TagProperties == null || rightTagValue.TagProperties == null)
							return false;

						if (!(leftTagValue.TagProperties is IStartTagProperties leftStartProperties) || !(rightTagValue.TagProperties is IStartTagProperties rightStartProperties))
							return false;

						if (leftStartProperties.SegmentationHint != rightStartProperties.SegmentationHint
							|| leftStartProperties.CanHide != rightStartProperties.CanHide
							|| leftStartProperties.IsSoftBreak != rightStartProperties.IsSoftBreak
							|| leftStartProperties.IsWordStop != rightStartProperties.IsWordStop)
							return false;

						if (leftStartProperties.Formatting == null != (rightStartProperties.Formatting == null))
							return false;

						if (leftStartProperties.Formatting == null)
						{
							// both formattings are null - can't align based on formatting. Compare based on tag content.
							return string.Equals(leftStartProperties.TagContent, rightStartProperties.TagContent,
								StringComparison.InvariantCultureIgnoreCase);
						}

						return leftStartProperties.Formatting.Equals(rightStartProperties.Formatting);
					}
				case IRevisionMarker leftRevisionValue when rightTag.Value is IRevisionMarker rightRevisionValue:
					{
						//this is possibly redundant already checked with statement
						//bool eqTagId = String.Equals(leftTag.Key.TagID, rightTag.Key.TagID);
						return leftRevisionValue.Equals(rightRevisionValue);
					}
				default:
					return false;
			}
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp"></param>
		/// <param name="fileProperties"></param>
		/// <param name="sp"></param>
		/// <param name="paragraphProperties"></param>
		/// <param name="stripTags"></param>
		/// <param name="excludeTagsInLockedContentText"></param>
		/// <param name="documentProperties"> </param>
		/// <param name="includeTrackChanges"> </param>
		/// <returns></returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool includeTrackChanges = false
			)
		{
			var tu = BuildLinguaTranslationUnit(lp, sp, paragraphProperties, stripTags, excludeTagsInLockedContentText, includeTrackChanges);
			SetTranslationUnitDocInfo(tu, documentProperties, fileProperties, sp);
			return tu;
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>        
		/// <param name="flags"></param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or 
		/// <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			LinguaTuBuilderSettings flags)
		{
			return BuildLinguaTranslationUnitInternal(lp, sp, paragraphProperties, flags.StripTags,
				 flags.ExcludeTagsInLockedContentText, flags.AcceptTrackChanges, flags.AlignTags, out _, flags.IncludeTrackChanges);
		}

		/// <summary>
		/// Converts a bilingual segment pair to a translation unit.
		/// </summary>
		/// <param name="lp">The segment pair's language direction (not null)</param>
		/// <param name="sp">The segment pair (not null)</param>
		/// <param name="paragraphProperties">The properties of the paragraph the segment appears in</param>
		/// <param name="stripTags">If <c>true</c>, tags will be stripped from the segment. The result will be a plain-text segment.</param>
		/// <param name="excludeTagsInLockedContentText">If <c>true</c>, tags which appear in locked content text will be excluded.</param>
		/// <param name="includeTrackChanges">If true, result will contain revision tags </param>
		/// <returns>A new translation unit which corresponds to <paramref name="sp"/>, or <c>null</c> if the source segment is empty.</returns>
		public static TranslationUnit BuildLinguaTranslationUnit(LanguagePair lp,
			ISegmentPair sp,
			IParagraphUnitProperties paragraphProperties,
			bool stripTags, bool excludeTagsInLockedContentText, bool includeTrackChanges = false)
		{
			var tu = BuildLinguaTranslationUnitInternal(
				lp, sp, paragraphProperties, stripTags,
				excludeTagsInLockedContentText, true, false, out _, includeTrackChanges);

			return tu;
		}

		/// <summary>
		/// return if the segment contains track changes
		/// </summary>
		/// <param name="data"></param>
		/// <param name="result"></param>
		/// <param name="flags"></param>
		/// <param name="tagAssociations"></param>
		/// <param name="textAssociations"></param>
		/// <returns></returns>
		private static bool AppendToLinguaSegment(IAbstractMarkupDataContainer data,
			Segment result, LinguaTuBuilderSettings flags,
			out List<KeyValuePair<Tag, IAbstractMarkupData>> tagAssociations,
			out List<KeyValuePair<Text, IAbstractMarkupData>> textAssociations
		   )
		{
			var builder = new LinguaSegmentBuilder(result, flags);
			builder.VisitChildNodes(data);
			tagAssociations = builder.TagAssociations;
			textAssociations = builder.TextAssociations;
			return builder.HasTrackChanges;
		}

		private static void SetTranslationUnitDocInfo(TranslationUnit tu,
			IDocumentProperties documentProperties, IFileProperties fileProperties, ISegmentPair sp)
		{
			if (tu == null) return;
			tu.DocumentProperties = documentProperties?.Clone() as IDocumentProperties;
			tu.FileProperties = fileProperties?.Clone() as IFileProperties;
			tu.DocumentSegmentPair = sp;
		}

	}
}
