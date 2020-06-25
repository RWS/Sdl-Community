using System.Linq;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using XLIFF.Manager.UnitTests.Common;
using Xunit;

namespace XLIFF.Manager.UnitTests
{
	public class ProjectsNavigationViewModelTests
	{
		private readonly TestDataUtil _testDataUtil;

		public ProjectsNavigationViewModelTests()
		{
			var pathInfo = new PathInfo();
			var imageService = new ImageService();

			_testDataUtil = new TestDataUtil(imageService);
		}

		[Fact]
		public void Constructor_AssignFilteredProjectModels_ReturnsEqual()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);

			// assert
			Assert.Equal(defaultTestProjectData.Count, model.FilteredProjects.Count);
		}

		[Fact]
		public void Constructor_AssignSelectedProjectModel_ReturnsSame()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);

			// assert
			// the first item in the collection is selected by default
			Assert.Same(defaultTestProjectData[0], model.SelectedProject);
		}

		[Fact]
		public void FilteredProjectModels_FiltersOnProjectName_ReturnsContains()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);
			model.FilterString = defaultTestProjectData[1].Name;

			// assert			
			Assert.Contains(defaultTestProjectData[1], model.FilteredProjects);
			Assert.Equal(defaultTestProjectData.Where(
				a => a.Name.Contains(defaultTestProjectData[1].Name)).ToList().Count, model.FilteredProjects.Count);
		}


		[Fact]
		public void FilteredProjectModels_FiltersOnPartialProjectName_ReturnsContains()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);
			model.FilterString = defaultTestProjectData[1].Name.Substring(defaultTestProjectData[1].Name.Length - 2);

			// assert			
			Assert.Contains(defaultTestProjectData[1], model.FilteredProjects);
			Assert.Equal(defaultTestProjectData.Where(
				a => a.Name.Contains(defaultTestProjectData[1].Name)).ToList().Count, model.FilteredProjects.Count);
		}

		[Fact]
		public void FilteredProjectModels_FiltersOnProjectNameWithDifferentCase_ReturnsContains()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);
			model.FilterString = defaultTestProjectData[1].Name.ToUpper();

			// assert			
			Assert.Contains(defaultTestProjectData[1], model.FilteredProjects);
			Assert.Equal(defaultTestProjectData.Where(
				a => a.Name.Contains(defaultTestProjectData[1].Name)).ToList().Count, model.FilteredProjects.Count);
		}


		[Fact]
		public void FilteredProjectModels_ClearFilterOnProjectName_ReturnsEqual()
		{
			// arrange
			var defaultTestProjectData = _testDataUtil.GetDefaultTestProjectData();

			// act
			var model = new ProjectsNavigationViewModel(defaultTestProjectData, null);
			model.FilterString = defaultTestProjectData[1].Name;
			model.FilterString = string.Empty;

			// assert	
			Assert.Equal(defaultTestProjectData.Count, model.FilteredProjects.Count);
		}
	}
}
