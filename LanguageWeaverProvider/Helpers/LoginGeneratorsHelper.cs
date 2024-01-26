using System;
using System.Security.Cryptography;
using System.Text;

namespace LanguageWeaverProvider.Extensions
{
	public static class LoginGeneratorsHelper
	{
		/// <summary>
		/// Base64url no-padding encodes the given input buffer.
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static string Base64urlencodeNoPadding(byte[] buffer)
		{
			var base64 = Convert.ToBase64String(buffer);

			// Converts base64 to base64url.
			base64 = base64.Replace("+", "-");
			base64 = base64.Replace("/", "_");
			// Strips padding.
			base64 = base64.Replace("=", "");

			return base64;
		}

		/// <summary>
		/// Returns URI-safe data with a given input length.
		/// </summary>
		/// <param name="length">Input length (nb. output will be longer)</param>
		/// <returns></returns>
		public static string RandomDataBase64url(uint length)
		{
			var rng = new RNGCryptoServiceProvider();
			var bytes = new byte[length];
			rng.GetBytes(bytes);
			return Base64urlencodeNoPadding(bytes);
		}

		/// <summary>
		/// Returns the SHA256 hash of the input string.
		/// </summary>
		/// <param name="inputStirng"></param>
		/// <returns></returns>
		public static byte[] Sha256(string inputStirng)
		{
			var bytes = Encoding.ASCII.GetBytes(inputStirng);
			var sha256 = new SHA256Managed();
			return sha256.ComputeHash(bytes);
		}
	}
}