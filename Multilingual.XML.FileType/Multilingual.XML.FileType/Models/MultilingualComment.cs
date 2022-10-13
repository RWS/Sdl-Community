using System;

namespace Multilingual.XML.FileType.Models
{
	public class MultilingualComment
	{
		public string Text { get; set; }

		/// <summary>Name of person or system that entered the comment</summary>
		public string Author { get; set; }

		/// <summary>
		/// Comments in a <see cref="T:Sdl.FileTypeSupport.Framework.NativeApi.ICommentProperties" /> collection are assigned
		/// incremented version numbers to better be able to track comment threads.
		/// The version number should be of the form "1.0", "2.0" etc. for
		/// compatibility with TRADOS and SDLX.
		/// </summary>
		public string Version { get; set; }

		/// <summary>Time at which the comment was created or last edited</summary>
		public DateTime Date { get; set; }

		public bool DateSpecified { get; set; }

		/// <summary>
		/// Indication of severity of the issue for which the comment has been added
		///
		/// The severity level has not been specified
		///		Undefined = 0,
		/// Informational purpose
		///		Low = 1,
		/// Warning, likely an important issue
		///		Medium = 2,
		/// Error, a severe issue
		///		High = 3,
		/// Sentinel, not used
		///		Invalid = 100, // 0x00000064
		/// </summary>
		public int Severity { get; set; }
	}
}
