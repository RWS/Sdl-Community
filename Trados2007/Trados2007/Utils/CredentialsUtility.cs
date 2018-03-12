// ---------------------------------
// <copyright file="CredentialsParser.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-06-20</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Stroes credentials parsing logic.
    /// </summary>
    public class CredentialsUtility
    {
        /// <summary>
        /// Stores standart windows username password separator
        /// </summary>
        private static readonly char passwordSeparatorChar = '\u25CF';

        /// <summary>
        /// Tries to parse given credentials.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns><c>true</c> if succeded, otherwise - <c>false</c></returns>
        public static bool TryParse(TranslationProviderCredential credential, out string username, out string password)
        {
            try
            {
                int separator = credential.Credential.IndexOf(CredentialsUtility.passwordSeparatorChar);
                username = credential.Credential.Substring(0, separator);
                password = credential.Credential.Substring(separator + 1, credential.Credential.Length - separator - 1);
                return true;
            }
            catch (Exception)
            {
                username = string.Empty;
                password = string.Empty;
                return false;
            }
        }

        public static TranslationProviderCredential CreateTranslationProviderCredential(string username, string password)
        {
            return new TranslationProviderCredential(String.Format("{0}{1}{2}", username, passwordSeparatorChar, password), true);
        }
    }
}
