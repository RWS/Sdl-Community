using Newtonsoft.Json;

namespace GroupshareExcelAddIn.Models
{
    public class Termbase
    {
        public string Description { get; set; }
        public string Id { get; set; }
        [JsonProperty("LinkedOragnizationIds")]
        public string[] LinkedOrganizationIds { get; set; }
        public string Name { get; set; }
        [JsonProperty("ParentOragnizationId")]
        public string ParentOrganizationId { get; set; }
        public string ResourceType { get; set; }
    }
}