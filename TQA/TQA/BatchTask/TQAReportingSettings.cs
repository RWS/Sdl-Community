using System;
using System.Collections.Generic;
using Sdl.Core.Settings;
using System.IO;
using Sdl.Core.Settings.Implementation.Json;
using Sdl.Core.Settings.Implementation.Xml;
using Sdl.TranslationStudioAutomation.IntegrationApi;


namespace Sdl.Community.TQA.BatchTask
{
	public class TQAReportingSettings : SettingsGroup
	{
		private const string TQAReportingReportFolder = "TQAReports";
		internal const string TQAReportingDefaultQuality = "Premium";
		internal string TQAReportOutputLocation
		{
			get => GetSetting<string>(nameof(TQAReportOutputLocation));
			set => GetSetting<string>(nameof(TQAReportOutputLocation)).Value = value;
		}

		internal string TQAReportingQuality
		{
			get => GetSetting<string>(nameof(TQAReportingQuality));
			set => GetSetting<string>(nameof(TQAReportingQuality)).Value = value;
		}

		internal List<string> TQAReportingQualities
		{

			get
			{
				var qualities = GetSetting<List<string>>(nameof(TQAReportingQualities)) ?? new List<string>();
				return qualities;
			} 
			set
			{
				var qualities = GetSetting<List<string>>(nameof(TQAReportingQualities));
				if (qualities != null)
					qualities.Value = value;
			
			}
			
		}

		internal static string GetReportingOutputFolder()
		{
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			var projectFolder = filesController != null ? Path.GetDirectoryName(filesController.CurrentProject.FilePath) : Path.GetTempPath();
			return Path.Combine(projectFolder ?? Path.GetTempPath(), TQAReportingReportFolder);
		}

	}
}
