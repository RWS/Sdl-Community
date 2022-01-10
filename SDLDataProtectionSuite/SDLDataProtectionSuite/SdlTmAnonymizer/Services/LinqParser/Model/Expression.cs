using System.Text.RegularExpressions;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.LinqParser.Model
{
	public class Expression
	{
		public string FirstTerm { get; set; }
		public bool IsAtomic => Regex.Match(Operator, @"[<>=]|[<>]=").Success;
		public string Operator { get; set; }
		public string SecondTerm { get; set; }
		public override string ToString()
		{
			return $"{FirstTerm} {Operator} {SecondTerm}";
		}
	}
}