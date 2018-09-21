using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Interfaces
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