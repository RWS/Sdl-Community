using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Threading.Tasks;

namespace Sdl.Community.FragmentAlignmentAutomation.Processors
{
    public class ModelBuilder
    {

        public event EventHandler<ProgressEventArgs> OnProgressChanged;

        private readonly FileBasedTranslationMemory _tm;

        public ModelBuilder(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public async Task BuildTranslationModel()
        {           
            var lastProgressNumber = 0;
            NotifySubscribers(0, 0, StringResources.FragmentAlignment_ProgressPreparingMessage);
            _tm.TranslationModelProgress += (o, args) =>
            {
                if (args.ProgressNumber > lastProgressNumber)
                {
                    var text = string.Empty;
                    if (args.ProgressLimit > 0)
                        text =
                            string.Format(
                                StringResources
                                    .FragmentAlignment_ProgressProcessedOfTranslationUnitsMessage
                                , ProcessorUtil.GetProgresssStageText(args.ProgressStage), args.ProgressNumber, args.ProgressLimit);

                    NotifySubscribers(args.ProgressLimit, args.ProgressNumber, text);
                    lastProgressNumber = args.ProgressNumber;
                }
            };

            var t = new Task(() => _tm.BuildModel());

          
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
                    Type = ProgressEventArgs.ProcessorType.ModelBuilder,
                    Description = description,
                    CurrentProgress = currentProgress,
                    TotalUnits = totalUnits
                });
            }               
        }
    }
}
