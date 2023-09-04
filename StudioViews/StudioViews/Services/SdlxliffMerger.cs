using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Community.StudioViews.Services
{
	public class SdlxliffMerger
	{
		/// <summary>
		/// Merges the <param name="files"/> to <param name="mergeFilePath"/> and optionally
		/// deletes the input <param name="files"/> if <param name="cleanup"/> is set to true
		/// </summary>
		/// <param name="files">List of files to merge</param>
		/// <param name="mergeFilePath">The full path to the output merged systemFile</param>
		/// <param name="cleanup">Deletes the input files if set to true</param>
		/// <param name="progressLogger"></param>
		/// <returns></returns>
		public bool MergeFiles(IReadOnlyList<string> files, string mergeFilePath, bool cleanup, Action<string, int, int> progressLogger)
		{
			if (files.Count <= 1)
			{
				return false;
			}

			var filesContent = new List<string>();
			var revDefsContent = new List<string>();
			var repDefsContent = new List<string>();
			var cmtDefsContent = new List<string>();

			for (var i = 0; i < files.Count; i++)
			{
				progressLogger.Invoke("Merging files...", i + 1, files.Count);

				string fileContent;
				using (var reader = new StreamReader(files[i], Encoding.UTF8))
				{
					fileContent = reader.ReadToEnd();
					reader.Close();
				}

				var fileDocInfoContent = GetElementContent(fileContent, "doc-info");

				var revDefContent = GetElementContent(fileDocInfoContent, "rev-defs");
				if (!string.IsNullOrEmpty(revDefContent.Trim()))
				{
					revDefsContent.Add(revDefContent);
				}

				var repDefContent = GetElementContent(fileDocInfoContent, "rep-defs");
				if (!string.IsNullOrEmpty(repDefContent.Trim()))
				{
					repDefsContent.Add(repDefContent);
				}

				var cmtDefContent = GetElementContent(fileDocInfoContent, "cmt-defs");
				if (!string.IsNullOrEmpty(cmtDefContent.Trim()))
				{
					cmtDefsContent.Add(cmtDefContent);
				}

				filesContent.Add(GetFileContent(fileContent));
			}

			var docInfoContent = BuildDocInfoContent(revDefsContent, repDefsContent, cmtDefsContent);
			WriteMergedFile(mergeFilePath, docInfoContent, filesContent);

			// cleanup individual export files
			if (cleanup)
			{
				DeleteFiles(files);
			}

			return true;
		}

		private static string GetElementContent(string content, string elementName)
		{
			var expression = @"<" + elementName + @"(|[^\>]*?)>(?<valueContent>(|.*?))</" + elementName + @">";
			var regexSearchSingleline = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var regexSearchMultiline = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);

			var matchSearch = regexSearchSingleline.Match(content, 0);
			var valueContent = string.Empty;
			if (matchSearch.Success)
			{
				valueContent = matchSearch.Groups["valueContent"].Value;
			}
			else
			{
				matchSearch = regexSearchMultiline.Match(content, 0);
				if (matchSearch.Success)
				{
					valueContent = matchSearch.Groups["valueContent"].Value;
				}
			}

			return valueContent;
		}

		private static string GetFileContent(string content)
		{
			// greedy match
			var expression = @"(?<file><file\s+.*</file>)";
			var regexFileSingleline = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var regexFileMultiline = new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Multiline);

			var matchFile = regexFileSingleline.Match(content, 0);
			var fileContent = string.Empty;
			if (matchFile.Success)
			{
				fileContent = matchFile.Groups["file"].Value;
			}
			else
			{
				matchFile = regexFileMultiline.Match(content, 0);
				if (matchFile.Success)
				{
					fileContent = matchFile.Groups["file"].Value;
				}
			}

			return fileContent;
		}

		private static string BuildDocInfoContent(IReadOnlyCollection<string> revDefs, IReadOnlyCollection<string> repDefs, IReadOnlyCollection<string> cmtDefs)
		{
			var content = new StringBuilder(@"<doc-info xmlns=""http://sdl.com/FileTypes/SdlXliff/1.0"">");

			if (revDefs.Count > 0)
			{
				content.Append(@"<rev-defs>");
				foreach (var value in revDefs)
				{
					content.Append(value);
				}
				content.Append(@"</rev-defs>");
			}

			if (repDefs.Count > 0)
			{
				content.Append(@"<rep-defs>");
				foreach (var value in repDefs)
				{
					content.Append(value);
				}
				content.Append(@"</rep-defs>");
			}

			if (cmtDefs.Count > 0)
			{
				content.Append(@"<cmt-defs>");
				foreach (var value in cmtDefs)
				{
					content.Append(value);
				}
				content.Append(@"</cmt-defs>");
			}

			content.Append(@"</doc-info>");

			return content.ToString();
		}

		private static void WriteMergedFile(string filePath, string docInfoContent, IEnumerable<string> filesContent)
		{
			using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				writer.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>");

				writer.Write(@"<xliff xmlns:sdl=""http://sdl.com/FileTypes/SdlXliff/1.0"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" version=""1.2"" sdl:version=""1.0"">");

				if (!string.IsNullOrEmpty(docInfoContent.Trim()))
				{
					writer.Write(docInfoContent);
				}

				foreach (var fileContent in filesContent)
				{
					if (!string.IsNullOrEmpty(fileContent.Trim()))
					{
						writer.Write(fileContent);
					}
				}

				writer.Write("</xliff>");
			}
		}

		private bool DeleteFiles(IEnumerable<string> files)
		{
			try
			{
				foreach (var path in files)
				{
					File.Delete(path);
				}

				return true;
			}
			catch
			{
				//catch all; ignore
				return false;
			}
		}
	}
}
