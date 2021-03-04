using System;
using System.IO;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class FilesServiceUnitTests
	{
		private readonly string _testingFilesPath;
		private readonly string _tmFilePath;
		private readonly string _transitFilePath;
		private readonly IFileService _fileService;

		public FilesServiceUnitTests()
		{
			_fileService = new FileService();
			_testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
			_tmFilePath = Path.Combine(_testingFilesPath, "_AEXTR_1.DEU");
			_transitFilePath = Path.Combine(_testingFilesPath, "FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.DEU");
		}

		[Fact]
		public void IsTmFile_ReturnsTrue()
		{
			var isTm = _fileService.IsTransitTm(_tmFilePath);
			Assert.True(isTm);
		}

		[Fact]
		public void IsTmFile_ReturnsFalse()
		{
			var isTm = _fileService.IsTransitTm(_transitFilePath);
			Assert.False(isTm);
		}

		[Fact]
		public void IsTmFile_FileDoesNotExist_ReturnsFalse()
		{
			var isTm = _fileService.IsTransitTm(Path.Combine(_testingFilesPath, "_AEXTR_2.DEU"));
			Assert.False(isTm);
		}
	}
}
