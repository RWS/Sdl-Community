﻿using System.Globalization;

namespace GoogleTranslatorProvider.Models
{
	public class V3LanguageModel
	{
		public bool SupportTarget { get; set; }

		public bool SupportSource { get; set; }

		public string GoogleLanguageCode { get; set; }

		public CultureInfo CultureInfo { get; set; }
	}
}