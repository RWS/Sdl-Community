using System;
using System.Windows.Documents;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Action
	{	
		public PersistentObjectToken TmId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Previous { get; set; }
		public string Value { get; set; }
		public string Result { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public Span PreviousSpan { get; set; }

		[JsonIgnore]
		[XmlIgnore]
		public Span ValueSpan { get; set; }
	}
}
