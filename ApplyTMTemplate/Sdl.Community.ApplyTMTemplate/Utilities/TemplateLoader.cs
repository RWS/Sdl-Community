using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TemplateLoader
	{
		private readonly string _path;
		private readonly Version _studioVersion;

		public TemplateLoader()
		{
			_studioVersion = new Toolkit.Core.Studio().GetStudioVersion().ExecutableVersion;

			if (_studioVersion.Major == 15)
			{
				_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					@"SDL\SDL Trados Studio\15.0.0.0\UserSettings.xml");

				DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Studio 2019");
			}
			else
			{
				_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					@"SDL\SDL Trados Studio\14.0.0.0\UserSettings.xml");

				DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Studio 2017");
			}
		}

		public string DefaultPath { get; set; }

		public string GetTmFolderPath()
		{
			var data = LoadDataFromFile(_path, "Setting");

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentTranslationMemoryFolder")
				{
					return setting?.InnerText;
				}
			}

			return DefaultPath;
		}

		public List<LanguageResourceBundle> GetLanguageResourceBundlesFromFile(string resourceTemplatePath, out string message, out List<int> unIDedLanguages)
		{
			if (ValidateFile(resourceTemplatePath, out message, out unIDedLanguages, out var lrt)) return null;

			var langResBundlesList = new List<LanguageResourceBundle>();

			foreach (XmlNode res in lrt)
			{
				var successful = int.TryParse(res?.Attributes?["Lcid"]?.Value, out var lcid);

				if (!successful) continue;
				var lr = langResBundlesList.FirstOrDefault(lrb => lrb.Language.LCID == lcid);

				if (lr == null)
				{
					CultureInfo culture;

					try
					{
						culture = CultureInfoExtensions.GetCultureInfo(lcid);
						if (CultureInfo.GetCultures(CultureTypes.AllCultures).Where(ci => ci.LCID == lcid).ToList().Count > 1) throw new Exception();
					}
					catch (Exception)
					{
						if (!unIDedLanguages.Exists(id => id == lcid))
						{
							unIDedLanguages.Add(lcid);
						}
						continue;
					}

					lr = new LanguageResourceBundle(culture);
					langResBundlesList.Add(lr);
				}

				AddLanguageResourceToBundle(lr, res);
			}

			return langResBundlesList;
		}

		private bool ValidateFile(string resourceTemplatePath, out string message, out List<int> unIDedLanguages, out XmlNodeList lrt)
		{
			message = "";
			unIDedLanguages = new List<int>();
			lrt = null;

			if (string.IsNullOrEmpty(resourceTemplatePath))
			{
				message = "Select a template";
				return true;
			}

			if (!File.Exists(resourceTemplatePath))
			{
				message = "The file path of the template is not correct!";
				return true;
			}

			if (Path.GetExtension(resourceTemplatePath) != ".resource")
			{
				message = @"The file is not of the required type, ""resource""";
				return true;
			}

			lrt = LoadDataFromFile(resourceTemplatePath, "LanguageResource");

			if (lrt.Count == 0)
			{
				message = "This template is corrupted or the file is not a template";
			}

			return false;
		}

		public XmlNodeList LoadDataFromFile(string filePath, string element)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);
			var data = doc.GetElementsByTagName(element);

			return data;
		}

		private void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle, XmlNode resource)
		{
			switch (resource?.Attributes?["Type"].Value)
			{
				case "Variables":
					{
						var vars = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

						langResBundle.Variables = new Wordlist();

						foreach (Match s in Regex.Matches(vars, @"([^\s]+)"))
						{
							langResBundle.Variables.Add(s.ToString());
						}

						return;
					}
				case "Abbreviations":
					{
						var abbrevs = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

						langResBundle.Abbreviations = new Wordlist();

						foreach (Match s in Regex.Matches(abbrevs, @"([^\s]+)"))
						{
							langResBundle.Abbreviations.Add(s.ToString());
						}

						return;
					}
				case "OrdinalFollowers":
					{
						var ordFollowers = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

						langResBundle.OrdinalFollowers = new Wordlist();

						foreach (Match s in Regex.Matches(ordFollowers, @"([^\s]+)"))
						{
							langResBundle.OrdinalFollowers.Add(s.ToString());
						}

						return;
					}
				case "SegmentationRules":
					{
						var segRules = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

						var xmlSerializer = new XmlSerializer(typeof(SegmentationRules));

						var stringReader = new StringReader(segRules);
						var segmentRules = (SegmentationRules)xmlSerializer.Deserialize(stringReader);

						langResBundle.SegmentationRules = segmentRules;
						break;
					}
			}
		}
	}
}