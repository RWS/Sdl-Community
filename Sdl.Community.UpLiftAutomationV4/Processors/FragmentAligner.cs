using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Threading.Tasks;

namespace Sdl.Community.FragmentAlignmentAutomation.Processors
{
    public class FragmentAligner
    {
        public event EventHandler<ProgressEventArgs> OnProgressChanged;

        private readonly FileBasedTranslationMemory _tm;
        private readonly bool _quickAlign;
        public FragmentAligner(FileBasedTranslationMemory tm, bool quickAlign = false)
        {
            _tm = tm;
            _quickAlign = quickAlign;
        }

        public async Task AlignTranslationUnits()
        {

            var iter = new RegularIterator();


            var translationUnitCount = _tm.GetTranslationUnitCount();

            var totalUnitsToProcess = _quickAlign
             ? _tm.UnalignedTranslationUnitCount
             : translationUnitCount;

            var lastProgressNumber = 0;

            NotifySubscribers(0, 0, StringResources.FragmentAlignment_ProgressPreparingMessage);

            var t = new Task(() =>
            {
                while (_tm.AlignTranslationUnits(false, _quickAlign, ref iter))
                {
                    if (iter.ProcessedTranslationUnits <= lastProgressNumber)
                        continue;
                    var text = string.Empty;
                    if (totalUnitsToProcess > 0)
                        text =
                            string.Format(
                                StringResources
                                    .FragmentAlignment_ProgressAlignedOfTranslationUnitsMessage
                                , iter.ProcessedTranslationUnits, totalUnitsToProcess);

                    NotifySubscribers(totalUnitsToProcess, iter.ProcessedTranslationUnits, text);
                    lastProgressNumber = iter.ProcessedTranslationUnits;
                }
            });

            
            t.ContinueWith(task =>
            {
                throw new Exception(ProcessorUtil.ExceptionToMsg(task.Exception));
            }, TaskContinuationOptions.OnlyOnFaulted);

            t.ContinueWith(task =>
            {
                if (task.IsFaulted)
                    throw new Exception(ProcessorUtil.ExceptionToMsg(task.Exception));
                if (!task.IsCompleted)
                    return;
                ProcessorUtil.UpdateTranslationMemory(_tm);
                NotifySubscribers(translationUnitCount, translationUnitCount, "Complete");

            }, TaskScheduler.FromCurrentSynchronizationContext());

            t.Start();
            await t;

        }

        private void NotifySubscribers(int totalUnits, int currentProgress, string description)
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged.Invoke(this, new ProgressEventArgs
                {
                    Type = ProgressEventArgs.ProcessorType.FragmentAligner,
                    Description = description,
                    CurrentProgress = currentProgress,
                    TotalUnits = totalUnits
                });
            } 
        }
    }
}
