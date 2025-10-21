using Newtonsoft.Json;
using NSubstitute;
using Trados_AI_Essentials.Interface;
using Trados_AI_Essentials.LC;
using Trados_AI_Essentials.Model;
using Trados_AI_Essentials.Model.Generative_Translation;

namespace Trados_AI_Essentials_Tests
{
    public class LCClientTests
    {
        private string GenerativeTranslationAddress { get; } =
            "https://api.eu.cloud.trados.com/lc-api/generative-translation/v1";

        [Fact]
        public async Task GetLLMTranslationEngines_ReturnsTranslationEnginesWithConnectedLLMs()
        {
            var responseString =
                "{\"items\":[{\"id\":\"6891f338e8610a6b72edd49e\",\"name\":\"1\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"669a0f2b9dbf0a59d7822bd6\",\"name\":\"1 DevTest\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"688b5d1df82caf62da0470e8\",\"name\":\"111111111\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"684fd2668bd4293da5a1a708\",\"name\":\"1_TE_en_de\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6839b1c18bd4293da5a1022c\",\"name\":\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAA\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6218c2bd1653710f88a23a73\",\"name\":\"Achim_bilingual_excel\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"669d68307ee8e50e6525e43e\",\"name\":\"Alternative Chameleon Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"637b2eb2775f8846a086965d\",\"name\":\"ArabicTE\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"616ee131e544324d88a9701d\",\"name\":\"Articulate RISE\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5f2c2e5c70ec4e19f5531634\",\"name\":\"Automotive\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6686832d2abbcc42a34174d8\",\"name\":\"Chameleon engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"668e31518be59b3664b44784\",\"name\":\"D1\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"684aed7b8bd4293da5a186a1\",\"name\":\"Dassault StringId Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"64462cd82d04d03cef2844e2\",\"name\":\"dDevTest\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"62987faa6bae312e0fb005c8\",\"name\":\"DeepL - new languages June 2022\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66f5110a932e4d0850ca3272\",\"name\":\"DeepL ZH\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6215f7ddbbcc9166a82d3913\",\"name\":\"Default Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"678cf3623358d5229512aed7\",\"name\":\"DEMO Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"621618bebbcc9166a82d39f0\",\"name\":\"Dev Testing - DeepL AddOn MT\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"67f8351b58a75c710b15c597\",\"name\":\"Dev Testing - Patrick en-it\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6217d2091653710f88a23778\",\"name\":\"Dev Testing Translation Engine 117\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"651d5978727a62340528ce5c\",\"name\":\"DevTest\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66a0e22b9dbf0a59d78244f6\",\"name\":\"DevTest ET\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"665f07d6e1c01b6ad58da216\",\"name\":\"DevTest2\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"68dbd443419b837040578fea\",\"name\":\"DevTesting - Connected AI\",\"definition\":{\"sequence\":{\"llm\":[\"68da3fe9f158e002080379d2\"]}}},{\"id\":\"68dbe0506169197b147e0531\",\"name\":\"DevTesting2 - Connected AI [Default]\",\"definition\":{\"sequence\":{\"llm\":[\"68da3fe9f158e002080379d2\"]}}},{\"id\":\"637c7ecdf67d700e9c20cb70\",\"name\":\"DLTest\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"64f5971c0d35c64fcd645fc6\",\"name\":\"En-De\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"60c0de8186f25f4e05fe0850\",\"name\":\"en-it-Patrick.A\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"68414dba8bd4293da5a1358c\",\"name\":\"Fleur\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6299d98a49ebc73d55278b9a\",\"name\":\"Google Only\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5fad3827d6e8c9438ad5ba73\",\"name\":\"Google V3 AutoML\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5f8b57a62f57c454877df604\",\"name\":\"Italian\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"64199fde07dbd92e73fbe581\",\"name\":\"KingFisher BMC\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6405f02707dbd92e73fb96b2\",\"name\":\"Language Weaver Stuff\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5f3c060b1a2acc71a1a4bc14\",\"name\":\"Literature\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"64b916206082ab007eee23dc\",\"name\":\"Lyds\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"65135ae7c182e835a35dbe49\",\"name\":\"Machine Translation Resources\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"67115e45af26bb29934e51ba\",\"name\":\"Many Lang\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"60663186de5b7c331708b2fd\",\"name\":\"MatsLinder\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5fcf8beec5d7a0349833875b\",\"name\":\"memsource comparison\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66a7a1fd271b01695053abcd\",\"name\":\"MSFT Test Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6241638b726aba6d9e256c2d\",\"name\":\"Nested files\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"62ac5b996bae312e0fb04886\",\"name\":\"New Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66a3e26a271b01695053a210\",\"name\":\"NJ_es-ES\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66a40b9a8f031a27d1347dbe\",\"name\":\"NJ_es-ES_MT_notPT\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"600158c688fbd02b33580e33\",\"name\":\"Paul\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66d5863e855b8104867262f5\",\"name\":\"Performance Amazon\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66d586657c9f755e376d5f5a\",\"name\":\"Performance DeepL\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66d5868b7c9f755e376d5f5b\",\"name\":\"Performance LW\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66bdfe3bec03c916b879dfbf\",\"name\":\"Pesudo-translate for Zh-KO\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"67458b26db8ba40641d7a5c0\",\"name\":\"PM Screen print\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66a0e9fe9dbf0a59d782453b\",\"name\":\"PMO-Tech DPK Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"669911097ee8e50e6525d726\",\"name\":\"PMO-Tech Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"65c4cf2d9b08946262500da6\",\"name\":\"PseudoTranslator\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66997c667ee8e50e6525d943\",\"name\":\"PT\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"629d180c6bae312e0fb0140b\",\"name\":\"Quick Cloud Projects (en(US) - de(DE))\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6759494d3358d5229511b8a9\",\"name\":\"ReplacementEngine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66bc863643d27a09214f4458\",\"name\":\"Resources\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"637b3125775f8846a086966a\",\"name\":\"RusTe\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"64dd304877409c403cd5967a\",\"name\":\"RWS Testing\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6298cc8549ebc73d5527874e\",\"name\":\"SDLPPX\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"63a2452c777b48722b61e332\",\"name\":\"sdlxliffs\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"61f07878bbcc9166a82cb418\",\"name\":\"Techy Stuff\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66d6e3f37c9f755e376d6590\",\"name\":\"test\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"669e57d69dbf0a59d7823880\",\"name\":\"Test\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5fb636c35bd2a67f453476bb\",\"name\":\"Test Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66fa40ef193f96313a5852bf\",\"name\":\"Test Translation Engine\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"672a0587e8ac597ccfad4985\",\"name\":\"Test22\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"6054609cde5b7c3317089449\",\"name\":\"TestDeepL\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"636d01b70878ce750966120a\",\"name\":\"testEng\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"5f33f7771a2acc71a1a4b4d3\",\"name\":\"Testing Paul\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"609a403e86f25f4e05fdbb2e\",\"name\":\"using cloud in local\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"637b2fcd775f8846a0869662\",\"name\":\"VietTE\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66eaef3371aaca173ba0808e\",\"name\":\"WelshIssue\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"618bf2e4aa1fc02a1f8f4ce7\",\"name\":\"XLIFF\",\"definition\":{\"sequence\":{\"llm\":[]}}},{\"id\":\"66bdfe93792bc549bca873cc\",\"name\":\"zh-ko\",\"definition\":{\"sequence\":{\"llm\":[]}}}],\"itemCount\":77}";
            var httpClient = Substitute.For<IHttpClient>();
            httpClient.SendAsync(null, null, null).ReturnsForAnyArgs(responseString);

            var lcClient = new LCClient(httpClient);
            var llmTranslationEngines = await lcClient.GetLLMTranslationEngines();

            Assert.Equal(2, llmTranslationEngines.Count);
        }

