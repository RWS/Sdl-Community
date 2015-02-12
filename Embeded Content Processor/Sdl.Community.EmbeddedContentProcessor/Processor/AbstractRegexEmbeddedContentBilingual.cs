using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.Community.EmbeddedContentProcessor.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public abstract class AbstractRegexEmbeddedContentBilingual : AbstractBilingualContentProcessor, ISettingsAware
    {
        private readonly IContentEvaluator _evaluator;
        private bool _isEnabled;
        private readonly List<string> _structureInfos;
        private readonly List<MatchRule> _matchRules;
        readonly EmbeddedContentRegexSettings _settings;

        public List<MatchRule> MatchRules { get { return _matchRules; } }

        protected abstract IMarkupDataVisitor GetContentVisitor();

        protected AbstractRegexEmbeddedContentBilingual(IContentEvaluator evaluator)
        {
            _evaluator = evaluator;
            _settings = new EmbeddedContentRegexSettings();
            _isEnabled = false;
            _structureInfos = new List<string>();
            _matchRules = new List<MatchRule>();
        }

        public void InitializeSettings(Core.Settings.ISettingsBundle settingsBundle, string configurationId)
        {
            _settings.PopulateFromSettingsBundle(settingsBundle, configurationId);

            ApplySettings();
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (_isEnabled)
            {
                ProcessParagraphUnit(paragraphUnit.Source);
                ProcessParagraphUnit(paragraphUnit.Target);
            }
            base.ProcessParagraphUnit(paragraphUnit);
        }

        private void ProcessParagraphUnit(IParagraph paragraph)
        {
            var visitor = GetContentVisitor();

            foreach (var markup in paragraph)
            {
                if (markup is IStructureTag) continue;
                markup.AcceptVisitor(visitor);
            }

            var paragraphCreator = visitor as IParagraphCreator;
            if (paragraphCreator == null) return;
            var sourceParagraph = paragraphCreator.GetParagraph();

            CopyParagraph(sourceParagraph, paragraph);
        }

        private static void CopyParagraph(IParagraph fromParagraph, IParagraph toParagraph)
        {
            toParagraph.Clear();

            while (fromParagraph.Count > 0)
            {
                IAbstractMarkupData markupData = fromParagraph[0];
                fromParagraph.RemoveAt(0);

                toParagraph.Add(markupData);
            }
        }

        private void ApplySettings()
        {
            _isEnabled = _settings.Enabled;
            _structureInfos.Clear();
            _matchRules.Clear();

            _structureInfos.AddRange(_settings.StructureInfos);
            _matchRules.AddRange(_settings.MatchRules);
        }
    }
}
