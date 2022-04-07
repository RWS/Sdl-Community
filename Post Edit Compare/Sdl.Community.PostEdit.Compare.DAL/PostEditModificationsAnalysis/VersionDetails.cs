using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class VersionDetails
	{
		public static string Versions { get; set; }
		public static string Language { get; set; }
		public static string NotTranslated { get; set; }
		public static string Draft { get; set; }
		public static string Translated { get; set; }
		public static string TranslationRejected { get; set; }
		public static string TranslationApproved { get; set; }

		public static string SignOffRejected { get; set; }
		public static string SignedOff { get; set; }
	}
}
