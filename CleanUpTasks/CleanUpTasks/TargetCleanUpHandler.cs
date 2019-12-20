using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks
{
	public class TargetCleanUpHandler : SegmentHandlerBase, ISegmentHandler
	{
		private readonly List<IPlaceholderTag> phTags = new List<IPlaceholderTag>();
		private readonly ICleanUpSourceSettings settings = null;
		private readonly List<ITagPair> tagPairs = new List<ITagPair>();

		public TargetCleanUpHandler(ICleanUpSourceSettings settings,
			IDocumentItemFactory itemFactory,
			ICleanUpMessageReporter reporter)
			: base(itemFactory, reporter)
		{
			this.settings = settings;
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			var tagContent = tag.TagProperties.TagContent;

			if (tagContent != null)
			{
				phTags.Add(tag);
			}
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
			ProcessPlaceholderTags();
			ProcessTagPairs();
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			var tagContent = tagPair.StartTagProperties.TagContent;

			if (tagContent != null)
			{
				// Remove spacing before performing comparison
				if (settings.Placeholders.Any(p => ComparePlaceholders(p, tagContent)))
				{
					tagPairs.Add(tagPair);
				}
			}

			VisitChildren(tagPair);
		}

		private static bool ComparePlaceholders(Placeholder p, string tagContent)
		{
			// Removes all spacing and single and double quotations before doing a comparison
			return Regex.Replace(p.Content, @"\s", "").Replace("\"", "").Replace("'", "")
				   == Regex.Replace(tagContent, @"\s", "").Replace("\"", "").Replace("'", "");
		}

		private bool IsTag(string text)
		{
			return text.Contains('<') && text.Contains('>');
		}

		private string ConvertTagToText(IPlaceholderTag phTag)
		{
			var content = phTag.TagProperties.TagContent;

			var text = string.Empty;
			if (!IsTag(content) && Regex.IsMatch(content, "(?:>|≤|<|≥)\\d+"))
			{
				text = content;
			}
			else
			{
				var parser = new HtmlParser(content);
				HtmlTag tag;
				if (parser.ParseNext("*", out tag))
				{
					if (tag.Attributes.Any())
					{
						var isTagPair = settings.Placeholders.First(p => Regex.Replace(p.Content, @"\s+", "") == Regex.Replace(content, @"\s+", "")).IsTagPair;
						if (isTagPair)
						{
							text = content;
						}
						else
						{
							text = tag.Attributes.Values.First();
						}
					}
					else
					{
						if (tag.HasEndTag)
						{
							text = $"</{tag.Name}>";
						}
						else if (tag.TrailingSlash)
						{
							// Check if there is a space before the trailing slash
							if (tag.SpaceBeforeTrailingSlash)
							{
								text = $"<{tag.Name} />";
							}
							else
							{
								text = $"<{tag.Name}/>";
							}
						}
						else
						{
							text = $"<{tag.Name}>";
						}
					}
				}
			}

			return text;
		}

		private void ProcessPlaceholderTags()
		{
			foreach (var tag in phTags)
			{
				var text = ConvertTagToText(tag);

				if (!string.IsNullOrEmpty(text))
				{
					var parent = tag.Parent;
					var index = tag.IndexInParent;

					var itext = CreateIText(text);

					tag.RemoveFromParent();

					parent.Insert(index++, itext);
				}
			}
		}

		private void ProcessTagPairs()
		{
			foreach (var pair in tagPairs)
			{
				var startTag = CreateIText(pair.StartTagProperties.TagContent);
				var endTag = CreateIText(pair.EndTagProperties.TagContent);
				var parent = pair.Parent;

				var index = pair.IndexInParent;

				var children = pair.ToList();
				children.Insert(0, startTag);
				children.Add(endTag);

				foreach (var item in children)
				{
					if (item.Parent != null)
					{
						item.RemoveFromParent();
					}

					if (index >= 0)
					{
						parent.Insert(index++, item);
					}
				}

				pair.RemoveFromParent();
			}
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		#region Not Used

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
		}

		public void VisitText(IText text)
		{
		}

		#endregion Not Used
	}
}