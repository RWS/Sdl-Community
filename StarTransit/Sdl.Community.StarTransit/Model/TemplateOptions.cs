using System;
using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Model
{
	public class TemplateOptions
	{
		public string CustomerId { get; set; }
		public string ProjectLocation { get; set; }
		public DateTime? DueDate { get; set; }
		public List<LanguagePair> TemplateLanguagePairDetails { get; set; }
	}
}
