using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.LinqParser;
using Xunit;

namespace DataProtectionSuiteTests.TmAnonymizerTests
{
	public class StringToLinqParserTests
	{
		private readonly Random _rnd = new Random();
		private readonly List<FilterableTm> _testList;

		public StringToLinqParserTests()
		{
			_testList = GetList(1, 100);
		}

		[Theory]
		[InlineData("(Id > 5 and Id < 35 or (Id > 3 and Id < 6)) and Id = 1 and Id = 16")]
		public void EvaluateQueryTest_AndTermWithParentheses(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedListIds = new List<int>();

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("Id between 5 and 35")]
		public void EvaluateQueryTest_Between(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(5, 35);
			var expectedListIds = expectedList.Select(tm => tm.Id);

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("Id not between 5 and 35")]
		public void EvaluateQueryTest_NotBetween(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(1, 4);
			expectedList.AddRange(GetList(36, 100));
			var expectedListIds = expectedList.Select(tm => tm.Id);

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("(Id > 5 (or Id < 33) and Id = 37")]
		public void EvaluateQueryTest_Other(string queryString)
		{
			Assert.ThrowsAny<Exception>(() => _testList.Evaluate(queryString));
		}

		[Theory]
		[InlineData("Id > 5 and (Id < 35 or Id = 1)")]
		public void EvaluateQueryTest_ParenthesesAtEnd(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(6, 34);
			var expectedListIds = expectedList.Select(tm => tm.Id).ToList();

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("(Id > 5 and Id < 35 or (Id > 3 and Id < 6)) or Id = 1")]
		public void EvaluateQueryTest_ParenthesesAtStartAndNested(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(6, 34);
			expectedList.AddRange(GetList(4, 5));
			expectedList.Add(new FilterableTm { Id = 1 });
			var expectedListIds = expectedList.Select(tm => tm.Id).ToList();

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("Id > 5 and Id < 35 or (Id > 3 and Id < 6) and Id = 1 and Id = 16")]
		public void EvaluateQueryTest_ParenthesesInMiddle(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(6, 34);
			var expectedListIds = expectedList.Select(tm => tm.Id).ToList();

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("Id > 5 and Id < 35 or Id = 1")]
		public void EvaluateQueryTest_WithoutParentheses(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(6, 34);
			expectedList.Add(new FilterableTm { Id = 1 });
			var expectedListIds = expectedList.Select(tm => tm.Id).ToList();

			Assert.Equal(expectedListIds, testListIds);
		}

		[Theory]
		[InlineData("ChangeDate = 18.01.2000", 1)]
		public void EvaluateQueryTest_WithoutParentheses_UsingChangeDate(string queryString, int changeDateMonth)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			var expectedList = GetList(1, 100, changeDateMonth);
			var expectedListIds = expectedList.Select(tm => tm.Id).ToList();

			Assert.Equal(expectedListIds, testListIds);
			Assert.True(expectedList.Select(ex => ex.ChangeDate).Distinct().Count() > 1);
		}
		
		[Theory]
		[InlineData("ChangeDate = 18.01.2000 00:00:00")]
		public void EvaluateQueryTest_WithoutParentheses_UsingChangeDateAndTime(string queryString)
		{
			var testList = _testList.Evaluate(queryString).ToList();
			var testListIds = testList.Select(tm => tm.Id).ToList();

			Assert.Empty(testListIds);
		}

		/// <summary>
		/// Creates a test list with IDs in increasing order from <paramref name="startId"/> to <paramref name="endInd"/> and with ChangeDate with months from 1 to 12
		/// unless the <paramref name="changeDateMonth"/> parameter is set, in which case it overwrites the numbering
		/// </summary>
		/// <param name="startId"></param>
		/// <param name="endInd"></param>
		/// <param name="changeDateMonth"></param>
		/// <returns></returns>
		private List<FilterableTm> GetList(int startId, int endInd, int changeDateMonth = 0)
		{
			var list = new List<FilterableTm>();
			for (var i = startId; i <= endInd; i++)
			{
				var month = i % 12 + 1;
				list.Add(new FilterableTm
				{
					Id = i,
					ChangeDate = DateTime.Parse($"18.{month}.2000 {GetRandomTime()}")
				});
			}

			var filteredTms = changeDateMonth != 0
				? list.Where(i => i.ChangeDate.Date == DateTime.Parse($"18.{changeDateMonth}.2000")).ToList()
				: list;
			return filteredTms;
		}

		private string GetRandomTime()
		{
			var start = DateTime.Today;

			var minutes = _rnd.Next(42);
			var t = start.Add(TimeSpan.FromMinutes(minutes));

			return t.TimeOfDay.ToString();
		}
	}
}