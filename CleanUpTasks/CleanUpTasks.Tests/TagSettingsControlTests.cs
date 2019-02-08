using System;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TagSettingsControlTests
    {
        [Fact]
        public void SetSettingsThrowsOnNull()
        {
            var control = new TagsSettingsControl();
            Assert.Throws<ArgumentNullException>(() => control.SetSettings(null));
        }
    }
}