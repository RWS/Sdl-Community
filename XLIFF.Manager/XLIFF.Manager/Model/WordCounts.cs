using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WordCounts: ICloneable
	{
		public WordCounts()
		{
			Processed = new List<WordCount>();
			Excluded = new List<WordCount>();
			NotProcessed = new List<WordCount>();
		}

		[XmlArray]
		[XmlArrayItem("WordCount", Type = typeof(WordCount))]
		public List<WordCount> Processed { get; set; }

		[XmlArray]
		[XmlArrayItem("WordCount", Type = typeof(WordCount))]
		public List<WordCount> Excluded { get; set; }

		[XmlArray]
		[XmlArrayItem("WordCount", Type = typeof(WordCount))]
		public List<WordCount> NotProcessed { get; set; }

		public object Clone()
		{
			var wordCounts = new WordCounts();
			
			wordCounts.Processed = new List<WordCount>();
			foreach (var wordCount in Processed)
			{
				wordCounts.Processed.Add(wordCount.Clone() as WordCount);
			}

			wordCounts.Excluded = new List<WordCount>();
			foreach (var wordCount in Excluded)
			{
				wordCounts.Excluded.Add(wordCount.Clone() as WordCount);
			}

			wordCounts.NotProcessed = new List<WordCount>();
			foreach (var wordCount in NotProcessed)
			{
				wordCounts.NotProcessed.Add(wordCount.Clone() as WordCount);
			}

			return wordCounts;
		}
	}
}
