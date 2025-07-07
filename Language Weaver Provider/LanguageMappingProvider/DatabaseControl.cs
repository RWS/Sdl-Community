using LanguageMappingProvider;
using System.Collections.Generic;
using System.Linq;
using Sdl.Core.Globalization;

namespace LanguageWeaverProvider.LanguageMappingProvider
{
	public static class DatabaseControl
    {
		public static LanguageMappingDatabase InitializeDatabase()
		{
			var supportedLanguages = GetLanguageCodes();
			return new LanguageMappingDatabase(Constants.DatabaseName, supportedLanguages);
		}

		private static List<LanguageMapping> GetLanguageCodes()
		{
			// delete this method when they create the endpoint to retrieve this
			var table = @"Afrikaans 	afr 	af
Albanian 	alb 	sq
Amharic 	amh 	am
Arabic 	ara 	ar
Arabic (Arabizi) 	arz 	ar-arabizi
Armenian 	arm 	hy
Azerbaijani 	aze 	az
Basque 	baq 	eu
Belarusian 	bel 	be
Bengali 	ben 	bn
Bihari 	bih 	bh
Bulgarian 	bul 	bg
Burmese 	bur 	my
Catalan 	cat 	ca
Cebuano 	ceb 	ceb
Cherokee 	chr 	chr
Chinese (Simplified Han, Hong Kong SAR) 	chi 	zh-Hans-HK
Chinese (Simplified Han, Macao SAR) 	chi 	zh-Hans-MO
Chinese (Simplified, China) 	chi 	zh-CN
Chinese (Simplified, Singapore) 	chi 	zh-SG
Chinese (Traditional, Hong Kong SAR) 	cht 	zh-HK
Chinese (Traditional, Macao SAR) 	cht 	zh-MO
Chinese (Traditional, Taiwan) 	cht 	zh-TW
Croatian 	hrv 	hr
Czech 	cze 	cs
Danish 	dan 	da
Dari 	fad 	prs-AF
Dutch 	dut 	nl
English 	eng 	en
Estonian 	est 	et
Finnish 	fin 	fi
French 	fra 	fr
French (Canada) 	frc 	fr-CA
Galician 	glg 	gl
Ganda 	lug 	lg
Georgian 	geo 	ka
German 	ger 	de
Greek 	gre 	el
Gujarati 	guj 	gu
Hausa 	hau 	ha
Hebrew 	heb 	he
Hindi 	hin 	hi
Hmong 	hmn 	hmn
Hungarian 	hun 	hu
Icelandic 	ice 	is
Indonesian 	ind 	id
Inuktitut 	iku 	iu
Irish 	gle 	ga
Italian 	ita 	it
Japanese 	jpn 	ja
Javanese 	jav 	jv
Kannada 	kan 	kn
Khmer 	khm 	km
Kinyarwanda 	kin 	rw
Korean 	kor 	ko
Kurdish 	kur 	ku
Latvian 	lav 	lv
Limbu 	lif 	lif
Lithuanian 	lit 	lt
Macedonian 	mac 	mk
Malay 	may 	ms
Malayalam 	mal 	ml
Maltese 	mlt 	mt
Marathi 	mar 	mr
Nepali 	nep 	ne
Norwegian 	nor 	no
Oriya 	ori 	or
Ossetian 	oss 	os
Pashto 	pus 	ps
Persian 	per 	fa
Polish 	pol 	pl
Portuguese 	por 	pt
Portuguese (Brazil) 	ptb 	pt-BR
Portuguese (Portugal) 	ptp 	pt-PT
Romanian 	rum 	ro
Russian 	rus 	ru
Serbian 	srp 	sr
Slovak 	slo 	sk
Slovenian 	slv 	sl
Somali 	som 	so
Spanish 	spa 	es
Sundanese 	sun 	su
Swahili 	swa 	sw
Swedish 	swe 	sv
Syriac 	syr 	syr
Tagalog 	tgl 	tl
Tajik 	tgk 	tg
Tamil 	tam 	ta
Telugu 	tel 	te
Thai 	tha 	th
Turkish 	tur 	tr
Ukrainian 	ukr 	uk
Urdu 	urd 	ur
Uzbek 	uzb 	uz
Vietnamese 	vie 	vi
Welsh 	wel 	cy
Yiddish 	yid 	yi";
			return GetLanguageCodes(table);
		}

		public static string GetLanguageCode(this CultureCode cultureCode)
		{
			var languageMappingDatabase = new LanguageMappingDatabase(Constants.DatabaseName, null);
			var languageMappings = languageMappingDatabase.GetMappedLanguages();
			var targetLanguage = languageMappings.FirstOrDefault(x => x.TradosCode == cultureCode.Name);
			var languageCode = targetLanguage?.LanguageCode ?? string.Empty;
			return languageCode;
		}

		private static List<LanguageMapping> GetLanguageCodes(string table)
		{
			var languageMappings = new List<LanguageMapping>();

			var lines = table.Split('\n');
			foreach (var line in lines)
			{
				var parts = line.Trim().Split('\t');
				var nameParts = parts[0].Trim().Split('(');
				var name = nameParts[0].Trim();
				var region = nameParts.Length > 1 ? nameParts[1].Trim(')', ' ') : null;
				var languageCodeParts = parts[1].Trim().Split(' ');
				var languageCode = languageCodeParts.Length > 1 ? languageCodeParts[1].Trim() : languageCodeParts[0].Trim();

				var mappedLanguage = new LanguageMapping
				{
					Name = name,
					Region = region,
					LanguageCode = languageCode
				};

				languageMappings.Add(mappedLanguage);
			}

			return languageMappings;
		}
	}
}