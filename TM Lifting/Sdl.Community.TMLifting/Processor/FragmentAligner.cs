
using System;
using System.Threading;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMLifting.Processor
{
    public class FragmentAligner
    {
        private readonly FileBasedTranslationMemory _tm;

        public FragmentAligner(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public void AlignTranslationUnits()
        {

			var _lastProgressNumber = 0;
			var cts = new CancellationTokenSource();
			var token = cts.Token;

			var progress = new Progress<int>(i =>
			{
				_lastProgressNumber = i;
			});

			_tm.AlignTranslationUnits(true, false, token, progress);

            _tm.Save();
			var unaligned = _tm.UnalignedTranslationUnitCount;
		}
    }
}