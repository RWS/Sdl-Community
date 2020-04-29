namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class RequestField
    {
        public RequestField(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
