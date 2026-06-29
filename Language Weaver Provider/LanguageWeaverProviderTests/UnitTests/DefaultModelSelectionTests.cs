using System.Collections.Generic;
using LanguageWeaverProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.ViewModel;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Offline unit tests for the default-model selection rule used when adding the Language Weaver
    /// provider to a project (<c>PairMappingViewModel.SelectDefault</c>). On Cloud the "Pro" model
    /// (a model whose <c>Model</c> id contains "pro") is preferred when present; otherwise — and for
    /// Edge — the first model is kept. No files, no network, no VM construction.
    /// </summary>
    public class DefaultModelSelectionTests
    {
        private static PairModel Model(string model, string name = null)
            => new() { Model = model, Name = name ?? model, DisplayName = name ?? model };

        [Fact]
        public void Cloud_PrefersProModel_EvenWhenNotFirst()
        {
            var generic = Model("generic");
            var pro = Model("lw-pro");

            var result = PairMappingViewModel.SelectDefault(PluginVersion.LanguageWeaverCloud, new List<PairModel> { generic, pro });

            Assert.Same(pro, result);
        }

        [Fact]
        public void Cloud_ProMatchIsCaseInsensitive()
        {
            var generic = Model("generic");
            var pro = Model("LW-PRO");

            var result = PairMappingViewModel.SelectDefault(PluginVersion.LanguageWeaverCloud, new List<PairModel> { generic, pro });

            Assert.Same(pro, result);
        }

        [Fact]
        public void Cloud_NoProModel_FallsBackToFirst()
        {
            var generic = Model("generic");
            var genericqe = Model("genericqe");

            var result = PairMappingViewModel.SelectDefault(PluginVersion.LanguageWeaverCloud, new List<PairModel> { generic, genericqe });

            Assert.Same(generic, result);
        }

        [Fact]
        public void Edge_DoesNotPreferPro_ReturnsFirst()
        {
            var generic = Model("generic");
            var pro = Model("lw-pro");

            var result = PairMappingViewModel.SelectDefault(PluginVersion.LanguageWeaverEdge, new List<PairModel> { generic, pro });

            Assert.Same(generic, result);
        }

        [Fact]
        public void Cloud_NullModelField_DoesNotMatchAndDoesNotThrow()
        {
            var unavailable = Model(null, "Model unavailable");
            var pro = Model("pro");

            var result = PairMappingViewModel.SelectDefault(PluginVersion.LanguageWeaverCloud, new List<PairModel> { unavailable, pro });

            Assert.Same(pro, result);
        }
    }
}
