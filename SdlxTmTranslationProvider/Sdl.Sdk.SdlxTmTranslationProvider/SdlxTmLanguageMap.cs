//-----------------------------------------------------------------------
// <copyright file="SdlxTmLanguageMap.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// A class to map language codes to cultures and LCIDs
    /// </summary>
    public class SdlxTmLanguageMap
    {
        /// <summary>
        /// The language XML document
        /// </summary>
        private static XmlDocument languageXml = null;

        /// <summary>
        /// Gets the culture information from iso code.
        /// </summary>
        /// <param name="isoCode">The iso code.</param>
        /// <returns>The culture information</returns>
        public static CultureInfo GetCultureInfoFromIsoCode(string isoCode)
        {
            CultureInfo returnValue = null;

            if (languageXml == null)
            {
                FillLanguageXml();
            }

            string cultureCode = languageXml.SelectSingleNode(string.Format("//Language[@SDLXCode='{0}']/@CultureInfo", isoCode)).Value;

            try
            {
                returnValue = new CultureInfo(cultureCode, false);
            }
            catch (Exception)
            {
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the LCID from iso code.
        /// </summary>
        /// <param name="isoCode">The iso code.</param>
        /// <returns>The associated LCID</returns>
        public static int GetLcidFromIsoCode(string isoCode)
        {
            int returnValue = 0;

            if (languageXml == null)
            {
                FillLanguageXml();
            }

            string lcid = languageXml.SelectSingleNode(string.Format("//Language[@SDLXCode='{0}']/@LCID", isoCode)).Value;

            if (!int.TryParse(lcid, out returnValue))
            {
                returnValue = 0;
            }

            return returnValue;
        }

        /// <summary>
        /// Fills the language XML.
        /// </summary>
        private static void FillLanguageXml()
        {
            languageXml = new XmlDocument();
            languageXml.LoadXml(PluginResources.LanguageInfo);

            return;
        }
    }
}
