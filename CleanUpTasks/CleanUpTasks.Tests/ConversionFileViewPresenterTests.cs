using System;
using System.IO;
using NSubstitute;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Dialogs;
using SDLCommunityCleanUpTasks.Models;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class ConversionFileViewPresenterTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void CheckSaveButtonDisablesButton()
        {
            // Arrange
            var view = utility.CreateConversionFileView();
            var dialog = Substitute.For<IFileDialog>();

            var presenter = new ConversionFileViewPresenter(view, dialog, ConversionFileViewMode.New, BatchTaskMode.Source);

            // Act
            presenter.CheckSaveButton();

            // Assert
            Assert.True(view.SaveButton.Enabled == false);
        }

        [Fact]
        public void CheckSaveButtonEnablesButton()
        {
            // Arrange
            var view = utility.CreateConversionFileView(convItem: new ConversionItem());
            var dialog = Substitute.For<IFileDialog>();

            var presenter = new ConversionFileViewPresenter(view, dialog, ConversionFileViewMode.New, BatchTaskMode.Source);

            // Act
            presenter.CheckSaveButton();

            // Assert
            Assert.True(view.SaveButton.Enabled == true);
            Assert.True(view.Replace.Enabled == true);
        }

        [Fact]
        public void ConstructorThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ConversionFileViewPresenter(null, null, ConversionFileViewMode.New, BatchTaskMode.Source));
        }

        [Fact(Skip = "Integration Test")]
        //[Fact]
        public void SaveFileSerializesToFile()
        {
            // Arrange
            var view = utility.CreateConversionFileView(convItem: new ConversionItem()
            {
                Search = new SearchText()
                {
                    Text = "Hello"
                },
                Replacement = new ReplacementText()
                {
                    Text = "There"
                }
            });

            var dialog = Substitute.For<IFileDialog>();
            var savePath = utility.CreatePath("savefiletest.xml");
            dialog.SaveFile(utility.SaveFolder).Returns(savePath);

            var presenter = new ConversionFileViewPresenter(view, dialog, ConversionFileViewMode.New, BatchTaskMode.Source);

            // Act
            presenter.SaveFile(utility.SaveFolder, true);

            // Assert
            Assert.True(File.Exists(savePath));
        }

        #region Fixture

        private readonly TestUtilities utility = null;

        public ConversionFileViewPresenterTests(TestUtilities utility)
        {
            this.utility = utility;
        }

        #endregion Fixture
    }
}