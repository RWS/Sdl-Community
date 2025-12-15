using Newtonsoft.Json;
using System.Reflection;

namespace LanguageWeaverProvider.Services.Model;

public class EdgeUserPermissions
{
    [JsonProperty("MANAGE_FEEDBACK")]
    public bool ManageFeedback { get; set; }

    [JsonProperty("SUBMIT_FEEDBACK")]
    public bool SubmitFeedback { get; set; }

    public static EdgeUserPermissions CreateAllTrue()
    {
        var instance = new EdgeUserPermissions();
        foreach (var prop in typeof(EdgeUserPermissions).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                prop.SetValue(instance, true);
            }
        }

        return instance;
    }
}
