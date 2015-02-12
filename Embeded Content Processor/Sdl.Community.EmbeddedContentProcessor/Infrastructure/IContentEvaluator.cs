using System.Collections.Generic;
using Sdl.Community.EmbeddedContentProcessor.Settings;

namespace Sdl.Community.EmbeddedContentProcessor.Infrastructure
{
    public interface IContentEvaluator
    {
        List<ContentMatch> Evaluate(string text, List<MatchRule> rules);
    }
}
