using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TMProvider;

namespace TradosPlugin
{
    public partial class ProviderItemControl : UserControl
    {
        // delete icon clicked
        public delegate void DeleteClickedHandler(TMProviderBase provider);
        public event DeleteClickedHandler DeleteClicked;

        // provider chosen
        public delegate void SelectedHandler(object sender, EventArgs e);
        public event SelectedHandler ItemSelected;

        // size has changed
        public delegate void SizeChangedHandler(ProviderItemControl sender);
        public event SizeChangedHandler SizeChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TMProviderBase Provider { get; private set; }

        private bool selected;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value) this.BackColor = selectedColor;
                else this.BackColor = unselectedColor;
                selected = value;
            }
        }

        /// <summary>
        /// The height coming from the parent form - the ideal height to fit the other UI elements (eg. header)
        /// </summary>
        private int idealHeight;
        private ProviderLayout currentLayout;
        private Color hoverColor = Color.FromArgb(245, 245, 245);
        private Color selectedColor = Color.FromArgb(221, 221, 221);
        private Color inactiveColor = Color.Black; // Color.FromArgb(180, 180, 180);
        private Color unselectedColor = SystemColors.Window;


        public ProviderItemControl(TMProviderBase p, int idealHeight, int width)
        {
            this.idealHeight = idealHeight < 24 ? 24 : idealHeight;
            this.Provider = p;

            InitializeComponent();

            this.pctDelete.Resize += pctDelete_Resize;
            this.pctWarning.Resize += pctWarning_Resize;
            this.lblProviderName.Text = p.ProviderName;

            this.Selected = false;
            setLayout(p);

            lblProviderName.Click += onClick;
            this.MouseEnter += new System.EventHandler(this.OnMouseHover);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.MouseHover += new System.EventHandler(this.OnMouseHover);
            this.pctDelete.MouseEnter += new System.EventHandler(this.OnMouseHover);
            this.pctDelete.MouseHover += new System.EventHandler(this.OnMouseHover);
            this.pctDelete.MouseLeave += OnMouseLeave;
            this.lblProviderName.MouseEnter += new System.EventHandler(this.OnMouseHover);
            this.lblProviderName.MouseHover += new System.EventHandler(this.OnMouseHover);
            this.lblProviderName.MouseLeave += OnMouseLeave;

            toolTip1.SetToolTip(pctDelete, PluginResources.mnuDeleteServer);
            toolTip1.SetToolTip(pctWarning, PluginResources.SelfSignedCertificate);
        }

    
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetSize(this.Width);
            if (SizeChanged != null) SizeChanged(this);
        }

        protected override void OnResize(EventArgs e)
        {
            // SetSize(this.Width);
            pctDelete.Top = this.Height / 2 - pctDelete.Height / 2;
            pctWarning.Top = this.Height / 2 - pctWarning.Height / 2;
            base.OnResize(e);
        }

 

        private bool didMouseReallyLeave(object sender, EventArgs e)
        {
            if (this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition))) return false;
            return true;
            //base.OnMouseLeave(e);
        }

        private void pctDelete_Resize(object sender, EventArgs e)
        {
            pctDelete.Width = pctDelete.Height;
        }

        void pctWarning_Resize(object sender, EventArgs e)
        {
            pctWarning.Width = pctWarning.Height;
        }



        public void RefreshProvider(TMProviderBase p)
        {
            this.Provider = p;
            setLayout(p);
        }

        /// <summary>
        /// Sets the width and height of the control, and arranges its controls.
        /// </summary>
        /// <param name="width"></param>
        public void SetSize(int width)
        {
            this.Width = width;
            // measure text in label and size height
            using (Graphics g = CreateGraphics())
            {
                SizeF size = g.MeasureString(lblProviderName.Text, lblProviderName.Font, lblProviderName.Width);
                int textHeight = (int)Math.Ceiling(size.Height);
                this.Height = textHeight > idealHeight ? idealHeight * 2 : idealHeight;
            }
            pctDelete.Left = this.Width - pctDelete.Width - 2;
            pctWarning.Left = pctDelete.Left - pctWarning.Width - 2;
        }

   

        public void OnMouseHover(object sender, EventArgs e)
        {
            this.pctDelete.Visible = true;
            if (!selected) this.BackColor = hoverColor;
        }

        public void OnMouseLeave(object sender, EventArgs e)
        {
            if (!didMouseReallyLeave(sender, e)) return;
            this.pctDelete.Visible = false;
            if (!selected) this.BackColor = unselectedColor;
        }

        private void onClick(object sender, EventArgs e)
        {
            if (ItemSelected != null) ItemSelected(this, e);
            Selected = true;
        }

        private void pctDelete_Click(object sender, EventArgs e)
        {
            // don't select the item, just delete
            // if (ItemSelected != null) ItemSelected(this, e);
            Selected = true;
            if (DeleteClicked != null) DeleteClicked(this.Provider);
        }

        private ProviderLayout getLayout(TMProviderBase provider)
        {
            bool isBold = false;
            Color foreColor;
            Color backColor;
            // not selected and not logged in
            if (!Selected && !provider.IsLoggedIn)
            {
                foreColor = inactiveColor;
            }
            // anything logged in
            else
            {
                foreColor = Color.Black;
            }
            // selected
            if (Selected)
            {
                backColor = selectedColor;
                foreColor = Color.Black;
            }
            // not selected
            else
            {
                backColor = unselectedColor;
            }
            return new ProviderLayout(foreColor, backColor, isBold);
        }

        private void setLayout(TMProviderBase p)
        {
            ProviderLayout layout = getLayout(p);
            this.BackColor = layout.BackColor;
            this.ForeColor = layout.ForeColor;
            if (layout.IsBold) this.lblProviderName.Font = new Font(lblProviderName.Font, FontStyle.Bold);

            pctWarning.Visible = false;
            lblProviderName.Width = pctDelete.Left - lblProviderName.Left;
            if (p is TMProviderMQServer)
            {
                if ((p as TMProviderMQServer).CertificateIsSelfSigned)
                {
                    lblProviderName.Width = pctWarning.Left - lblProviderName.Left;
                    pctWarning.Visible = true;
                    SetSize(this.Width);
                    if (SizeChanged != null) SizeChanged(this);
                }
            }
        }

        private class ProviderLayout
        {
            public Color ForeColor;
            public Color BackColor;
            public bool IsBold;

            public ProviderLayout(Color foreColor, Color backColor, bool isBold)
            {
                this.ForeColor = foreColor;
                this.BackColor = backColor;
                this.IsBold = isBold;
            }
        }

    }
}
