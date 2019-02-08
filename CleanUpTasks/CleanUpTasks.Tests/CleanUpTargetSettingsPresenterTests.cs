using System;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class CleanUpTargetSettingsPresenterTests
    {
        [Fact]
        public void ConstructorThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CleanUpTargetSettingsPresenter(null, null));
        }
    }
}