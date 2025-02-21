using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Multilingual.Excel.FileType.Constants;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.Services.Entities
{
	public class EntityService
	{
		public EntityService(
			EntityContext entityContext,
			SdlFrameworkService sdlFrameworkService,
			EntityMarkerConversionService entityMarkerConversionService)
		{
			EntityContext = entityContext;
			FrameworkService = sdlFrameworkService;
			EntityMarkerConversionService = entityMarkerConversionService;
		}

		public SdlFrameworkService FrameworkService { get; }

		public EntityMarkerConversionService EntityMarkerConversionService { get; }

		public EntityContext EntityContext { get; }

		public string ConvertKnownCharactersToEntities(string text, bool useReaderMapping = false)
		{
			if (text == null)
			{
				return string.Empty;
			}

			var displayText = new StringBuilder();
			foreach (var character in text)
			{
				var encodedCharacter = EntityContext.ConvertCharacterToEntity(character, useReaderMapping);
				displayText.Append(encodedCharacter ?? character.ToString());
			}

			return displayText.ToString();
		}

		public IEnumerable<IAbstractMarkupData> ConvertKnownEntitiesInMarkupData(string textValue, EntityRule? ruleType)
		{
			textValue = UnEscapeXmlCharacters(textValue);
			textValue = MarkupKnownEntities(textValue);

			while (!string.IsNullOrEmpty(textValue))
			{
				var beginSdlEntityPosition = textValue.IndexOf(EntityConstants.BeginSdlEntityRefEscape, StringComparison.Ordinal);
				if (beginSdlEntityPosition != -1)
				{
					var fragment = textValue.Substring(0, beginSdlEntityPosition);
					fragment = EntityMarkerConversionService.BackwardEntityMarkersConversion(fragment);

					var text = FrameworkService.CreateText(fragment);
					if (text != null)
						yield return text;

					textValue = textValue.Remove(0, beginSdlEntityPosition + EntityConstants.BeginSdlEntityRefEscape.Length);
				}
				else
				{
					var fragment = textValue.Substring(0, textValue.Length);
					fragment = EntityMarkerConversionService.BackwardEntityMarkersConversion(fragment);

					var text = FrameworkService.CreateText(fragment);
					if (text != null)
						yield return text;

					textValue = string.Empty;
				}

				var endSdlEntityPosition = textValue.IndexOf(EntityConstants.EndSdlEntityRefEscape, StringComparison.Ordinal);
				if (endSdlEntityPosition != -1)
				{
					var entityName = textValue.Substring(0, endSdlEntityPosition);
					string symbolCharacter;

					if (IsNumericEntity(entityName))
					{
						yield return CreateMarkupForNumericEntity(entityName);
					}
					else if ((symbolCharacter = EntityContext.GetSymbolCharacter(entityName, ruleType)) != null)
						yield return FrameworkService.CreateText(symbolCharacter);
					else
						yield return CreateEntityPlaceholder(entityName);

					textValue = textValue.Remove(0, endSdlEntityPosition + EntityConstants.EndSdlEntityRefEscape.Length);
				}
			}
		}

		public void UnEscapeXmlCharacters(XmlNode node)
		{
			if (node.InnerText.Length > 0)
			{
				node.InnerText = UnEscapeXmlCharacters(node.InnerText);
			}

			if (node.HasChildNodes)
			{
				foreach (XmlNode childNode in node.ChildNodes)
				{
					UnEscapeXmlCharacters(childNode);
				}
			}
		}

		public string UnEscapeXmlCharacters(string textValue)
		{
			if (string.IsNullOrEmpty(textValue))
			{
				return textValue;
			}
			textValue = textValue.Replace(EntityConstants.AmpersandEntityRefEscape, EntityConstants.AmpersandEntityRef);
			textValue = textValue.Replace(EntityConstants.LessThanEntityRefEscape, EntityConstants.LessThanEntityRef);
			textValue = textValue.Replace(EntityConstants.GreaterThanEntityRefEscape, EntityConstants.GreaterThanEntityRef);

			return textValue;
		}

		public string EscapeXmlCharacters(string textValue)
		{
			textValue = Regex.Replace(textValue, "&(?!amp;)", EntityConstants.AmpersandEntityRefEscape);
			textValue = Regex.Replace(textValue, @"<(?=\s)", EntityConstants.LessThanEntityRefEscape);
			textValue = Regex.Replace(textValue, @"(?<=\s)>", EntityConstants.GreaterThanEntityRefEscape);

			return textValue;
		}

		public string MarkupSelfClosingHtmlTags(string textValue)
		{
			var htmlSelfClosingTags = new Regex(@"\<\s*(br)\s*\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			textValue = htmlSelfClosingTags.Replace(textValue, "<$1/>");

			return textValue;
		}

		public string MarkupKnownEntities(string textValue)
		{
			if (string.IsNullOrEmpty(textValue))
			{
				return textValue;
			}

			var regex = new Regex(@"(?<prefix>&amp;|&)(?<code>[^ ;]+)(?<suffix>;)", RegexOptions.None);

			var result = regex.Replace(textValue, MarkupEntities);

			return result;
		}

		private string MarkupEntities(Match match)
		{
			var code = match.Groups["code"].Value;

			return EntityConstants.BeginSdlEntityRefEscape + code + EntityConstants.EndSdlEntityRefEscape;
		}

		private IAbstractMarkupData CreateMarkupForNumericEntity(string entityName)
		{
			if (EntityContext.ConvertNumericEntitiesToPlaceholder)
			{
				return CreateEntityPlaceholder(entityName);
			}

			var character = EntityContext.GetCharacterByIndex(entityName.Substring(1));
			return FrameworkService.CreateText(character);
		}

		private bool IsNumericEntity(string entityName)
		{
			return entityName.StartsWith(EntityConstants.NumericEntityMarker);
		}

		private IAbstractMarkupData CreateEntityPlaceholder(string text)
		{
			var placeholder = FrameworkService.CreatePlaceholderTag();
			placeholder.Properties.TagContent = $"&{text};";
			placeholder.Properties.DisplayText = $"&{text};";

			placeholder.Properties.SetMetaData(XmlConstants.TagTypeMetaKey, TagType.Entity.ToString());

			return placeholder;
		}

		public string ConvertKnownEntitiesInText(string textValue, EntityRule ruleType)
		{
			var encodedText = new StringBuilder();

			var fragments = ConvertKnownEntitiesInMarkupData(textValue, ruleType);
			foreach (var fragment in fragments)
			{
				if (fragment is IText textFragment)
				{
					encodedText.Append(textFragment.Properties.Text);
				}
				else
				{
					encodedText.Append(fragment);
				}
			}

			return encodedText.ToString();
		}

		public string ConvertEntityPlaceholderToText(string entityCode)
		{
			if (entityCode.Length <= 1)
			{
				return entityCode;
			}

			var entityName = entityCode.Substring(1, entityCode.Length - 2);
			var symbol = EntityContext.GetSymbolCharacter(entityName, EntityRule.Writer);

			return symbol ?? entityCode;
		}
	}
}
