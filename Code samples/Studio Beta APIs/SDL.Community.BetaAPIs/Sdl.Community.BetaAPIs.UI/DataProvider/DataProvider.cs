using Newtonsoft.Json;
using Sdl.Community.BetaAPIs.UI.DataProvider;
using Sdl.Community.BetaAPIs.UI.DesignTimeData;
using Sdl.Community.BetaAPIs.UI.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.BetaAPIs.UI.DataProvider
{
    public class JsonDataProvider : IAPIDataProvider
    {
        public JsonDataProvider()
        {
        }
        public IEnumerable<API> LoadAPIs()
        {
            string apiResource = LoadResource();
            return Deserialize(apiResource);

        }

        private IEnumerable<API> Deserialize(string apiResource)
        {
           return JsonConvert.DeserializeObject<List<API>>(apiResource);
        }

        private string LoadResource()
        {
            var assembly = typeof(JsonDataProvider).Assembly;
            var resourceName = "Sdl.Community.BetaAPIs.UI.Resources.apis.json";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
