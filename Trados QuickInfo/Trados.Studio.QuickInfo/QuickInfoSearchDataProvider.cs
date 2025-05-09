using QuickInfo;
using Sdl.TellMe.ProviderApi;
using System.Collections.Generic;
using System.Drawing;
using TradosStudioQuickInfo.Processors;

namespace TradosStudioQuickInfo
{
    public class QuickInfoSearchDataProvider : ISearchDataProvider
    {
        public string Name => "info";

        public Icon ProviderIcon => PluginResources.info_1930258;
        public QuickInfoSearchDataProvider()
        {
           
        }

        public IEnumerable<ISearchDataProvider> GetProviderForQuery(string query)
        {
            if (query.ToLower().Contains("info")) yield return this;
        }
        public static Engine Instance { get; } = new Engine(
            typeof(Engine).Assembly,
            typeof(Ip).Assembly);
        public IEnumerable<ITellMeAction> SearchForSuggestion(string query)
        {
            //var engine = new Engine
            //(
            //    typeof(QuickInfo.Color),
            //    typeof(QuickInfo.DateTime),
            //    typeof(Emoticons),
            //    typeof(Factor),
            //    typeof(Hex),
            //    typeof(Roman),
            //    typeof(HttpStatusCode),
            //    typeof(QuickInfo.Math),
            //    typeof(NumberList),
            //    typeof(RandomGuid),
            //    typeof(UnitConverter),
            //    typeof(UrlDecode),
            //    typeof(Ip)

            //);


            var queryInfo = new Query(query);
            var answers = Instance.GetResults(queryInfo);

            foreach (var (processorName, resultNode) in answers)
            {
                var actions = QuickInfoActionRenderer.GenerateActions(processorName, resultNode);
                foreach (var action in actions)
                {
                    yield return action;
                }
            }
           
        }
    }
}
