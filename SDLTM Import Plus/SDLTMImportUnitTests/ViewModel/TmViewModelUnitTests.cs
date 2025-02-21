using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Moq;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;
using SDLTM.Import.Service;
using SDLTM.Import.ViewModel;
using Xunit;

namespace SDLTMImportUnitTests.ViewModel
{
	public class TmViewModelUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly Mock<IDialogService> _dialogMockService;
		private readonly TmViewModel _tmViewModel;
		private readonly List<string> _xliffsList;
		private readonly List<string> _tmxList;
		private readonly List<string> _tmsList;
		private const string MessgeBoxTitle = "Please confirm";
		private const string XliffMessageBoxMessage = "Are you sure you want to delete selected files?";
		private const string TmsMessageBoxMessage = "Are you sure you want to delete selected TM(s)?";

		public TmViewModelUnitTests()
		{
			_dialogMockService = new Mock<IDialogService>();
			var firstTm = Path.Combine($"{_testingFilesPath}", "SDLImportFields.sdltm");
			var secondTm = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_tmsList = new List<string>{firstTm,secondTm};

			var firstFile = Path.Combine($"{_testingFilesPath}", "#1 - quickplace.docx.sdlxliff");
			var secondFile = Path.Combine($"{_testingFilesPath}", "TTSExample.docx.sdlxliff");
			_xliffsList = new List<string>{firstFile,secondFile};

			var firstTmx = Path.Combine($"{_testingFilesPath}", "export.tmx");
			_tmxList = new List<string>{firstTmx};
			var wizardModel = new WizardModel
			{
				TmsList = new ObservableCollection<TmDetails>(),
				FilesList = new ObservableCollection<FileDetails>()
			};
			var dataService = new TmVmService();
			_tmViewModel = new TmViewModel(wizardModel, dataService, _dialogMockService.Object, null);
		}

		[Fact]
		public void AddCommandWithNullParameter()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(new List<string>());
			_dialogMockService.Setup(f => f.ShowFolderDialog(It.IsAny<string>())).Returns(string.Empty);

