using Newtonsoft.Json;
using System;
using System.Globalization;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Model
{
    /// <summary>
    /// The trial period from <c>GET /account-portal/v1/weaver/trial</c>. Unlike the details endpoint this one
    /// carries dates, which is what makes DET-421's day-based prompt schedule implementable.
    /// <para>
    /// The account is identified by the <c>X-Tenant</c> header rather than a path segment, so the response
    /// describes whichever tenant the header named.
    /// </para>
    /// </summary>
    public class TrialPeriod
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>Length of the trial in days, as granted (14 at the time of writing).</summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Duplicates <c>trialStatus</c> from the details endpoint: NOT_STARTED, IN_PROGRESS, ENDED, CANCELLED.
        /// Entitlement is still decided from the details response; this is read only for the dates.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>Format is <c>yyyy/MM/dd</c>, which is not what Newtonsoft parses by default.</summary>
        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("subscriptionId")]
        public int? SubscriptionId { get; set; }

        /// <summary>
        /// Whole days from today until <see cref="EndDate"/>, or <c>null</c> when the date is absent or not in
        /// the expected format. Never negative: a date in the past reads as 0 days left rather than a
        /// nonsensical countdown.
        /// <para>
        /// Deliberately date-only arithmetic. The endpoint sends no time or offset, so treating it as an
        /// instant would make the number flip a day early or late depending on the user's clock; the count is
        /// of calendar days, which is what the copy claims.
        /// </para>
        /// </summary>
        public int? RemainingDays()
        {
            if (!TryParseDate(EndDate, out var endDate))
            {
                return null;
            }

            var daysLeft = (endDate - DateTime.Today).Days;
            return daysLeft < 0 ? 0 : daysLeft;
        }

        private static bool TryParseDate(string value, out DateTime date)
            => DateTime.TryParseExact(
                value,
                new[] { "yyyy/MM/dd", "yyyy-MM-dd" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date);
    }
}
