using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.Anonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class SegmentVisitor: IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private ISegment _segment;
		private IPropertiesFactory _propertiesFactory;
		//	private List<string> _patterns = new List<string>{ @"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)", @"\b(?:\d[ -]*?){13,16}\b" };
		private List<string> _patterns = new List<string> { @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", @"\b(?:\d[ -]*?){13,16}\b" };

		public void ReplaceText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_segment = segment;
			_propertiesFactory = propertiesFactory;
			VisitChildren(segment);
		}

		private string Anonymizer(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var result = regex.Replace(text, new MatchEvaluator(Process));
				if (!string.IsNullOrWhiteSpace(result))
				{
					return result;
				}
			}
			return string.Empty;
		}

		private static string Process(Match match)
		{
			var encryptedText = AnonymizeData.EncryptData(match.ToString(), "Andrea");
			return string.Concat("{", encryptedText, "}");

		}

		private bool ShouldAnonymize(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var match = regex.Match(text);
				if (match.Success)
				{
					return true;
				}
			}
			return false;
		}

		private List<AnonymizedData> GetAnonymizedData(string segmentText)
		{
			var anonymizedData = new List<AnonymizedData>();
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var matches = regex.Matches(segmentText);
				foreach (Match match in matches)
				{
					var data = new AnonymizedData
					{
						MatchText = match.Value,
						PositionInOriginalText = match.Index,
						EncryptedText = AnonymizeData.EncryptData(match.ToString(), "Andrea")
					};
					anonymizedData.Add(data);
				}
			}
			return anonymizedData;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				var anonymizedText = Anonymizer(tagPair.StartTagProperties.TagContent);
				tagPair.StartTagProperties.TagContent = anonymizedText;
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			var markUpCollection = new List<IAbstractMarkupData>();
			var subSegmentText = new List<IText>();
			var shouldAnonymize = ShouldAnonymize(text.Properties.Text);
			var indexInParent = text.IndexInParent;
			var originalSegmentClone = text.Clone();
			if (shouldAnonymize)
			{
				var anonymizedData = GetAnonymizedData(text.Properties.Text);

				//Trebuie facuta o metoda recurenta
				//if (anonymizedData.Count > 1)
				//{
					
				//}

				for (var i = 0; i < anonymizedData.Count; i++)
				{
					//that means is first time we split the original text
					if (subSegmentText.Count.Equals(0))
					{
						if (anonymizedData[i].PositionInOriginalText.Equals(0))
						{
							var subSegment = text.Split(anonymizedData[i].MatchText.Length);
							var tag = _factory.CreatePlaceholderTag(
								_propertiesFactory.CreatePlaceholderTagProperties(Anonymizer(text.Properties.Text)));
							//Add encrypted tag to collection
							markUpCollection.Add(tag);

							subSegmentText.Add(subSegment);
						}
						else
						{
							var subSegment = text.Split(anonymizedData[i].PositionInOriginalText);
							//da eroare daca il adaugam la colectie se pare ca in container deja e cuvantul
							if (!ShouldAnonymize(text.Properties.Text))
							{
								markUpCollection.Add(text);
							}
							if (ShouldAnonymize(subSegment.Properties.Text))
							{
								var remainingData = GetAnonymizedData(subSegment.Properties.Text);
								//Trebuie facut cu for
								var tag = _factory.CreatePlaceholderTag(
									_propertiesFactory.CreatePlaceholderTagProperties(Anonymizer(remainingData[0].EncryptedText)));
								//Add encrypted tag to collection
								markUpCollection.Add(tag);
								//for (int j = 0; j < remainingData.Count; j++)
								//{
								//	//trebuie vazut
								//}
							}
							else
							{
								//Add encrypted tag to collection
								markUpCollection.Add(subSegment);
							}
							

						}//else need to be implemented
					}
					else
					{
						var remainingData = GetAnonymizedData(subSegmentText[i - 1].Properties.Text);
						for (int j = 0; j < remainingData.Count; j++)
						{
							var subSegment = subSegmentText[i - 1].Split(remainingData[j].PositionInOriginalText);
							if (!ShouldAnonymize(subSegmentText[i - 1].Properties.Text))
							{
								markUpCollection.Add(subSegmentText[i - 1]);
							}//else need to me implemented
							var tag = _factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(Anonymizer(subSegment.Properties.Text)));
							//Add encrypted tag to collection
							markUpCollection.Add(tag);

						}
					}
				}

				var abstractMarkupData = text.Parent.AllSubItems.FirstOrDefault(n => n.Equals(originalSegmentClone));
				if (abstractMarkupData != null)
				{
					var elementContainer = abstractMarkupData.Parent;
					//aici ar trebui facut cu for, daca elementul deja exista ordinea in care pune datele e gresita
					foreach (var markupData in markUpCollection)
					{
						if (!elementContainer.Contains(markupData))
						{
							elementContainer.Add(markupData);
						}
						
					}
					elementContainer.RemoveAt(indexInParent);
				}
			}
		}


		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			
		}
		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
				return;
			foreach (var item in container.ToList())
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
