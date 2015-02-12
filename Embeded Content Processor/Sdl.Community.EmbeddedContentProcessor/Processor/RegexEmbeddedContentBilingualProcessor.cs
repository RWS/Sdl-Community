using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class RegexEmbeddedContentBilingualProcessor : AbstractRegexEmbeddedContentBilingual
    {
        private readonly IContentEvaluator _evaluator;
        public RegexEmbeddedContentBilingualProcessor(IContentEvaluator evaluator):base(evaluator)
        {
            _evaluator = evaluator;
        }

        protected override IMarkupDataVisitor GetContentVisitor()
        {
            return new ProcessorEmbeddedContentVisitor(ItemFactory, _evaluator, MatchRules);

        }
    }
}
