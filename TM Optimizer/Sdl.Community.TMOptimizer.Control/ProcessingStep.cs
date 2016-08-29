using System;
using System.ComponentModel;
using Sdl.Community.TMOptimizerLib;

namespace Sdl.Community.TMOptimizer.Control
{
    public abstract class ProcessingStep : INotifyPropertyChanged
    {
        private int _progress;

        protected ProcessingStep(string name)
        {
            Name = name;
            ProcessingState = ProcessingState.NotProcessing;
        }

        public ProcessingContext Context
        {
            get;
            set;
        }


        public string Name
        {
            get;
            private set;
        }

        public int Progress
        {
            get
            {
                return _progress;
            }
            private set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }



        public Exception Error
        {
            get;
            set;
        }

        private ProcessingState _processingState;
        public ProcessingState ProcessingState
        {
            get
            {
                return _processingState;
            }
            set
            {
                _processingState = value;
                OnPropertyChanged("ProcessingState");
            }
        }

        public bool CancelRequested
        {
            get;
            private set;
        }

        public void RequestCancelAsync()
        {
            ProcessingState = ProcessingState.Canceling;
            CancelRequested = true;
        }

        public void Execute()
        {
            try
            {
                ProcessingState = ProcessingState.Processing;
                ExecuteImpl();

                if (CancelRequested)
                {
                    ProcessingState = ProcessingState.Canceled;
                }
                else
                {
                    ProcessingState = ProcessingState.Completed;
                }
            }
            catch (Exception e)
            {
                Error = e;
                ProcessingState = ProcessingState.Failed;
                return;
            }
            
        }

        protected abstract void ExecuteImpl();

        internal void ReportProgress(int progress)
        {
            Progress = progress;
            OnReportProgress();
        }

        /// <summary>
        /// Attached Progress event and takes care of forwarding cancellation requests to the processor.
        /// </summary>
        /// <param name="processor"></param>
        protected void AttachProcessorEvents(ProcessorBase processor)
        {
            processor.Progress += (sender, e) => 
                {
                    if (CancelRequested)
                    {
                        e.Cancel = true;
                        return;
                    }

                    ReportProgress(e.Progress);
                };
        }

        protected virtual void OnReportProgress()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
