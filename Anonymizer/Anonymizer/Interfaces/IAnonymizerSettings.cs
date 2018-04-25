using System.ComponentModel;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Interfaces
{
	public interface IAnonymizerSettings
	{
		BindingList<RegexPattern> RegexPatterns { get; set; }
		string EncryptionKey { get; set; }
		bool DefaultListAlreadyAdded { get; set; }
		BindingList<RegexPattern> GetRegexPatterns();
		string GetEncryptionKey();
		bool SelectAll { get; set; }
	}
}