        [Fact]
        public async Task Translate_ShouldCallEndpointWithCorrectPayload()
        {
            // Arrange
            var request = new TranslationRequest
            {
                SourceLanguage = "en-US",
                TargetLanguage = "it-IT",
                Source = "The cardiac surgeon performed an open heart surgery to fix a damaged blood vessel.",
                TranslationProfileId = "68dbe0506169197b147e0531",
                UserPrompt = "Provide a translation of the following 'Source' text to 'Italian' maintaining all of the formatting tags in the translation.",
                IncludeUserResources = true
            };

            var responseSerialized =
                """{"translation":"Il cardiologo ha eseguito un'operazione coronarica per riparare un vaso sanguigno danneggiato","languagePairModel":{"sourceLanguageCode":"en-US","sourceLanguageName":"English (United States)","targetLanguageCode":"it-IT","targetLanguageName":"Italian (Italy)"},"usedTranslationResources":[{"resourceType":"Terminology","terminologyResources":[{"originResource":"Dev Exp Team - en-US - it-IT","sourceTerm":"open heart surgery","termTranslations":[{"termTranslation":"operazione coronarica","termStatus":"Preferred"}]},{"originResource":"Dev Exp Team - en-US - it-IT","sourceTerm":"blood vessel","termTranslations":[{"termTranslation":"arteria","termStatus":"Draft"},{"termTranslation":"vaso sanguigno","termStatus":"Preferred"}]},{"originResource":"Dev Exp Team - en-US - it-IT","sourceTerm":"cardiac surgeon","termTranslations":[{"termTranslation":"cardiologo","termStatus":"Preferred"}]}]}],"model":"eu.anthropic.claude-3-5-sonnet-20240620-v1:0"}""";

            var httpClient = Substitute.For<IHttpClient>();
            httpClient.SendAsync(null, null, null)
                .ReturnsForAnyArgs(responseSerialized);

            // Act
            var lcClient = new LCClient(httpClient);
            var result = await lcClient.TranslateAsync(request);

            // Assert
            await httpClient.Received(1).SendAsync(HttpMethod.Post, request,
                $"{GenerativeTranslationAddress}/translate?includeTranslationResources={request.IncludeUserResources}");

            Assert.Equivalent(JsonConvert.DeserializeObject<GenerativeTranslationResult>(responseSerialized), result);
        }
    }
}