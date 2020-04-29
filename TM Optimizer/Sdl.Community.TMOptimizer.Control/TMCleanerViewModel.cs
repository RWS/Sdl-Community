using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Sdl.Community.TMOptimizerLib;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMOptimizer.Control
{
	public class TMCleanerViewModel : INotifyPropertyChanged
    {
        public TMCleanerViewModel()
        {
            InputTmxFiles = new ObservableCollection<InputTmxFile>();
            Mode = WizardMode.ConvertTranslationMemory;
            OutputMethod = OutputMethod.CreateNewTranslationMemory;
            InputTranslationMemory = new TranslationMemoryReference();
            OutputTranslationMemory = new TranslationMemoryReference();
            ProcessingSteps = new ObservableCollection<ProcessingStep>();
            Settings = new Settings();
            ProcessingState = ProcessingState.NotProcessing;
            CanFinish = false;
        }

        BackgroundWorker _worker;
        public event PropertyChangedEventHandler PropertyChanged;

        private WizardMode _mode;
        public WizardMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
                OnPropertyChanged("UseExistingTranslationMemory");
            }
        }

        
        public bool UseExistingTranslationMemory
        {
            get
            {
                return Mode == WizardMode.CleanExistingTranslationMemory;
            }
        }

        public ObservableCollection<InputTmxFile> InputTmxFiles
        {
            get;
            private set;
        }

        private TranslationMemoryReference _inputTranslationMemory;
        public TranslationMemoryReference InputTranslationMemory
        {
            get
            {
                return _inputTranslationMemory;
            }
            set
            {
                _inputTranslationMemory = value;
                OnPropertyChanged("InputTranslationMemory");
            }
        }

        private OutputMethod _outputMethod;
        public OutputMethod OutputMethod
        {
            get
            {
                return _outputMethod;
            }
            set
            {
                _outputMethod = value;
                OnPropertyChanged("OutputMethod");
            }
        }

        private string _newOutputTranslationMemoryFilePath;
        public string NewOutputTranslationMemoryFilePath
        {
            get
            {
                return _newOutputTranslationMemoryFilePath;
            }
            set
            {
                _newOutputTranslationMemoryFilePath = value;
                OnPropertyChanged("NewOutputTranslationMemoryFilePath");
            }
        }

        public Language SourceLanguage
        {
            get;
            set;
        }

        public Language TargetLanguage
        {
            get;
            set;
        }

        public Settings Settings
        {
            get;
            private set;
        }

        private TranslationMemoryReference _outputTranslationMemory;
        public TranslationMemoryReference OutputTranslationMemory
        {
            get
            {
                return _outputTranslationMemory;
            }
            set
            {
                _outputTranslationMemory = value;
                OnPropertyChanged("OutputTranslationMemory");
            }
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
                OnPropertyChanged("IsProcessing");
                OnPropertyChanged("IsCompleted");
            }
        }

        public bool IsProcessing
        {
            get
            {
                return ProcessingState == ProcessingState.Processing || ProcessingState == ProcessingState.Canceling;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return ProcessingState == ProcessingState.Completed;
            }
        }

        private bool _canFinish;
        public bool CanFinish
        {
            get
            {
                return _canFinish;
            }
            set
            {
                _canFinish = value;
                OnPropertyChanged("CanFinish");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

        public void SelectExistingTranslationMemory()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Select TM to optimize";
            dlg.DefaultExt = ".sdltm";
            dlg.Filter = "SDL Trados Studio TMs (.sdltm)|*.sdltm";

            if (!String.IsNullOrEmpty(InputTranslationMemory.FilePath))
            {
                dlg.FileName = InputTranslationMemory.FilePath;
            }

            if (dlg.ShowDialog() == true)
            {
                InputTranslationMemory.FilePath = dlg.FileName;
            }
        }

        public void AddTmxInputFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Select TMX files to clean up";
            dlg.DefaultExt = ".tmx";
            dlg.Filter = "TRADOS Workbench TMX (.tmx)|*.tmx";
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == true)
            {
                string[] tmxFiles = dlg.FileNames;
                _worker = new BackgroundWorker();
                _worker.DoWork += (sender, e) =>
                {
                    foreach (string fileName in tmxFiles)
                    {
                        InputTmxFile tmxFile = new InputTmxFile(new TmxFile(fileName));

                        try
                        {
                            SafeAddInputTmxFile(tmxFile);
                            tmxFile.TmxFile.Detect();
                        }
                        catch (Exception ex)
                        {
                            DisplayError(ex);
                            SafeRemoveInputTmxFile(tmxFile);
                        }
                    }
                };
                _worker.RunWorkerCompleted += (sender, e) =>
                {
                    
                };
                
                _worker.RunWorkerAsync();               
            }
        }

        public void SafeAddInputTmxFile(InputTmxFile tmxFile)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                InputTmxFiles.Add(tmxFile);
            }));
        }

        public void SafeRemoveInputTmxFile(InputTmxFile tmxFile)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                InputTmxFiles.Remove(tmxFile);
            }));
        }

        public void SelectNewOutputTranslationMemory()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = "Specify output TM file name";
            dlg.DefaultExt = ".sdltm";
            dlg.Filter = "SDL Trados Studio TMs (.sdltm)|*.sdltm";

            if (!String.IsNullOrEmpty(NewOutputTranslationMemoryFilePath))
            {
                dlg.FileName = NewOutputTranslationMemoryFilePath;
            }

            if (dlg.ShowDialog() == true)
            {
                NewOutputTranslationMemoryFilePath = dlg.FileName;
            }
        }

        public void SelectExistingOutputTranslationMemory()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Select output TM";
            dlg.DefaultExt = ".sdltm";
            dlg.Filter = "SDL Trados Studio TMs (.sdltm)|*.sdltm";

            if (!String.IsNullOrEmpty(OutputTranslationMemory.FilePath))
            {
                dlg.FileName = OutputTranslationMemory.FilePath;
            }

            if (dlg.ShowDialog() == true)
            {
                OutputTranslationMemory.FilePath = dlg.FileName;
            }
        }
        
        /// <summary>
        /// Validates the input sources (TMX and optional TM)
        /// </summary>
        /// <returns></returns>
        public bool ValidateInput()
        {
            int totalTuCount = 0;
            FileBasedTranslationMemory tm = null;

            if (UseExistingTranslationMemory)
            {
                if (InputTranslationMemory == null || String.IsNullOrEmpty(InputTranslationMemory.FilePath))
                {
                    ShowError("Select the Studio translation memory to clean up.");
                    return false;
                }

                try
                {
                    // try to access the TM
                    tm = InputTranslationMemory.TranslationMemory;
                    totalTuCount += tm.LanguageDirection.GetTranslationUnitCount();
                }
                catch (Exception ex)
                {
                    ShowError("Failed to open the translation memory: \r\n" + ex.ToString());
                    return false;
                }
            }
            
            if (InputTmxFiles.Count == 0)
            {
                ShowError("Select at least one input TMX file.");
                return false;
            }

            if (InputTmxFiles.Any(f => f.TmxFile.IsDetecting))
            {
                ShowError("Please wait while the TMX files are being analyzed.");
                return false;
            }

            if (InputTmxFiles.Any(f => f.TmxFile.DetectInfo.DetectedVersion != DetectInfo.Versions.Workbench))
            {
                ShowError("One or more TMX files are not Workbench export files.");
                return false;
            }

            if (InputTmxFiles.Any(f => f.TmxFile.DetectInfo == null || f.TmxFile.DetectInfo.SourceLanguage == null || f.TmxFile.DetectInfo.TargetLanguage == null))
            {
                ShowError("The source and target language of one or more TMX files could not be detected. Make sure the TMX file is valid or remove it from the list.");
                return false;
            }

            CultureInfo sourceCulture = tm != null ? tm.LanguageDirection.SourceLanguage : InputTmxFiles.First().TmxFile.DetectInfo.SourceLanguage.CultureInfo;
            CultureInfo targetCulture = tm != null ? tm.LanguageDirection.TargetLanguage : InputTmxFiles.First().TmxFile.DetectInfo.TargetLanguage.CultureInfo;

            foreach (InputTmxFile f in InputTmxFiles)
            {
                if (!CultureInfoExtensions.AreCompatible(f.TmxFile.DetectInfo.SourceLanguage.CultureInfo, sourceCulture)
                    || !CultureInfoExtensions.AreCompatible(f.TmxFile.DetectInfo.TargetLanguage.CultureInfo, targetCulture))
                {
                    ShowError("Make sure all the selected TMX files have compatible languages. The main language has to match.");
                    return false;
                }

                totalTuCount += f.TmxFile.GetDetectInfo().TuCount;
            }

            SourceLanguage = new Language(sourceCulture);
            TargetLanguage = new Language(targetCulture);

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(Application.Current.MainWindow, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool StartProcessing()
        {
            if (!ValidateOutput())
            {
                return false;
            }

            CreateProcessingSteps();

            ExecuteProcessingStepsAsync();

            return true;
        }

        public void CancelAsync()
        {
            if (_worker != null)
            {
                ProcessingState = ProcessingState.Canceling;
                
                foreach (ProcessingStep step in ProcessingSteps)
                {
                    step.RequestCancelAsync();
                }
            }
        }

        public bool ValidateOutput()
        {
            if (OutputMethod == OutputMethod.UpdateExistingTranslationMemory)
            {
                if (OutputTranslationMemory == null || String.IsNullOrEmpty(OutputTranslationMemory.FilePath))
                {
                    ShowError("Select the Studio translation memory to import the clean translation units into.");
                    return false;
                }

                try
                {
                    // try to access the TM
                    FileBasedTranslationMemory tm = OutputTranslationMemory.TranslationMemory;

                    if (!CultureInfoExtensions.AreCompatible(tm.LanguageDirection.SourceLanguage, SourceLanguage.CultureInfo)
                     || !CultureInfoExtensions.AreCompatible(tm.LanguageDirection.TargetLanguage, TargetLanguage.CultureInfo))
                    {
                        ShowError("The languages of the selected translation memory are not compatible with the languages of the input TMX files. The main language has to match.");
                        return false;
                    }

                    SourceLanguage = new Language(tm.LanguageDirection.SourceLanguage);
                    TargetLanguage = new Language(tm.LanguageDirection.TargetLanguage);
                }
                catch (Exception ex)
                {
                    ShowError("Failed to open the translation memory: \r\n" + ex.ToString());
                    return false;
                }
            }
            else
            {
                // new TM
                if (String.IsNullOrEmpty(NewOutputTranslationMemoryFilePath))
                {
                    ShowError("Select the location where the new translation memory should be created.");
                    return false;
                }
            }

            return true;
        }

        public void CreateProcessingSteps()
        {
            ProcessingSteps.Clear();
            ProcessingContext context = new ProcessingContext();
            context.Settings = Settings;
            InputTmxFile existingTmtmxFile = null;
            if (Mode == WizardMode.CleanExistingTranslationMemory)
            {
                existingTmtmxFile = new InputTmxFile(new TmxFile(context.GetTempTmxFile()));
                existingTmtmxFile.CleanTmxFile = new TmxFile(context.GetTempTmxFile());
                
                // export TM
                ProcessingSteps.Add(new ExportStudioTmStep(InputTranslationMemory.TranslationMemory, existingTmtmxFile.TmxFile));
                // strip workbench TUs
                ProcessingSteps.Add(new StripWorkbenchTusStep(InputTranslationMemory.TranslationMemory.Name, existingTmtmxFile.TmxFile, existingTmtmxFile.CleanTmxFile));
            }

            foreach (InputTmxFile inputTmxFile in InputTmxFiles)
            {
                // clean TMX
                inputTmxFile.CleanTmxFile = new TmxFile(context.GetTempTmxFile());
                ProcessingSteps.Add(new CleanWorkbenchTmxStep(inputTmxFile.TmxFile, inputTmxFile.CleanTmxFile, null));
            }

            TranslationMemoryReference outputTranslationMemory;

            if (OutputMethod == OutputMethod.CreateNewTranslationMemory)
            {
                outputTranslationMemory = new TranslationMemoryReference { FilePath = NewOutputTranslationMemoryFilePath };
                // create output TM
                ProcessingSteps.Add(new CreateStudioTmStep(outputTranslationMemory, SourceLanguage, TargetLanguage, InputTranslationMemory));
            }
            else
            {
                outputTranslationMemory = OutputTranslationMemory;
            }

            foreach (InputTmxFile inputTmxFile in InputTmxFiles)
            {
                // import clean TMX
                ProcessingSteps.Add(new ImportStudioTmStep(outputTranslationMemory, Path.GetFileName(inputTmxFile.TmxFile.FilePath), inputTmxFile.CleanTmxFile));
            }

            if (existingTmtmxFile != null)
            {
                ProcessingSteps.Add(new ImportStudioTmStep(outputTranslationMemory, Path.GetFileName(InputTranslationMemory.FilePath), existingTmtmxFile.CleanTmxFile));
            }

            ProcessingSteps.Add(new DeleteTempFilesStep());

            // set context of all steps
            foreach (ProcessingStep step in ProcessingSteps)
            {
                step.Context = context;
            }
        }

        public void ExecuteProcessingStepsAsync()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += (sender, e) => 
            {
                foreach (ProcessingStep step in ProcessingSteps)
                {
                    step.Execute();

                    if (step.Error != null)
                    {
                        DisplayError(step.Error);
                        return;
                    }

                    step.ReportProgress(100);
                }
            };
            _worker.RunWorkerCompleted += (sender, e) => 
            {
                if (ProcessingState == ProcessingState.Canceling)
                {
                    ProcessingState = ProcessingState.Canceled;
                }
                else if (ProcessingSteps.Any(s => s.Error != null))
                {
                    ProcessingState = ProcessingState.Failed;
                }
                else
                {
                    ProcessingState = ProcessingState.Completed;
                }

                CanFinish = true;
            };
            ProcessingState = ProcessingState.Processing;
            _worker.RunWorkerAsync();
        }

        private void DisplayError(Exception e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                MessageBox.Show(Application.Current.MainWindow, e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public ObservableCollection<ProcessingStep> ProcessingSteps
        {
            get;
            private set;
        }

        public void OpenContainerFolder()
        {
            string dirName = null;
            if (OutputMethod == OutputMethod.CreateNewTranslationMemory)
            {
                dirName = Path.GetDirectoryName(NewOutputTranslationMemoryFilePath);
            }
            else
            {
                dirName = Path.GetDirectoryName(OutputTranslationMemory.FilePath);
            }

            Process.Start(dirName);
        }

        public void OpenOutputTMInStudio()
        {
            string filePath = null;
            if (OutputMethod == OutputMethod.CreateNewTranslationMemory)
            {
                filePath = NewOutputTranslationMemoryFilePath;
            }
            else
            {
                filePath = OutputTranslationMemory.FilePath;
            }

            Process.Start(filePath);
        }
    }
}