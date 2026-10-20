using System.Collections.Generic;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Offline unit tests for <c>CloudModelFlagsRefresher.ApplyServerFlags</c>: saved Cloud pair mappings must pick up
    /// capability flags (qeSupport, adaptation) from the live language-pairs response without losing user selections.
    /// No network: the "live" models stand in for <c>GET /v4/accounts/{id}/subscriptions/language-pairs</c>.
    /// </summary>
    public class CloudModelFlagsRefresherTests
    {
        private static PairModel Model(string model, bool qeSupport = false)
            => new() { Name = $"engfra_{model}", Model = model, DisplayName = model, SourceLanguageId = "eng", TargetLanguageId = "fra", QeSupport = qeSupport };

        // After JSON deserialization SelectedModel is a separate instance from the matching entry in Models.
        private static PairMapping Mapping(List<PairModel> models, PairModel selectedModel)
            => new() { DisplayName = "English - French", Models = models, SelectedModel = selectedModel };

        [Fact]
        public void ModelGainsQeSupport_UpdatesModelsAndSelectedModelInPlace()
        {
            var cachedPro = Model("pro");
            var selectedPro = Model("pro");
            selectedPro.LinguisticOptions = [new LinguisticOption { Id = "formality", Name = "Formality", SystemDefault = "Default", Values = ["Default", "Polite"], SelectedValue = "Polite" }];
            var mapping = Mapping([Model("generic"), cachedPro], selectedPro);

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([mapping], [Model("generic"), Model("pro", qeSupport: true)]);

            Assert.True(changed);
            Assert.True(cachedPro.QeSupport);
            Assert.Same(selectedPro, mapping.SelectedModel);
            Assert.True(mapping.SelectedModel.QeSupport);
            Assert.Equal("Polite", mapping.SelectedModel.LinguisticOptions[0].SelectedValue);
        }

        [Fact]
        public void ModelLosesQeSupport_IsUpdated()
        {
            var selected = Model("genericqe", qeSupport: true);
            var mapping = Mapping([Model("genericqe", qeSupport: true)], selected);

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([mapping], [Model("genericqe")]);

            Assert.True(changed);
            Assert.False(selected.QeSupport);
            Assert.False(mapping.Models[0].QeSupport);
        }

        [Fact]
        public void AdaptationFlags_AreCopied()
        {
            var selected = Model("pro");
            var live = Model("pro");
            live.AdaptationSupport = true;
            live.AutoAdaptiveSupport = true;

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([Mapping([Model("pro")], selected)], [live]);

            Assert.True(changed);
            Assert.True(selected.AdaptationSupport);
            Assert.True(selected.AutoAdaptiveSupport);
        }

        [Fact]
        public void NothingChanged_ReturnsFalse()
        {
            var mapping = Mapping([Model("generic"), Model("pro", qeSupport: true)], Model("pro", qeSupport: true));

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([mapping], [Model("generic"), Model("pro", qeSupport: true)]);

            Assert.False(changed);
        }

        [Fact]
        public void ModelMissingFromApi_KeepsCachedValues()
        {
            var selected = Model("retired", qeSupport: true);
            var mapping = Mapping([Model("retired", qeSupport: true)], selected);

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([mapping], [Model("generic")]);

            Assert.False(changed);
            Assert.True(selected.QeSupport);
            Assert.Single(mapping.Models);
        }

        [Fact]
        public void NewApiModel_IsNotAddedToCachedModels()
        {
            var mapping = Mapping([Model("generic")], Model("generic"));

            CloudModelFlagsRefresher.ApplyServerFlags([mapping], [Model("generic"), Model("pro", qeSupport: true)]);

            Assert.Single(mapping.Models);
        }

        [Fact]
        public void OtherLanguagePairWithSameModelId_DoesNotLeak()
        {
            // Matching is on Name ("engfra_pro"), not on Model ("pro"), which repeats across language pairs.
            var selected = Model("pro");
            var engDeuPro = new PairModel { Name = "engdeu_pro", Model = "pro", SourceLanguageId = "eng", TargetLanguageId = "deu", QeSupport = true };

            var changed = CloudModelFlagsRefresher.ApplyServerFlags([Mapping([Model("pro")], selected)], [engDeuPro]);

            Assert.False(changed);
            Assert.False(selected.QeSupport);
        }

        [Fact]
        public void PlaceholderAndNullEntries_DoNotThrow()
        {
            // CreatePairMappings adds a placeholder (no Model id) when a pair has no engine; Models may also be null.
            var placeholder = new PairModel { Name = "Model unavailable", DisplayName = "Model unavailable" };
            var mappings = new List<PairMapping>
            {
                Mapping([placeholder], placeholder),
                Mapping(null, null),
                null
            };

            var changed = CloudModelFlagsRefresher.ApplyServerFlags(mappings, [Model("generic"), null]);

            Assert.False(changed);
        }

        [Fact]
        public void NullInputs_ReturnFalse()
        {
            Assert.False(CloudModelFlagsRefresher.ApplyServerFlags(null, [Model("generic")]));
            Assert.False(CloudModelFlagsRefresher.ApplyServerFlags([Mapping([Model("generic")], Model("generic"))], null));
        }
    }
}
