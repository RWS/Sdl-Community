using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.LinqParser;
using Xunit;

namespace DataProtectionSuiteTests.TmAnonymizerTests
{
	public class StringToLinqParser
	{
		private readonly List<FilterableTm> _testList;

		public StringToLinqParser()
		{
			_testList = GetList(1, 100);
		}

		public static List<FilterableTm> GetList(int startId, int endInd)
		{
			var list = new List<FilterableTm>();
			for (var i = startId; i <= endInd; i++) list.Add(new FilterableTm { Id = i });
			return list;
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
		[InlineData("(Id > 5 (or Id < 33) and Id = 37")]
		public void EvaluateQueryTest_Other(string queryString)
		{
			Assert.ThrowsAny<Exception>(() => _testList.Evaluate(queryString));
		}
	}
}