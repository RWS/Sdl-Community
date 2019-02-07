using System;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class ConversionSettingsControlTests
    {
        [Fact]
        public void SetPresenterThrowsOnNull()
        {
            var control = new ConversionsSettingsControl();

            Assert.Throws<ArgumentNullException>(() => control.SetPresenter(null));
        }

        [Fact]
        public void SetSettingsThrowsOnNull()
        {
            var control = new ConversionsSettingsControl();

            Assert.Throws<ArgumentNullException>(() => control.SetSettings(null, BatchTaskMode.Source));
        }
    }
}