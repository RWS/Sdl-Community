using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology
{
    public  class ReadExcelFile
    {
        //  public static Dictionary<string, List<Term>> _textDictionary= new Dictionary<string, List<Term>>();
        public static List<Tuple<int, string, List<Term>>> _termsList = new List<Tuple<int, string, List<Term>>>();
        public static void ReadExcel()
        {
            try
            {
                var package = new ExcelPackage(new FileInfo(@"C:\Users\aghisa\Desktop\glossary_example.xlsx"));
                var workSheet = package.Workbook.Worksheets[1];
                var start = workSheet.Dimension.Start;
                var end = workSheet.Dimension.End;
                
                var rowEntry = new List<string>();
                for (var row = start.Row+1; row <= end.Row; row++)
                {
                    //Row by row
                    for (var col = start.Column; col <= end.Column; col++)
                    {
                        var cellValue = workSheet.Cells[row, col].Text;
                        rowEntry.Add(cellValue);
                    }
                    _termsList.Add(new Tuple<int, string, List<Term>>(row,rowEntry[0], ParseRow(rowEntry)));;
                    rowEntry.Clear();
                }
            }catch(Exception ex) { }
           
               
        }

        public static List<Term> ParseRow(List<string> row)
        {
            var excelEntryList = new List<Term>();
            var target = ParseCell(row[1]);
            var status = ParseCell(row[2]);
            for (var i = 0; i < target.Count; i++)
            {
                excelEntryList.Add(new Term
                {
                    Target = target[i],
                    Status = status[i]
                });
            }
           // _textDictionary.Add(row[0], excelEntryList);
            target.Clear();
            status.Clear();
            return excelEntryList;
        }

        private static List<string> ParseCell(string cellValue)
        {
            var values = cellValue.Split('|').Select(value => value.Trim()).ToList();
            return values;
        }
    }
}
