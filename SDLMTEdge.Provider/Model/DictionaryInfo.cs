﻿using System.Collections.Generic;

namespace Sdl.Community.MTEdge.Provider.Model
{
	public class DictionaryInfo
	{
		public List<DictionaryModel> Dictionaries { get; set; }

		public int Page { get; set; }

		public int PerPage { get; set; }

		public int TotalPages { get; set; }

		public int TotalItems { get; set; }
	}
}