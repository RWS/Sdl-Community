using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Multilingual.XML.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.FileType.Processors
{
	public class PlaceholdersProcessor : AbstractBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly BilingualParser _parser;

		public PlaceholdersProcessor(SegmentBuilder segmentBuilder, BilingualParser parser)
		{
			_segmentBuilder = segmentBuilder;
			_parser = parser;
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			Output.Initialize(documentInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				Output.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			if (_parser.PlaceholderPatternSettings.PlaceablePatternsProcess && _parser.PlaceholderPatternSettings.PlaceablePatterns.Count(a => a.Selected) > 0)
			{
				if (paragraphUnit.SegmentPairs.Any())
				{
					foreach (var segmentPair in paragraphUnit.SegmentPairs)
					{
						var sourceContainer = segmentPair.Source.Clone() as ISegment;
						sourceContainer?.Clear();

						UpdateSourceSegment(sourceContainer, segmentPair.Source);
					}
				}
				else
				{
					var sourceContainer = paragraphUnit.Source.Clone() as IParagraph;
					sourceContainer?.Clear();

					UpdateSourceSegment(sourceContainer, paragraphUnit.Source);
				}
			}

			Output.ProcessParagraphUnit(paragraphUnit);
		}

		private void UpdateSourceSegment(IAbstractMarkupDataContainer sourceContainer, IAbstractMarkupDataContainer origSourceContainer)
		{
			var containerStack = new Stack<IAbstractMarkupDataContainer>();
			containerStack.Push(sourceContainer);

			ProcessItems(origSourceContainer, ref containerStack);

			var updatedContainer = containerStack.FirstOrDefault();
			if (updatedContainer != null)
			{
				origSourceContainer.Clear();
				foreach (var item in updatedContainer)
				{
					origSourceContainer.Add(item.Clone() as IAbstractMarkupData);
				}
			}
		}

		private void ProcessItems(IAbstractMarkupDataContainer sourceContainer, ref Stack<IAbstractMarkupDataContainer> containers)
		{
			foreach (var item in sourceContainer)
			{
				var container = containers.Peek();

				if (item is ITagPair tagPair)
				{
					var startTagProperties = tagPair.StartTagProperties;
					var endTagProperties = tagPair.EndTagProperties;

					var newTagPair = _segmentBuilder.CreateTagPair(startTagProperties, endTagProperties);

					container.Add(newTagPair);
					containers.Push(newTagPair);

					ProcessItems(tagPair, ref containers);

					containers.Pop();
				}
				else if (item is IText text)
				{
					var abstractMarkupData = new List<IAbstractMarkupData>
					{
						text.Clone() as IAbstractMarkupData
					};

					foreach (var placeholderPattern in _parser.PlaceholderPatternSettings.PlaceablePatterns.Where(a => a.Selected))
					{
						var regex = new Regex(placeholderPattern.Pattern, RegexOptions.IgnoreCase);
						abstractMarkupData = GetMarkupData(abstractMarkupData, placeholderPattern.SegmentationHint, regex);
					}

					foreach (var markupData in abstractMarkupData)
					{
						container.Add(markupData);
					}
				}
				else
				{
					container.Add(item.Clone() as IAbstractMarkupData);
				}
			}
		}

		private List<IAbstractMarkupData> GetMarkupData(IEnumerable<IAbstractMarkupData> abstractMarkupData, SegmentationHint segmentationHint, Regex regex)
		{
			var newAbstractMarkupData = new List<IAbstractMarkupData>();
			foreach (var markupData in abstractMarkupData)
			{
				if (markupData is IText markupText)
				{
					var matches = regex.Matches(markupText.Properties.Text);
					if (matches.Count > 0)
					{
						var startIndex = 0;
						foreach (Match match in matches)
						{
							var prefix = markupText.Properties.Text.Substring(startIndex, match.Index - startIndex);
							if (!string.IsNullOrEmpty(prefix))
							{
								newAbstractMarkupData.Add(_segmentBuilder.Text(prefix));
							}

							var placeholder = markupText.Properties.Text.Substring(match.Index, match.Length);
							newAbstractMarkupData.Add(_segmentBuilder.CreatePlaceholderTag(placeholder, false, segmentationHint));

							startIndex = match.Index + match.Length;
						}

						if (startIndex < markupText.Properties.Text.Length)
						{
							var suffix = markupText.Properties.Text.Substring(startIndex);
							newAbstractMarkupData.Add(_segmentBuilder.Text(suffix));
						}
					}
					else
					{
						newAbstractMarkupData.Add(markupData.Clone() as IAbstractMarkupData);
					}
				}
				else
				{
					newAbstractMarkupData.Add(markupData.Clone() as IAbstractMarkupData);
				}
			}

			return newAbstractMarkupData;
		}
	}
}
