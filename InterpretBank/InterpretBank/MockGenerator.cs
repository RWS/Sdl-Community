using System.Collections.Generic;
using InterpretBank.TermSearch;
using NSubstitute;

namespace InterpretBankTests;

public class MockGenerator
{
	//TODO: rename class; rethink purpose
	public ITermSearchService GetTermSearchService()
	{
		var termSearchService = Substitute.For<ITermSearchService>();
		termSearchService
			.GetFuzzyTerms(default, default, default)
			.ReturnsForAnyArgs(new List<TermEntry>
			{
				new() { Text = "firstTerm", Extra1 = "firstTermCommentary", Extra2 = "firstTermSecondCommentary" },
				new()
				{
					Text = "secondTerm", Extra1 = "secondTermCommentary", Extra2 = "secondTermSecondCommentary"
				}
			});

		return termSearchService;
	}
}