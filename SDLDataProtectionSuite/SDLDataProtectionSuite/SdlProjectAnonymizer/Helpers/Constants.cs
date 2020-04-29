using System;
using System.ComponentModel;
using System.IO;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public static class Constants
	{
		public static string AcceptFolderPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			@"SDL Community\SDLProjectAnonymizer");

		public static string Key = @"dGhpc2lzdGhlZW5jb2RlZGt0eQ==";

		public static string AcceptFilePath = Path.Combine(AcceptFolderPath, "settings.json");

		public static BindingList<RegexPattern> GetDefaultRegexPatterns()
		{
			return new BindingList<RegexPattern>
			{
				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description = "Email addresses",
					Pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",
					IsDefaultPath = true
				},

				new RegexPattern
				{
					Id =Guid.NewGuid().ToString(),
					Description = "PCI (Payment Card Industry)",
					Pattern = @"\b(?:\d[ -]*?){13,16}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description = "IP4 Address",
					Pattern = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description = "IP6 Address",
					Pattern = @"\b(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description = "MAC Address",
					Pattern = @"\b[0-9A-F]{2}([-:]?)(?:[0-9A-F]{2}\1){4}[0-9A-F]{2}\b",
					IsDefaultPath = true
				},

				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description = "UK National Insurance Number",
					Pattern = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b",
					IsDefaultPath = true
				},
				new RegexPattern
				{
					Id = Guid.NewGuid().ToString(),
					Description= "Social Security Numbers",
					Pattern = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
					IsDefaultPath = true
				}
			};
		}

		public static string GetGridDescription()
		{
			return
				StringResources.Activate_the_Enable_checkbox_for_all_rules_that_should_be_applied_to_this_project +
				Environment.NewLine + StringResources.Activate_the_Enable_checkbox_for_all_data_that_should_be_encrypted_using_an_encryption_key +
				Environment.NewLine + StringResources.New_regular_expressions_or_plain_text__can_be_added_either_by_typing_them_into_ +
				Environment.NewLine + StringResources.To_edit_a_rule_double_click_on_the_cell +
				Environment.NewLine + StringResources.To_remove_a_rule_select_the_rows_and_hit_the_Delete_key + StringResources.Select_all_rules_with_Ctrl_A;
		}

		public static string GetKeyDescription()
		{
			return StringResources.Add_an_encryption_key +
				   Environment.NewLine + StringResources.Do_not_forget_this_or_you_wont_be_able_to_decrypt_the_data_later;
		}

		public static string AcceptDescription()
		{
			return
				StringResources.SDLProjectAnonymizer_AcceptDescription_Line01 +
				Environment.NewLine +
				StringResources.SDLProjectAnonymizer_AcceptDescription_Line02;
		}
	}
}