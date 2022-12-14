using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Db
{
	public class TmxImportReport
	{
		public int TUsRead { get; set; } = 0;
		public int TUsImportedSuccessfully { get; set; } = 0;
		public int TUsWithSyntaxErrors { get; set; } = 0;
		public int TUsWithInvalidChars { get; set; } = 0;
		public int LanguageCount { get; set; } = 0;
		public DateTime StartTime { get; private set; } = DateTime.MinValue;
		public DateTime EndTime { get; set; } = DateTime.MinValue;
		public string FatalError { get; set; } = "";

		public static TmxImportReport StartNow()
		{
			var r = new TmxImportReport();
			r.StartTime = DateTime.Now;
			return r;
		}

		public bool IsFatalError => FatalError != "";
		public bool IsStarted => StartTime != DateTime.MinValue;
		public bool IsEnded => EndTime != DateTime.MinValue;
		public int ReportTimeSecs => IsStarted ? ((int)((IsEnded ? EndTime : DateTime.Now) - StartTime).TotalSeconds) : 0;

		public TmxImportReport Copy()
		{
			var copy = new TmxImportReport();
			copy.CopyFrom(this);
			return copy;
		}

		public void CopyFrom(TmxImportReport other)
		{
			TUsRead = other.TUsRead;
			TUsImportedSuccessfully = other.TUsImportedSuccessfully;
			TUsWithSyntaxErrors = other.TUsWithSyntaxErrors;
			TUsWithInvalidChars = other.TUsWithInvalidChars;
			LanguageCount = other.LanguageCount;
			StartTime = other.StartTime;
			EndTime = other.EndTime;
			FatalError = other.FatalError;
		}
	}
}
