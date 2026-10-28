using System.ComponentModel;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public enum TagHandlingVersion
    {
        [Description("v2")]
        [ApiValue("v2")]
        V2 = 0,

        [Description("v1")]
        [ApiValue("v1")]
        V1 = 1
    }
}
