// Copyright (c) 2009 Jeff Beeghly
// mailto:jeffb42@hotmail.com
// Originally published at http://www.codeproject.com/KB/miscctrl/CheckGBAndRadioGB.aspx
// under the title, "CheckGroupBox and RadioGroupBox"
//
// This file and the accompanying files of this project may be freely used provided the following
// conditions are met:
//        * This copyright statement is not removed or modified.
//        * The code is not sold in uncompiled form.  (Release as a compiled binary which is part
//          of an application is fine)
//        * The design, code, or compiled binaries are not "Re-branded".
//
// Optional:
//        * I receive credit in the about box of the released product (something along the lines of
//          "CheckGroupBox Copyright (c) 2009 Jeff Beeghly").
//        * I receive a fully licensed copy of the product (regardless of whether the product is
//          is free, shrinkwrap, or commercial).  This is optional, though if you release products
//          which use code I've contributed to, I would appreciate a fully licensed copy.
//
// In addition, you may not:
//        * Publicly release modified versions of the code or publicly release works derived from
//          the code without express written authorization.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace UIToolbox
{
	/// <summary>
	/// CheckGroupBox is a GroupBox with an embeded CheckBox.
	/// </summary>
	[ToolboxBitmap(typeof(CheckGroupBox), "CheckGroupBox.bmp")]
    public partial class CheckGroupBox : GroupBox
    {
        // Constants
        private const int CHECKBOX_X_OFFSET = 10;

        private const int CHECKBOX_Y_OFFSET = 0;

        // Members
        private bool m_bDisableChildrenIfUnchecked;

        /// <summary>
        /// CheckGroupBox public constructor.
        /// </summary>
        public CheckGroupBox()
        {
            this.InitializeComponent();
            this.m_bDisableChildrenIfUnchecked = true;
            this.m_checkBox.Parent = this;
            this.m_checkBox.Location = new System.Drawing.Point(CHECKBOX_X_OFFSET, CHECKBOX_Y_OFFSET);
            this.Checked = true;

            if (VisualStyleInformation.IsSupportedByOS &&
                VisualStyleInformation.IsEnabledByUser &&
                Application.RenderWithVisualStyles)
            {
                // Set the color of the CheckBox's text to the color of the label in a standard groupbox control.
                VisualStyleRenderer vsr = new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal);
                Color groupBoxTextColor = vsr.GetColor(ColorProperty.TextColor);
                this.m_checkBox.ForeColor = groupBoxTextColor;
            }
        }

        #region Properties

        /// <summary>
        /// The text associated with the control.
        /// </summary>
        public override string Text
        {
            get
            {
                if (this.Site != null && this.Site.DesignMode == true)
                {
                    // Design-time mode
                    return this.m_checkBox.Text;
                }
                else
                {
                    // Run-time
                    return " "; // Set the text of the GroupBox to a space, so the gap appears before the CheckBox.
                }
            }
            set
            {
                base.Text = " "; // Set the text of the GroupBox to a space, so the gap appears before the CheckBox.
                this.m_checkBox.Text = value;
            }
        }

        /// <summary>
        /// Indicates whether the component is checked or not.
        /// </summary>
        [Description("Indicates whether the component is checked or not.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool Checked
        {
            get
            {
                return this.m_checkBox.Checked;
            }
            set
            {
                if (this.m_checkBox.Checked != value)
                {
                    this.m_checkBox.Checked = value;
                }
            }
        }

        /// <summary>
        /// Indicates the state of the component.
        /// </summary>
        [Description("Indicates the state of the component.")]
        [Category("Appearance")]
        [DefaultValue(CheckState.Checked)]
        public CheckState CheckState
        {
            get
            {
                return this.m_checkBox.CheckState;
            }
            set
            {
                if (this.m_checkBox.CheckState != value)
                {
                    this.m_checkBox.CheckState = value;
                }
            }
        }

        /// <summary>
        /// Determines if child controls of the GroupBox are disabled when the CheckBox is unchecked.
        /// </summary>
        [Description("Determines if child controls of the GroupBox are disabled when the CheckBox is unchecked.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool DisableChildrenIfUnchecked
        {
            get
            {
                return this.m_bDisableChildrenIfUnchecked;
            }
            set
            {
                if (this.m_bDisableChildrenIfUnchecked != value)
                {
                    this.m_bDisableChildrenIfUnchecked = value;
                }
            }
        }

        #endregion Properties

        #region Event Handlers

        /// <summary>
        /// Occurs whenever the Checked property of the CheckBox is changed.
        /// </summary>
        [Description("Occurs whenever the Checked property of the CheckBox is changed.")]
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Occurs whenever the CheckState property of the CheckBox is changed.
        /// </summary>
        [Description("Occurs whenever the CheckState property of the CheckBox is changed.")]
        public event EventHandler CheckStateChanged;

        /// <summary>
        /// Raises the System.Windows.Forms.CheckBox.checkBox_CheckedChanged event.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Raises the System.Windows.Forms.CheckBox.CheckStateChanged event.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
        }

        #endregion Event Handlers

        #region Events

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.m_bDisableChildrenIfUnchecked == true)
            {
                bool bEnabled = this.m_checkBox.Checked;
                foreach (Control control in this.Controls)
                {
                    if (control != this.m_checkBox)
                    {
                        control.Enabled = bEnabled;
                    }
                }
            }

            if (CheckedChanged != null)
            {
                CheckedChanged(sender, e);
            }
        }

        private void checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckStateChanged != null)
            {
                CheckStateChanged(sender, e);
            }
        }

        private void CheckGroupBox_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.m_bDisableChildrenIfUnchecked == true)
            {
                e.Control.Enabled = this.Checked;
            }
        }

        #endregion Events
    }
}