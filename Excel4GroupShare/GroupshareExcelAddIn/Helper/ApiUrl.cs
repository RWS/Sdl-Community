using GroupshareExcelAddIn.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace GroupshareExcelAddIn.Helper
{
    public class ApiUrl
    {
        public static string BaseUrl { get; set; }

        public static string DefinitionsUrl => $"{BaseUrl}/api/properties-service/resource-types";
        public static string LanguagesUrl => $"{BaseUrl}/classic/resources/sdl-common/locale/languages/locale-en.json";
        public static string UsersUrl => $"{BaseUrl}/api/management/v2/users";
        private static string CustomFieldUrl => $"{BaseUrl}/api/properties-service/definitions";
        private static string OrganizationsUrl => $"{BaseUrl}/api/management/v2/organizations";
        private static string ProjectsUrl => $"{BaseUrl}/api/projectserver/v2/projects";
        private static string TermbaseUrl => $"{BaseUrl}/api/management/v2/organizationresources";
        private static string TmsUrl => $"{BaseUrl}/api/tmservice/tms";

        public static void AddRequestHeaders(HttpClient httpClient, HttpRequestMessage request, string gsToken)
        {
            httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/x-www-form-urlencoded");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", gsToken);
        }

        public static string GetCustomFieldsQueryString(ResourceFilter filter)
        {
            var url = CustomFieldUrl;
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            var showAllChecked = true;
            string organizationId = null;

            if (filter?.Organization != null)
            {
                organizationId = filter.Organization.UniqueId.ToString();
                showAllChecked = false;
            }

            query["filter"] = JsonConvert.SerializeObject(new
            {
                tenantId = organizationId,
                showAll = showAllChecked,
                resourceTypes = filter?.SecondParameter.ResourceTypes,
                showHidden = false
            }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            var encodedQuery = query.ToString().Replace("+", "%20");
            builder.Query = encodedQuery;
            return builder.ToString();
        }

        public static string GetOrganizationsQueryString(bool flatten)
        {
            var baseUrl = OrganizationsUrl;
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["flatten"] = flatten.ToString();
            query["hideImplicitLibs"] = "false";

            builder.Query = query.ToString();
            return builder.ToString();
        }

        public static string GetProjectsQueryString(ProjectQuery projectFilter)
        {
            var baseUrl = ProjectsUrl;
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["page"] = projectFilter.Page.ToString();
            query["start"] = projectFilter.StartItem.ToString();
            query["limit"] = projectFilter.PageSize.ToString();

            if (projectFilter.Filter != null)
            {
                query["filter"] = JsonConvert.SerializeObject(projectFilter.Filter, Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateFormatString = "yyyy-MM-d" });
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }

        public static string GetTermbasesQueryString(ResourceFilter filter)
        {
            var url = TermbaseUrl;
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["resourceType"] = "tb";

            if (filter?.Organization != null)
            {
                query["organizationId"] = filter.Organization.UniqueId.ToString();
            }

            builder.Query = query.ToString();
            return builder.ToString();
        }

        public static string GetTmsQueryString(ResourceFilter filter)
        {
            var baseUrl = TmsUrl;
            var builder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            if (filter != null)
            {
                if (filter.Organization != null)
                {
                    query["$location"] = filter.Organization?.Path;
                    query["$includeSubLocations"] = filter.IncludeSubOrganizations.ToString();
                }

                if (filter.SecondParameter.LanguagePair != null)
                {
                    var sourceLangParameter = string.Empty;
                    if (filter.SecondParameter.LanguagePair.SourceLanguage != null)
                    {
                        sourceLangParameter = $"(ld/Source eq '{filter.SecondParameter.LanguagePair.SourceLanguage.Code}')";
                    }

                    var targetLangParam = string.Empty;
                    if (filter.SecondParameter.LanguagePair.TargetLanguage != null)
                    {
                        var isSourceLangEmpty = string.IsNullOrEmpty(sourceLangParameter);
                        targetLangParam = $"{(!isSourceLangEmpty ? " and " : string.Empty)}(ld/Target eq '{filter.SecondParameter.LanguagePair.TargetLanguage.Code}')";
                    }

                    if (!string.IsNullOrEmpty(sourceLangParameter) || !string.IsNullOrEmpty(targetLangParam))
                    {
                        query["$filter"] = $"(LanguageDirections/any(ld: {sourceLangParameter}{targetLangParam}))";
                    }
                }
            }

            //query.ToString url encodes the string (e.g. + instead of spaces) but GS needs %20
            var encodedQuery = query.ToString().Replace("+", "%20");
            builder.Query = encodedQuery;
            return builder.ToString();
        }
    }
}