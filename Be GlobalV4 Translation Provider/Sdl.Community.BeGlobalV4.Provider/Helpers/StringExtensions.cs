using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.Community.MTCloud.Provider.Helpers
{
	public static class StringExtensions
	{
		/// <summary>
		///     Returns a string array that contains the substrings in this instance that are delimited by specified indexes.
		/// </summary>
		/// <param name="source">The original string.</param>
		/// <param name="index">An index that delimits the substrings in this string.</param>
		/// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more indexes.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="index" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">An <paramref name="index" /> is less than zero or greater than the length of this instance.</exception>
		public static string[] SplitAt(this string source, params int[] index)
		{
			index = index.Distinct().OrderBy(x => x).ToArray();
			var output = new string[index.Length + 1];
			var pos = 0;

			for (var i = 0; i < index.Length; pos = index[i++])
				output[i] = source.Substring(pos, index[i] - pos);

			output[index.Length] = source.Substring(pos);
			return output;
		}
		
		/// <summary>
		/// Algorythm to encrypt data
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string EncryptData(string input)
		{
			byte[] inputArray = Encoding.UTF8.GetBytes(input);
			var tripleDES = new TripleDESCryptoServiceProvider();
			tripleDES.Key = Encoding.UTF8.GetBytes("gtlw-5ur7-amoqp3");
			tripleDES.Mode = CipherMode.ECB;
			tripleDES.Padding = PaddingMode.PKCS7;
			var cTransform = tripleDES.CreateEncryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
			tripleDES.Clear();
			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}

		/// <summary>
		/// Algorithm to decrypt data
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string Decrypt(string input)
		{
			try
			{
				byte[] inputArray = Convert.FromBase64String(input);
				var tripleDES = new TripleDESCryptoServiceProvider();
				tripleDES.Key = Encoding.UTF8.GetBytes("gtlw-5ur7-amoqp3");
				tripleDES.Mode = CipherMode.ECB;
				tripleDES.Padding = PaddingMode.PKCS7;
				var cTransform = tripleDES.CreateDecryptor();
				byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
				tripleDES.Clear();
				return Encoding.UTF8.GetString(resultArray);
			}
			catch
			{
				// return string.Empty in case the data coulnd't be decrypted,
				// so user can enter back the credentials through the Login tab.
				return string.Empty;
			}
		}
	}
}
