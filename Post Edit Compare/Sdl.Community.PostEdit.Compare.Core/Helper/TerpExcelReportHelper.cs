using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;
using static Sdl.Community.PostEdit.Compare.Core.Reports.Report;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class TerpExcelReportHelper
	{
		public static List<TERpModel> CreateTerpExcelDataModels(TERpAnalysisData terpData)
		{
			var terpModelList = new List<TERpModel>();

			var terp0 = GetTerp0Values(terpData);
			var terp01 = GetTerp01Values(terpData);
			var terp06 = GetTerp06Values(terpData);
			var terp10 = GetTerp10Values(terpData);
			var terp20 = GetTerp20Values(terpData);
			var terp30 = GetTerp30Values(terpData);
			var terp40 = GetTerp40Values(terpData);
			var terp50 = GetTerp50Values(terpData);
			var terpTotal = GetTerpTotalValues(terpData);

			terpModelList.AddRange(terp0);
			terpModelList.AddRange(terp01);
			terpModelList.AddRange(terp06);
			terpModelList.AddRange(terp10);
			terpModelList.AddRange(terp20);
			terpModelList.AddRange(terp30);
			terpModelList.AddRange(terp40);
			terpModelList.AddRange(terp50);
			terpModelList.AddRange(terpTotal);
			return terpModelList;
		}

		private static List<TERpModel> GetTerp0Values(TERpAnalysisData terpData)
		{
			var terp0MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Segments,terpData.terp00Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Words,terpData.terp00SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.RefWords,terpData.terp00NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Errors,terpData.terp00NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Ins,terpData.terp00Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Del,terpData.terp00Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Sub,terpData.terp00Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp00,Constants.Shft,terpData.terp00Shft)
				},
			};
			return terp0MatchValueList;
		}
		private static List<TERpModel> GetTerp06Values(TERpAnalysisData terpData)
		{
			var terp06MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Segments,terpData.terp06Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Words,terpData.terp06SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.RefWords,terpData.terp06NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Errors,terpData.terp06NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Ins,terpData.terp06Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Del,terpData.terp06Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Sub,terpData.terp06Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp06,Constants.Shft,terpData.terp06Shft)
				},
			};
			return terp06MatchValueList;
		}
		private static List<TERpModel> GetTerp01Values(TERpAnalysisData terpData)
		{
			var terp01MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Segments,terpData.terp01Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Words,terpData.terp01SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.RefWords,terpData.terp01NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Errors,terpData.terp01NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Ins,terpData.terp01Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Del,terpData.terp01Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Sub,terpData.terp01Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp01,Constants.Shft,terpData.terp01Shft)
				},
			};
			return terp01MatchValueList;
		}
		private static List<TERpModel> GetTerp10Values(TERpAnalysisData terpData)
		{
			var terp10MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Segments,terpData.terp10Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Words,terpData.terp10SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.RefWords,terpData.terp10NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Errors,terpData.terp10NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Ins,terpData.terp10Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Del,terpData.terp10Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Sub,terpData.terp10Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp10,Constants.Shft,terpData.terp10Shft)
				},
			};
			return terp10MatchValueList;
		}
		private static List<TERpModel> GetTerp20Values(TERpAnalysisData terpData)
		{
			var terp20MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Segments,terpData.terp20Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Words,terpData.terp20SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.RefWords,terpData.terp20NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Errors,terpData.terp20NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Ins,terpData.terp20Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Del,terpData.terp20Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Sub,terpData.terp20Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp20,Constants.Shft,terpData.terp20Shft)
				},
			};
			return terp20MatchValueList;
		}
		private static List<TERpModel> GetTerp30Values(TERpAnalysisData terpData)
		{
			var terp30MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Segments,terpData.terp30Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Words,terpData.terp30SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.RefWords,terpData.terp30NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Errors,terpData.terp30NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Ins,terpData.terp30Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Del,terpData.terp30Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Sub,terpData.terp30Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp30,Constants.Shft,terpData.terp30Shft)
				},
			};
			return terp30MatchValueList;
		}
		private static List<TERpModel> GetTerp40Values(TERpAnalysisData terpData)
		{
			var terp40MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Segments,terpData.terp40Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Words,terpData.terp40SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.RefWords,terpData.terp40NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Errors,terpData.terp40NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Ins,terpData.terp40Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Del,terpData.terp40Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Sub,terpData.terp40Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp40,Constants.Shft,terpData.terp40Shft)
				},
			};
			return terp40MatchValueList;
		}
		private static List<TERpModel> GetTerp50Values(TERpAnalysisData terpData)
		{
			var terp50MatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Segments,terpData.terp50Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Words,terpData.terp50SrcWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.RefWords,terpData.terp50NumWd)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Errors,terpData.terp50NumEr)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Ins,terpData.terp50Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Del,terpData.terp50Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Sub,terpData.terp50Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Terp50,Constants.Shft,terpData.terp50Shft)
				},
			};
			return terp50MatchValueList;
		}
		private static List<TERpModel> GetTerpTotalValues(TERpAnalysisData terpData)
		{
			var totalValues = GetTerpTotalValueSegments(terpData);
			var terpTotalMatchValueList = new List<TERpModel>
			{
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Segments,totalValues.Segments)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Words,totalValues.Words)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.RefWords,totalValues.RefWords)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Errors,totalValues.Errors)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Ins,totalValues.Ins)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Del,totalValues.Del)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Sub,totalValues.Sub)
				},
				new TERpModel
				{
					TERpAnalyseResult = Tuple.Create(Constants.Total,Constants.Shft,totalValues.Shft)
				},
			};
			return terpTotalMatchValueList;
		}

		private static TERpTotalValues GetTerpTotalValueSegments(TERpAnalysisData terpData)
		{
			var terpTotal = new TERpTotalValues();
			var totalSegments = terpData.terp00Segments + terpData.terp01Segments+ terpData.terp06Segments + terpData.terp20Segments + terpData.terp10Segments
				+ terpData.terp30Segments + terpData.terp40Segments + terpData.terp50Segments;

			var totalWords = terpData.terp00SrcWd + terpData.terp01SrcWd + terpData.terp06SrcWd + terpData.terp20SrcWd + terpData.terp10SrcWd
				+ terpData.terp30SrcWd + terpData.terp40SrcWd + terpData.terp50SrcWd;

			var refWords = terpData.terp00NumWd + terpData.terp01NumWd + terpData.terp06NumWd + terpData.terp20NumWd + terpData.terp10NumWd
			+ terpData.terp30NumWd + terpData.terp40NumWd + terpData.terp50NumWd;

			var errors = terpData.terp00NumEr + terpData.terp01NumEr + terpData.terp06NumEr + terpData.terp20NumEr + terpData.terp10NumEr
			+ terpData.terp30NumEr + terpData.terp40NumEr + terpData.terp50NumEr;

			var ins = terpData.terp00Ins + terpData.terp01Ins + terpData.terp06Ins + terpData.terp20Ins + terpData.terp10Ins
			+ terpData.terp30Ins + terpData.terp40Ins + terpData.terp50Ins;

			var del = terpData.terp00Del + terpData.terp01Del + terpData.terp06Del + terpData.terp20Del + terpData.terp10Del
			+ terpData.terp30Del + terpData.terp40Del + terpData.terp50Del;

			var sub = terpData.terp00Sub + terpData.terp01Sub + terpData.terp06Sub + terpData.terp20Sub + terpData.terp10Sub
			+ terpData.terp30Sub + terpData.terp40Sub + terpData.terp50Sub;

			var shft = terpData.terp00Shft + terpData.terp01Shft + terpData.terp06Shft + terpData.terp20Shft + terpData.terp10Shft
			+ terpData.terp30Shft + terpData.terp40Shft + terpData.terp50Shft;

			terpTotal.Segments = totalSegments;
			terpTotal.Words = totalWords;
			terpTotal.RefWords = refWords;
			terpTotal.Errors = errors;
			terpTotal.Ins = ins;
			terpTotal.Del = del;
			terpTotal.Sub = sub;
			terpTotal.Shft = shft;

			return terpTotal;
		}
	}
}
