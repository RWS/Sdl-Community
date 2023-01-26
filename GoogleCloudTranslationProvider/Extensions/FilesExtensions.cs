using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Extensions
{
	public static class FilesExtensions
	{
		public static string ShortenFilePath(this string filePath)
		{
			var lastDirectory = Path.GetFileName(Path.GetDirectoryName(filePath));
			var fileName = Path.GetFileName(filePath);
			var output = $@"...\{lastDirectory}\{fileName}";
			return output;
		}

		public static (bool Success, string ErrorMessage) OpenFolderAndSelectFile(this string filePath)
		{
			var (success, errorMessage) = filePath.IsValidPath();
			if (!success)
			{
				return (false, errorMessage);
			}

			var pidl = ILCreateFromPathW(filePath);
			SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
			ILFree(pidl);
			return (true, null);
		}

		public static (bool Success, object OperationResult) VerifyPathAndReadJsonFile(this string filePath)
		{
			var (success, errorMessage) = filePath.IsValidPath();
			if (!success)
			{
				return (success, errorMessage);
			}

			return filePath.ReadJsonFileOnPath();
		}

		public static (bool Success, string ErrorMessage) VerifyAndDownloadJsonFile(this string uri, string filePath, string fileName)
		{
			if (string.IsNullOrEmpty(uri))
			{
				return (false, PluginResources.FileValidation_EmptyUri);
			}

			uri = uri.EnsureUriIsValid();
			if (!OnlineJsonFileIsValid(uri))
			{
				return (false, PluginResources.FileValidation_MissingJsonFile);
			}

			try
			{
				using var webClient = new WebClient();
				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}

				var fullPath = Path.Combine(filePath, fileName.EndsWith(".json") ? fileName : $"{fileName}.json");
				if (File.Exists(fullPath))
				{
					File.Delete(fullPath);
				}

				webClient.DownloadFile(uri, fullPath);
				return (true, string.Empty);
			}
			catch
			{
				return (false, PluginResources.FileValidation_DownloadFailed);
			}
		}

		public static bool OnlineJsonFileIsValid(this string uri)
		{
			if (string.IsNullOrEmpty(uri))
			{
				return false;
			}

			try
			{
				using var webClient = new WebClient();
				var downloadedContent = webClient.DownloadString(new Uri(uri));
				return TryReadJsonContent(downloadedContent).Success;
			}
			catch
			{
				return false;
			}
		}

		private static string EnsureUriIsValid(this string uri)
		{
			try
			{
				var targetUri = new Uri(uri);
				if (targetUri.Host.Contains("dropbox") && targetUri.Query != "?dl=1")
				{
					uri = uri.Substring(0, uri.Length - targetUri.Query.Length) + "?dl=1";
				}
				else if (targetUri.Host.Contains("dropbox"))
				{


				}

				_ = new Uri(uri);
			}
			catch { }

			return uri;
		}

		private static (bool Success, object OperationResult) ReadJsonFileOnPath(this string filePath)
		{
			try
			{
				var jsonContentString = new StreamReader(filePath).ReadToEnd();
				return TryReadJsonContent(jsonContentString);
			}
			catch
			{
				return (false, PluginResources.FileValidation_ReadingJsonFailed);
			}
		}

		private static (bool Success, object OperationResult) TryReadJsonContent(string jsonContent)
		{
			var JsonExpectedKeys = new HashSet<string>()
			{
				"auth_provider_x509_cert_url",
				"auth_uri",
				"client_email",
				"client_id",
				"client_x509_cert_url",
				"private_key",
				"private_key_id",
				"token_uri",
				"project_id",
				"type"
			};

			try
			{
				var jsonContentDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
				var fileKeys = new HashSet<string>(jsonContentDictionary.Keys);
				var success = fileKeys.SetEquals(JsonExpectedKeys) && jsonContentDictionary.Values.All(value => !string.IsNullOrEmpty(value));
				if (success)
				{
					return (success, jsonContentDictionary);
				}

				var difference = fileKeys.Count > jsonContentDictionary.Count ? "more" : "less";
				return (success, string.Format(PluginResources.FileValidation_JsonFileFields, difference));
			}
			catch
			{
				return (false, PluginResources.FileValidation_ReadingJsonFailed);
			}
		}

		private static (bool Success, string ErrorMessage) IsValidPath(this string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return (false, PluginResources.FileValidation_UnsetFilePath);
			}

			var selectedDirectory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(selectedDirectory))
			{
				return (false, string.Format(PluginResources.FileValidation_PathDoesNotExists, selectedDirectory));
			}

			if (!File.Exists(filePath))
			{
				var selectedFile = Path.GetFileName(filePath);
				return (false, string.Format(PluginResources.FileValidation_FileDoesNotExists, selectedFile));
			}

			if (!Path.GetExtension(filePath).Equals(".json"))
			{
				return (false, PluginResources.FileValidation_NotJsonType);
			}

			return (true, null);
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr ILCreateFromPathW(string pszPath);

		[DllImport("shell32.dll")]
		private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

		[DllImport("shell32.dll")]
		private static extern void ILFree(IntPtr pidl);
	}
}