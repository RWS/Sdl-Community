
using System.Text.Json.Serialization;

namespace Sdl.Community.DeeplAddon.Models
{
    public class Endpoints
    {
        /// <summary>
        /// The LanguageCloud Automatic Task Submit endpoint.
        /// </summary>
        [JsonPropertyName("lc.automatictask.submit")]
        public string LCAutomaticTaskSubmit { get; set; }
    }
}