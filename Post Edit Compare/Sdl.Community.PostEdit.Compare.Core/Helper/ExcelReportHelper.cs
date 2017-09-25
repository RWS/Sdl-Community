using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;
using static Sdl.Community.PostEdit.Compare.Core.Reports.Report;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class ExcelReportHelper
	{
		public static PEMModel CreateExcelDataModels(PEMpAnalysisData analysisData)
		{
			var pemDataModel = new PEMModel
			{
				Hundred = new PEMResults
				{
					SegmentsNo = analysisData.exactSegments,
					CharactersNo = analysisData.exactCharacters,
					WordsNo = analysisData.exactWords,
					Percent = analysisData.exactPercent,
					Tags = analysisData.exactTags

				},
				Fuzzy99 = new PEMResults
				{
					SegmentsNo = analysisData.fuzzy99Segments,
					CharactersNo = analysisData.fuzzy99Characters,
					WordsNo = analysisData.fuzzy99Words,
					Percent = analysisData.fuzzy99Percent,
					Tags = analysisData.fuzzy99Tags

				},
				Fuzzy94 = new PEMResults
				{
					SegmentsNo = analysisData.fuzzy94Segments,
					CharactersNo = analysisData.fuzzy94Characters,
					WordsNo = analysisData.fuzzy94Words,
					Percent = analysisData.fuzzy94Percent,
					Tags = analysisData.fuzzy94Tags

				},
				Fuzzy84= new PEMResults
				{
					SegmentsNo = analysisData.fuzzy84Segments,
					CharactersNo = analysisData.fuzzy84Characters,
					WordsNo = analysisData.fuzzy84Words,
					Percent = analysisData.fuzzy84Percent,
					Tags = analysisData.fuzzy84Tags

				},
				Fuzzy74 = new PEMResults
				{
					SegmentsNo = analysisData.fuzzy74Segments,
					CharactersNo = analysisData.fuzzy74Characters,
					WordsNo = analysisData.fuzzy74Words,
					Percent = analysisData.fuzzy74Percent,
					Tags = analysisData.fuzzy74Tags

				},
				New = new PEMResults
				{
					SegmentsNo = analysisData.newSegments,
					CharactersNo = analysisData.newCharacters,
					WordsNo = analysisData.newWords,
					Percent = analysisData.newPercent,
					Tags = analysisData.newTags

				},
				Total = new PEMResults
				{
					SegmentsNo = analysisData.totalSegments,
					CharactersNo = analysisData.totalCharacters,
					WordsNo = analysisData.totalWords,
					Percent = analysisData.totalPercent,
					Tags = analysisData.totalTags

				},
			};
			return pemDataModel;
		}
	}
}
