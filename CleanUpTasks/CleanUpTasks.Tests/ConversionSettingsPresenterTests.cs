using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using NSubstitute;
using Sdl.Community.CleanUpTasks.Dialogs;
using Sdl.Community.CleanUpTasks.Models;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class ConversionSettingsPresenterTests : IClassFixture<TestUtilities>
    {
        [Fact]
        public void AddFileAddsToConversionFilesList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            var path = utility.CreatePath("somefile.xml");
            var directory = Path.GetDirectoryName(path);
            control.Settings.LastFileDirectory = directory;
            dialog.GetFile(directory).Returns(new List<string> { path });

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.AddFile();

            // Assert
            Assert.NotEmpty(control.Settings.ConversionFiles);
        }

        [Fact]
        public void AddFileAddsDoesNotAllowDuplicates()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            var path = utility.CreatePath("somefile.xml");
            var directory = Path.GetDirectoryName(path);
            control.Settings.LastFileDirectory = directory;
            dialog.GetFile(directory).Returns(new List<string> { path });

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.AddFile();
            presenter.AddFile();

            // Assert
            var count = control.Settings.ConversionFiles.Count;
            Assert.True(1 == count, $"Actual: {count}");
        }

        [Fact(Skip = "Shows AddFile throws on invalid file")]
        public void AddFileThrowsOnInvalidFile()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            var path = utility.CreatePath("somerandomfile.xml");
            var directory = Path.GetDirectoryName(path);
            control.Settings.LastFileDirectory = directory;
            dialog.GetFile(directory).Returns(new List<string> { path });

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => presenter.AddFile());
        }

        [Fact]
        public void ConstructorThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ConversionSettingsPresenter(null, null));
        }

        [Fact]
        public void GenerateFileAddsFileToFileList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();

            var dialog = Substitute.For<IFileDialog>();
            var view = Substitute.For<IConversionFileView>();

            view.ShowDialog().Returns(DialogResult.OK);
            view.BindingSource.Returns(new BindingSource());
            view.BindingSource.DataSource = new BindingListView<ConversionItem>(new List<ConversionItem>() { new ConversionItem() });
            view.SavedFilePath.Returns(utility.CreatePath("somefile.xml"));

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.GenerateFile(view);

            // Assert
            Assert.True(control.FileList.Items.Count == 1);
        }

        [Fact]
        public void RemoveFileRemovesSelectedFileFromList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            control.FileList.Items.Add(new ConversionFile { FileName = "file1", FullPath = "full path for file 1" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file2", FullPath = "full path for file 2" }, true);
            control.FileList.SelectedItem = control.FileList.Items[0];

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.RemoveFile();

            Assert.True(control.FileList.Items.Count == 1);
        }

        [Fact]
        public void UpButtonSwitchesOrderOfList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            control.FileList.Items.Add(new ConversionFile { FileName = "file1", FullPath = "full path for file 1" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file2", FullPath = "full path for file 2" }, true);
            control.FileList.SelectedItem = control.FileList.Items[1];
            control.FileList.SelectedIndex = 1;

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.UpClick();

            var convFile = control.FileList.Items[0] as ConversionFile;
            Assert.Equal(0, control.FileList.SelectedIndex);
            Assert.Equal("file2", convFile.FileName);
        }

        [Fact]
        public void DownButtonSwitchesOrderOfList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            control.FileList.Items.Add(new ConversionFile { FileName = "file1", FullPath = "full path for file 1" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file2", FullPath = "full path for file 2" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file3", FullPath = "full path for file 3" }, true);
            control.FileList.SelectedItem = control.FileList.Items[1];
            control.FileList.SelectedIndex = 1;

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.DownClick();

            var convFile = control.FileList.Items[2] as ConversionFile;
            Assert.Equal(2, control.FileList.SelectedIndex);
            Assert.Equal("file2", convFile.FileName);
        }

        [Fact]
        public void UpButtonDoesNotSwitchOrderOfList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            control.FileList.Items.Add(new ConversionFile { FileName = "file1", FullPath = "full path for file 1" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file2", FullPath = "full path for file 2" }, true);
            control.FileList.SelectedItem = control.FileList.Items[0];
            control.FileList.SelectedIndex = 0;

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.UpClick();

            var convFile = control.FileList.Items[0] as ConversionFile;
            Assert.Equal(0, control.FileList.SelectedIndex);
            Assert.Equal("file1", convFile.FileName);
        }

        [Fact]
        public void DownButtonDoesNotSwitchOrderOfList()
        {
            // Arrange
            var control = utility.CreateConversionsSettingsControl();
            var dialog = Substitute.For<IFileDialog>();

            control.FileList.Items.Add(new ConversionFile { FileName = "file1", FullPath = "full path for file 1" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file2", FullPath = "full path for file 2" }, true);
            control.FileList.Items.Add(new ConversionFile { FileName = "file3", FullPath = "full path for file 3" }, true);
            control.FileList.SelectedItem = control.FileList.Items[2];
            control.FileList.SelectedIndex = 2;

            var presenter = new ConversionSettingsPresenter(control, dialog);

            // Act
            presenter.DownClick();

            var convFile = control.FileList.Items[2] as ConversionFile;
            Assert.Equal(2, control.FileList.SelectedIndex);
            Assert.Equal("file3", convFile.FileName);
        }

        #region Fixture

        private readonly TestUtilities utility = null;

        public ConversionSettingsPresenterTests(TestUtilities utility)
        {
            this.utility = utility;
        }

        #endregion Fixture
    }
}