using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Helpers
{
    public static class OptionsHelper
    {
        public static bool TryGetLanguagesSupported(string state, out Dictionary<string,string> result)
        {
            result = new Dictionary<string,string>();
            if (string.IsNullOrEmpty(state)) { return false; }

            try
            {
                var options = JsonConvert.DeserializeObject<TranslationOptions>(state);
                result = options?.LanguagesSupported ?? new Dictionary<string, string>();
                return result.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
