using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class UsefulTipsService
	{
		private readonly XmlRootAttribute _tipsRoot;
		private readonly TipsProvider _tipsProvider;
		private readonly Model.PathInfo _pluginPathInfo;

		public UsefulTipsService(TipsProvider tipsProvider, Model.PathInfo pluginPathInfo)
		{
			_tipsRoot = new XmlRootAttribute("Tips");
			_tipsProvider = tipsProvider;
			_pluginPathInfo = pluginPathInfo;
		}

		public int AddUsefulTips(ImportTips importTips, bool overwrite)
		{
			return _tipsProvider.ImportTips(importTips, overwrite);
		}

		public int RemoveUsefulTips(RemoveTips removeContext)
		{
			return _tipsProvider.RemoveTips(removeContext);
		}

		public List<TipLanguage> GetPluginUsefulTips()
		{
			var tipContexts = new List<TipLanguage>();
			foreach (var language in _tipsProvider.SupportedLanguages)
			{
				var tipsLanguageFullPath = GetTipsLanguagePath(language);
				var resources = GetImportResources(tipsLanguageFullPath, language);
				if (resources == null)
				{
					continue;
				}

				var tipsImportFile = Path.Combine(tipsLanguageFullPath, "TipsImport.xml");
				var tips = ReadTipsImportFile(tipsImportFile);

				if (tips?.Count > 0)
				{
					// remove the tips import xml file from the list of resources
					RemoveResourceFromList(resources, tipsImportFile);

					tipContexts.Add(new TipLanguage
					{
						LanguageId = language,
						Tips = tips,
						Resources = resources
					});
				}
			}

			return tipContexts;
		}

		public Tip GetExistingTip(IEnumerable<Tip> tips, Tip tip)
		{
			var content = Path.GetFileName(tip.Content);
			var existingTip = tips?.FirstOrDefault(a =>
				a.Category.Equals(tip.Category, StringComparison.InvariantCultureIgnoreCase) &&
				a.Context.Equals(tip.Context, StringComparison.InvariantCultureIgnoreCase) &&
				string.Compare(Path.GetFileName(a.Content), content, StringComparison.InvariantCultureIgnoreCase) == 0);

			return existingTip;
		}

		private static List<Tip> GetTips(string filePath, XmlSerializer serializer)
		{
			using (var fileStream = new StreamReader(filePath, Encoding.UTF8, true))
			{
				return (List<Tip>)serializer.Deserialize(fileStream);
			}
		}

		private string GetTipsLanguagePath(string languageId)
		{
			var tipsLanguageFullPath = Path.Combine(_pluginPathInfo.TipsFullPath, languageId);
			if (!Directory.Exists(tipsLanguageFullPath))
			{
				Directory.CreateDirectory(tipsLanguageFullPath);
			}

			return tipsLanguageFullPath;
		}

		private List<ResourceFile> GetImportResources(string tipsLanguageFullPath, string languageId)
		{

			var resourceFiles = new List<ResourceFile>();

			RemoveAllFilesInPath(tipsLanguageFullPath);

			var tipsOutputFile = Path.Combine(tipsLanguageFullPath, languageId + ".zip");
			var success = WriteEmbeddedResourceToFile(languageId, tipsOutputFile);
			if (success)
			{
				try
				{
					ZipFile.ExtractToDirectory(tipsOutputFile, tipsLanguageFullPath);

					resourceFiles = GetResourceFiles(tipsLanguageFullPath);

					// remove the zip file from the list of resources
					RemoveResourceFromList(resourceFiles, tipsOutputFile);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					//ignore; catch all
				}
			}

			return resourceFiles;
		}

		private static List<ResourceFile> GetResourceFiles(string path)
		{
			var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
			var resourceFiles = new List<ResourceFile>();
			foreach (var file in files)
			{
				var resourceFile = new ResourceFile
				{
					FullPath = file,
					RelativePath = file.Substring(path.Length + 1)
				};
				resourceFiles.Add(resourceFile);
			}

			return resourceFiles;
		}

		private static void RemoveResourceFromList(ICollection<ResourceFile> resources, string resource)
		{
			var contentFile = resources.FirstOrDefault(a => string.Compare(a.FullPath, resource, StringComparison.InvariantCultureIgnoreCase) == 0);
			if (contentFile != null)
			{
				resources.Remove(contentFile);
			}
		}

		private bool WriteEmbeddedResourceToFile(string languageId, string tipsContentFileOutput)
		{
			var tipsEmbeddedResource = typeof(PluginResources).Namespace + ".SdlTmAnonymizer" + ".UsefulTips." + languageId + "." + languageId + ".zip";
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tipsEmbeddedResource))
			{
				if (stream == null)
				{
					return false;
				}

				using (var reader = new BinaryReader(stream))
				{
					using (Stream writer = File.Create(tipsContentFileOutput))
					{
						var buffer = new byte[2048];
						while (true)
						{
							var current = reader.Read(buffer, 0, buffer.Length);
							if (current == 0)
							{
								break;
							}

							writer.Write(buffer, 0, current);
						}
					}
				}
			}

			return true;
		}

		private void RemoveAllFilesInPath(string tipsLanguageFullPath)
		{
			try
			{
				if (string.IsNullOrEmpty(tipsLanguageFullPath?.Trim()))
				{
					return;
				}

				var files = Directory.GetFiles(tipsLanguageFullPath, "*.*", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					File.Delete(file);
				}

				var directories = Directory.GetDirectories(tipsLanguageFullPath, "*", SearchOption.AllDirectories);
				foreach (var directory in directories)
				{
					Directory.Delete(directory);
				}
			}
			catch
			{
				//ignore; catch all
			}
		}

		private List<Tip> ReadTipsImportFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}

			try
			{
				var serializer = new XmlSerializer(typeof(List<Tip>), _tipsRoot);
				return GetTips(filePath, serializer);
			}
			catch
			{
				try
				{
					var serializer = new XmlSerializer(typeof(List<Tip>));
					return GetTips(filePath, serializer);
				}
				catch (Exception ex1)
				{
					Trace.WriteLine(ex1);
				}

				return null;
			}
		}
	}
}
