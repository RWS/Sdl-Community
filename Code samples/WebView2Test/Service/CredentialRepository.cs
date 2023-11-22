using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using WebView2Test.Model;

namespace WebView2Test.Service
{
	public class CredentialRepository
	{
		private const string CredentialsFolder = @"Trados\AppStore\WebViewTest";
		private static readonly byte[] AdditionalEntropy = { 9, 8, 7, 6, 5 };

		private string CredentialsFilepath =>
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				CredentialsFolder, "Credentials.txt");

		public void ClearCredentials()
		{
			try
			{
				File.Delete(CredentialsFilepath);
			}
			catch { }
		}

		public Credential LoadCredentials()
		{
			if (!File.Exists(CredentialsFilepath)) return null;

			var encryptedCredentials = File.ReadAllBytes(CredentialsFilepath);
			var credentialBytes = Unprotect(encryptedCredentials);

			var serializedCredentials = Encoding.UTF8.GetString(credentialBytes);
			return JsonConvert.DeserializeObject<Credential>(serializedCredentials);
		}

		public void SaveCredentials(Credential credentials)
		{
			var serializedCredentials = JsonConvert.SerializeObject(credentials);

			var credentialBytes = Encoding.UTF8.GetBytes(serializedCredentials);
			var encryptedCredentials = Protect(credentialBytes);

			var directoryInfo = Directory.GetParent(CredentialsFilepath).FullName;
			if (!Directory.Exists(directoryInfo)) Directory.CreateDirectory(directoryInfo);

			File.WriteAllBytes(CredentialsFilepath, encryptedCredentials);
		}

		private static byte[] Protect(byte[] data)
		{
			try
			{
				// Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
				// only by the same current user.
				return ProtectedData.Protect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
			}
			catch (CryptographicException e)
			{
				Console.WriteLine("Data was not encrypted. An error occurred.");
				Console.WriteLine(e.ToString());
				return null;
			}
		}

		private static byte[] Unprotect(byte[] data)
		{
			try
			{
				//Decrypt the data using DataProtectionScope.CurrentUser.
				return ProtectedData.Unprotect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
			}
			catch (CryptographicException e)
			{
				Console.WriteLine("Data was not decrypted. An error occurred.");
				Console.WriteLine(e.ToString());
				return null;
			}
		}
	}
}