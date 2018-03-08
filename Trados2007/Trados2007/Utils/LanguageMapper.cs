// ---------------------------------
// <copyright file="LanguageMapper.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-08</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Encapsulates logic for converting Trados 2007 locales to corresponding .Net and SDL Trados Studio analogues. Singleton.
    /// </summary>
    public sealed class LanguageMapper
    {
        /// <summary>
        /// Stores trados 2007 locales - .net mappings
        /// </summary>
        private static readonly IDictionary<int, CultureInfo> Mapper = new Dictionary<int, CultureInfo>();

        /// <summary>
        /// Initializes static members of the <see cref="LanguageMapper"/> class.
        /// </summary>
        static LanguageMapper()
        {
            // CultureInfo caching is not stupid! 
            // This is done because some Trados 2007 locale codes could differ from .Net\Trados Studio ones.
            // Firstly, they are directly mirrored. Then missing\changed cultures are separately inserted into Mapper.
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var cultureInfo in cultures)
            {
                if (!Mapper.ContainsKey(cultureInfo.LCID))
                {
                    Mapper.Add(cultureInfo.LCID, cultureInfo);
                }
            }

            // Spanish (Traditional) is supported by Windows, but for some reason
            // not listed in any GetCultures list. Required since that is what Trados
            // maps Spanish (Spain) to.
            CultureInfo ci = new CultureInfo(1034, false);
            if (!Mapper.ContainsKey(ci.LCID))
            {
                Mapper.Add(ci.LCID, ci);
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="LanguageMapper"/> class from being created.
        /// </summary>
        private LanguageMapper()
        {
        }

        /// <summary>
        /// Converts Trados 2007 locale into its <see cref="CultureInfo"/> analogue.
        /// </summary>
        /// <param name="trados2007Locale">The trados2007 locale.</param>
        /// <returns>Exisitng <see cref="CultureInfo"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if corresponding locale is not found.</exception>
        public static CultureInfo ToCultureInfo(string trados2007Locale)
        {
            int locale;
            bool success = int.TryParse(trados2007Locale, out locale);

            if (!success)
            {
                throw new ArgumentException(string.Format(PluginResources.Exception_TradosLocaleNotMapped, trados2007Locale));
            }

            switch (locale)
            {
                //map Trados specific Scottish to Windows Culture
                case 1084:
                    locale = 1169;
                    break;
            }

            if (!Mapper.ContainsKey(locale))
            {
                throw new ArgumentException(string.Format(PluginResources.Exception_TradosLocaleNotMapped, trados2007Locale));
            }

            return Mapper[locale];
        }
    }
}
