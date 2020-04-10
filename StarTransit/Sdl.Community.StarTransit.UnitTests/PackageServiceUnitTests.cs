using System;
using System.IO;
using NSubstitute;
using Sdl.Community.StarTransit.Shared.Services;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class PackageServiceUnitTests
	{
		private StarTransitConfiguration _starTransitConfiguration;
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		
		public PackageServiceUnitTests()
		{
			var packageService = Substitute.For<PackageService>();
			_starTransitConfiguration = new StarTransitConfiguration(packageService);
		}

		[Fact]
		public void OpenPackage_IsNotNullPackage_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.NotNull(packageModel);
		}

		[Fact]
		public void OpenPackage_IsNullPackage_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND")?.Result;
			Assert.Null(packageModel.PathToPrjFile);
		}

		[Fact]
		public void OpenPackage_CheckPackageName_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.Equal("693203001_Trumpf_ID", packageModel?.Name);
		}

		[Fact]
		public void OpenPackage_CheckWrongPackageName_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.NotEqual("Trumpf_ID", packageModel?.Name);
		}
	}
}