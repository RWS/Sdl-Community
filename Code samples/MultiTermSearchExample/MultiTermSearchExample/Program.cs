using System;
using System.Collections.Generic;
using MultiTermSearchExample.Model;

namespace MultiTermSearchExample
{
	class Program
	{
		static void Main(string[] args)
		{
			Execute();
		}

		private static void Execute()
		{
			var userId = string.Empty;
			var password = string.Empty;
			var termbasePath = "<termbase_path>";

			var provider = new TerminologyProvider(termbasePath, userId, password);

			var maximumHits = 100;
			var fuzzySearch = true;
			var searchExpression = "<search text>";
			var sourceIndex = "English";
			var targetIndex = "German";

			var termHits = provider.SearchTerm(
				searchExpression, fuzzySearch, maximumHits, sourceIndex, targetIndex);

			// write results to the console
			Console.WriteLine("Term hits: {0}\n", termHits?.Count);
			if (termHits == null)
			{
				return;
			}

			WriteOutputToConsole(termHits);

			Console.ReadLine();
		}

		private static void WriteOutputToConsole(List<TermHit> termHits)
		{
			foreach (var termHit in termHits)
			{
				Console.WriteLine("Found: '{0}'; Match score: {1}%; Index: {2}; Language: {3}",
					termHit.Text, termHit.SearchScore, termHit.ParentEntryId, termHit.Language);

				foreach (var termEntry in termHit.TermEntries)
				{
					Console.WriteLine("  Term: '{0}'; Read only: {1}; Language: {2}",
						termEntry.Term, termEntry.ReadOnly, termEntry.Language);
					
					WriteEntryFields("Term Fields", termEntry.TermFields);
					WriteEntryFields("Index Fields", termEntry.IndexFields);
				}

				Console.WriteLine();
			}
		}

		private static void WriteEntryFields(string name, List<EntryField> entryFields)
		{
			if (entryFields.Count > 0)
			{
				Console.WriteLine("    {0}: {1}", name, entryFields.Count);
				foreach (var entryField in entryFields)
				{
					Console.WriteLine("      Name: {0}; Value: {1}",
						entryField.Name, entryField.Value);
				}
			}
		}
	}
}
