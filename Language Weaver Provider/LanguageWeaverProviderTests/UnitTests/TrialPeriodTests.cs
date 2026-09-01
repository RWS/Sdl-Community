using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using System;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
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
        [InlineData("10/09/2026")]
        public void AnUnusableEndDate_IsUnknownRatherThanZero(string endDate)
        {
            var trial = new TrialPeriod { EndDate = endDate };

            Assert.Null(trial.RemainingDays());
        }

        [Fact]
        public void AnIsoStyleDate_IsAlsoAccepted()
        {
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
