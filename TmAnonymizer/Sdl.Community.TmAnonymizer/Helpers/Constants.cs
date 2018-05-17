using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class Constants
	{
		public static ObservableCollection<Rule> GetDefaultRules()
		{
			return new ObservableCollection<Rule>
			{
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "Email addresses",
					Name = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "PCI (Payment Card Industry)",
					Name = @"\b(?:\d[ -]*?){13,16}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "IP4 Address",
					Name = @"\b(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "MAC Address",
					Name = @"\b[0-9A-F]{2}([-:]?)(?:[0-9A-F]{2}\1){4}[0-9A-F]{2}\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "UK National Insurance Number",
					Name = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b"
				},
				new Rule
				{
					Id = Guid.NewGuid().ToString(),
					Description = "Social Security Numbers",
					Name = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b"
				}
			};
			
		}

		public static string TmBackupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			@"SDL Community\TmAnonymizer Backup");
	}
}

