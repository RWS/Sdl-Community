using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public  class ReadExcelTerminologyService
    {
        //  public static Dictionary<string, List<Term>> _textDictionary= new Dictionary<string, List<Term>>();
        //public static List<Tuple<int, string, List<Term>>> _termsList = new List<Tuple<int, string, List<Term>>>();
        private readonly List<Entry> _entries;
        private readonly ProviderSettings _providerSettings;

        public ReadExcelTerminologyService(ProviderSettings providerSettings)
        {
            _entries = new List<Entry>();
            _providerSettings = providerSettings;
        }


        public void ReadExcel()
        {
            
            try
            {
                var workSheet = GetTerminologyWorksheet();
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
                 //   _termsList.Add(new Tuple<int, string, List<Term>>(row,rowEntry[0], ParseRow(rowEntry)));;
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

        public ExcelWorksheet GetTerminologyWorksheet()
        {
          var excelPackage =  
                new ExcelPackage(new FileInfo(_providerSettings.TermFilePath));

            var worksheet = string.IsNullOrEmpty(_providerSettings.WorksheetName)
                ? excelPackage.Workbook.Worksheets[1] 
                : excelPackage.Workbook.Worksheets[_providerSettings.WorksheetName];

            return worksheet;
        }

        public List<string> GetTerms(ExcelWorksheet worksheet)
        {
            var result = new List<string>();
            //var worksheet.CellsfilteredCells = worksheet.Cells
            //    .Where(cell =>
            //        !(_providerSettings.HasHeader
            //          && cell.Address.Equals($"{_providerSettings.SourceColumn}1")));
            var index = 0;
            

            foreach (var cell in worksheet.Cells[$"{_providerSettings.SourceColumn}:{_providerSettings.ApprovedColumn}"])
            {
                if (_providerSettings.HasHeader && index == 0)
                {
                    index++;
                }
                var entry = new Entry
                {
                     Id = index,
                      Languages = new List<IEntryLanguage>() { new EntryLanguage
                      {
                         Name = "",
                          Terms = new List<IEntryTerm>()
                          {
                              new EntryTerm
                              {
                                   Value = "",
                                    Fields = new List<IEntryField>
                                    {
                                        new EntryField
                                        {
                                             
                                        }
                                    }
                              }
                          }  
                      } }
                };
                result.Add(GetSource(cell, index));
                index++;
            }

            //return filteredCells.Cells[$"{_providerSettings.SourceColumn}:{_providerSettings.SourceColumn}"]

            //    .Select(cell => (string) cell.Value)
            //    .ToList();

            return result;
        }

        private string GetSource(ExcelRangeBase cell, int index)
        {
            return cell.Text;
        }
    }
}
