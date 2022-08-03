using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TQA.Model
{
	internal enum TQStandardType
	{
		tqsEmpty,
		tqsJ2450,
		tqsMQM,
		tqsOther
	}
	internal static class TQStandardsFactory
	{
		//the file names that will be used for the output reports
		const string TQS_J2450_ReportFileName = "SDL-5-401-F001 Quality Evaluation Form_XXX_XXXXXX_XXX_XX";
		const string TQS_MQM_ReportFileName = "RWS-5-401-F002-MQM Quality Evaluation Form-XXX_XXXXXX_XXX-";

		//the templates (excel macros files) used to generate the reports.
		const string TQS_J2450_ReportTemplate = "SDL-5-401-F001-QE Form_XXX_XXXXXX_XXX_XX.XLSM";
		const string TQS_MQM_ReportTemplate = "RWS-5-401-F002-MQM QE Form-XXX_XXXXXX_XXX-.XLSM";
		const string TQS_EmptyProfile = "";
		const string TQS_NotSupportedProfile = "not supported TQS profile";
		public const string reportingFileExtension = ".xlsm";
		public const string excelFormatFile = "xlsm";

		internal static List<string> TQStandardDescriptions => new List<string> { "TQA Profile - Empty", "TQA Profile - J2450 Standard", "TQA Profile - MQM Standard", "TQA Profile - not compatible" };
		internal static List<string> TQS_J2450_Qualities => new List<string> { "Premium", "Value", "Economy" };
		internal static List<string> TQS_MQM_Qualities => new List<string> { "Premium", "Premium Plus", "Creative Plus", "Value", "Economy" };
		internal static List<string> TQS_ReportTemplates => new List<string> { TQS_EmptyProfile, TQS_J2450_ReportTemplate, TQS_MQM_ReportTemplate, TQS_NotSupportedProfile };
		internal static List<string> TQS_OutputFiles => new List<string> { TQS_EmptyProfile, TQS_J2450_ReportFileName, TQS_MQM_ReportFileName, TQS_NotSupportedProfile.ToUpper() };

		internal static List<string> GetTQSQualities(TQStandardType standardType)
		{
			switch (standardType)
			{
				case TQStandardType.tqsJ2450:
					return TQS_J2450_Qualities;
				case TQStandardType.tqsMQM:
					return TQS_MQM_Qualities;
				case TQStandardType.tqsEmpty:
					return new List<string>();
				case TQStandardType.tqsOther:
					return new List<string>();
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, standardType));
			}
		}

		internal static string GetCurrentTQStandardDescription(TQStandardType tqaStandardType)
		{
			return TQStandardDescriptions[(int)tqaStandardType];
		}

		internal static string GetReportTemplateFileForTQStandard(TQStandardType tqaStandardType)
		{
			return TQS_ReportTemplates[(int)tqaStandardType];
		}

		internal static byte[] GetReportTemplateForTQStandard(TQStandardType tqaStandardType)
		{
			switch (tqaStandardType)
			{
				case TQStandardType.tqsJ2450:
					return PluginResources.SDL_5_401_F001_QE_Form_XXX_XXXXXX_XXX_XX;
				case TQStandardType.tqsMQM:
					return PluginResources.RWS_5_401_F002_MQM_QE_Form_XXX_XXXXXX_XXX_;
				//case TQStandardType.tqsEmpty:
				//	return null;
				//case TQStandardType.tqsOther:
				//	return null;
				default:
					throw new NotSupportedException(string.Format(PluginResources.MsgTQAProfileStandardNotSupported, tqaStandardType));
			}
		}

		internal static string GetReportOutputFilenameForTQStandard(TQStandardType tqaStandardType)
		{
			return TQS_OutputFiles[(int)tqaStandardType];
		}
	}
}
