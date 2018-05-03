using System;
using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Constants
	{
		public static BindingList<RegexPattern> GetDefaultRegexPatterns()
		{
			return new BindingList<RegexPattern>
			{
				new RegexPattern
				{
					Id = "1",
					Description = "email",
					Pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
					IsDefaultPath = true
				},

				new RegexPattern
				{
					Id = "2",
					Description = "PCI",
					Pattern = @"\b(?:\d[ -]*?){13,16}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "3",
					Description = "IP6 Address",
					Pattern = @"(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "4",
					Description = "Social Security Numbers",
					Pattern = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "5",
					Description = "Telephone Numbers",
					Pattern = @"\b\d{4}\s\d+-\d+\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "6",
					Description = "Car Registrations",
					Pattern = @"\b\p{Lu}+\s\p{Lu}+\s\d+\b|\b\p{Lu}+\s\d+\s\p{Lu}+\b|\b\p{Lu}+\d+\s\p{Lu}+\b|\b\p{Lu}+\s\d+\p{Lu}+\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "7",
					Description = "Passport Numbers",
					Pattern = @"\b\d{9}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "8",
					Description = "National Insurance Number",
					Pattern = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = "9",
					Description= "Date of Birth",
					Pattern = @"\b\d{2}/\d{2}/\d{4}\b",
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
				Environment.NewLine + @"To edit a rule double click on the cell."+
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
