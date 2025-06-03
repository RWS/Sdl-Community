using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using XLIFF.Manager.UnitTests.Common;
using Xunit;

namespace XLIFF.Manager.UnitTests
{
	public class ProjectFileActivityViewModelTests
	{
		private readonly TestDataUtil _testDataUtil;

		public ProjectFileActivityViewModelTests()
		{
			var pathInfo = new PathInfo();
			var imageService = new ImageService();

			_testDataUtil = new TestDataUtil(imageService);
		}

		[Fact]
		public void Constructor_AssignProjectFileActivityModels_ReturnsEqual()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();
			var projectFileActivities = defaultTestProjectData[1].ProjectFiles[1].ProjectFileActivities;

			// act
			var model = new ProjectFileActivityViewModel(projectFileActivities);

			// assert
			Assert.Equal(projectFileActivities.Count, model.ProjectFileActivities.Count);
		}


		[Fact]
		public void Constructor_AssignSelectedProjectFileActivityModels_ReturnsSame()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();
			var projectFileActivities = defaultTestProjectData[1].ProjectFiles[1].ProjectFileActivities;

			// act
			var model = new ProjectFileActivityViewModel(projectFileActivities);

			// assert	
			// the first item in the collection is selected by default
			Assert.Same(projectFileActivities[0], model.SelectedProjectFileActivity);
		}
	}
}
