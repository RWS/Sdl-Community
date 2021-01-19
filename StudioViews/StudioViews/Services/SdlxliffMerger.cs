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
		/// <returns></returns>
		public bool MergeFiles(IReadOnlyList<string> files, string mergeFilePath, bool cleanup)
		{
			if (files.Count <= 1)
			{
				return false;
			}

			var filesContent = new List<string>();
			var xliffElement = string.Empty;
			for (var i = 0; i < files.Count; i++)
			{
				string content;
				using (var reader = new StreamReader(files[i], Encoding.UTF8))
				{
					content = reader.ReadToEnd();
					reader.Close();
				}

				filesContent.Add(GetFileContent(content));

				if (i == 0)
				{
					xliffElement = GetXliffFileContent(content);
				}
			}

			WriteMergedFile(mergeFilePath, xliffElement, filesContent);

			// cleanup individual export files
			if (cleanup)
			{
				DeleteFiles(files);
			}

			return true;
		}

		private static string GetXliffFileContent(string content)
		{
			var regexXliff = new Regex(@"(?<xliff><xliff\s+[^\>]*?>)", RegexOptions.IgnoreCase);
			var matchXliff = regexXliff.Match(content);
			if (matchXliff.Success)
			{
				return matchXliff.Groups["xliff"].Value;
			}

			return string.Empty;
		}

		private static string GetFileContent(string content)
		{
			// greedy match
			var regexFileSingleline = new Regex(@"(?<file><file\s+.*</file>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var regexFileMultiline = new Regex(@"(?<file><file\s+.*</file>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			
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

		private static void WriteMergedFile(string name, string xliffFileContent, IEnumerable<string> filesContent)
		{
			using (var writer = new StreamWriter(name, false, Encoding.UTF8))
			{
				writer.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>");
				writer.Write(xliffFileContent);
				foreach (var fileContent in filesContent)
				{
					writer.Write(fileContent);
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
