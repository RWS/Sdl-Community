using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NSubstitute;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Configuration;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Sdl.Community.Qualitivity.Tests
{
	public class TrackedEditorEventsTests
	{
		[Theory]
		[InlineData(Tracked.TimerState.None)]
		[InlineData(Tracked.TimerState.Stopped)]
		[InlineData(Tracked.TimerState.Deleted)]
		public void StartTrackingIfAutoStartEnabled_TrackerNotRunning_Starts(Tracked.TimerState state)
		{
			GivenAutoStart(true, state);

			TrackedEditorEvents.StartTrackingIfAutoStartEnabled();

			Assert.Equal(Tracked.TimerState.Started, Tracked.TrackingState);
		}

		[Fact]
		public void StartTrackingIfAutoStartEnabled_TrackerPaused_StaysPaused()
		{
			GivenAutoStart(true, Tracked.TimerState.Paused);

			TrackedEditorEvents.StartTrackingIfAutoStartEnabled();

			Assert.Equal(Tracked.TimerState.Paused, Tracked.TrackingState);
		}

		[Fact]
		public void StartTrackingIfAutoStartEnabled_SettingOff_StaysStopped()
		{
			GivenAutoStart(false, Tracked.TimerState.Stopped);

			TrackedEditorEvents.StartTrackingIfAutoStartEnabled();

			Assert.Equal(Tracked.TimerState.Stopped, Tracked.TrackingState);
		}

		[Fact]
		public void StartTrackingOnOpening_NoDocument_StaysStopped()
		{
			GivenAutoStart(true, Tracked.TimerState.Stopped);

			TrackedEditorEvents.StartTrackingOnOpening(null);

			Assert.Equal(Tracked.TimerState.Stopped, Tracked.TrackingState);
		}

		[Fact]
		public void StartTrackingOnOpening_Document_Starts()
		{
			GivenAutoStart(true, Tracked.TimerState.Stopped);

			TrackedEditorEvents.StartTrackingOnOpening(Substitute.For<IStudioDocument>());

			Assert.Equal(Tracked.TimerState.Started, Tracked.TrackingState);
		}

		[Fact]
		public void StartTrackingTimerIfStarted_TrackerStartedWithTimerStopped_RunsTimer()
		{
			GivenTracker(Tracked.TimerState.Started);

			Tracked.StartTrackingTimerIfStarted();

			Assert.True(Tracked.TrackingTimer.IsRunning);
			Assert.NotNull(Tracked.TrackingStart);
		}

		[Fact]
		public void StartTrackingTimerIfStarted_TrackerPaused_LeavesTimerStopped()
		{
			GivenTracker(Tracked.TimerState.Paused);

			Tracked.StartTrackingTimerIfStarted();

			Assert.False(Tracked.TrackingTimer.IsRunning);
		}

		[Fact]
		public void StartTrackingTimerIfStarted_TimerAlreadyRunning_KeepsTrackingStart()
		{
			GivenTracker(Tracked.TimerState.Started);
			Tracked.TrackingTimer.Start();
			var start = new DateTime(2026, 11, 5, 9, 0, 0);
			Tracked.TrackingStart = start;

			Tracked.StartTrackingTimerIfStarted();

			Assert.Equal(start, Tracked.TrackingStart);
		}

		[Fact]
		public void RunWhenActiveFileIsReady_ActiveFileStillMissing_KeepsWaiting()
		{
			var document = Substitute.For<IStudioDocument>();
			var runs = 0;

			TrackedEditorEvents.RunWhenActiveFileIsReady(document, () => runs++);
			document.ActiveSegmentChanged += Raise.Event();

			Assert.Equal(0, runs);
		}

		[Fact]
		public void RunWhenActiveFileIsReady_ActiveFileArrives_RunsOnce()
		{
			var document = Substitute.For<IStudioDocument>();
			var runs = 0;

			TrackedEditorEvents.RunWhenActiveFileIsReady(document, () => runs++);
			document.ActiveFile.Returns((ProjectFile)FormatterServices.GetUninitializedObject(typeof(ProjectFile)));
			document.ActiveSegmentChanged += Raise.Event();
			document.ActiveSegmentChanged += Raise.Event();

			Assert.Equal(1, runs);
		}

		[Fact]
		public void StopTrackingTimerIfLastDocument_LastOpenDocument_StopsTimer()
		{
			GivenRunningTimer();
			var closing = Substitute.For<IStudioDocument>();

			Tracked.StopTrackingTimerIfLastDocument(new[] { closing }, closing);

			Assert.False(Tracked.TrackingTimer.IsRunning);
		}

		[Fact]
		public void StopTrackingTimerIfLastDocument_UntrackedDocumentStillOpen_KeepsTimerRunning()
		{
			GivenRunningTimer();
			var closing = Substitute.For<IStudioDocument>();

			Tracked.StopTrackingTimerIfLastDocument(new[] { closing, Substitute.For<IStudioDocument>() }, closing);

			Assert.True(Tracked.TrackingTimer.IsRunning);
		}

		[Fact]
		public void StopTrackingIfLastDocument_LastOpenDocument_Stops()
		{
			GivenNoTrackedDocuments(Tracked.TimerState.Started);
			var closing = Substitute.For<IStudioDocument>();

			Tracked.StopTrackingIfLastDocument(new[] { closing }, closing);

			Assert.Equal(Tracked.TimerState.Stopped, Tracked.TrackingState);
		}

		[Theory]
		[InlineData(Tracked.TimerState.Started)]
		[InlineData(Tracked.TimerState.Paused)]
		public void StopTrackingIfLastDocument_UntrackedDocumentStillOpen_KeepsState(Tracked.TimerState state)
		{
			GivenNoTrackedDocuments(state);
			var closing = Substitute.For<IStudioDocument>();

			Tracked.StopTrackingIfLastDocument(new[] { closing, Substitute.For<IStudioDocument>() }, closing);

			Assert.Equal(state, Tracked.TrackingState);
		}

		[Fact]
		public void TrackedDocumentsFor_ClosingDocumentIsNotActive_ReturnsClosingDocumentsEntry()
		{
			var active = DocumentWithFile(Guid.NewGuid());
			var closing = DocumentWithFile(Guid.NewGuid());
			var activeEntry = new TrackedDocuments();
			var closingEntry = new TrackedDocuments();
			Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>
			{
				{ active.Files.First().Id.ToString(), activeEntry },
				{ closing.Files.First().Id.ToString(), closingEntry }
			};
			Tracked.ActiveDocument = active;

			Assert.Same(closingEntry, Tracked.TrackedDocumentsFor(closing));
		}

		private static IStudioDocument DocumentWithFile(Guid fileId)
		{
			var file = (ProjectFile)FormatterServices.GetUninitializedObject(typeof(ProjectFile));
			typeof(ProjectFile).GetProperty(nameof(ProjectFile.Id)).SetValue(file, fileId);
			var document = Substitute.For<IStudioDocument>();
			document.Files.Returns(new[] { file });
			return document;
		}

		private static void GivenNoTrackedDocuments(Tracked.TimerState state)
		{
			GivenTracker(state);
			Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();
		}

		private static void GivenRunningTimer()
		{
			GivenTracker(Tracked.TimerState.Started);
			Tracked.TrackingTimer.Start();
		}

		private static void GivenTracker(Tracked.TimerState state)
		{
			Tracked.Reset();
			Tracked.TrackingState = state;
		}

		private static void GivenAutoStart(bool enabled, Tracked.TimerState state)
		{
			Tracked.TrackingState = state;
			Tracked.Settings = new Settings();
			Tracked.Settings.GetTrackingProperty("autoStartTrackingOnDocumentOpenEvent").Value = enabled.ToString();
		}
	}
}