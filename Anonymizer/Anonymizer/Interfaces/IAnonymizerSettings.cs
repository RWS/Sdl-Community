using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Anonymizer.Models;

namespace Sdl.Community.Anonymizer.Interfaces
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
