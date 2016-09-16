using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdlXliff.Toolkit.Integration.Controls
{
    public class ControlColumn : DataGridViewColumn
    {
        public ControlColumn()
            : base(new ControlCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(ControlCell)))
                {
                    throw new InvalidCastException("Must be a MyCell");
                }
                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// file path filter
        /// </summary>
        public bool FileFilter
        {
            get;
            set;
        }

        /// <summary>
        /// search or replace
        /// </summary>
        public bool IsSearch
        {
            get;
            set;
        }

        /// <summary>
        /// search in tags
        /// </summary>
        public bool SearchInTags
        {
            get;
            set;
        }

        /// <summary>
        /// search in source
        /// </summary>
        public bool SearchInSource
        {
            get;
            set;
        }

        /// <summary>
        /// search in target
        /// </summary>
        public bool SearchInTarget
        {
            get;
            set;
        }
    }
}
