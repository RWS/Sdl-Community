using System.Windows.Forms;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Controls.Interfaces;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{
    public partial class ProjectTermsViewPartControl : UserControl
    {
        public ProjectTermsViewPartControl()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitializeComponent();
        }

        public void GenerateWordCloud(ProjectTermsViewModel viewModel)
        {
            viewModel.GenerateWordCloudAsync((result)
                =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(Form.ActiveForm, "Failed to generate word cloud: " + result.Exception.Message, "Error");
                }
                else
                {
                    _cloudControl.WeightedTerms = result.WeightedTerms;
                }


            });
        }

        public void Resetcloud()
        {
            _cloudControl.WeightedTerms = new List<ITerm>();
        }
    }
}
