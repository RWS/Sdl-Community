// ---------------------------------
// <copyright file="ITrados2007TranslationProvider.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Trados 2007 translation provider
    /// </summary>
    public interface ITrados2007TranslationProvider : ITranslationProvider
    {
        /// <summary>
        /// Gets the supported language direction.
        /// </summary>
        /// <remarks>Trados 2007 Translation Provider is able to support only one language direction</remarks>
        LanguagePair LanguageDirection { get; }
    }
}