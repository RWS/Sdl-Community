using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Windows.Forms;

namespace Sdl.Community.TermInjector
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class TermInjectorTranslationOptions
    {
        #region "TranslationMethod"
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.TranslationMemory;
        #endregion

        #region "TranslationProviderUriBuilder"
        TranslationProviderUriBuilder _uriBuilder;

        public TermInjectorTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TermInjectorTranslationProvider.TermInjectorTranslationProviderScheme);
        }

        public TermInjectorTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }
        #endregion

        /// <summary>
        /// Set and retrieve the name and path of the delimited glossary file.
        /// </summary>
        #region "GlossaryFileName"
        public string GlossaryFileName
        {
            get { return GetStringParameter("glossaryfile"); }
            set { SetStringParameter("glossaryfile", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the name and path of the delimited regex file.
        /// </summary>
        #region "RegexFileName"
        public string RegexFileName
        {
            get { return GetStringParameter("regexfile"); }
            set { SetStringParameter("regexfile", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the name and path of the tm file.
        /// </summary>
        #region "TMFileName"
        public string TMFileName
        {
            get { return GetStringParameter("tmfile"); }
            set { SetStringParameter("tmfile", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the separator character used for adding terms.
        /// </summary>
        #region "TermAdditionSeparator"
        public string TermAdditionSeparator
        {
            get { return GetStringParameter("termadditionseparator"); }
            set { SetStringParameter("termadditionseparator", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the token boundary characters used in term recognition.
        /// </summary>
        #region "TokenBoundaryCharacters"
        public string TokenBoundaryCharacters
        {
            get { return GetStringParameter("tokenboundarycharacters"); }
            set { SetStringParameter("tokenboundarycharacters", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the the boolean indicating whether boundary characters are used.
        /// </summary>
        #region "UseBoundaryCharacters"
        public string UseBoundaryCharacters
        {
            get { return GetStringParameter("useboundarycharacters"); }
            set { SetStringParameter("useboundarycharacters", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the delimiter character.
        /// </summary>
        #region "Delimiter"
        public string Delimiter
        {
            get { return GetStringParameter("delimiter"); }
            set { SetStringParameter("delimiter", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the boolean value for matching case.
        /// </summary>
        #region "Matchcase"
        public string MatchCase
        {
            get { return GetStringParameter("matchcase"); }
            set { SetStringParameter("matchcase", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the boolean value for using regexes.
        /// </summary>
        #region "Regex"
        public string UseRegex
        {
            get { return GetStringParameter("useregex"); }
            set { SetStringParameter("useregex", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the boolean value for injecting into 100 percent matches.
        /// </summary>
        #region "Inject into full matches"
        public string InjectIntoFullMatches
        {
            get { return GetStringParameter("injectfullmatches"); }
            set { SetStringParameter("injectfullmatches", value); }
        }
        #endregion


        /// <summary>
        /// Set and retrieve the percentage used in new segments that have terms injected into them.
        /// </summary>
        #region "Injected new segment percentage"
        public int NewSegmentPercentage
        {
            get { return Convert.ToInt32(GetStringParameter("newsegmentpercentage")); }
            set { SetStringParameter("newsegmentpercentage", value.ToString()); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the Boolean for injecting new terms into the start of fuzzy segments.
        /// </summary>
        #region "Inject new terms into fuzzies"
        public string InjectNewTermsIntoFuzzies
        {
            get { return GetStringParameter("injectnewtermsintofuzzies"); }
            set { SetStringParameter("injectnewtermsintofuzzies", value); }
        }
        #endregion

        /// <summary>
        /// Set and retrieve the GUID used to avoid duplicate instances.
        /// </summary>
        #region "Instance GUID"
        public string InstanceGUID
        {
            get { return GetStringParameter("instanceguid"); }
            set { SetStringParameter("instanceguid", value); }
        }
        #endregion

        #region "SetStringParameter"
        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }
        #endregion

        #region "GetStringParameter"
        private string GetStringParameter(string p)
        {
            string paramString = _uriBuilder[p];
            return paramString;
        }
        #endregion

        #region "Uri"
        public Uri Uri
        {
            get
            {
                return _uriBuilder.Uri;
            }
        }
        #endregion
    }
}