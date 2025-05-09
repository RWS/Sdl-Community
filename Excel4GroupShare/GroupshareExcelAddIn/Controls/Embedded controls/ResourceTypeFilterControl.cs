using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls.Embedded_controls
{
    public partial class ResourceTypeFilterControl : UserControl, IFilterControl
    {
        public ResourceTypeFilterControl()
        {
            InitializeComponent();
        }

        public FilterParameter FilterParameter
        {
            get
            {
                var filterParameter = new FilterParameter { ResourceTypes = new List<string>() };

                if (_projectTypeCheckBox.Checked)
                {
                    filterParameter.ResourceTypes.Add("Project");
                }

                if (_projectTemplateTypeCheckBox.Checked)
                {
                    filterParameter.ResourceTypes.Add("Project Template");
                }

                return filterParameter;
            }
        }
    }
}