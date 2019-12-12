using SDLCommunityCleanUpTasks.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class XmlReportGeneratorTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void GeneratesXml()
        {
            // Arrange
            var generator = new XmlReportGenerator(utility.SaveFolder);

            generator.AddFile("file1.xml");
            generator.AddLockItem("1", "locked content", "structure match");
            generator.AddTagItem("3", "<SomeTag>");
            generator.AddConversionItem("4", "before", "after", "searchText", "replaceText");

            // Act
            var xml = generator.ToString();

            // Assert
            Assert.NotNull(xml);
            output.WriteLine(xml);
        }

        #region Fixture

        private readonly ITestOutputHelper output;
        private readonly TestUtilities utility = null;

        public XmlReportGeneratorTests(TestUtilities utility, ITestOutputHelper output)
        {
            this.utility = utility;
            this.output = output;
        }

        #endregion Fixture
    }
}