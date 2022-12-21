using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace GoogleTranslatorProvider.Extensions
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

		public static (bool Success, string ErrorMessage) VerifyAndDownloadJsonFile(this string uri, string location)
		{
			if (string.IsNullOrEmpty(uri))
			{
				return (false, "The URL can not be empty");
			}

			if (!OnlineJsonFileIsValid(uri))
			{
				return (false, "There is no valid json file that could be downloaded.");
			}

			try
			{
				using var webClient = new WebClient();
				if (!Directory.Exists(Constants.DefaultDownloadableLocation))
				{
					Directory.CreateDirectory(Constants.DefaultDownloadableLocation);
				}

				var filePath = location;
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}

				webClient.DownloadFile(uri, filePath);
				return (true, string.Empty);
			}
			catch
			{
				return (false, "An error occured while downloading the file");
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

		private static (bool Success, object OperationResult) ReadJsonFileOnPath(this string filePath)
		{
			try
			{
				var jsonContentString = new StreamReader(filePath).ReadToEnd();
				return TryReadJsonContent(jsonContentString);
			}
			catch
			{
				return (false, "Unexpected error while reading the json file. The file might be corrupted.");
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
				return (success, $"The selected json file has {difference} fields than it is supposed to have. Please download the file again and use an unedited file.");
			}
			catch
			{
				return (false, "Unexpected error while reading the json file. The file might be corrupted.");
			}
		}

		private static (bool Success, string ErrorMessage) IsValidPath(this string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return (false, "The json file path was not set.");
			}

			var selectedDirectory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(selectedDirectory))
			{
				return (false, $"The path does not exists\nPath: {selectedDirectory}.");
			}

			if (!File.Exists(filePath))
			{
				var selectedFile = Path.GetFileName(filePath);
				return (false, $"The file {selectedFile} could not be found at location {selectedDirectory}.");
			}

			if (!Path.GetExtension(filePath).Equals(".json"))
			{
				return (false, "The selected file is not json type.");
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