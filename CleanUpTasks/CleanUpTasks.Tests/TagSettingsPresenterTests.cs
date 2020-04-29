using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Models;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TagSettingsPresenterTests : IClassFixture<TestUtilities>
	{
		[Fact]
		public void ConstructorThrowsOnException()
		{
			Assert.Throws<ArgumentNullException>(() => new TagSettingsPresenter(null));
		}

		[Fact]
		public void InitializeAddsFormatTagsToList()
		{
			// Arrange
			var control = utility.CreateTagSettingsControl(formatTagList: new Dictionary<string, bool>()
			{
				{ "<sometag>", false }
			});

			var presenter = new TagSettingsPresenter(control);

			// Act
			presenter.Initialize();

			// Assert
			Assert.NotEmpty(control.FormatTagList.Items);
		}

		[Fact]
		public void InitializeAddsPlaceholderTagsToList()
		{
			// Arrange
			var control = utility.CreateTagSettingsControl(placeHolderTagList: new Dictionary<string, bool>()
			{
				{ "<placeholder />", false }
			});

			var presenter = new TagSettingsPresenter(control);

			// Act
			presenter.Initialize();

			// Assert
			Assert.NotEmpty(control.PlaceholderTagList.Items);
		}

		[Fact]
		public void InitializeSkipsDuplicateFormatTags()
		{
			// Arrange
			var formatList = new CheckedListBox();
			formatList.Items.Add("<sometag>", true);

			var control = utility.CreateTagSettingsControl(formatList: formatList,
			formatTagList: new Dictionary<string, bool>()
			{
				{ "<sometag>", false },
			});

			var presenter = new TagSettingsPresenter(control);

			// Act
			presenter.Initialize();

			// Assert
			Assert.Equal(1, control.FormatTagList.Items.Count);
		}

		[Fact]
		public void InitializeSkipsPlaceholdersCreatedByPlugIn()
		{
			// Arrange
			var control = utility.CreateTagSettingsControl(new List<Placeholder>()
			{
				new Placeholder { Content = "<placeholder />", IsTagPair = false }
			}
			,
			placeHolderTagList: new Dictionary<string, bool>()
			{
				{ "<placeholder />", false }
			});

			var presenter = new TagSettingsPresenter(control);

			// Act
			presenter.Initialize();

			// Assert
			Assert.Empty(control.PlaceholderTagList.Items);
		}

		#region Fixture

		private readonly TestUtilities utility = null;

		public TagSettingsPresenterTests(TestUtilities utility)
		{
			this.utility = utility;
		}

		#endregion Fixture
	}
}