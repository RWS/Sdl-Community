using System;
using System.Collections.Generic;
using System.IO;
using InterpretBank.GlossaryExchangeService.ExchangeFormats;
using InterpretBank.GlossaryExchangeService.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers;

namespace InterpretBank.GlossaryExchangeService
{
	public class GlossaryExchangeService
	{

		public void ExportTerms(Format export, string path, IEnumerable<string[]> terms, string glossaryName = null,
			string subGlossaryName = null)
		{
			switch (export)
			{
				case Format.Tbx:
					new TbxExport(new TbxDocumentWrapper(), path).ExportTerms(terms, glossaryName, subGlossaryName);
					break;

				case Format.Excel:
					new ExcelExport(new ExcelDocumentWrapper(), path).ExportTerms(terms);
					break;
			}
		}

		public IEnumerable<string[]> ImportTerms(string path)
		{
			var extension = Path.GetExtension(path).TrimStart('.');

            IImport import = extension switch
            {
                "xlsx" => new ExcelImport(new ExcelDocumentWrapper(), path),
                "tbx" => new TbxImport(new TbxDocumentWrapper(), path),
                _ => throw new ArgumentException(
                    string.Format(
                        PluginResources
                            .GlossaryExchangeServiceManager_CreateFileReader_The_file_type__0__is_not_supported,
                        extension), extension)
            };

			return import.ImportTerms();
		}
	}
}