using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using NSubstitute;
using Sdl.Community.Qualitivity.Panels.Timers;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class QualitivityEditorControlTests
	{
		private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private const string StoppedDocument = "first.sdlxliff";

		[Fact]
		public void UpdateTimerControls_StoppedOnDisplayedDocument_KeepsLastValues()
		{
			using (var control = new QualitivityEditorControl())
			{
				GivenStoppedPanel(control, activeFile: StoppedDocument);

				UpdateTimerControls(control);

				Assert.Equal(StoppedDocument, Label(control, "label_document").Text);
				Assert.Equal("00:00:16", Label(control, "label_elapsed_time_document").Text);
				Assert.Equal("00:00:58", Label(control, "label_elapsed_time").Text);
			}
		}

		[Fact]
		public void UpdateTimerControls_StoppedAndAnotherDocumentActive_ClearsPanel()
		{
			using (var control = new QualitivityEditorControl())
			{
				GivenStoppedPanel(control, activeFile: "second.sdlxliff");

				UpdateTimerControls(control);

				Assert.Equal("none", Label(control, "label_document").Text);
				Assert.Equal("00:00:00", Label(control, "label_elapsed_time_document").Text);
				Assert.Equal("00:00:00", Label(control, "label_elapsed_time").Text);
			}
		}

		private static void GivenStoppedPanel(QualitivityEditorControl control, string activeFile)
		{
			Label(control, "label_document").Text = StoppedDocument;
			Label(control, "label_elapsed_time_document").Text = "00:00:16";
			Label(control, "label_elapsed_time").Text = "00:00:58";

			var file = (ProjectFile)FormatterServices.GetUninitializedObject(typeof(ProjectFile));
			typeof(ProjectFile).GetProperty(nameof(ProjectFile.Name)).SetValue(file, activeFile);

			var document = Substitute.For<IStudioDocument>();
			document.Files.Returns(new[] { file });
			document.ActiveFile.Returns(file);

			Tracked.Reset();
			Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();
			Tracked.ActiveDocument = document;
		}

		private static void UpdateTimerControls(QualitivityEditorControl control) =>
			typeof(QualitivityEditorControl).GetMethod("update_timer_controls", Instance).Invoke(control, null);

		private static Label Label(QualitivityEditorControl control, string name) =>
			(Label)typeof(QualitivityEditorControl).GetField(name, Instance).GetValue(control);
	}
}