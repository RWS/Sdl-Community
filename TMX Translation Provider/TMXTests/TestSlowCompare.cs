using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Search;

namespace TMXTests
{
	public class TestSlowCompare
	{
		private static int StringIntCompare(string a, string b)
		{
			return Math.Min( (int)(SlowCompareTexts.Compare(a, b) * 100 + .5), 100);
		}

		[Fact]
		public void Test()
		{
			// Debug.Assert -> Assert.True

			Assert.True( StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction Health and Safety for Subcontractors and the Sustainability Code") == 97);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains the both Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code") == 100);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both Interserve Construction Health Safety Code for Subcontractors and the Sustainability Code") == 95);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction and Safety Code Subcontractors and the Sustainability Code") == 92);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety, Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction Health and Safety. Code for Subcontractors and the Sustainability Code") == 96);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety,	Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code") == 96);
			Assert.True(StringIntCompare(
				"document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code") == 85);
			Assert.True(StringIntCompare(
				"This document contains the Interserve Construction Health Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction Health and Safety Code Subcontractors and the Sustainability Code") == 94);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code Subcontractors and the Sustainability Code",
				"This document contains both Interserve Construction Health Safety Code for Subcontractors and the Sustainability Code") == 92);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Construction and Safety Code for Subcontractors and the Sustainability Code") == 85);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains both the Interserve Construction and Safety for Subcontractors and the Sustainability Code") == 91);
			Assert.True(StringIntCompare(
				"This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code",
				"This document contains the Interserve Construction Safety Subcontractors and Sustainability trilu") == 71);

		}

	}

}