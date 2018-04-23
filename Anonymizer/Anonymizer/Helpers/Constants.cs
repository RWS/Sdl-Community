using System;
using System.ComponentModel;
using System.Text;
using Sdl.Community.Anonymizer.Models;

namespace Sdl.Community.Anonymizer.Helpers
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
				@"Select from the grid bellow which rules should be applied. To use a rule enable the checkbox  in the column 'Enable'.
				If the data matching the expression should be encrypted check 'Encrypt' box. By default the batch task tag  matching data." +
				Environment.NewLine + @"New expressions can be added by filling the empty row at the end of the grid." +
				Environment.NewLine + @"To edit an expression double click on the cell." +
				Environment.NewLine + @"To remove a rule, select the rows and hit 'Delete' button";
		}
	}
}
