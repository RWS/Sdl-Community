using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using XLIFF.Manager.UnitTests.Common;
using Xunit;

namespace XLIFF.Manager.UnitTests
{
	public class ProjectFilesViewModelTests
	{
		private readonly TestDataUtil _testDataUtil;

		public ProjectFilesViewModelTests()
		{
			var pathInfo = new PathInfo();
			var imageService = new ImageService();

			_testDataUtil = new TestDataUtil(imageService);
		}

		[Fact]
		public void Constructor_AssignProjectFileActionModels_ReturnsEqual()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();
			var projectFileActions = defaultTestProjectData[1].ProjectFiles;

			// act
			var model = new ProjectFilesViewModel(projectFileActions);

			// assert
			Assert.Equal(projectFileActions.Count, model.ProjectFiles.Count);
		}


		[Fact]
		public void Constructor_AssignSelectedProjectFileActionModels_ReturnsSame()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();
			var projectFileActions = defaultTestProjectData[1].ProjectFiles;

			// act
			var model = new ProjectFilesViewModel(projectFileActions);

			// assert	
			// the first item in the collection is selected by default
			Assert.Same(projectFileActions[0], model.SelectedProjectFile);
		}
	}
}
