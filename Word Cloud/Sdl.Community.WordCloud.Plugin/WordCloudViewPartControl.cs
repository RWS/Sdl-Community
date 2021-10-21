using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.WordCloud.Controls.Geometry;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.WordCloud.Plugin
{
    public partial class WordCloudViewPartControl : UserControl, IUIControl
	{
        public WordCloudViewPartControl()
        {
            InitializeComponent();
            _generatingLabel.Visible = false;
            _progressBar.Visible = false;

            _cloudControl.Click += CloudControlClick;
        }

        WordCloudViewModel _viewModel; 
        internal WordCloudViewModel ViewModel 
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = value;
                _viewModel.SelectedProjectChanged += _viewModel_SelectedProjectChanged;
            }
        }

        void _viewModel_SelectedProjectChanged(object sender, EventArgs e)
        {
            _generateButton.Enabled = (ViewModel.Project != null) && (ViewModel.Project.GetProjectInfo().ProjectType != ProjectAutomation.Core.ProjectType.InLanguageCloud); 
            _generatingLabel.Visible = false;
            _progressBar.Visible = false;
            
            _cloudControl.WeightedWords = ViewModel.Words;
            _cloudControl.Invalidate();
        }

        private void _generateButton_Click(object sender, EventArgs e)
        {
            GenerateWordCloud();
        }

        public void GenerateWordCloud()
        {
            _progressBar.Value = 0;
            _generatingLabel.Visible = true;
            _progressBar.Visible = true;
            ViewModel.GenerateWordCloudAsync((result)
                =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(Form.ActiveForm, "Failed to generate word cloud: " + result.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _cloudControl.WeightedWords = result.WeightedWords;
                    
                }

                _progressBar.Visible = false;
                _generatingLabel.Visible = false;

            }, (progress) => { _progressBar.Value = progress; });
        }

        private void CloudControlClick(object sender, EventArgs e)
        {
            LayoutItem itemUderMouse;
            Point mousePositionRelativeToControl = _cloudControl.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            if (!_cloudControl.TryGetItemAtLocation(mousePositionRelativeToControl, out itemUderMouse))
            {
                return;
            }

            MessageBox.Show(
                string.Format("{0}: {1} occurrences.", itemUderMouse.Word.Text, itemUderMouse.Word.Occurrences),
                "Word Statistics");
        }
    }
}
