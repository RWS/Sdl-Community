using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using System;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the day count derived from the trial-period response. The dates arrive as <c>yyyy/MM/dd</c>,
    /// which Newtonsoft does not parse by default, so <c>RemainingDays</c> parses them itself.
    /// </summary>
    public class TrialPeriodTests
    {
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
            var trial = new TrialPeriod();

            Assert.Null(trial.RemainingDays());
            Assert.Null(trial.Limit);
        }
    }
}
