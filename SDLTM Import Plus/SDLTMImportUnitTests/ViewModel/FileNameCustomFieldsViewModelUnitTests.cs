using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Moq;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;
using SDLTM.Import.Service;
using SDLTM.Import.ViewModel;
using Xunit;

namespace SDLTMImportUnitTests.ViewModel
{
	public class FileNameCustomFieldsViewModelUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly Mock<IDialogService> _dialogMockService;
		private readonly List<string> _tmsList;
		private readonly TmViewModel _tmViewModel;
		private readonly FileNameCustomFieldViewModel _fileNameCustomFieldsViewModel;

		public FileNameCustomFieldsViewModelUnitTests()
		{
			_dialogMockService = new Mock<IDialogService>();
			var firstTm = Path.Combine($"{_testingFilesPath}", "SDLImportFields.sdltm");
			var secondTm = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			var thirdTm = Path.Combine($"{_testingFilesPath}", "SDLTMImport No Fields.sdltm");
			_tmsList = new List<string> { firstTm, secondTm, thirdTm };
			var wizardModel = new WizardModel
			{
				TmsList = new ObservableCollection<TmDetails>(),
				FilesList = new ObservableCollection<FileDetails>()
			};
			var dataService = new TmVmService();
			_fileNameCustomFieldsViewModel = new FileNameCustomFieldViewModel(wizardModel, null);
			_tmViewModel = new TmViewModel(wizardModel, dataService, _dialogMockService.Object, null);
		}

		[Theory]
		[InlineData(true)]
		public void ShouldUseFileNameValueForAllFieldsPropertyChangedEvent(bool shouldUse)
		{
			var fired = false;
			_fileNameCustomFieldsViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_fileNameCustomFieldsViewModel.ShouldUseFileNameValueForAllFields))
				{
					fired = true;
				}
			};
			_fileNameCustomFieldsViewModel.ShouldUseFileNameValueForAllFields = shouldUse;

			Assert.True(fired);
		}
		[Theory]
		[InlineData(false)]
		public void ShouldUseFileNameValueForAllFieldsNoPropertyChangedEvent(bool shouldUse)
		{
			var fired = false;
			_fileNameCustomFieldsViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_fileNameCustomFieldsViewModel.ShouldUseFileNameValueForAllFields))
				{
					fired = true;
				}
			};
			_fileNameCustomFieldsViewModel.ShouldUseFileNameValueForAllFields = shouldUse;
			Assert.False(fired);
		}

		[Theory]
		[InlineData("FileNameFromUnitTestName")]
		public void SetXliffCustomFieldNameForAllTms(string fieldName)
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmsList);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			_fileNameCustomFieldsViewModel.ShouldUseFileNameValueForAllFields = true;
			_fileNameCustomFieldsViewModel.FileNameValueForFields = fieldName;

			Assert.All(_fileNameCustomFieldsViewModel.TmsList, tm => Assert.Equal(fieldName, tm.FileNameCustomField));
		}
		[Fact]
		public void CustomFileNameFieldAlreadyExists()
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { tmWithCustomFields });
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _fileNameCustomFieldsViewModel.TmsList[0];

			var fieldName = addedTm.AvailableCustomFields[0].Name;
			addedTm.FileNameCustomField = fieldName;

			Assert.True(addedTm.CustomFileNameFieldAlreadyExists);
		}
		[Theory]
		[InlineData("RandomName")]
		public void CustomFieldDoesNotExists(string fieldName)
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { tmWithCustomFields });
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _fileNameCustomFieldsViewModel.TmsList[0];

			addedTm.FileNameCustomField = fieldName;
			Assert.False(addedTm.CustomFileNameFieldAlreadyExists);
			Assert.True(addedTm.FileNameCustomField == fieldName);
		}

		[Theory]
		[InlineData("RandomName")]
		public void EditXliffFieldEventRaised(string fieldName)
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { tmWithCustomFields });
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _fileNameCustomFieldsViewModel.TmsList[0];
			var fired = false;
			addedTm.EditFileNameFieldEventRaised += tm =>
			{
				fired = true;
			};
			addedTm.FileNameCustomField = fieldName;
			Assert.True(fired);
		}
	}
}
