using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processors.Extensions
{
    internal static class FragmentAlignmentExtensions
    {

        public static int MinimumTuThreshold = 1000;
        public static int RecommendedTuThreshold = 5000;

        public static bool CanBuildModelClientFlag(this FileBasedTranslationMemory fileBasedTranslationMemory)
        {
            return fileBasedTranslationMemory.FGASupport != FGASupport.NonAutomatic
                        && fileBasedTranslationMemory.CanBuildModel
                        && fileBasedTranslationMemory.GetTranslationUnitCount() >= MinimumTuThreshold;
        }
        public static bool ShouldBuildModelClientFlag(this FileBasedTranslationMemory fileBasedTranslationMemory)
        {
            return fileBasedTranslationMemory.CanBuildModelClientFlag()
                    && fileBasedTranslationMemory.ShouldBuildModel
                    && fileBasedTranslationMemory.GetTranslationUnitCount() >= RecommendedTuThreshold;
        }

        public static bool CanRemoveTranslationModelClientFlag(this FileBasedTranslationMemory fileBasedTranslationMemory)
        {
            return fileBasedTranslationMemory.TranslationModelExists();
        }
        public static bool CanRemoveAlignmentClientFlag(this FileBasedTranslationMemory fileBasedTranslationMemory)
        {
            return fileBasedTranslationMemory.TranslationModelExists()
                    && fileBasedTranslationMemory.CanBuildModelClientFlag()
                    && fileBasedTranslationMemory.UnalignedTranslationUnitCount < fileBasedTranslationMemory.GetTranslationUnitCount();
        }

        public static bool TranslationModelExists(this FileBasedTranslationMemory fileBasedTranslationMemory)
        {
            return fileBasedTranslationMemory.FGASupport != FGASupport.NonAutomatic
                   && fileBasedTranslationMemory.ModelDetails != null && fileBasedTranslationMemory.ModelDetails.ModelDate != null;
        }
    }
}
