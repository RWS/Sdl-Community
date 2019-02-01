using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TemplateLoader
	{
		public string GetTmTemplateFolderPath()
		{
			var data =
				LoadDataFromFile(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						@"SDL\SDL Trados Studio\15.0.0.0\UserSettings.xml"), "Setting");

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentLanguageResourceGroupFolder")
				{
					return setting.InnerText;
				}
			}

			return null;
		}

		public string GetTmFolderPath()
		{
			var data =
				LoadDataFromFile(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						@"SDL\SDL Trados Studio\15.0.0.0\UserSettings.xml"), "Setting");

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentTranslationMemoryFolder")
				{
					return setting.InnerText;
				}
			}

			return null;
		}

		public List<LanguageResourceBundle> GetLanguageResourceBundlesFromFile(string resourceTemplatePath)
		{
			if (string.IsNullOrEmpty(resourceTemplatePath))
			{
				MessageBox.Show("Please select a Resource Template", "Resource Template", MessageBoxButtons.OK);
				return null;
			}

			var lrt = LoadDataFromFile(resourceTemplatePath, "LanguageResource");

			var langResBundlesList = new List<LanguageResourceBundle>();
			var defaultLangResProvider = new DefaultLanguageResourceProvider();

			foreach (XmlNode res in lrt)
			{
				var lr = langResBundlesList.FirstOrDefault(lrb => lrb.Language.LCID == int.Parse(res.Attributes["Lcid"].Value));

				if (lr == null)
				{
					lr = defaultLangResProvider.GetDefaultLanguageResources(CultureInfo.GetCultureInfo(int.Parse(res.Attributes["Lcid"].Value)));
					langResBundlesList.Add(lr);
				}

				AddLanguageResourceToBundle(lr, res);
			}

			return langResBundlesList;
		}

		private void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle, XmlNode resource)
		{
			if (resource.Attributes["Type"].Value == "Variables")
			{
				var vars = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.Variables = new Wordlist();

				foreach (Match s in Regex.Matches(vars, @"([^\s]+)"))
				{
					langResBundle.Variables.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "Abbreviations")
			{
				var abbrevs = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.Abbreviations = new Wordlist();

				foreach (Match s in Regex.Matches(abbrevs, @"([^\s]+)"))
				{
					langResBundle.Abbreviations.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "OrdinalFollowers")
			{
				var ordFollowers = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.OrdinalFollowers = new Wordlist();

				foreach (Match s in Regex.Matches(ordFollowers, @"([^\s]+)"))
				{
					langResBundle.OrdinalFollowers.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "SegmentationRules")
			{
				var segRules = Convert.FromBase64String(resource.InnerText);

				var stream = new MemoryStream(segRules);

				var segmentRules = SegmentationRules.Load(stream,
					CultureInfo.GetCultureInfo(langResBundle.Language.LCID), null);

				langResBundle.SegmentationRules = segmentRules;
			}
		}

		public XmlNodeList LoadDataFromFile(string filePath, string element)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);
			var data = doc.GetElementsByTagName(element);

			return data;
		}
	}
}