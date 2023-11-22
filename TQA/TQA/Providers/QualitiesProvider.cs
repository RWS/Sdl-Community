using System;
using System.Collections.Generic;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.TQA.Providers
{
	public class QualitiesProvider
	{
		private static readonly List<string> TqaJ2450Qualities = new List<string> { "Premium", "Value", "Economy" };
		private static readonly List<string> TqaMqmQualities = new List<string> { "Premium", "Premium Plus", "Creative Plus", "Value", "Economy" };

		public List<string> GetQualities(TQAProfileType standardType)
		{
			switch (standardType)
			{
				case TQAProfileType.tqsJ2450:
					return TqaJ2450Qualities;
				case TQAProfileType.tqsMQM:
					return TqaMqmQualities;
				case TQAProfileType.tqsEmpty:
					return new List<string>();
				case TQAProfileType.tqsOther:
					return new List<string>();
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, standardType));
			}
		}

		public void UpdateQualityValue(IProject project, string quality)
		{
			var tqaReportingSettings = project.GetSettings().GetSettingsGroup("TQAReportingSettings");
			if (tqaReportingSettings != null)
			{
				tqaReportingSettings.GetSetting<string>("TQAReportingQuality").Value = quality;
			}
		}

		public string GetQuality(IProject project)
		{
			var tqaReportingSettings = project.GetSettings().GetSettingsGroup("TQAReportingSettings");
			return tqaReportingSettings?.GetSetting<string>("TQAReportingQuality").Value;
		}
	}
}
