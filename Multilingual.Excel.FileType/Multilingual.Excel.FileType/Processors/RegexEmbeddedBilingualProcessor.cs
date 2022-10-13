using System.Collections.Generic;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Multilingual.XML.FileType.Processors
{
    public class RegexEmbeddedBilingualProcessor : AbstractBilingualContentProcessor, ISettingsAware
    {
        private bool _isEnabled;
        private bool _filterByStructureInfo;
        private List<string> _structureInfos;
        private List<MatchRule> _matchRules;
        EmbeddedContentRegexSettings _settings;
        private readonly EmbeddedContentTagPropertiesCloner _embeddedContentTagPropertiesCloner;

        public RegexEmbeddedBilingualProcessor()
            : this(null)
        {
            _settings = GetProcessorSettings();
        }

        public RegexEmbeddedBilingualProcessor(EmbeddedContentRegexSettings settings)
        {
            _settings = settings;
            _isEnabled = false;
            _filterByStructureInfo = true;
            _structureInfos = new List<string>();
            _matchRules = new List<MatchRule>();

            _embeddedContentTagPropertiesCloner = new EmbeddedContentTagPropertiesCloner();
        }

        protected virtual EmbeddedContentRegexSettings GetProcessorSettings()
        {
            return new EmbeddedContentRegexSettings();
        }

        public List<string> StructureInfo
        {
            get { return _structureInfos; }
            set { _structureInfos = value; }
        }

        public List<MatchRule> MatchRules
        {
            get { return _matchRules; }
            set { _matchRules = value; }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        internal bool FilterByStructureInfo
        {
            get => _filterByStructureInfo;
            set => _filterByStructureInfo = value;
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (_isEnabled
                && (!_filterByStructureInfo || RegexProcessorHelper.HasEmbeddedContext(_structureInfos, paragraphUnit.Properties.Contexts)))
            {
                ProcessParagraph(paragraphUnit.Source);
                ProcessParagraph(paragraphUnit.Target);

               _embeddedContentTagPropertiesCloner.Process(paragraphUnit);
            }

            base.ProcessParagraphUnit(paragraphUnit);
        }


        public EmbeddedContentRegexSettings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                ApplySettings();
            }
        }

        private void ProcessParagraph(IParagraph paragraph)
        {
            EmbeddedContentVisitor visitor = new EmbeddedContentVisitor(ItemFactory, _matchRules);

            MergeTextElements(paragraph);

            visitor.VisitParagraph(paragraph);

            IParagraph sourceParagraph = visitor.GeneratedParagraph;

            CopyParagraphContents(sourceParagraph, paragraph);
        }

        private void MergeTextElements(IAbstractMarkupDataContainer container)
        {
            var limit = container.Count;
            var currentElementIndex = 0;

            while (currentElementIndex < limit)
            {
                if (container[currentElementIndex] is IAbstractMarkupDataContainer)
                {
                    MergeTextElements((IAbstractMarkupDataContainer) container[currentElementIndex]);
                }

                if (!(container[currentElementIndex] is IText))
                {
                    currentElementIndex++;
                    continue;
                }

                var destinationTextElement = container[currentElementIndex] as IText;
                currentElementIndex++;
                if (currentElementIndex == limit)
                    return;

                while (container[currentElementIndex] is IText)
                {
                    var sourceTextElement = container[currentElementIndex] as IText;
                    destinationTextElement.Properties.Text = destinationTextElement.Properties.Text + sourceTextElement.Properties.Text;

                    sourceTextElement.RemoveFromParent();

                    limit--;
                    if (currentElementIndex == limit)
                        return;
                }
            }
        }


        private void CopyParagraphContents(IParagraph fromParagraph, IParagraph toParagraph)
        {
            toParagraph.Clear();

            while (fromParagraph.Count > 0)
            {
                IAbstractMarkupData markup = fromParagraph[0];
                fromParagraph.RemoveAt(0);

                toParagraph.Add(markup);
            }
        }

        private void ApplySettings()
        {
            _isEnabled = _settings.Enabled;
            _filterByStructureInfo = _settings.FilterByStructureInfo;
            _structureInfos.Clear();
          
            foreach (var item in _settings.StructureInfos)
                _structureInfos.Add(item);

            _matchRules.Clear();

            foreach (var rule in _settings.MatchRules)
                _matchRules.Add(rule);
        }
  
        #region ISettingsAware Members

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            _settings.PopulateFromSettingsBundle(settingsBundle, configurationId);

            ApplySettings();
        }

        #endregion
    }
}
