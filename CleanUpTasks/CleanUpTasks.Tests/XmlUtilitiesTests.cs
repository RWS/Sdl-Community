using System;
using System.Collections.Generic;
using System.IO;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class XmlUtilitiesTests : IClassFixture<TestUtilities>
	{
		[Fact]
		public void DeserializeThrowsWithRandomFile()
		{
			var path = Utility.CreatePath("somerandomfile.xml");

			Assert.Throws<InvalidOperationException>(() => XmlUtilities.Deserialize(path));
		}

		//[Fact(Skip = "Integration Test")]
		[Fact]
		public void DeserializeTest()
		{
			var path = Utility.CreatePath("serializetest.xml");

			var list = XmlUtilities.Deserialize(path);

			Assert.NotEmpty(list.Items);
		}

		//[Fact(Skip = "Integration Test")]
		[Fact]
		public void SerializeTest()
		{
			// Arrange
			var path = Utility.CreatePath("serializetest.xml");
			var list = new ConversionItemList()
			{
				Items = new List<ConversionItem>()
				{
					new ConversionItem
					{
						Replacement = new ReplacementText
						{
							Text = "Hello"
						},
						Search = new SearchText
						{
							Text = "There",
							VbStrConv = new List<Microsoft.VisualBasic.VbStrConv>() { Microsoft.VisualBasic.VbStrConv.Wide,
																					  Microsoft.VisualBasic.VbStrConv.Lowercase }
						}
					}
				}
			};

			// Act
			XmlUtilities.Serialize(list, path);

			// Assert
			Assert.True(File.Exists(path));
		}

		[Fact]
		public void SerializeThrowsOnEmptyList()
		{
			var list = new ConversionItemList();

			Assert.Throws<ArgumentException>(() => XmlUtilities.Serialize(list, "some path"));
		}

		#region Fixture

		public XmlUtilitiesTests(TestUtilities utility)
		{
			Utility = utility;
		}

		public TestUtilities Utility { get; }

		#endregion Fixture
	}
}