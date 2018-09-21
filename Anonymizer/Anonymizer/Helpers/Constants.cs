using System;
using System.ComponentModel;
using System.IO;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Constants
	{
		public static string AcceptFolderPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			@"SDL Community\Anonymizer Projects Accept");

		public static string Key = @"dGhpc2lzdGhlZW5jb2RlZGt0eQ==";

		public static string AcceptFilePath = Path.Combine(AcceptFolderPath, "accept.json");

		public static BindingList<RegexPattern> GetDefaultRegexPatterns()
		{
			return new BindingList<RegexPattern>
			{
				new RegexPattern
				{
					Id = "1",
					Description = "Email addresses",
					Pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
					IsDefaultPath = true
				},

				new RegexPattern
				{
					Id = "2",
					Description = "PCI (Payment Card Industry)",
					Pattern = @"\b(?:\d[ -]*?){13,16}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "3",
					Description = "IP4 Address",
					Pattern = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "4",
					Description = "IP6 Address",
					Pattern = @"\b(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "5",
					Description = "MAC Address",
					Pattern = @"\b[0-9A-F]{2}([-:]?)(?:[0-9A-F]{2}\1){4}[0-9A-F]{2}\b",
					IsDefaultPath = true
				},

				new RegexPattern
				{
					Id = "8",
					Description = "UK National Insurance Number",
					Pattern = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "9",
					Description= "Social Security Numbers",
					Pattern = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
					IsDefaultPath = true
				}
			};
		}

		public static string GetGridDescription()
		{
			return
				@"Activate the “Enable” checkbox for all rules that should be applied to this project." +
				Environment.NewLine + @"Activate the “Encrypt” checkbox for all data that should be encrypted using an encryption key." +
				Environment.NewLine + @"New regular expressions, or plain text, can be added either by typing them into the empty row at the end of the grid, or by importing an Excel file containing the rules or lists of names, addresses etc. " +
				Environment.NewLine + @"To edit a rule double click on the cell." +
				Environment.NewLine + @"To remove a rule select the rows and hit the ‘Delete’ key.  Select all rules with Ctrl+A.";
		}

		public static string GetKeyDescription()
		{
			return @"Add an encryption key" +
				   Environment.NewLine + @"(Do not forget this or you won’t be able to decrypt the data later)";
		}

		public static string AcceptDescription()
		{
			return
				@"The tool has been designed to help the Client create specific rules in accordance with their requirements and tag identifiable information." +
				Environment.NewLine +
				@"SDL accepts no liability associated with creating such tags or any errors or omissions associated with the use of the tool or any deliverables.";
		}
	}
}