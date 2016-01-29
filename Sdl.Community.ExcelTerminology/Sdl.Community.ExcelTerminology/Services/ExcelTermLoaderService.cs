using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public  class ExcelTermLoaderService : IExcelTermLoaderService
    {
        private readonly ProviderSettings _providerSettings;

        public ExcelTermLoaderService(ProviderSettings providerSettings)
        {
            _providerSettings = providerSettings;
        }

        public Dictionary<int, ExcelTerm> LoadTerms()
        {

            var workSheet = GetTerminologyWorksheet();
            return GetTermsFromExcel(workSheet);
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

        public Dictionary<int, ExcelTerm> GetTermsFromExcel(ExcelWorksheet worksheet)
        {
            var result = new Dictionary<int, ExcelTerm>();

            var excelRangeAddress = _providerSettings.GetExcelRangeAddress();

            foreach (var cell in worksheet.Cells[excelRangeAddress])
            {
                var excellCellAddress = new ExcelCellAddress(cell.Address);

                if (_providerSettings.HasHeader && excellCellAddress.Row == 1)
                {
                    continue;
                }
                var id = excellCellAddress.Row;
                if (!result.ContainsKey(id))
                {
                    result[id] = new ExcelTerm();
                }

                SetCellValue(result[id], cell, excellCellAddress.Column);
               
            }
            return result;
        }

        private void SetCellValue(ExcelTerm excelTerm, ExcelRangeBase cell, int columnIndex)
        {
            var columnLetter = ExcelCellAddress.GetColumnLetter(columnIndex);
            if (columnLetter == _providerSettings.SourceColumn)
            {
                excelTerm.Source = cell.Text;
                excelTerm.SourceCulture = _providerSettings.SourceLanguage;
            }

            if (columnLetter == _providerSettings.TargetColumn)
            {
                excelTerm.Target = cell.Text;
                excelTerm.TargetCulture = _providerSettings.TargetLanguage;
            }

            if (columnLetter == _providerSettings.ApprovedColumn)
            {
                excelTerm.Approved = cell.Text;
            }
        }

    }
}
