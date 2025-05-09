using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls
{
    public partial class ProgressBarForm : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        private readonly CancellationTokenSource _dataRetrievalCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationTokenSource _dataWritingCancellationTokenSource = new CancellationTokenSource();
        private string _firstLabelText;
        private bool _firstStep = true;
        private int _noOfPhases;
        private int _phase;
        private string _secondLabelText;
        private string _thirdLabelText;

        public ProgressBarForm(string firstLabel = "", string secondLabel = "", string thirdLabel = "")
        {
            InitializeComponent();
            SetToIndefinite();

            ConfigureProgressBar(firstLabel != string.Empty, secondLabel != string.Empty, thirdLabel != string.Empty);

            _firstLabelText = firstLabel;
            _secondLabelText = secondLabel;
            _thirdLabelText = thirdLabel;

            _firstLabel.Text = firstLabel;
            _secondLabel.Text = secondLabel;
            _thirdLabel.Text = thirdLabel;

            ShowingProgress += OnShowingProgress;

            Globals.ThisAddIn.Shutdown += ThisAddIn_Shutdown;
        }

        public delegate void ShowingProgressEventHandler();

        public event ShowingProgressEventHandler ShowingProgress;

        public CancellationToken DataRetrievalCancellationToken => _dataRetrievalCancellationTokenSource.Token;

        public CancellationToken DataWritingCancellationToken => _dataWritingCancellationTokenSource.Token;

        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public void SetToDefinite()
        {
            _firstProgressBar.Style = ProgressBarStyle.Continuous;
            _secondProgressBar.Style = ProgressBarStyle.Continuous;
        }

        public void ShowProgress(Progress progress, int barIndex = 1)
        {
            if (IsDisposed) return;
            ShowingProgress?.Invoke();

            switch (barIndex)
            {
                case 1:
                    _phase = _phase > 1 ? _phase : 1;
                    ShowProgress(_firstProgressBar, progress, _firstLabel);
                    break;

                case 2:
                    _phase = _phase > 2 ? _phase : 2;
                    ShowProgress(_secondProgressBar, progress, _secondLabel);
                    break;

                case 3:
                    _phase = 3;
                    ShowProgress(_thirdProgressBar, progress, _thirdLabel);
                    UpdateUi();
                    break;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_phase == 0) Close();
            if (_phase < _noOfPhases)
            {
                _dataRetrievalCancellationTokenSource.Cancel();
                return;
            }
            _dataWritingCancellationTokenSource.Cancel();
        }

        private void ConfigureProgressBar(bool firstBarVisible, bool secondBarVisible, bool thirdBarVisible)
        {
            _noOfPhases = GetNoOfPhases(firstBarVisible, secondBarVisible, thirdBarVisible);
            _firstProgressBar.Visible = firstBarVisible;
            _secondProgressBar.Visible = secondBarVisible;
            _thirdProgressBar.Visible = thirdBarVisible;

            _firstLabel.Visible = firstBarVisible;
            _secondLabel.Visible = secondBarVisible;
            _thirdLabel.Visible = thirdBarVisible;
        }

        private int GetNoOfPhases(bool firstBarVisible, bool secondBarVisible, bool thirdBarVisible)
        {
            return (firstBarVisible ? 1 : 0) + (secondBarVisible ? 1 : 0) + (thirdBarVisible ? 1 : 0);
        }

        private void OnShowingProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(OnShowingProgress));
                return;
            }
            if ((_firstStep))
            {
                _firstStep = false;
                SetToDefinite();
            }
            ShowingProgress -= OnShowingProgress;
        }

        private void SetToIndefinite()
        {
            _firstProgressBar.Style = ProgressBarStyle.Marquee;
            _firstProgressBar.MarqueeAnimationSpeed = 30;
            _secondProgressBar.Style = ProgressBarStyle.Marquee;
            _secondProgressBar.MarqueeAnimationSpeed = 30;
        }

        private void ShowProgress(ProgressBar progressBar, Progress progress, Label label)
        {
            if (_phase == _noOfPhases && progress.Numerator == progress.Denominator)
            {
                Close();
                return;
            }
            if (progress.Denominator != 0 && progress.Denominator != progressBar.Maximum)
            {
                progressBar.Maximum = progress.Denominator;
            }
            progressBar.Value = progress.Numerator;

            string text;
            switch (label.Name)
            {
                case "_firstLabel":
                    text = _firstLabelText;
                    break;

                case "_secondLabel":
                    text = _secondLabelText;
                    break;

                case "_thirdLabel":
                    text = _thirdLabelText;
                    break;

                default:
                    text = string.Empty;
                    break;
            }

            label.Text = $"{text} ({progress.Numerator}/{progressBar.Maximum})";
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            _dataRetrievalCancellationTokenSource.Cancel();
            _dataWritingCancellationTokenSource.Cancel();
        }

        private void UpdateUi()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => cancelButton.Text = Resources.Cancel_writing));
            }
            else
            {
                cancelButton.Text = Resources.Cancel_writing;
            }
        }
    }
}