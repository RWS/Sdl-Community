namespace LanguageWeaverProvider.Model
{
    public class CloudCredentials
    {
        public string AccountId { get; set; }

        public string AccountRegion { get; set; }

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string ConnectionCode { get; set; }
        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public override string ToString()
        {
            var password = string.IsNullOrWhiteSpace(UserPassword) ? "NULL" : "PRESENT";
            var clientSecret = string.IsNullOrWhiteSpace(ClientSecret) ? "NULL" : "PRESENT";
            return $"AccountId: {AccountId}, AccountRegion: {AccountRegion}, UserName: {UserName}, " +
                   $"ClientID: {ClientID}, Password: {password}, ClientSecret: {clientSecret}, ConnectionCode: {ConnectionCode}";
        }
    }
}