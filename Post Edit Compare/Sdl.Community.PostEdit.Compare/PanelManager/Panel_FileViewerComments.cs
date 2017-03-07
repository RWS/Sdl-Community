using System;
using WeifenLuo.WinFormsUI.Docking;

namespace PostEdit.Compare
{
    public partial class PanelFileViewerComments : DockContent
    {
        public PanelFileViewerComments()
        {
            InitializeComponent();
         
        }

        private void ToolStripMenuItem_Floating_Click(object sender, EventArgs e)
        {
            if (this.DockState != DockState.Float)
                this.DockState = DockState.Float;
        }

        private void ToolStripMenuItem_Dockable_Click(object sender, EventArgs e)
        {
            switch (this.DockState)
            {
                case DockState.DockBottomAutoHide: this.DockState = DockState.DockBottom; break;
                case DockState.DockLeftAutoHide: this.DockState = DockState.DockLeft; break;
                case DockState.DockRightAutoHide: this.DockState = DockState.DockRight; break;
                case DockState.DockTopAutoHide: this.DockState = DockState.DockTop; break;
                case DockState.Float: this.DockState = DockState.DockBottom; break;
                case DockState.Unknown: this.DockState = DockState.DockBottom; break;
            }
        }

        private void ToolStripMenuItem_AutoHide_Click(object sender, EventArgs e)
        {
            switch (this.DockState)
            {
                case DockState.DockBottom: this.DockState = DockState.DockBottomAutoHide; break;
                case DockState.DockLeft: this.DockState = DockState.DockLeftAutoHide; break;
                case DockState.DockRight: this.DockState = DockState.DockRightAutoHide; break;
                case DockState.DockTop: this.DockState = DockState.DockTopAutoHide; break;
                case DockState.Float: this.DockState = DockState.DockBottomAutoHide; break;
                case DockState.Unknown: this.DockState = DockState.DockBottomAutoHide; break;
            }
        }
    }
}
