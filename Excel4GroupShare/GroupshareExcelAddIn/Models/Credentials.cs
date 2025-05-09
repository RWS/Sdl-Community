using Newtonsoft.Json;

namespace GroupshareExcelAddIn.Models
{
    public class Credentials
    {
        [JsonIgnore]
        public string Password { get; set; }
        public bool Remember { get; set; }
        public string ServerURI { get; set; }
        public string UserName { get; set; }
    }
}