			_tmViewModel.AddFilesCommand.Execute(null);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Never);
			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Never);

			Assert.True(_tmViewModel.TmsList.Count==0);
			Assert.True(_tmViewModel.FilesList.Count==0);
		}

		[Fact]
		public void AddCommandWithEmptyParameter()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(new List<string>());
			_dialogMockService.Setup(f => f.ShowFolderDialog(It.IsAny<string>())).Returns(string.Empty);

			_tmViewModel.AddFilesCommand.Execute(string.Empty);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Never);
			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Never);

			Assert.True(_tmViewModel.TmsList.Count == 0);
			Assert.True(_tmViewModel.FilesList.Count == 0);
		}
		[Fact]
		public void AddCommandWithRandomParameter()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(new List<string>());
			_dialogMockService.Setup(f => f.ShowFolderDialog(It.IsAny<string>())).Returns(string.Empty);

			_tmViewModel.AddFilesCommand.Execute("random");

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Never);
			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Never);

			Assert.True(_tmViewModel.TmsList.Count == 0);
			Assert.True(_tmViewModel.FilesList.Count == 0);
		}

		[Fact]
		public void ChooseTMsFromTmGrid()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(_tmsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Once);
			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Never);

			var tmsCount = _tmViewModel.TmsList.Count;
			var filesCount = _tmViewModel.FilesList.Count;

			Assert.True(tmsCount == 2);
			Assert.True(filesCount==0);
		}

		[Fact]
		public void ChooseTMsFromTmGridWithDuplicates()
		{
			var tm = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			var tmList = new List<string>{tm};
			tmList.AddRange(_tmsList);
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(tmList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Once);
			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Never);

			var tmsCount = _tmViewModel.TmsList.Count;
			var filesCount = _tmViewModel.FilesList.Count;

			Assert.True(tmsCount == 2);
			Assert.True(filesCount == 0);
		}

		[Fact]
		public void ChooseFilesFromFileGrid()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Once);
			var fileCount = _tmViewModel.FilesList.Count;
			var tmCount = _tmViewModel.TmsList.Count;

			Assert.True(fileCount == 2);
			Assert.True(tmCount==0);
		}

		[Fact]
		public void XliffFileType()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);

			var fileType = _tmViewModel.FilesList?[0].FileType;
			Assert.True(fileType== FileTypes.Xliff);
		}
		[Fact]
		public void TmxFileType()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmxList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);

			var fileType = _tmViewModel.FilesList?[0].FileType;
			Assert.True(fileType == FileTypes.Tmx);
		}

		[Fact]
		public void GetXliffLanguagePair()
		{
			var file = Path.Combine($"{_testingFilesPath}", "TTSExample.docx.sdlxliff");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string>{ file });

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			var sourceLanguage = _tmViewModel.FilesList?[0].SourceLanguage;
			var targetLanguage = _tmViewModel.FilesList?[0].TargetLanguage;

			Assert.True(sourceLanguage.Equals(new CultureInfo("en-us")));
			Assert.True(targetLanguage.Equals(new CultureInfo("de-de")));
		}

		[Fact]
		public void GetTmXLaguagePair()
		{
			var file = Path.Combine($"{_testingFilesPath}", "export.tmx");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { file });

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			var sourceLanguage = _tmViewModel.FilesList?[0].SourceLanguage;
			var targetLanguage = _tmViewModel.FilesList?[0].TargetLanguage;

			Assert.True(sourceLanguage.Equals(new CultureInfo("cy-GB")));
			Assert.True(targetLanguage.Equals(new CultureInfo("en-GB")));
		}

		[Fact]
		public void ChooseFilesFromFileGridWithDuplicates()
		{
			var xliff = Path.Combine($"{_testingFilesPath}", "#1 - quickplace.docx.sdlxliff");
			var xliffList = new List<string>{xliff};
			xliffList.AddRange(_xliffsList);

			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(xliffList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);

			_dialogMockService.Verify(f => f.ShowFileDialog(It.IsAny<string>()), Times.Once);
			var fileCount = _tmViewModel.FilesList.Count;
			var tmCount = _tmViewModel.TmsList.Count;

			Assert.True(fileCount == 2);
			Assert.True(tmCount == 0);
		}

		[Fact]
		public void LoadTmsFromFolder()
		{
			_dialogMockService.Setup(f => f.ShowFolderDialog(It.IsAny<string>())).Returns(_testingFilesPath);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.OpenFolderTGrid);

			_dialogMockService.Verify(f=>f.ShowFolderDialog(It.IsAny<string>()), Times.Once);
			var filesGridCound = _tmViewModel.FilesList.Count;
			var tmsGridCound = _tmViewModel.TmsList.Count;

			Assert.True(filesGridCound ==0);
			Assert.True(tmsGridCound >0);
		}

		[Fact]
		public void LoadFilesFromFolder()
		{
			_dialogMockService.Setup(f => f.ShowFolderDialog(It.IsAny<string>())).Returns(_testingFilesPath);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.OpenFolderFGrid);

			_dialogMockService.Verify(f => f.ShowFolderDialog(It.IsAny<string>()), Times.Once);
			var filesGridCound = _tmViewModel.FilesList.Count;
			var tmsGridCound = _tmViewModel.TmsList.Count;

			Assert.True(filesGridCound > 0);
			Assert.True(tmsGridCound == 0);
		}

		[Fact]
		public void RemoveCommandNullParameter()
		{
			_tmViewModel.RemoveFilesCommand.Execute(null);
		}

		[Fact]
		public void RemoveCommandIncorrectParameter()
		{
			_tmViewModel.RemoveFilesCommand.Execute("radom"); // throws exception which needs to be catched
			_tmViewModel.RemoveFilesCommand.Execute(string.Empty);
			_tmViewModel.RemoveFilesCommand.Execute(new List<TmDetails>
			{
				new TmDetails
				{
					Name = "Test tm"
				}
			});

			_tmViewModel.RemoveFilesCommand.Execute(new List<FileDetails>
			{
				new FileDetails
				{
					Name = "Test file"
				}
			});

			_tmViewModel.RemoveFilesCommand.Execute(new TmDetails
			{
				Name = "Test tm"
			});
			Assert.True(_tmViewModel.TmsList.Count == 0);
			Assert.True(_tmViewModel.FilesList.Count == 0);
		}

		[Fact]
		public void RemoveTm()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(_tmsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			Assert.True(_tmViewModel.TmsList.Count == 2);

			var tmToBeRemoved = _tmViewModel.TmsList[0];
			_tmViewModel.RemoveFilesCommand.Execute(new List<TmDetails>{tmToBeRemoved});
			Assert.True(_tmViewModel.TmsList.Count == 1);

			var tmExists = _tmViewModel.TmsList.Any(t => t.Id.Equals(tmToBeRemoved.Id));
			Assert.False(tmExists);
		}

		[Fact]
		public void RemoveTms()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(_tmsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			Assert.True(_tmViewModel.TmsList.Count == 2);

			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.TmsList);
			Assert.True(_tmViewModel.TmsList.Count == 0);
		}

		[Fact]
		public void RemoveXliff()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			Assert.True(_tmViewModel.FilesList.Count == 2);

			var xliffToBeRemoved = _tmViewModel.FilesList[0];
			_tmViewModel.RemoveFilesCommand.Execute(new List<FileDetails>{xliffToBeRemoved});

			Assert.True(_tmViewModel.FilesList.Count == 1);

			var itemExists = _tmViewModel.FilesList.Any(f => f.Id.Equals(xliffToBeRemoved.Id));
			Assert.False(itemExists);
		}

		[Fact]
		public void RemoveTmx()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmxList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			Assert.True(_tmViewModel.FilesList.Count == 1);

			var tmxToBeRemoved = _tmViewModel.FilesList[0];
			_tmViewModel.RemoveFilesCommand.Execute(new List<FileDetails> { tmxToBeRemoved });

			Assert.True(_tmViewModel.FilesList.Count == 0);

			var itemExists = _tmViewModel.FilesList.Any(f => f.Id.Equals(tmxToBeRemoved.Id));
			Assert.False(itemExists);
		}

		[Fact]
		public void RemoveXliffs()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			Assert.True(_tmViewModel.FilesList.Count == 2);

			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.FilesList);
			Assert.True(_tmViewModel.FilesList.Count == 0);
		}

		[Fact]
		public void RemoveTmxFiles()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmxList);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			Assert.True(_tmViewModel.FilesList.Count == 1);

			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.FilesList);
			Assert.True(_tmViewModel.FilesList.Count == 0);
		}

		[Fact]
		public void ShouldDisplayCorrectXliffMessageDialog()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);
			_dialogMockService.Setup(m=>m.ShowYesNoDialogResult(MessgeBoxTitle,XliffMessageBoxMessage)).Returns(MessageDialogResult.Yes);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.FilesList);

			_dialogMockService.Verify(m => m.ShowYesNoDialogResult(MessgeBoxTitle, XliffMessageBoxMessage),Times.Once);
		}

		[Fact]
		public void ShouldDisplayCorrectTMsMessageDialog()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>()))
				.Returns(_tmsList);
			_dialogMockService.Setup(m => m.ShowYesNoDialogResult(MessgeBoxTitle, TmsMessageBoxMessage))
				.Returns(MessageDialogResult.Yes);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.TmsList);

			_dialogMockService.Verify(m => m.ShowYesNoDialogResult(MessgeBoxTitle, TmsMessageBoxMessage), Times.Once);
		}

		[Fact]
		public void DeleteCommandFileYesDialogResult()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);
			_dialogMockService.Setup(m => m.ShowYesNoDialogResult(MessgeBoxTitle, XliffMessageBoxMessage)).Returns(MessageDialogResult.Yes);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.FilesList);

			Assert.True(_tmViewModel.FilesList.Count==0);
		}

		[Fact]
		public void DeleteCommandFileNoDialogResult()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_xliffsList);
			_dialogMockService.Setup(m => m.ShowYesNoDialogResult(MessgeBoxTitle, XliffMessageBoxMessage)).Returns(MessageDialogResult.No);

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseFilesFGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.FilesList);

			Assert.True(_tmViewModel.FilesList.Count == 2);
		}

		[Fact]
		public void DeleteCommandTmYesDialogResult()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmsList);
			_dialogMockService.Setup(m => m.ShowYesNoDialogResult(MessgeBoxTitle, TmsMessageBoxMessage)).Returns(MessageDialogResult.Yes);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.TmsList);

			Assert.True(_tmViewModel.TmsList.Count ==0);
		}
		[Fact]
		public void DeleteCommandTmNoDialogResult()
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmsList);
			_dialogMockService.Setup(m => m.ShowYesNoDialogResult(MessgeBoxTitle, TmsMessageBoxMessage)).Returns(MessageDialogResult.No);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			_tmViewModel.RemoveFilesCommand.Execute(_tmViewModel.TmsList);

			Assert.True(_tmViewModel.TmsList.Count == 2);
		}
	}
}
