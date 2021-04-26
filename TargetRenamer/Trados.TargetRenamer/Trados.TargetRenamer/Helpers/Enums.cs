using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trados.TargetRenamer.Helpers
{
	public enum ReplacementEnum
	{
		Prefix,

		Suffix,

		RegularExpression,
	}

	public enum TargetLanguageEnum
	{
		TargetLanguage,

		[Description("Custom String")]
		CustomString,
	}
}
