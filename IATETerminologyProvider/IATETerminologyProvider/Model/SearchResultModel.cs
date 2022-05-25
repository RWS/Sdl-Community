﻿using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SearchResultModel : ISearchResult
	{
		public int Id { get; set; }
		public string ItemId { get; set; }
		public string Text { get; set; }
		public int Score { get; set; }
		public ILanguage Language { get; set; }
		public string Definition { get; set; }
		public string Domain { get; set; }
		public string Subdomain { get; set; }
		public string TermType { get; set; }
		public int DisplayOrder { get; set; }
		public int Evaluation { get; set; }
	}
}