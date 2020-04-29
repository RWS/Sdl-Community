
namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    public sealed class StandardResponse<InnerType>
    {
        public InnerType data { get; set; }
        public object responseDetails = null;
        public System.Net.HttpStatusCode responseStatus = System.Net.HttpStatusCode.OK;
    }
}
