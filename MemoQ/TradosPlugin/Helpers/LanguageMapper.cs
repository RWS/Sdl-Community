using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using TMProvider;

namespace TradosPlugin
{
    public static class LanguageMapper
    {
        /// <summary>
        /// The dictionary for mapping the CultureInfo language code to memoQ language codes, for all languages.
        /// </summary>
        private static Dictionary<string, Language> languages = new Dictionary<string, Language>();
        // no such mapping
        //private static Dictionary<string, string> threeLetterToTwoLetter = new Dictionary<string, string>();
        static LanguageMapper()
        {
            fillLanguages();
        }

        /// <summary>
        /// Gets the 3+2 letter memoQ code. RETURNS NULL if there's no such language.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns>Null, if the code for the full culture is not found.</returns>
        public static string GetThreeLetterMemoQCodeFull(string cultureName)
        {
            Language l;
            if (languages.TryGetValue(cultureName.ToLower(), out l)) return l.ThreeLetterCodeFull;
            else return null;
        }

        /// <summary>
        /// Gets the 3+2 letter memoQ code for the culture's main language (first 2 letters). RETURNS NULL if there's no such language.
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns>Null, if the code for the full culture is not found.</returns>        
        public static string GetThreeLetterMemoCodeForMainLang(string cultureName)
        {
            if (cultureName.Length > 2 && cultureName[2] == '-')
            {
                Language l;
                // first 2 letters of code
                // no such languages in our list
                if (!languages.TryGetValue(cultureName.ToLower().Substring(0, 2), out l)) return null;
                else
                {
                    if (l.ThreeLetterCodeFull.Length < 3) throw new InvalidOperationException("The language code must contain at least 3 letters.");
                    else return l.ThreeLetterCodeFull.Substring(0,3);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the 3+2 letter memoQ code for the full culture, or for the main language if the full culture doesn't exist. RETURNS NULL if nothing is found.
        /// </summary>
        /// <returns>Null, if the code for the full culture is not found.</returns>        
        public static string GetThreeLetterForMainLangIfDoesntExist(string cultureName)
        {
            string code = GetThreeLetterMemoQCodeFull(cultureName);
            if (code != null) return code;
            else
            {
               code = GetThreeLetterMemoCodeForMainLang(cultureName);
               return code;
            }
        }

        /// <summary>
        /// Tries to find a cultureinfo for the code. But if there are several cultures that could be mapped to a code, and we don't know which one
        ///  is the real one, we'll return null. If the language has no variant (eg. Hungarian), still the culture with the variant is returned. (HU-hu)
        /// </summary>
        /// <param name="threeLetterCode"></param>
        /// <returns>A matching cultureinfo, or null if the mapping would be ambiguous or the language can't be found.</returns>
        //public static CultureInfo GetCultureInfoFrom3LetterCode(string threeLetterCode)
        //{
        //    string code = null;
        //    CultureInfo c = null;
        //    threeLetterToTwoLetter.TryGetValue(threeLetterCode, out code);
        //    if (!String.IsNullOrEmpty(code)) c = new CultureInfo(code);
        //    return c;
        //}

        //public static string GetThreeLetterMemoQCodeMain(string cultureName)
        //{
        //    Language l;
        //    if (languages.TryGetValue(cultureName.ToLower(), out l)) return l.ThreeLetterCodeMain;
        //    else return null;
        //}

        /// <summary>
        /// Fills the mapper with all the languages that can be mapped.
        /// </summary>
        private static void fillLanguages()
        {
            languages.Add("af", new Language("afr"));
            languages.Add("af-za", new Language("afr"));
            languages.Add("am", new Language("amh"));
            languages.Add("am-et", new Language("amh"));
            languages.Add("ar", new Language("ara"));
            languages.Add("ar-ae", new Language("ara-ae"));
            languages.Add("ar-bh", new Language("ara-bh"));
            languages.Add("ar-dz", new Language("ara-dz"));
            languages.Add("ar-eg", new Language("ara-eg"));
            languages.Add("ar-iq", new Language("ara-iq"));
            languages.Add("ar-jo", new Language("ara-jo"));
            languages.Add("ar-kw", new Language("ara-kw"));
            languages.Add("ar-lb", new Language("ara-lb"));
            languages.Add("ar-ly", new Language("ara-ly"));
            languages.Add("ar-ma", new Language("ara-ma"));
            languages.Add("ar-om", new Language("ara-om"));
            languages.Add("ar-qa", new Language("ara-qa"));
            languages.Add("ar-sa", new Language("ara-sa"));
            languages.Add("ar-sy", new Language("ara-sy"));
            languages.Add("ar-tn", new Language("ara-tn"));
            languages.Add("ar-ye", new Language("ara-ye"));
            languages.Add("as", new Language("asm"));
            languages.Add("as-in", new Language("asm"));
            languages.Add("az", new Language("aze"));
            languages.Add("az-cyrl-az", new Language("azf"));
            languages.Add("az-latn-az", new Language("aze"));
            languages.Add("be", new Language("bel"));
            languages.Add("bg", new Language("bul"));
            languages.Add("bg-bg", new Language("bul"));
            languages.Add("bn", new Language("ben"));
            languages.Add("bn-bd", new Language("ben-bd"));
            languages.Add("bn-in", new Language("ben-in"));
            languages.Add("bs-cyrl", new Language("boc"));
            languages.Add("bs-cyrl-ba", new Language("boc"));
            languages.Add("bs-latn", new Language("bos"));
            languages.Add("bs-latn-ba", new Language("bos"));
            languages.Add("ca", new Language("cat"));
            languages.Add("cs", new Language("cze"));
            languages.Add("cs-cz", new Language("cze"));
            languages.Add("cy-gb", new Language("wel"));
            languages.Add("da", new Language("dan"));
            languages.Add("da-dk", new Language("dan"));
            languages.Add("de", new Language("ger"));
            languages.Add("de-at", new Language("ger-at"));
            languages.Add("de-ch", new Language("ger-ch"));
            languages.Add("de-de", new Language("ger-de"));
            languages.Add("de-li", new Language("ger-li"));
            languages.Add("de-lu", new Language("ger-lu"));
            languages.Add("el", new Language("gre"));
            languages.Add("en", new Language("eng"));
            languages.Add("en-029", new Language("eng-cb"));
            languages.Add("en-au", new Language("eng-au"));
            languages.Add("en-bz", new Language("eng-bz"));
            languages.Add("en-ca", new Language("eng-ca"));
            languages.Add("en-gb", new Language("eng-gb"));
            languages.Add("en-ie", new Language("eng-ie"));
            languages.Add("en-jm", new Language("eng-jm"));
            languages.Add("en-my", new Language("eng"));
            languages.Add("en-nz", new Language("eng-nz"));
            languages.Add("en-ph", new Language("eng-ph"));
            languages.Add("en-tt", new Language("eng-tt"));
            languages.Add("en-us", new Language("eng-us"));
            languages.Add("en-za", new Language("eng-za"));
            languages.Add("en-zw", new Language("eng-zw"));
            languages.Add("eo", new Language("epo"));
            languages.Add("es", new Language("spa"));
            languages.Add("es-ar", new Language("spa-ar"));
            languages.Add("es-bo", new Language("spa-bo"));
            languages.Add("es-cl", new Language("spa-cl"));
            languages.Add("es-co", new Language("spa-co"));
            languages.Add("es-cr", new Language("spa-cr"));
            languages.Add("es-do", new Language("spa-do"));
            languages.Add("es-ec", new Language("spa-ec"));
            languages.Add("es-es", new Language("spa-es"));
            languages.Add("es-gt", new Language("spa-gt"));
            languages.Add("es-hn", new Language("spa-hn"));
            languages.Add("es-mx", new Language("spa-mx"));
            languages.Add("es-ni", new Language("spa-ni"));
            languages.Add("es-pa", new Language("spa-pa"));
            languages.Add("es-pe", new Language("spa-pe"));
            languages.Add("es-pr", new Language("spa-pr"));
            languages.Add("es-py", new Language("spa-py"));
            languages.Add("es-sv", new Language("spa-sv"));
            languages.Add("es-us", new Language("spa-us"));
            languages.Add("es-uy", new Language("spa-uy"));
            languages.Add("es-ve", new Language("spa-ve"));
            languages.Add("et", new Language("est"));
            languages.Add("et-ee", new Language("est"));
            languages.Add("eu", new Language("baq"));
            languages.Add("eu-es", new Language("baq"));
            languages.Add("fi", new Language("fin"));
            languages.Add("fi-fi", new Language("fin"));
            languages.Add("fil-ph", new Language("fil"));
            languages.Add("fr", new Language("fre"));
            languages.Add("fr-be", new Language("fre-be"));
            languages.Add("fr-ca", new Language("fre-ca"));
            languages.Add("fr-ch", new Language("fre-ch"));
            languages.Add("fr-fr", new Language("fre-fr"));
            languages.Add("fr-lu", new Language("fre-lu"));
            languages.Add("fr-mc", new Language("fre-mc"));
            languages.Add("fy-nl", new Language("fry"));
            languages.Add("ga-ie", new Language("gle"));
            languages.Add("gl", new Language("glg"));
            languages.Add("gl-es", new Language("glg"));
            languages.Add("gu", new Language("guj"));
            languages.Add("ha-latn-ng", new Language("hau"));
            languages.Add("he", new Language("heb"));
            languages.Add("he-il", new Language("heb"));
            languages.Add("hi", new Language("hin"));
            languages.Add("hi-in", new Language("hin"));
            languages.Add("hr", new Language("hrv"));
            languages.Add("hr-ba", new Language("hrv"));
            languages.Add("hr-hr", new Language("hrv"));
            languages.Add("hu", new Language("hun"));
            languages.Add("hu-hu", new Language("hun"));
            languages.Add("hy", new Language("hye"));
            languages.Add("hy-am", new Language("hye"));
            languages.Add("id", new Language("ind"));
            languages.Add("id-id", new Language("ind"));
            languages.Add("ig", new Language("ibo"));
            languages.Add("is", new Language("ice"));
            languages.Add("is-is", new Language("ice"));
            languages.Add("it", new Language("ita"));
            languages.Add("it-ch", new Language("ita-ch"));
            languages.Add("it-it", new Language("ita-it"));
            languages.Add("ja", new Language("jpn"));
            languages.Add("ja-jp", new Language("jpn"));
            languages.Add("ka", new Language("kat"));
            languages.Add("ka-ge", new Language("kat"));
            languages.Add("kk", new Language("kaz"));
            languages.Add("kk-kz", new Language("kaz"));
            languages.Add("km-kh", new Language("khm"));
            languages.Add("kn", new Language("kan"));
            languages.Add("kn-in", new Language("kan"));
            languages.Add("ko", new Language("kor"));
            languages.Add("ko-kr", new Language("kor"));
            languages.Add("ky", new Language("kir"));
            languages.Add("ky-kg", new Language("kir"));
            languages.Add("lb-lu", new Language("ltz"));
            languages.Add("lo-la", new Language("lao"));
            languages.Add("lt", new Language("lit"));
            languages.Add("lt-lt", new Language("lit"));
            languages.Add("lv", new Language("lav"));
            languages.Add("lv-lv", new Language("lav"));
            languages.Add("mi-nz", new Language("mri"));
            languages.Add("mk", new Language("mac"));
            languages.Add("mk-mk", new Language("mac"));
            languages.Add("ml-in", new Language("mal"));
            languages.Add("mn", new Language("khk"));
            languages.Add("mn-mn", new Language("khk"));
            languages.Add("mn-mong-cn", new Language("khk"));
            languages.Add("mr", new Language("mar"));
            languages.Add("mr-in", new Language("mar"));
            languages.Add("ms", new Language("msa"));
            languages.Add("ms-bn", new Language("msa"));
            languages.Add("ms-my", new Language("msa"));
            languages.Add("mt-mt", new Language("mlt"));
            languages.Add("nb-no", new Language("nnb"));
            languages.Add("ne-np", new Language("nep"));
            languages.Add("nl", new Language("dut"));
            languages.Add("nl-be", new Language("dut-be"));
            languages.Add("nl-nl", new Language("dut-nl"));
            languages.Add("nn-no", new Language("nno"));
            languages.Add("no", new Language("nor"));
            languages.Add("nso-za", new Language("sot"));
            languages.Add("or-in", new Language("ori"));
            languages.Add("pa", new Language("pan"));
            // languages.Add("pa", new Language("pnb")); -- there's only one punjabi, what to do? have several mappings?
            languages.Add("pa-in", new Language("pan"));
            languages.Add("pl", new Language("pol"));
            languages.Add("pl-pl", new Language("pol"));
            languages.Add("prs-af", new Language("prs"));
            languages.Add("ps-af", new Language("pbu"));
            languages.Add("pt", new Language("por"));
            languages.Add("pt-br", new Language("por-br"));
            languages.Add("pt-pt", new Language("por-pt"));
            languages.Add("ro", new Language("rum"));
            languages.Add("ro-ro", new Language("rum"));
            languages.Add("ru", new Language("rus"));
            languages.Add("ru-ru", new Language("rus"));
            languages.Add("rw-rw", new Language("kin"));
            languages.Add("sa", new Language("san"));
            languages.Add("sa-in", new Language("san"));
            languages.Add("si-lk", new Language("sin"));
            languages.Add("sk", new Language("slo"));
            languages.Add("sk-sk", new Language("slo"));
            languages.Add("sl", new Language("slv"));
            languages.Add("sl-si", new Language("slv"));
            languages.Add("sq-al", new Language("alb"));
            languages.Add("sr-cyrl-ba", new Language("scc"));
            languages.Add("sr-cyrl-cs", new Language("scc"));
            languages.Add("sr-cyrl-me", new Language("scc"));
            languages.Add("sr-cyrl-sp", new Language("scc"));
            languages.Add("sr-latn-ba", new Language("scr"));
            languages.Add("sr-latn-cs", new Language("scr"));
            languages.Add("sr-latn-me", new Language("scr"));
            languages.Add("sr-latn-sp", new Language("scr"));
            languages.Add("sv", new Language("swe"));
            languages.Add("sv-fi", new Language("swe-fi"));
            languages.Add("sv-se", new Language("swe-se"));
            languages.Add("sw", new Language("swa"));
            languages.Add("sw-ke", new Language("swa"));
            languages.Add("ta", new Language("tam"));
            languages.Add("ta-in", new Language("tam"));
            languages.Add("te", new Language("tel"));
            languages.Add("te-in", new Language("tel"));
            languages.Add("tg-cyrl-tj", new Language("tgk"));
            languages.Add("th", new Language("tha"));
            languages.Add("th-th", new Language("tha"));
            languages.Add("tk-tm", new Language("tuk"));
            languages.Add("tn-za", new Language("tsn"));
            languages.Add("tr", new Language("tur"));
            languages.Add("tr-tr", new Language("tur"));
            languages.Add("tzm-latn-dz", new Language("tzm"));
            languages.Add("uk", new Language("ukr"));
            languages.Add("uk-ua", new Language("ukr"));
            languages.Add("ur", new Language("urd"));
            languages.Add("ur-pk", new Language("urd"));
            languages.Add("uz", new Language("uzn"));
            languages.Add("uz-cyrl-uz", new Language("uzn"));
            languages.Add("uz-latn-uz", new Language("uzb"));
            languages.Add("vi", new Language("vie"));
            languages.Add("vi-vn", new Language("vie"));
            languages.Add("xh-za", new Language("xho"));
            languages.Add("yo-ng", new Language("yor"));
            languages.Add("zh-cn", new Language("zho-cn"));
            languages.Add("zh-hans", new Language("zho-cn"));
            languages.Add("zh-hant", new Language("zho-tw"));
            languages.Add("zh-hk", new Language("zho-hk"));
            languages.Add("zh-mo", new Language("zho-mo"));
            languages.Add("zh-sg", new Language("zho-sg"));
            languages.Add("zh-tw", new Language("zho-tw"));
            languages.Add("zu-za", new Language("zul"));
            // other (missing) language codes
            languages.Add("sq", new Language("alb"));
            languages.Add("az-cy", new Language("azf"));
            languages.Add("az-lt", new Language("aze"));
            languages.Add("sh-b2", new Language("bos"));
            languages.Add("sh-hr", new Language("hrv"));
            languages.Add("sh-b1", new Language("hrv"));
            languages.Add("en-cb", new Language("eng-cb"));
            languages.Add("en-uk", new Language("eng-gb"));
            languages.Add("fa", new Language("fas"));
            languages.Add("ml", new Language("mal"));
            languages.Add("mt", new Language("mlt"));
            languages.Add("mi", new Language("mri"));
            languages.Add("ne", new Language("nep"));
            languages.Add("ns", new Language("sot"));
            languages.Add("no-no", new Language("nnb"));
            languages.Add("no-ny", new Language("nno"));
            languages.Add("ga-ct", new Language("gla"));
            languages.Add("sh-yu", new Language("scc"));
            languages.Add("sh-b4", new Language("scc"));
            languages.Add("sh-sr", new Language("scr"));
            languages.Add("sh-b3", new Language("scr"));
            languages.Add("st", new Language("sot"));
            languages.Add("es-em", new Language("spa"));
            languages.Add("tl", new Language("tgl"));
            languages.Add("tn", new Language("tsn"));
            languages.Add("uz-cy", new Language("uzn"));
            languages.Add("uz-lt", new Language("uzb"));
            languages.Add("cy", new Language("wel"));
            languages.Add("xh", new Language("xho"));
            languages.Add("zu", new Language("zul"));
            languages.Add("fr-cd", new Language("fre-ca"));


            // no such mapping!
            // backwards
            // languages with no 1:n mapping are not in the list (eg. hrv)
            //threeLetterToTwoLetter.Add("afr", "af-ZA");
            //threeLetterToTwoLetter.Add("alb", "sq-AL");
            //threeLetterToTwoLetter.Add("amh", "am-ET");
            //threeLetterToTwoLetter.Add("asm", "as-IN");
            //threeLetterToTwoLetter.Add("aze", "az-Latn-AZ");
            //threeLetterToTwoLetter.Add("azf", "az-Cyrl-AZ");
            //threeLetterToTwoLetter.Add("baq", "eu-ES");
            //threeLetterToTwoLetter.Add("bel", "be");
            //threeLetterToTwoLetter.Add("ben", "bn");
            //threeLetterToTwoLetter.Add("ben-BD", "bn-BD");
            //threeLetterToTwoLetter.Add("ben-IN", "bn-IN");
            //threeLetterToTwoLetter.Add("bos", "bs-Latn");
            //threeLetterToTwoLetter.Add("boc", "bs-Cyrl");
            //threeLetterToTwoLetter.Add("bul", "bg-BG");
            //threeLetterToTwoLetter.Add("cat", "ca");
            //threeLetterToTwoLetter.Add("cze", "cs-CZ");
            //threeLetterToTwoLetter.Add("dan", "da-dk");
            //threeLetterToTwoLetter.Add("dut", "nl");
            //threeLetterToTwoLetter.Add("dut-NL", "nl-NL");
            //threeLetterToTwoLetter.Add("dut-BE", "nl-BE");
            //threeLetterToTwoLetter.Add("eng-AU", "en-au");
            //threeLetterToTwoLetter.Add("eng-BZ", "en-bz");
            //threeLetterToTwoLetter.Add("eng-CA", "en-ca");
            //threeLetterToTwoLetter.Add("eng-CB", "en-029");
            //threeLetterToTwoLetter.Add("eng-IE", "en-ie");
            //threeLetterToTwoLetter.Add("eng-JM", "en-jm");
            //threeLetterToTwoLetter.Add("eng-NZ", "en-nz");
            //threeLetterToTwoLetter.Add("eng-PH", "en-ph");
            //threeLetterToTwoLetter.Add("eng-ZA", "en-za");
            //threeLetterToTwoLetter.Add("eng-TT", "en-tt");
            //threeLetterToTwoLetter.Add("eng-GB", "en-gb");
            //threeLetterToTwoLetter.Add("eng-US", "en-us");
            //threeLetterToTwoLetter.Add("eng-ZW", "en-zw");
            //threeLetterToTwoLetter.Add("epo", "eo");
            //threeLetterToTwoLetter.Add("est", "et-EE");
            //threeLetterToTwoLetter.Add("fil", "fil-PH");
            //threeLetterToTwoLetter.Add("fin", "fi-FI");
            //threeLetterToTwoLetter.Add("fre", "fr");
            //threeLetterToTwoLetter.Add("fre-BE", "fr-be");
            //threeLetterToTwoLetter.Add("fre-CA", "fr-ca");
            //threeLetterToTwoLetter.Add("fre-FR", "fr-fr");
            //threeLetterToTwoLetter.Add("fre-LU", "fr-lu");
            //threeLetterToTwoLetter.Add("fre-MC", "fr-mc");
            //threeLetterToTwoLetter.Add("fre-CH", "fr-ch");
            //threeLetterToTwoLetter.Add("fry", "fy-NL");
            //threeLetterToTwoLetter.Add("ger", "de");
            //threeLetterToTwoLetter.Add("ger-AT", "de-at");
            //threeLetterToTwoLetter.Add("ger-DE", "de-de");
            //threeLetterToTwoLetter.Add("ger-LI", "de-li");
            //threeLetterToTwoLetter.Add("ger-LU", "de-lu");
            //threeLetterToTwoLetter.Add("ger-CH", "de-ch");
            //threeLetterToTwoLetter.Add("gle", "ga-IE");
            //threeLetterToTwoLetter.Add("glg", "gl-ES");
            //threeLetterToTwoLetter.Add("gre", "el");
            //threeLetterToTwoLetter.Add("guj", "gu");
            //threeLetterToTwoLetter.Add("hau", "ha-Latn-NG");
            //threeLetterToTwoLetter.Add("hin", "hi-IN");
            //threeLetterToTwoLetter.Add("hun", "hu-HU");
            //threeLetterToTwoLetter.Add("hye", "hy-AM");
            //threeLetterToTwoLetter.Add("ibo", "ig");
            //threeLetterToTwoLetter.Add("ice", "is-IS");
            //threeLetterToTwoLetter.Add("ind", "id-id");
            //threeLetterToTwoLetter.Add("ita", "it");
            //threeLetterToTwoLetter.Add("ita-IT", "it-it");
            //threeLetterToTwoLetter.Add("ita-CH", "it-ch");
            //threeLetterToTwoLetter.Add("kaz", "kk-kz");
            //threeLetterToTwoLetter.Add("kan", "kn-in");
            //threeLetterToTwoLetter.Add("kat", "ka-GE");
            //threeLetterToTwoLetter.Add("khk", "mn-mn");
            //threeLetterToTwoLetter.Add("khm", "km-KH");
            //threeLetterToTwoLetter.Add("kir", "ky-kg");
            //threeLetterToTwoLetter.Add("lao", "lo-LA");
            //threeLetterToTwoLetter.Add("lav", "lv-lv");
            //threeLetterToTwoLetter.Add("lit", "lt-lt");
            //threeLetterToTwoLetter.Add("ltz", "lb-LU");
            //threeLetterToTwoLetter.Add("mac", "mk-mk");
            //threeLetterToTwoLetter.Add("mal", "ml-in");
            //threeLetterToTwoLetter.Add("mar", "mr-IN");
            //threeLetterToTwoLetter.Add("mlt", "mt-MT");
            //threeLetterToTwoLetter.Add("mri", "mi-NZ");
            //threeLetterToTwoLetter.Add("nep", "ne-NP");
            //threeLetterToTwoLetter.Add("nor", "no");
            //threeLetterToTwoLetter.Add("nnb", "nb-no");
            //threeLetterToTwoLetter.Add("nno", "nn-no");
            //threeLetterToTwoLetter.Add("ori", "or-IN");
            //threeLetterToTwoLetter.Add("pan", "pa-IN");
            //threeLetterToTwoLetter.Add("pnb", "pa");
            //threeLetterToTwoLetter.Add("pbu", "ps-AF");
            //threeLetterToTwoLetter.Add("pol", "pl-pl");
            //threeLetterToTwoLetter.Add("por", "pt");
            //threeLetterToTwoLetter.Add("por-PT", "pt-pt");
            //threeLetterToTwoLetter.Add("por-BR", "pt-br");
            //threeLetterToTwoLetter.Add("rum", "ro-ro");
            //threeLetterToTwoLetter.Add("rus", "ru-ru");
            //threeLetterToTwoLetter.Add("san", "sa-in");
            //threeLetterToTwoLetter.Add("sin", "si-LK");
            //threeLetterToTwoLetter.Add("slo", "sk-sk");
            //threeLetterToTwoLetter.Add("slv", "sl-si");
            //threeLetterToTwoLetter.Add("sot", "nso-za");
            //threeLetterToTwoLetter.Add("spa", "es");
            //threeLetterToTwoLetter.Add("spa-AR", "es-ar");
            //threeLetterToTwoLetter.Add("spa-BO", "es-bo");
            //threeLetterToTwoLetter.Add("spa-CL", "es-cl");
            //threeLetterToTwoLetter.Add("spa-CO", "es-co");
            //threeLetterToTwoLetter.Add("spa-CR", "es-cr");
            //threeLetterToTwoLetter.Add("spa-DO", "es-do");
            //threeLetterToTwoLetter.Add("spa-EC", "es-ec");
            //threeLetterToTwoLetter.Add("spa-SV", "es-sv");
            //threeLetterToTwoLetter.Add("spa-GT", "es-gt");
            //threeLetterToTwoLetter.Add("spa-HN", "es-hn");
            //threeLetterToTwoLetter.Add("spa-MX", "es-mx");
            //threeLetterToTwoLetter.Add("spa-NI", "es-ni");
            //threeLetterToTwoLetter.Add("spa-PA", "es-pa");
            //threeLetterToTwoLetter.Add("spa-PY", "es-py");
            //threeLetterToTwoLetter.Add("spa-PE", "es-pe");
            //threeLetterToTwoLetter.Add("spa-PR", "es-pr");
            //threeLetterToTwoLetter.Add("spa-ES", "es-es");
            //threeLetterToTwoLetter.Add("spa-UY", "es-uy");
            //threeLetterToTwoLetter.Add("spa-VE", "es-ve");
            //threeLetterToTwoLetter.Add("spa-US", "es-us");
            //threeLetterToTwoLetter.Add("swa", "sw-ke");
            //threeLetterToTwoLetter.Add("swe", "sv");
            //threeLetterToTwoLetter.Add("swe-SE", "sv-se");
            //threeLetterToTwoLetter.Add("swe-FI", "sv-fi");
            //threeLetterToTwoLetter.Add("tam", "ta-in");
            //threeLetterToTwoLetter.Add("tel", "te-in");
            //threeLetterToTwoLetter.Add("tgk", "tg-Cyrl-TJ");
            //threeLetterToTwoLetter.Add("tuk", "tk-TM");
            //threeLetterToTwoLetter.Add("tur", "tr-tr");
            //threeLetterToTwoLetter.Add("tzm", "tzm-Latn-DZ");
            //threeLetterToTwoLetter.Add("ukr", "uk-ua");
            //threeLetterToTwoLetter.Add("urd", "ur-pk");
            //threeLetterToTwoLetter.Add("uzn", "uz-Cyrl-UZ");
            //threeLetterToTwoLetter.Add("vie", "vi-vn");
            //threeLetterToTwoLetter.Add("wel", "cy-gb");
            //threeLetterToTwoLetter.Add("xho", "xh-ZA");
            //threeLetterToTwoLetter.Add("yor", "yo-NG");
            //threeLetterToTwoLetter.Add("zul", "zu-ZA");
            //threeLetterToTwoLetter.Add("ara", "ar");
            //threeLetterToTwoLetter.Add("ara-DZ", "ar-DZ");
            //threeLetterToTwoLetter.Add("ara-BH", "ar-BH");
            //threeLetterToTwoLetter.Add("ara-EG", "ar-EG");
            //threeLetterToTwoLetter.Add("ara-IQ", "ar-IQ");
            //threeLetterToTwoLetter.Add("ara-JO", "ar-JO");
            //threeLetterToTwoLetter.Add("ara-KW", "ar-KW");
            //threeLetterToTwoLetter.Add("ara-LB", "ar-LB");
            //threeLetterToTwoLetter.Add("ara-LY", "ar-LY");
            //threeLetterToTwoLetter.Add("ara-MA", "ar-MA");
            //threeLetterToTwoLetter.Add("ara-OM", "ar-OM");
            //threeLetterToTwoLetter.Add("ara-QA", "ar-QA");
            //threeLetterToTwoLetter.Add("ara-SA", "ar-SA");
            //threeLetterToTwoLetter.Add("ara-SY", "ar-SY");
            //threeLetterToTwoLetter.Add("ara-TN", "ar-TN");
            //threeLetterToTwoLetter.Add("ara-AE", "ar-AE");
            //threeLetterToTwoLetter.Add("ara-YE", "ar-YE");
            //threeLetterToTwoLetter.Add("heb", "he-il");
            //threeLetterToTwoLetter.Add("zho-HK", "zh-HK");
            //threeLetterToTwoLetter.Add("zho-MO", "zh-MO");
            //threeLetterToTwoLetter.Add("zho-CN", "zh-CN");
            //threeLetterToTwoLetter.Add("zho-SG", "zh-SG");
            //threeLetterToTwoLetter.Add("zho-TW", "zh-TW");
            //threeLetterToTwoLetter.Add("jpn", "ja-jp");
            //threeLetterToTwoLetter.Add("kor", "ko-kr");
            //threeLetterToTwoLetter.Add("tha", "th-th");
            //threeLetterToTwoLetter.Add("prs", "prs-AF");
            //threeLetterToTwoLetter.Add("kin", "rw-RW");
            //threeLetterToTwoLetter.Add("tsn", "tn-ZA");
            //threeLetterToTwoLetter.Add("uzb", "uz-Latn-UZ");

        }


    }
}
