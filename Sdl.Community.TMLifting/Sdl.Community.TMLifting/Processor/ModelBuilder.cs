using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMLifting.Processor
{
    public class ModelBuilder
    {
        private readonly FileBasedTranslationMemory _tm;

        public ModelBuilder(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public void BuildTranslationModel()
        {
            _tm.BuildModel();
        }
    }
}