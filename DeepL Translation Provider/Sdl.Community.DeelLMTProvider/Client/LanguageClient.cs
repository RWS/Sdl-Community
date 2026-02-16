using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public static class LanguageClient
    {
        public static List<SupportedLanguage> AllSupportedSourceLanguages { get; set; } =
        [
            new() { Language = "ACE", Name = "Acehnese", SupportsOptions = false },
            new() { Language = "AF", Name = "Afrikaans", SupportsOptions = false },
            new() { Language = "AN", Name = "Aragonese", SupportsOptions = false },
            new() { Language = "AR", Name = "Arabic", SupportsOptions = true },
            new() { Language = "AS", Name = "Assamese", SupportsOptions = false },
            new() { Language = "AY", Name = "Aymara", SupportsOptions = false },
            new() { Language = "AZ", Name = "Azerbaijani", SupportsOptions = false },
            new() { Language = "BA", Name = "Bashkir", SupportsOptions = false },
            new() { Language = "BE", Name = "Belarusian", SupportsOptions = false },
            new() { Language = "BG", Name = "Bulgarian", SupportsOptions = true },
            new() { Language = "BHO", Name = "Bhojpuri", SupportsOptions = false },
            new() { Language = "BN", Name = "Bengali", SupportsOptions = false },
            new() { Language = "BR", Name = "Breton", SupportsOptions = false },
            new() { Language = "BS", Name = "Bosnian", SupportsOptions = false },
            new() { Language = "CA", Name = "Catalan", SupportsOptions = false },
            new() { Language = "CEB", Name = "Cebuano", SupportsOptions = false },
            new() { Language = "CKB", Name = "Kurdish (Sorani)", SupportsOptions = false },
            new() { Language = "CS", Name = "Czech", SupportsOptions = true },
            new() { Language = "CY", Name = "Welsh", SupportsOptions = false },
            new() { Language = "DA", Name = "Danish", SupportsOptions = true },
            new() { Language = "DE", Name = "German", SupportsOptions = true },
            new() { Language = "EL", Name = "Greek", SupportsOptions = true },
            new() { Language = "EN", Name = "English (all English variants)", SupportsOptions = true },
            new() { Language = "EO", Name = "Esperanto", SupportsOptions = false },
            new() { Language = "ES", Name = "Spanish (all Spanish variants)", SupportsOptions = true },
            new() { Language = "ET", Name = "Estonian", SupportsOptions = true },
            new() { Language = "EU", Name = "Basque", SupportsOptions = false },
            new() { Language = "FA", Name = "Persian", SupportsOptions = false },
            new() { Language = "FI", Name = "Finnish", SupportsOptions = true },
            new() { Language = "FR", Name = "French", SupportsOptions = true },
            new() { Language = "GA", Name = "Irish", SupportsOptions = false },
            new() { Language = "GL", Name = "Galician", SupportsOptions = false },
            new() { Language = "GN", Name = "Guarani", SupportsOptions = false },
            new() { Language = "GOM", Name = "Konkani", SupportsOptions = false },
            new() { Language = "GU", Name = "Gujarati", SupportsOptions = false },
            new() { Language = "HA", Name = "Hausa", SupportsOptions = false },
            new() { Language = "HE", Name = "Hebrew", SupportsOptions = true },
            new() { Language = "HI", Name = "Hindi", SupportsOptions = false },
            new() { Language = "HR", Name = "Croatian", SupportsOptions = false },
            new() { Language = "HT", Name = "Haitian Creole", SupportsOptions = false },
            new() { Language = "HU", Name = "Hungarian", SupportsOptions = true },
            new() { Language = "HY", Name = "Armenian", SupportsOptions = false },
            new() { Language = "ID", Name = "Indonesian", SupportsOptions = true },
            new() { Language = "IG", Name = "Igbo", SupportsOptions = false },
            new() { Language = "IS", Name = "Icelandic", SupportsOptions = false },
            new() { Language = "IT", Name = "Italian", SupportsOptions = true },
            new() { Language = "JA", Name = "Japanese", SupportsOptions = true },
            new() { Language = "JV", Name = "Javanese", SupportsOptions = false },
            new() { Language = "KA", Name = "Georgian", SupportsOptions = false },
            new() { Language = "KK", Name = "Kazakh", SupportsOptions = false },
            new() { Language = "KMR", Name = "Kurdish (Kurmanji)", SupportsOptions = false },
            new() { Language = "KO", Name = "Korean", SupportsOptions = true },
            new() { Language = "KY", Name = "Kyrgyz", SupportsOptions = false },
            new() { Language = "LA", Name = "Latin", SupportsOptions = false },
            new() { Language = "LB", Name = "Luxembourgish", SupportsOptions = false },
            new() { Language = "LMO", Name = "Lombard", SupportsOptions = false },
            new() { Language = "LN", Name = "Lingala", SupportsOptions = false },
            new() { Language = "LT", Name = "Lithuanian", SupportsOptions = true },
            new() { Language = "LV", Name = "Latvian", SupportsOptions = true },
            new() { Language = "MAI", Name = "Maithili", SupportsOptions = false },
            new() { Language = "MG", Name = "Malagasy", SupportsOptions = false },
            new() { Language = "MI", Name = "Maori", SupportsOptions = false },
            new() { Language = "MK", Name = "Macedonian", SupportsOptions = false },
            new() { Language = "ML", Name = "Malayalam", SupportsOptions = false },
            new() { Language = "MN", Name = "Mongolian", SupportsOptions = false },
            new() { Language = "MR", Name = "Marathi", SupportsOptions = false },
            new() { Language = "MS", Name = "Malay", SupportsOptions = false },
            new() { Language = "MT", Name = "Maltese", SupportsOptions = false },
            new() { Language = "MY", Name = "Burmese", SupportsOptions = false },
            new() { Language = "NB", Name = "Norwegian Bokmål", SupportsOptions = true },
            new() { Language = "NE", Name = "Nepali", SupportsOptions = false },
            new() { Language = "NL", Name = "Dutch", SupportsOptions = true },
            new() { Language = "OC", Name = "Occitan", SupportsOptions = false },
            new() { Language = "OM", Name = "Oromo", SupportsOptions = false },
            new() { Language = "PA", Name = "Punjabi", SupportsOptions = false },
            new() { Language = "PAG", Name = "Pangasinan", SupportsOptions = false },
            new() { Language = "PAM", Name = "Kapampangan", SupportsOptions = false },
            new() { Language = "PL", Name = "Polish", SupportsOptions = true },
            new() { Language = "PRS", Name = "Dari", SupportsOptions = false },
            new() { Language = "PS", Name = "Pashto", SupportsOptions = false },
            new() { Language = "PT", Name = "Portuguese (all Portuguese variants)", SupportsOptions = true },
            new() { Language = "QU", Name = "Quechua", SupportsOptions = false },
            new() { Language = "RO", Name = "Romanian", SupportsOptions = true },
            new() { Language = "RU", Name = "Russian", SupportsOptions = true },
            new() { Language = "SA", Name = "Sanskrit", SupportsOptions = false },
            new() { Language = "SCN", Name = "Sicilian", SupportsOptions = false },
            new() { Language = "SK", Name = "Slovak", SupportsOptions = true },
            new() { Language = "SL", Name = "Slovenian", SupportsOptions = true },
            new() { Language = "SQ", Name = "Albanian", SupportsOptions = false },
            new() { Language = "SR", Name = "Serbian", SupportsOptions = false },
            new() { Language = "ST", Name = "Sesotho", SupportsOptions = false },
            new() { Language = "SU", Name = "Sundanese", SupportsOptions = false },
            new() { Language = "SV", Name = "Swedish", SupportsOptions = true },
            new() { Language = "SW", Name = "Swahili", SupportsOptions = false },
            new() { Language = "TA", Name = "Tamil", SupportsOptions = false },
            new() { Language = "TE", Name = "Telugu", SupportsOptions = false },
            new() { Language = "TG", Name = "Tajik", SupportsOptions = false },
            new() { Language = "TH", Name = "Thai", SupportsOptions = true },
            new() { Language = "TK", Name = "Turkmen", SupportsOptions = false },
            new() { Language = "TL", Name = "Tagalog", SupportsOptions = false },
            new() { Language = "TN", Name = "Tswana", SupportsOptions = false },
            new() { Language = "TR", Name = "Turkish", SupportsOptions = true },
            new() { Language = "TS", Name = "Tsonga", SupportsOptions = false },
            new() { Language = "TT", Name = "Tatar", SupportsOptions = false },
            new() { Language = "UK", Name = "Ukrainian", SupportsOptions = true },
            new() { Language = "UR", Name = "Urdu", SupportsOptions = false },
            new() { Language = "UZ", Name = "Uzbek", SupportsOptions = false },
            new() { Language = "VI", Name = "Vietnamese", SupportsOptions = true },
            new() { Language = "WO", Name = "Wolof", SupportsOptions = false },
            new() { Language = "XH", Name = "Xhosa", SupportsOptions = false },
            new() { Language = "YI", Name = "Yiddish", SupportsOptions = false },
            new() { Language = "YUE", Name = "Cantonese", SupportsOptions = false },
            new() { Language = "ZH", Name = "Chinese (all Chinese variants)", SupportsOptions = true },
            new() { Language = "ZU", Name = "Zulu", SupportsOptions = false }
        ];

        public static List<SupportedLanguage> SupportedLanguagesVariants { get; } =
        [
            new() { Language = "EN-GB", Name = "English (British)", SupportsOptions = true },
            new() { Language = "EN-US", Name = "English (American)", SupportsOptions = true },
            new() { Language = "ES-419", Name = "Spanish (Latin American)", SupportsOptions = true },
            new() { Language = "PT-BR", Name = "Portuguese (Brazilian)", SupportsOptions = true },
            new()
            {
                Language = "PT-PT",
                Name = "Portuguese (all Portuguese variants excluding Brazilian Portuguese)",
                SupportsOptions = true
            },
            new() { Language = "ZH-HANS", Name = "Chinese (simplified)", SupportsOptions = true },
            new() { Language = "ZH-HANT", Name = "Chinese (traditional)", SupportsOptions = true }
        ];

        public static List<SupportedLanguage> GetSupportedLanguages(string type, string apiKey, string chosenBaseUrl)
        {
            var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = AppInitializer.Client.PostAsync($"{chosenBaseUrl}/languages", content).Result;
            response.EnsureSuccessStatusCode();

            var supportedLanguages = GetHardcodedSupportedLanguages(type);

            var apiResponseLanguages = JsonConvert.DeserializeObject<List<SupportedLanguage>>(response.Content?.ReadAsStringAsync().Result ?? "[]");
            AddLanguages(ref supportedLanguages, apiResponseLanguages);

            return supportedLanguages;
        }

        private static List<SupportedLanguage> GetHardcodedSupportedLanguages(string type)
        {
            //TODO: Remove this method and related properties when DeepL fixes the "/languages" endpoint
            var supportedLanguages = AllSupportedSourceLanguages.ToList();
            if (type == "target") AddLanguages(ref supportedLanguages, SupportedLanguagesVariants);
            return supportedLanguages;
        }

        private static void AddLanguages(ref List<SupportedLanguage> currentLanguages, List<SupportedLanguage> newLanguages)
        {
            foreach (var newLanguage in newLanguages)
            {
                if (currentLanguages.Any(l => l.Language == newLanguage.Language)) continue;
                currentLanguages.Add(newLanguage);
            }
        }
    }
}