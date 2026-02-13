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
            new() { Language = "ACE", Name = "Acehnese", SupportsFormality = false },
            new() { Language = "AF", Name = "Afrikaans", SupportsFormality = false },
            new() { Language = "AN", Name = "Aragonese", SupportsFormality = false },
            new() { Language = "AR", Name = "Arabic", SupportsFormality = true },
            new() { Language = "AS", Name = "Assamese", SupportsFormality = false },
            new() { Language = "AY", Name = "Aymara", SupportsFormality = false },
            new() { Language = "AZ", Name = "Azerbaijani", SupportsFormality = false },
            new() { Language = "BA", Name = "Bashkir", SupportsFormality = false },
            new() { Language = "BE", Name = "Belarusian", SupportsFormality = false },
            new() { Language = "BG", Name = "Bulgarian", SupportsFormality = true },
            new() { Language = "BHO", Name = "Bhojpuri", SupportsFormality = false },
            new() { Language = "BN", Name = "Bengali", SupportsFormality = false },
            new() { Language = "BR", Name = "Breton", SupportsFormality = false },
            new() { Language = "BS", Name = "Bosnian", SupportsFormality = false },
            new() { Language = "CA", Name = "Catalan", SupportsFormality = false },
            new() { Language = "CEB", Name = "Cebuano", SupportsFormality = false },
            new() { Language = "CKB", Name = "Kurdish (Sorani)", SupportsFormality = false },
            new() { Language = "CS", Name = "Czech", SupportsFormality = true },
            new() { Language = "CY", Name = "Welsh", SupportsFormality = false },
            new() { Language = "DA", Name = "Danish", SupportsFormality = true },
            new() { Language = "DE", Name = "German", SupportsFormality = true },
            new() { Language = "EL", Name = "Greek", SupportsFormality = true },
            new() { Language = "EN", Name = "English (all English variants)", SupportsFormality = true },
            new() { Language = "EO", Name = "Esperanto", SupportsFormality = false },
            new() { Language = "ES", Name = "Spanish (all Spanish variants)", SupportsFormality = true },
            new() { Language = "ET", Name = "Estonian", SupportsFormality = true },
            new() { Language = "EU", Name = "Basque", SupportsFormality = false },
            new() { Language = "FA", Name = "Persian", SupportsFormality = false },
            new() { Language = "FI", Name = "Finnish", SupportsFormality = true },
            new() { Language = "FR", Name = "French", SupportsFormality = true },
            new() { Language = "GA", Name = "Irish", SupportsFormality = false },
            new() { Language = "GL", Name = "Galician", SupportsFormality = false },
            new() { Language = "GN", Name = "Guarani", SupportsFormality = false },
            new() { Language = "GOM", Name = "Konkani", SupportsFormality = false },
            new() { Language = "GU", Name = "Gujarati", SupportsFormality = false },
            new() { Language = "HA", Name = "Hausa", SupportsFormality = false },
            new() { Language = "HE", Name = "Hebrew", SupportsFormality = true },
            new() { Language = "HI", Name = "Hindi", SupportsFormality = false },
            new() { Language = "HR", Name = "Croatian", SupportsFormality = false },
            new() { Language = "HT", Name = "Haitian Creole", SupportsFormality = false },
            new() { Language = "HU", Name = "Hungarian", SupportsFormality = true },
            new() { Language = "HY", Name = "Armenian", SupportsFormality = false },
            new() { Language = "ID", Name = "Indonesian", SupportsFormality = true },
            new() { Language = "IG", Name = "Igbo", SupportsFormality = false },
            new() { Language = "IS", Name = "Icelandic", SupportsFormality = false },
            new() { Language = "IT", Name = "Italian", SupportsFormality = true },
            new() { Language = "JA", Name = "Japanese", SupportsFormality = true },
            new() { Language = "JV", Name = "Javanese", SupportsFormality = false },
            new() { Language = "KA", Name = "Georgian", SupportsFormality = false },
            new() { Language = "KK", Name = "Kazakh", SupportsFormality = false },
            new() { Language = "KMR", Name = "Kurdish (Kurmanji)", SupportsFormality = false },
            new() { Language = "KO", Name = "Korean", SupportsFormality = true },
            new() { Language = "KY", Name = "Kyrgyz", SupportsFormality = false },
            new() { Language = "LA", Name = "Latin", SupportsFormality = false },
            new() { Language = "LB", Name = "Luxembourgish", SupportsFormality = false },
            new() { Language = "LMO", Name = "Lombard", SupportsFormality = false },
            new() { Language = "LN", Name = "Lingala", SupportsFormality = false },
            new() { Language = "LT", Name = "Lithuanian", SupportsFormality = true },
            new() { Language = "LV", Name = "Latvian", SupportsFormality = true },
            new() { Language = "MAI", Name = "Maithili", SupportsFormality = false },
            new() { Language = "MG", Name = "Malagasy", SupportsFormality = false },
            new() { Language = "MI", Name = "Maori", SupportsFormality = false },
            new() { Language = "MK", Name = "Macedonian", SupportsFormality = false },
            new() { Language = "ML", Name = "Malayalam", SupportsFormality = false },
            new() { Language = "MN", Name = "Mongolian", SupportsFormality = false },
            new() { Language = "MR", Name = "Marathi", SupportsFormality = false },
            new() { Language = "MS", Name = "Malay", SupportsFormality = false },
            new() { Language = "MT", Name = "Maltese", SupportsFormality = false },
            new() { Language = "MY", Name = "Burmese", SupportsFormality = false },
            new() { Language = "NB", Name = "Norwegian Bokmål", SupportsFormality = true },
            new() { Language = "NE", Name = "Nepali", SupportsFormality = false },
            new() { Language = "NL", Name = "Dutch", SupportsFormality = true },
            new() { Language = "OC", Name = "Occitan", SupportsFormality = false },
            new() { Language = "OM", Name = "Oromo", SupportsFormality = false },
            new() { Language = "PA", Name = "Punjabi", SupportsFormality = false },
            new() { Language = "PAG", Name = "Pangasinan", SupportsFormality = false },
            new() { Language = "PAM", Name = "Kapampangan", SupportsFormality = false },
            new() { Language = "PL", Name = "Polish", SupportsFormality = true },
            new() { Language = "PRS", Name = "Dari", SupportsFormality = false },
            new() { Language = "PS", Name = "Pashto", SupportsFormality = false },
            new() { Language = "PT", Name = "Portuguese (all Portuguese variants)", SupportsFormality = true },
            new() { Language = "QU", Name = "Quechua", SupportsFormality = false },
            new() { Language = "RO", Name = "Romanian", SupportsFormality = true },
            new() { Language = "RU", Name = "Russian", SupportsFormality = true },
            new() { Language = "SA", Name = "Sanskrit", SupportsFormality = false },
            new() { Language = "SCN", Name = "Sicilian", SupportsFormality = false },
            new() { Language = "SK", Name = "Slovak", SupportsFormality = true },
            new() { Language = "SL", Name = "Slovenian", SupportsFormality = true },
            new() { Language = "SQ", Name = "Albanian", SupportsFormality = false },
            new() { Language = "SR", Name = "Serbian", SupportsFormality = false },
            new() { Language = "ST", Name = "Sesotho", SupportsFormality = false },
            new() { Language = "SU", Name = "Sundanese", SupportsFormality = false },
            new() { Language = "SV", Name = "Swedish", SupportsFormality = true },
            new() { Language = "SW", Name = "Swahili", SupportsFormality = false },
            new() { Language = "TA", Name = "Tamil", SupportsFormality = false },
            new() { Language = "TE", Name = "Telugu", SupportsFormality = false },
            new() { Language = "TG", Name = "Tajik", SupportsFormality = false },
            new() { Language = "TH", Name = "Thai", SupportsFormality = true },
            new() { Language = "TK", Name = "Turkmen", SupportsFormality = false },
            new() { Language = "TL", Name = "Tagalog", SupportsFormality = false },
            new() { Language = "TN", Name = "Tswana", SupportsFormality = false },
            new() { Language = "TR", Name = "Turkish", SupportsFormality = true },
            new() { Language = "TS", Name = "Tsonga", SupportsFormality = false },
            new() { Language = "TT", Name = "Tatar", SupportsFormality = false },
            new() { Language = "UK", Name = "Ukrainian", SupportsFormality = true },
            new() { Language = "UR", Name = "Urdu", SupportsFormality = false },
            new() { Language = "UZ", Name = "Uzbek", SupportsFormality = false },
            new() { Language = "VI", Name = "Vietnamese", SupportsFormality = true },
            new() { Language = "WO", Name = "Wolof", SupportsFormality = false },
            new() { Language = "XH", Name = "Xhosa", SupportsFormality = false },
            new() { Language = "YI", Name = "Yiddish", SupportsFormality = false },
            new() { Language = "YUE", Name = "Cantonese", SupportsFormality = false },
            new() { Language = "ZH", Name = "Chinese (all Chinese variants)", SupportsFormality = true },
            new() { Language = "ZU", Name = "Zulu", SupportsFormality = false }
        ];

        public static List<SupportedLanguage> SupportedLanguagesVariants { get; } =
        [
            new() { Language = "EN-GB", Name = "English (British)", SupportsFormality = true },
            new() { Language = "EN-US", Name = "English (American)", SupportsFormality = true },
            new() { Language = "ES-419", Name = "Spanish (Latin American)", SupportsFormality = true },
            new() { Language = "PT-BR", Name = "Portuguese (Brazilian)", SupportsFormality = true },
            new()
            {
                Language = "PT-PT",
                Name = "Portuguese (all Portuguese variants excluding Brazilian Portuguese)",
                SupportsFormality = true
            },
            new() { Language = "ZH-HANS", Name = "Chinese (simplified)", SupportsFormality = true },
            new() { Language = "ZH-HANT", Name = "Chinese (traditional)", SupportsFormality = true }
        ];


        public static List<SupportedLanguage> GetSupportedLanguages(string type, string apiKey, string chosenBaseUrl)
        {
            var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = AppInitializer.Client.PostAsync($"{chosenBaseUrl}/languages", content).Result;
            response.EnsureSuccessStatusCode();

            var supportedLanguages = JsonConvert.DeserializeObject<List<SupportedLanguage>>(response.Content?.ReadAsStringAsync().Result ?? "[]");

            //TODO: Remove this class and related properties when DeepL fixes the "/languages" endpoint
            AddListOfSupportedLanguagesNotInTheApiResponse(ref supportedLanguages, type);

            return supportedLanguages;
        }

        private static void AddListOfSupportedLanguagesNotInTheApiResponse(ref List<SupportedLanguage> supportedLanguages, string type)
        {
            supportedLanguages.AddRange(AllSupportedSourceLanguages);
            if (type == "target") supportedLanguages.AddRange(SupportedLanguagesVariants);

            supportedLanguages = supportedLanguages
                .GroupBy(l => l.Language.Trim().ToUpperInvariant())
                .Select(g => g.First())
                .ToList();
        }
    }
}