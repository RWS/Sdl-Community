using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	/// <summary>
	/// A match which includes the output produced by an FST.
	/// </summary>
	public class FSTMatch : Match
	{
		private string _Output;

		public FSTMatch(int index, int length, string output)
			: base(index, length)
		{
			_Output = output;
		}

		public string Output
		{
			get { return _Output; }
			set { _Output = value; }
		}
	}
}
