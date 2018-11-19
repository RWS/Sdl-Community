using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services
{
	public class AnonymizerSettingsService
	{
		private readonly string _projectFile;

		public AnonymizerSettingsService(string projectFile)
		{
			_projectFile = projectFile;
		}

		/// <summary>
		/// Attempts to parse the 'RegexPatterns' from the project file.
		/// </summary>
		/// <returns></returns>
		public List<RegexPattern> GetRegexPatternSettings()
		{
			var regexPatternsResults = new List<RegexPattern>();

			if (string.IsNullOrEmpty(_projectFile) || !File.Exists(_projectFile))
			{
				return regexPatternsResults;
			}

			string settingsContent;
			using (var reader = new StreamReader(_projectFile, Encoding.UTF8))
			{
				settingsContent = reader.ReadToEnd();
			}

			var regexAnonymizerSettings = new Regex(@"\<SettingsGroup Id=""AnonymizerSettings""\>(?<AnonymizerSettings>.*)\</SettingsGroup\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			var matchAnonymizerSettings = regexAnonymizerSettings.Match(settingsContent);
			if (matchAnonymizerSettings.Success)
			{
				var anonymizerSettingsXml = matchAnonymizerSettings.Groups["AnonymizerSettings"].Value;

				var regexRegexPatterns = new Regex(@"\<Setting Id\=""RegexPatterns""\>(?<RegexPatterns>.*?)\<\/Setting\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				var matchRegexPatterns = regexRegexPatterns.Match(anonymizerSettingsXml);
				if (matchRegexPatterns.Success)
				{
					var regexPatternsXml = matchRegexPatterns.Groups["RegexPatterns"].Value;

					var regexRegexPattern = new Regex(@"\<RegexPattern\>(?<RegexPattern>.*?)\<\/RegexPattern\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					var matchesRegexPatternList = regexRegexPattern.Matches(regexPatternsXml);

					var regexId = new Regex(@"\<Id\>(?<Id>.*?)\<\/Id\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					var regexDescription = new Regex(@"\<Description\>(?<Description>.*?)\<\/Description\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					var regexPattern = new Regex(@"\<Pattern\>(?<Pattern>.*?)\<\/Pattern\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					var regexIsDefaultPath = new Regex(@"\<IsDefaultPath\>(?<IsDefaultPath>.*?)\<\/IsDefaultPath\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);					
					var regexShouldEnable = new Regex(@"\<ShouldEnable\>(?<ShouldEnable>.*?)\<\/ShouldEnable\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					var regexShouldEncrypt = new Regex(@"\<ShouldEncrypt\>(?<ShouldEncrypt>.*?)\<\/ShouldEncrypt\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
					
					foreach (Match matchRegexPattern in matchesRegexPatternList)
					{
						var newRegexPattern = new RegexPattern();

						var targetPatternXml = matchRegexPattern.Groups["RegexPattern"].Value;
						
						var matchId = regexId.Match(targetPatternXml);
						if (matchId.Success)
						{
							newRegexPattern.Id = matchId.Groups["Id"].Value;
						}

						var matchDescription = regexDescription.Match(targetPatternXml);
						if (matchDescription.Success)
						{
							newRegexPattern.Description = matchDescription.Groups["Description"].Value;
						}

						var matchPattern = regexPattern.Match(targetPatternXml);
						if (matchPattern.Success)
						{
							newRegexPattern.Pattern = matchPattern.Groups["Pattern"].Value;
						}

						var matchIsDefaultPath = regexIsDefaultPath.Match(targetPatternXml);
						if (matchIsDefaultPath.Success)
						{
							var value = matchIsDefaultPath.Groups["IsDefaultPath"].Value;
							newRegexPattern.IsDefaultPath = !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
						}

						var matchShouldEnable = regexShouldEnable.Match(targetPatternXml);
						if (matchShouldEnable.Success)
						{
							var value = matchShouldEnable.Groups["ShouldEnable"].Value;
							newRegexPattern.ShouldEnable = !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
						}

						var matchShouldEncrypt = regexShouldEncrypt.Match(targetPatternXml);
						if (matchShouldEncrypt.Success)
						{
							var value = matchShouldEncrypt.Groups["ShouldEncrypt"].Value;
							newRegexPattern.ShouldEncrypt = !string.IsNullOrEmpty(value) && Convert.ToBoolean(value);
						}

						regexPatternsResults.Add(newRegexPattern);
					}
				}
			}

			return regexPatternsResults;
		}
	}
}
