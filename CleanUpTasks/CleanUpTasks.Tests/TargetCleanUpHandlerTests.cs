using System;
using System.Collections.Generic;
using NSubstitute;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TargetCleanUpHandlerTests : IClassFixture<TestUtilities>
	{
		[Fact]
		public void ConstructorThrowsOnNull()
		{
			Assert.Throws<ArgumentNullException>(() => new TargetCleanUpHandler(null, null, null));
		}

		[Fact]
		public void PlaceholderTagIsRemovedAttribute()
		{
			// Arrange
			var segment = utility.CreateSegment();
			var phTag = Substitute.For<IPlaceholderTag>();
			phTag.Parent = segment;
			phTag.TagProperties.Returns(Substitute.For<IAbstractTagProperties>());
			phTag.TagProperties.TagContent = "<locked name=\"some text\">";

			var settings = utility.CreateSettings();
			settings.Placeholders = new List<Placeholder>()
			{
				new Placeholder()
				{
					Content = "<locked name=\"some text\">",
					IsTagPair = false
				}
			};

			var factory = DefaultDocumentItemFactory.CreateInstance();
			var reporter = Substitute.For<ICleanUpMessageReporter>();

			TargetCleanUpHandler handler = new TargetCleanUpHandler(settings, factory, reporter);

			// Act
			handler.VisitPlaceholderTag(phTag);
			handler.VisitSegment(segment);

			// Assert
			phTag.Received().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Is<IText>(txt => txt.Properties.Text == "some text"));
		}

		[Fact]
		public void PlaceholderTagIsRemovedElement()
		{
			// Arrange
			var segment = utility.CreateSegment();
			var phTag = Substitute.For<IPlaceholderTag>();
			phTag.Parent = segment;
			phTag.TagProperties.Returns(Substitute.For<IAbstractTagProperties>());
			phTag.TagProperties.TagContent = "<html>";

			var settings = utility.CreateSettings();
			settings.Placeholders = new List<Placeholder>()
			{
				new Placeholder()
				{
					Content = "<html>",
					IsTagPair = true
				}
			};

			var factory = DefaultDocumentItemFactory.CreateInstance();
			var reporter = Substitute.For<ICleanUpMessageReporter>();

			TargetCleanUpHandler handler = new TargetCleanUpHandler(settings, factory, reporter);

			// Act
			handler.VisitPlaceholderTag(phTag);
			handler.VisitSegment(segment);

			// Assert
			phTag.Received().RemoveFromParent();
			segment.Received().Insert(Arg.Any<int>(), Arg.Is<IText>(txt => txt.Properties.Text == "<html>"));
		}

		#region Fixture

		private readonly TestUtilities utility = null;

		public TargetCleanUpHandlerTests(TestUtilities utility)
		{
			this.utility = utility;
		}

		#endregion Fixture
	}
}