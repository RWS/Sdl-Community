using System;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services.Model
{
	public class CloudAccountSubscription
	{
		private const string ActiveValue = "yes";

		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("accountId")]
		public string AccountID { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("startDate")]
		public string StartDate { get; set; }

		[JsonProperty("endDate")]
		public string EndDate { get; set; }

		[JsonProperty("active")]
		public string Active { get; set; }

		[JsonIgnore]
		public bool IsActive => !string.IsNullOrEmpty(Active) && Active.Equals(ActiveValue) && !IsExpirated();

		private bool IsExpirated()
		{
			var currentDate = DateTime.Now;
			var endDate = DateTime.ParseExact(EndDate, "yyyy/MM/dd", null);

			return endDate < currentDate;
		}
	}
}
