using System.ComponentModel;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Interfaces
{
	public interface IAnonymizerSettings
	{
		BindingList<RegexPattern> RegexPatterns { get; set; }
		string EncryptionKey { get; set; }
		bool DefaultListAlreadyAdded { get; set; }

		bool SelectAll { get; set; }

		bool EnableAll { get; set; }

		bool EncryptAll { get; set; }

		BindingList<RegexPattern> GetRegexPatterns();

		string GetEncryptionKey();
	}
}