using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
    public class ProviderSettingsTest
    {
        [Fact]

        public void Get_Excel_Range_With_Approved()
        {
            var providerSettings = TestHelper.CreateProviderSettings();

            const string expected = "A:C";
            var actual = providerSettings.GetExcelRangeAddress();

            Assert.Equal(expected, actual);
        }

        [Fact]

        public void Get_Excel_Range_Without_Approved()
        {
            var providerSettings = TestHelper.CreateProviderSettings();
            providerSettings.ApprovedColumn = string.Empty;
            const string expected = "A:B";
            var actual = providerSettings.GetExcelRangeAddress();

            Assert.Equal(expected, actual);
        }
    }
}
