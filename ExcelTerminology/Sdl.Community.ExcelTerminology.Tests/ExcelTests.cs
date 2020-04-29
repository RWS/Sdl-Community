using System;
using System.IO;
using OfficeOpenXml;
using Xunit;

namespace Sdl.Community.ExcelTerminology.Tests
{
	public class ExcelTests
    {
        [Theory]
        [InlineData("A1","A",1)]
        [InlineData("AB15", "AB", 15)]
        public void Get_Excel_Address_From_String(string address,string expectedColumn,long expectedRow)
        {
            var excellAddress = new ExcelCellAddress(address);
            Assert.Equal(ExcelCellAddress.GetColumnLetter(excellAddress.Column), expectedColumn);
            Assert.Equal(excellAddress.Row, expectedRow);
        }

        [Theory]
        [InlineData(@"exceltbx://",@"C:\Temp\en-nl (large glossary example).xlsx")]
        [InlineData(@"exceltbx://", @"C:\Temp\glossary_example.xlsx")]
        public void Validate_Uri(string template, string path)
        {
            var uriString = template + Path.GetFileName(path);

            var result = new Uri(uriString.RemoveUriForbiddenCharacters());
            Assert.NotNull(result);
        }
    }
}