using System.ComponentModel;
using System.IO;
using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Interfaces;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services;
using Sdl.Core.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	public class AnonymizerSettings : SettingsGroup, IAnonymizerSettings
	{
		private readonly BindingList<RegexPattern> _regexPatterns = new BindingList<RegexPattern>();

		public bool? IsOldVersion
		{
			get => GetSetting<bool?>(nameof(IsOldVersion));
			set => GetSetting<bool?>(nameof(IsOldVersion)).Value = value;
		}

		public State EncryptionState
		{
			get => GetSetting<State>(nameof(EncryptionState));
			set => GetSetting<State>(nameof(EncryptionState)).Value = value;
		}

		public bool? ShouldAnonymize
		{
			get => GetSetting<bool?>(nameof(ShouldAnonymize));
			set => GetSetting<bool?>(nameof(ShouldAnonymize)).Value = value;
		}

		public bool? ShouldDeanonymize
		{
			get => GetSetting<bool?>(nameof(ShouldDeanonymize));
			set => GetSetting<bool?>(nameof(ShouldDeanonymize)).Value = value;
		}

		public BindingList<RegexPattern> RegexPatterns
		{
			get
			{
				BindingList<RegexPattern> regexPatterns;
				try
				{
					regexPatterns = GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns));
				}
				catch
				{
					var projectFile = GetProjectFile();

					if (string.IsNullOrEmpty(projectFile) || !File.Exists(projectFile))
					{
						return new BindingList<RegexPattern>();
					}

					var settingsService = new AnonymizerSettingsService(projectFile);
					regexPatterns = new BindingList<RegexPattern>(settingsService.GetRegexPatternSettings());
				}

				return regexPatterns;
			}
			set => GetSetting<BindingList<RegexPattern>>(nameof(RegexPatterns)).Value = value;
		}

		private static string GetProjectFile()
		{
			var projectFile = string.Empty;

			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			if (ActiveViewService.Instance.ProjectsViewIsActive)
			{
				var selectedProjects = projectsController?.SelectedProjects?.ToList();
				if (selectedProjects?.Count > 0)
				{
					projectFile = selectedProjects[0].FilePath;
				}
				else if (projectsController?.CurrentProject != null)
				{
					projectFile = projectsController.CurrentProject.FilePath;
				}
			}
			else if (projectsController?.CurrentProject != null)
			{
				projectFile = projectsController.CurrentProject.FilePath;
			}

			return projectFile;
		}

		public string EncryptionKey
		{
			get => GetSetting<string>(nameof(EncryptionKey));
			set => GetSetting<string>(nameof(EncryptionKey)).Value = AnonymizeData.EncryptData(value, Constants.Key);
		}

		public bool DefaultListAlreadyAdded
		{
			get => GetSetting<bool>(nameof(DefaultListAlreadyAdded));
			set => GetSetting<bool>(nameof(DefaultListAlreadyAdded)).Value = value;
		}

		public bool SelectAll
		{
			get => GetSetting<bool>(nameof(SelectAll));
			set => GetSetting<bool>(nameof(SelectAll)).Value = value;
		}

		public bool EnableAll
		{
			get => GetSetting<bool>(nameof(EnableAll));
			set => GetSetting<bool>(nameof(EnableAll)).Value = value;
		}

		public bool EncryptAll
		{
			get => GetSetting<bool>(nameof(EncryptAll));
			set => GetSetting<bool>(nameof(EncryptAll)).Value = value;
		}

		public bool IgnoreEncrypted
		{
			get => GetSetting<bool>(nameof(IgnoreEncrypted));
			set => GetSetting<bool>(nameof(IgnoreEncrypted)).Value = value;
		}

		//Initialize settings with default regex list
		public void AddPattern(RegexPattern pattern)
		{
			_regexPatterns.Add(pattern);
		}

		public BindingList<RegexPattern> GetRegexPatterns()
		{
			return RegexPatterns;
		}

		public string GetEncryptionKey()
		{
			return EncryptionKey;
		}
		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(RegexPatterns):
					return _regexPatterns;

				case nameof(EncryptionKey):
					return "<dummy-encryption-key>";
			}
			return base.GetDefaultValue(settingId);
		}

		public bool? HasBeenCheckedByControl
		{
			get => GetSetting<bool?>(nameof(HasBeenCheckedByControl));
			set => GetSetting<bool?>(nameof(HasBeenCheckedByControl)).Value = value;
		}
	}
}