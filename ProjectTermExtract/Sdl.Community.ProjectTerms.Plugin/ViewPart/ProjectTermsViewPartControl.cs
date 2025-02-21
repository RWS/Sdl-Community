using System.Windows.Forms;
using System;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{
    public partial class ProjectTermsViewPartControl : UserControl, IUIControl
    {
        ProjectTermsViewModel viewModel;
        internal ProjectTermsViewModel ViewModel
        {
            get
            {
                return viewModel;
            }

            set
            {
                viewModel = value;
                viewModel.SelectedProjectChanged += viewModel_SelectedProjectChanged;
            }
        }

        public ProjectTermsViewPartControl()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitializeComponent();
        }

        void viewModel_SelectedProjectChanged(object sender, EventArgs e)
        {
            cloudControl.WeightedTerms = ViewModel.Terms;
            cloudControl.Invalidate();
        }

        public void GenerateWordCloud()
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
                    cloudControl.WeightedTerms = result.WeightedTerms;
                }
            });
        }
    }
}
