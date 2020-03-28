using System;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.Community.MTCloud.Provider.Extensions
{
	public static class StringExtensions
	{	
		public static string Encrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			var inputArray = Encoding.UTF8.GetBytes(input);
			var tripleDes = new TripleDESCryptoServiceProvider
			{
				Key = Encoding.UTF8.GetBytes("gtlw-5ur7-amoqp3"),
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			var cTransform = tripleDes.CreateEncryptor();
			var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);

			tripleDes.Clear();

			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}

		public static string Decrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			try
			{
				var inputArray = Convert.FromBase64String(input);
				var tripleDes = new TripleDESCryptoServiceProvider
				{
					Key = Encoding.UTF8.GetBytes("gtlw-5ur7-amoqp3"),
					Mode = CipherMode.ECB,
					Padding = PaddingMode.PKCS7
				};

				var cTransform = tripleDes.CreateDecryptor();
				var resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
				tripleDes.Clear();

				return Encoding.UTF8.GetString(resultArray);
			}
			catch
			{
				// catch all; ignore
			}

			return string.Empty;
		}
	}
}
