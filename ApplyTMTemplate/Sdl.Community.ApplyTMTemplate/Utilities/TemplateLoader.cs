using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Sdl.Community.ApplyTMTemplate.Models;
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
				message = PluginResources.Select_A_Template;
				return true;
			}

			if (!File.Exists(resourceTemplatePath))
			{
				message = PluginResources.Template_filePath_Not_Correct;
				return true;
			}

			if (Path.GetExtension(resourceTemplatePath) != ".resource")
			{
				message = PluginResources.The_file_is_not_of_the_required_type +  @"""resource""";
				return true;
			}

			lrt = LoadDataFromFile(resourceTemplatePath, "LanguageResource");

			if (lrt.Count == 0)
			{
				message = PluginResources.Template_corrupted_or_file_not_template;
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
			var allResourceTypes = new List<string>() { "Variables" , "Abbreviations", "OrdinalFollowers" };
			
			var resourceAdder = new Resource();
			foreach (var resourceType in allResourceTypes)
			{
				if (resourceType == resource?.Attributes?["Type"].Value)
				{
					resourceAdder.SetResourceType(new WordlistResource(resource, resourceType));
					resourceAdder.AddLanguageResourceToBundle(langResBundle);
				}
			}
			
			if (resource?.Attributes?["Type"].Value == "SegmentationRules")
			{
				resourceAdder.SetResourceType(new SegmentationRulesResource(resource));
				resourceAdder.AddLanguageResourceToBundle(langResBundle);
			}
		}
	}
}