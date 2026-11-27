using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;

namespace MultiTermStandAloneTestApp
{
	public partial class MainWindow : Window
	{
		private const int PageSize = 200;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var path = "";
			var termbaseUri = new Uri("ttb." + new Uri(path).AbsoluteUri);
			var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
			if (provider == null || !provider.Initialize() || !(provider is IStudioTermbase termbase))
			{
				return;
			}

			var numberOfEntriesForLanguage = GetNumberOfEntriesForEachLanguage(provider, termbase);
		}

		private Dictionary<string, int> GetNumberOfEntriesForEachLanguage(ITerminologyProvider provider, IStudioTermbase termbase)
		{
			var numberOfEntries = provider.Definition.Languages.ToDictionary(language => language.Name, language => 0);

			var lastSeenId = 0;
			while (true)
			{
				var page = termbase.GetEntries(lastSeenId, PageSize);
				if (page == null || page.Count == 0)
				{
					break;
				}

				foreach (var entryLanguage in page.SelectMany(entry => entry.Languages ?? Enumerable.Empty<EntryLanguage>()))
				{
					if (entryLanguage.Name != null && numberOfEntries.ContainsKey(entryLanguage.Name))
					{
						numberOfEntries[entryLanguage.Name]++;
					}
				}

				lastSeenId = page[page.Count - 1].Id;
				if (page.Count < PageSize)
				{
					break;
				}
			}

			return numberOfEntries;
		}
	}
}