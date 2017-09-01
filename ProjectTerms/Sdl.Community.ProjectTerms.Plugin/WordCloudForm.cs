using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class WordCloudForm : Form
    {
        public WordCloudForm()
        {
            InitializeComponent();
        }

        public void PopulateWordCloud(IEnumerable<ITerm> terms)
        {
            _cloudControl.WeightedTerms = terms;
        }
    }
}
