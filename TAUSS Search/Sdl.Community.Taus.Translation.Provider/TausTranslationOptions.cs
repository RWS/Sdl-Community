using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Taus.Translation.Provider
{
    /// <summary>
    /// This class is used to hold the provider plug-in settings. 
    /// All settings are automatically stored in a URI.
    /// </summary>
    public class TausTranslationOptions
    {
        #region "TranslationMethod"
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.TranslationMemory;


        #endregion

        #region "TranslationProviderUriBuilder"

        private readonly TranslationProviderUriBuilder _uriBuilder;        


        public TausTranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TausTranslationProvider.TausTranslationProviderScheme);
        }

        public TausTranslationOptions(Uri uri)
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }



        #endregion

        public string IgnoreTranslatedSegments
        {
            get { return GetStringParameter("ignoreTranslatedSegments"); }
            set { SetStringParameter("ignoreTranslatedSegments", value); }
        }

        public string SearchTimeout
        {
            get { return GetStringParameter("searchTimeout"); }
            set { SetStringParameter("searchTimeout", value); }
        }
       

        public string ConnectionAppKey
        {
            get { return GetStringParameter("connectionAppKey"); }
            set { SetStringParameter("connectionAppKey", value); }
        }

        public string ConnectionUserName
        {
            get { return GetStringParameter("connectionUserName"); }
            set { SetStringParameter("connectionUserName", value); }
        }

        public string ConnectionUserPassword
        {
            get { return GetStringParameter("connectionUserPassword"); }
            set { SetStringParameter("connectionUserPassword", value); }
        }
        public string ConnectionAuthKey
        {
            get { return GetStringParameter("connectionAuthKey"); }
            set { SetStringParameter("connectionAuthKey", value); }
        }

       

        public string SearchCriteriaIndustryId
        {
            get { return GetStringParameter("SearchCriteriaIndustryId"); }
            set { SetStringParameter("SearchCriteriaIndustryId", value); }
        }
        public string SearchCriteriaIndustryName
        {
            get { return GetStringParameter("searchCriteriaIndustryName"); }
            set { SetStringParameter("searchCriteriaIndustryName", value); }
        }

        public string SearchCriteriaContentTypeId
        {
            get { return GetStringParameter("searchCriteriaContentTypeId"); }
            set { SetStringParameter("searchCriteriaContentTypeId", value); }
        }
        public string SearchCriteriaContentTypeName
        {
            get { return GetStringParameter("searchCriteriaContentTypeName"); }
            set { SetStringParameter("searchCriteriaContentTypeName", value); }
        }

        public string SearchCriteriaProviderId
        {
            get { return GetStringParameter("searchCriteriaProviderId"); }
            set { SetStringParameter("searchCriteriaProviderId", value); }
        }
        public string SearchCriteriaProviderName
        {
            get { return GetStringParameter("searchCriteriaProviderName"); }
            set { SetStringParameter("searchCriteriaProviderName", value); }
        }

        public string SearchCriteriaProductId
        {
            get { return GetStringParameter("searchCriteriaProductId"); }
            set { SetStringParameter("searchCriteriaProductId", value); }
        }
        public string SearchCriteriaProductName
        {
            get { return GetStringParameter("searchCriteriaProductName"); }
            set { SetStringParameter("searchCriteriaProductName", value); }
        }

        public string SearchCriteriaOwnerId
        {
            get { return GetStringParameter("searchCriteriaOwnerId"); }
            set { SetStringParameter("searchCriteriaOwnerId", value); }
        }
        public string SearchCriteriaOwnerName
        {
            get { return GetStringParameter("searchCriteriaOwnerName"); }
            set { SetStringParameter("searchCriteriaOwnerName", value); }
        }

        public string SearchCriteriaText
        {
            get { return GetStringParameter("searchCriteriaText"); }
            set { SetStringParameter("searchCriteriaText", value); }
        }

     


        #region "SetStringParameter"
        private void SetStringParameter(string p, string value)
        {
            _uriBuilder[p] = value;
        }
        #endregion

        #region "GetStringParameter"
        private string GetStringParameter(string p)
        {
            var paramString = _uriBuilder[p];
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
