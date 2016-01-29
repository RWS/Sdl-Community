using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Xunit;
using Sdl.Community.ExcelTerminology;

namespace Sdl.Community.ExcelTerminology.Tests
{
    public class ExcelTests
    {
        [Theory]
        [InlineData("A1","A",1)]
        [InlineData("AB15", "AB", 15)]

        public void Get_Excel_Address_From_String(string address
            ,string expectedColumn,long expectedRow)
        {
            var excellAddress = new ExcelCellAddress(address);

            Assert.Equal(ExcelCellAddress.GetColumnLetter(excellAddress.Column), expectedColumn);
            Assert.Equal(excellAddress.Row, expectedRow);

        }
    }
}
