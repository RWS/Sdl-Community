using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using Newtonsoft.Json;
using System;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the trial-period response from <c>GET /account-portal/v1/weaver/trial</c> and the day count
    /// derived from it. The dates arrive as <c>yyyy/MM/dd</c>, which Newtonsoft does not parse by default.
    /// </summary>
    public class TrialPeriodTests
    {
        [Fact]
        public void TheConfirmedResponseShape_Deserialises()
        {
            // The example given by the service team, verbatim.
            const string json = @"{
                ""id"": 403,
                ""limit"": 14,
                ""status"": ""IN_PROGRESS"",
                ""startDate"": ""2026/08/26"",
                ""endDate"": ""2026/09/10"",
                ""subscriptionId"": 1129
            }";

            var trial = JsonConvert.DeserializeObject<TrialPeriod>(json);

            Assert.Equal(403, trial.Id);
            Assert.Equal(14, trial.Limit);
            Assert.Equal("IN_PROGRESS", trial.Status);
            Assert.Equal("2026/08/26", trial.StartDate);
            Assert.Equal("2026/09/10", trial.EndDate);
            Assert.Equal(1129, trial.SubscriptionId);
        }

        [Fact]
        public void RemainingDays_CountsCalendarDaysToTheEndDate()
        {
            var trial = new TrialPeriod
            {
                EndDate = DateTime.Today.AddDays(7).ToString("yyyy/MM/dd")
            };

            Assert.Equal(7, trial.RemainingDays());
        }

        [Fact]
        public void ATrialEndingToday_HasZeroDaysLeft()
        {
            var trial = new TrialPeriod
            {
                EndDate = DateTime.Today.ToString("yyyy/MM/dd")
            };

            Assert.Equal(0, trial.RemainingDays());
        }

        [Fact]
        public void AnEndDateInThePast_ClampsToZeroRatherThanGoingNegative()
        {
            var trial = new TrialPeriod
            {
                EndDate = DateTime.Today.AddDays(-3).ToString("yyyy/MM/dd")
            };

            Assert.Equal(0, trial.RemainingDays());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not a date")]
        [InlineData("10/09/2026")] // day-first: ambiguous, and not the documented format
        public void AnUnusableEndDate_IsUnknownRatherThanZero(string endDate)
        {
            // Null and zero must stay distinct: zero means "ends today" and prompts, null means the countdown
            // could not be read and stays silent.
            var trial = new TrialPeriod { EndDate = endDate };

            Assert.Null(trial.RemainingDays());
        }

        [Fact]
        public void AnIsoStyleDate_IsAlsoAccepted()
        {
            // Not the documented format, but a plausible drift; cheaper to tolerate than to field a bug for.
            var trial = new TrialPeriod
            {
                EndDate = DateTime.Today.AddDays(2).ToString("yyyy-MM-dd")
            };

            Assert.Equal(2, trial.RemainingDays());
        }

        [Fact]
        public void AMissingEndDate_DoesNotThrow()
        {
            var trial = JsonConvert.DeserializeObject<TrialPeriod>(@"{""status"":""IN_PROGRESS""}");

            Assert.Null(trial.RemainingDays());
            Assert.Null(trial.Limit);
        }
    }
}
