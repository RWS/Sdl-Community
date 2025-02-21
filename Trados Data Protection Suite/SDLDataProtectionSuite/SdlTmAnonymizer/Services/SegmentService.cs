using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class SegmentService
	{
		private XmlSerializer _segmentSerializer;

		public Segment BuildSegment(System.Globalization.CultureInfo culture, params object[] elements)
		{
			var segment = new Segment(culture);

			foreach (var element in elements)
			{
				switch (element)
				{
					case string _:
						{
							segment.Add(new Text(element as string));
							break;
						}
					case Tag _:
						{
							segment.Add(element as Tag);
							break;
						}
					case SegmentElement _:
						{
							segment.Add(element as SegmentElement);
							break;
						}
					default:
						{
							throw new InvalidOperationException("Unexpected parameter");
						}
				}
			}

			return segment;
		}

		public Segment BuildSegment(System.Globalization.CultureInfo culture, List<SegmentElement> elements)
		{	
			var segment = new Segment(culture);

			foreach (var element in elements)
			{
				segment.Add(element);
			}

			return segment;
		}


		public string MakeString(IAbstractMarkupDataContainer container)
		{
			// iterate through the segment and concatenate textual elements and tags
			var sb = new StringBuilder();

			foreach (var item in container)
			{
				switch (item)
				{
					case IText txt:
						{
							sb.Append(txt.Properties.Text);
							break;
						}
					case ITagPair tagPair:
						{
							sb.Append(tagPair.StartTagProperties.TagContent);
							sb.Append(MakeString(tagPair));
							sb.Append(tagPair.EndTagProperties.TagContent);
							break;
						}
					case IAbstractTag tag:
						{
							sb.Append(tag.TagProperties.TagContent);
							break;
						}
					case IAbstractMarkupDataContainer subcontainer:
						{
							sb.Append(MakeString(subcontainer));
							break;
						}
				}

				// if we get here we encountered an unexpected element type
				sb.Append(item);
			}

			return sb.ToString();
		}

		public Segment DeserializeSegment(string databaseString)
		{
			if (_segmentSerializer == null)
			{
				_segmentSerializer = new XmlSerializer(typeof(Segment));
			}

			using (var rdr = new StringReader(databaseString))
			{
				var settings = new XmlReaderSettings
				{
					CheckCharacters = false
				};

				var xmlRdr = XmlReader.Create(rdr, settings);

				if (_segmentSerializer.Deserialize(xmlRdr) is Segment result)
				{
					return !result.IsValid()
						? throw new LanguagePlatformException(ErrorCode.DAInvalidSegmentAfterDeserialization)
						: result;
				}
			}

			return null;
		}

		public string SerializeSegment(Segment segment)
		{
			if (segment == null)
			{
				throw new ArgumentNullException(nameof(segment));
			}

			if (!segment.IsValid())
			{
				throw new LanguagePlatformException(ErrorCode.InvalidSegment);
			}

			if (_segmentSerializer == null)
			{
				_segmentSerializer = new XmlSerializer(typeof(Segment));
			}

			var settings = new XmlWriterSettings
			{
				Indent = false,
				NewLineOnAttributes = false,
				OmitXmlDeclaration = true,
				CheckCharacters = false,
				NewLineHandling = NewLineHandling.Entitize
			};

			var sb = new StringBuilder();
			var wtr = XmlWriter.Create(sb, settings);

			_segmentSerializer.Serialize(wtr, segment);

			wtr.Flush();

			return sb.ToString();
		}
	}
}
