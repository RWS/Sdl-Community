using LanguageWeaverProvider.CohereSubscription.Decision.Services;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins DET-421's active-trial schedule: silent for days 14-8, then a countdown over the final week.
    /// </summary>
    public class ActiveTrialPromptScheduleTests
    {
        private static CohereSubscriptionData TrialWith(int? daysRemaining) => new CohereSubscriptionData
        {
            IsCohereDetected = true,
            IsTrial = true,
            IsTrialExpired = false,
            IsPaid = false,
            IsAdmin = true,
            TrialRemainingDays = daysRemaining
        };

        [Theory]
        [InlineData(14)]
        [InlineData(10)]
        [InlineData(8)]
        public void EarlyInTheTrial_ShowsNothing(int daysRemaining)
        {
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(daysRemaining));

            Assert.Null(viewModel);
        }

        [Theory]
        [InlineData(7)]
        [InlineData(3)]
        [InlineData(1)]
        [InlineData(0)]
        public void OverTheFinalWeek_Prompts(int daysRemaining)
        {
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(daysRemaining));

            Assert.NotNull(viewModel);
        }

        [Fact]
        public void SevenDaysLeft_IsTheFirstDayThatPrompts()
        {
            // The threshold is inclusive; 8 is the last silent day.
            Assert.Null(new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(8)));
            Assert.NotNull(new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(7)));
        }

        [Fact]
        public void AnUnknownDayCount_ShowsNothing()
        {
            // The trial endpoint could not be read. Prompting here would mean copy that cannot say how long
            // is left - precisely what the schedule exists to prevent.
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(null));

            Assert.Null(viewModel);
        }

        [Fact]
        public void APaidAccount_IsNeverPromptedRegardlessOfDaysLeft()
        {
            var data = TrialWith(2);
            data.IsPaid = true;

            Assert.Null(new CohereSubscriptionDecisionService().BuildViewModel(data));
        }

        [Theory]
        [InlineData(0, "ends today")]
        [InlineData(1, "ends in 1 day")]
        [InlineData(5, "ends in 5 days")]
        public void TheCopy_NamesTheDaysRemaining(int daysRemaining, string expected)
        {
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(TrialWith(daysRemaining));

            Assert.Contains(expected, viewModel.Description);
        }

        [Fact]
        public void TheNonAdminCopy_AlsoNamesTheDaysRemaining()
        {
            var data = TrialWith(4);
            data.IsAdmin = false;

            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(data);

            Assert.Contains("ends in 4 days", viewModel.Description);
        }
    }
}
