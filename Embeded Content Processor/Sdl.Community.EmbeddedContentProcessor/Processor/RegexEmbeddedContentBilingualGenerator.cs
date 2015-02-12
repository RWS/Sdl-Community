using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class RegexEmbeddedContentBilingualGenerator : AbstractRegexEmbeddedContentBilingual
    {
        private readonly IContentEvaluator _evaluator;
        public RegexEmbeddedContentBilingualGenerator(IContentEvaluator evaluator)
            : base(evaluator)
        {
            _evaluator = evaluator;
        }

        protected override IMarkupDataVisitor GetContentVisitor()
        {
            return new GeneratorEmbeddedContentVisitor(ItemFactory);

        }
    }
}
