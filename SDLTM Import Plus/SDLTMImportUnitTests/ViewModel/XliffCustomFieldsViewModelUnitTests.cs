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
	public class XliffCustomFieldsViewModelUnitTests
	{
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly Mock<IDialogService> _dialogMockService;
		private readonly List<string> _tmsList;
		private readonly TmViewModel _tmViewModel;
		private readonly XliffCustomFieldsViewModel _xliffCustomFieldsViewModel;

		public XliffCustomFieldsViewModelUnitTests()
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
			_xliffCustomFieldsViewModel = new XliffCustomFieldsViewModel(wizardModel,null);
			_tmViewModel = new TmViewModel(wizardModel, dataService, _dialogMockService.Object, null);
		}

		[Fact]
		public void LoadTmWithNoCustomFileds()
		{
			var tmWithNoCustomFields = Path.Combine($"{_testingFilesPath}", "SDLTMImport No Fields.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string>{tmWithNoCustomFields});

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			var availableCustomFileds = _xliffCustomFieldsViewModel.TmsList?[0].AvailableCustomFields.Count;
			Assert.True(availableCustomFileds ==0);
		}
		[Fact]
		public void LoadTmWithCustomFileds()
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { tmWithCustomFields });

			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			var availableCustomFileds = _xliffCustomFieldsViewModel.TmsList?[0].AvailableCustomFields.Count;
			Assert.True(availableCustomFileds > 0);
		}


		[Theory]
		[InlineData(true)]
		public void ShouldUseXliffValueForAllFieldsPropertyChangedEvent(bool shouldUse)
		{
			var fired = false;
			_xliffCustomFieldsViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_xliffCustomFieldsViewModel.ShouldUseXliffValueForAllFields))
				{
					fired = true;
				}
			};
			_xliffCustomFieldsViewModel.ShouldUseXliffValueForAllFields = shouldUse;

			Assert.True(fired);
		}

		[Theory]
		[InlineData(false)]
		public void ShouldUseXliffValueForAllFieldsNoPropertyChangedEvent(bool shouldUse)
		{
			var fired = false;
			_xliffCustomFieldsViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(_xliffCustomFieldsViewModel.ShouldUseXliffValueForAllFields))
				{
					fired = true;
				}
			};
			_xliffCustomFieldsViewModel.ShouldUseXliffValueForAllFields = shouldUse;
			Assert.False(fired);
		}

		[Theory]
		[InlineData("SegmentIdFromUnitTestName")]
		public void SetXliffCustomFieldNameForAllTms(string fieldName)
		{
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(_tmsList);
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);

			_xliffCustomFieldsViewModel.ShouldUseXliffValueForAllFields = true;
			_xliffCustomFieldsViewModel.XliffValueForFields = fieldName;

			Assert.All(_xliffCustomFieldsViewModel.TmsList, tm => Assert.Equal(fieldName, tm.SegmentIdCustomFieldName));
		}

		[Fact]
		public void CustomXliffFieldAlreadyExists()
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> {tmWithCustomFields});
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _xliffCustomFieldsViewModel.TmsList[0];

			var segmentFieldName = addedTm.AvailableCustomFields[0].Name;
			addedTm.SegmentIdCustomFieldName = segmentFieldName;

			Assert.True(addedTm.CustomXliffFieldAlreadyExists);
		}

		[Theory]
		[InlineData("RandomName")]
		public void CustomFieldDoesNotExists(string fieldName)
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> { tmWithCustomFields });
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _xliffCustomFieldsViewModel.TmsList[0];

			addedTm.SegmentIdCustomFieldName = fieldName;
			Assert.False(addedTm.CustomXliffFieldAlreadyExists);
			Assert.True(addedTm.SegmentIdCustomFieldName==fieldName);
		}

		[Theory]
		[InlineData("RandomName")]
		public void EditXliffFieldEventRaised(string fieldName)
		{
			var tmWithCustomFields = Path.Combine($"{_testingFilesPath}", "SDL Import EN-FR.sdltm");
			_dialogMockService.Setup(f => f.ShowFileDialog(It.IsAny<string>())).Returns(new List<string> {tmWithCustomFields});
			_tmViewModel.AddFilesCommand.Execute(AddOptions.ChooseTmTGrid);
			var addedTm = _xliffCustomFieldsViewModel.TmsList[0];
			var fired = false;
			addedTm.EditXliffFieldEventRaised += tm =>
			{
				fired = true;
			};
			addedTm.SegmentIdCustomFieldName = fieldName;
			Assert.True(fired);
		}
	}
}
