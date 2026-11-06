using System;
using System.Collections.Generic;
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
		public void StopTrackingTimerIfLastDocument_LastTrackedDocument_StopsTimer()
		{
			GivenRunningTimerWithTrackedDocuments(1);

			Tracked.StopTrackingTimerIfLastDocument();

			Assert.False(Tracked.TrackingTimer.IsRunning);
		}

		[Fact]
		public void StopTrackingTimerIfLastDocument_OtherDocumentsTracked_KeepsTimerRunning()
		{
			GivenRunningTimerWithTrackedDocuments(2);

			Tracked.StopTrackingTimerIfLastDocument();

			Assert.True(Tracked.TrackingTimer.IsRunning);
		}

		private static void GivenRunningTimerWithTrackedDocuments(int count)
		{
			GivenTracker(Tracked.TimerState.Started);
			Tracked.TrackingTimer.Start();
			Tracked.DictCacheDocumentItems = new Dictionary<string, TrackedDocuments>();
			for (var i = 0; i < count; i++)
			{
				Tracked.DictCacheDocumentItems.Add(i.ToString(), new TrackedDocuments());
			}
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