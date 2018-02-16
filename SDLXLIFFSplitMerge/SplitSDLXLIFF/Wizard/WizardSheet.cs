using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Utilities.SplitSDLXLIFF.Wizard
{
	/// <summary>
	/// Summary description for WizardSheet.
	/// </summary>
	public class WizardSheet : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WizardSheet()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pagePanel = new System.Windows.Forms.Panel();
            this.btnStart = new System.Windows.Forms.Button();
            this.etchedLine1 = new Sdl.Utilities.SplitSDLXLIFF.Wizard.EtchedLine();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.backButton.Location = new System.Drawing.Point(134, 8);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "< &Back";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nextButton.Location = new System.Drawing.Point(208, 8);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 1;
            this.nextButton.Text = "&Next >";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            this.finishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.finishButton.Location = new System.Drawing.Point(208, 8);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Text = "&Finish >";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(296, 8);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.btnStart);
            this.buttonPanel.Controls.Add(this.btnClose);
            this.buttonPanel.Controls.Add(this.etchedLine1);
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Controls.Add(this.backButton);
            this.buttonPanel.Controls.Add(this.finishButton);
            this.buttonPanel.Controls.Add(this.nextButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 101);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(384, 40);
            this.buttonPanel.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Location = new System.Drawing.Point(208, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pagePanel
            // 
            this.pagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pagePanel.Location = new System.Drawing.Point(0, 0);
            this.pagePanel.Name = "pagePanel";
            this.pagePanel.Size = new System.Drawing.Size(384, 101);
            this.pagePanel.TabIndex = 5;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStart.Location = new System.Drawing.Point(133, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "< &Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // etchedLine1
            // 
            this.etchedLine1.Dock = System.Windows.Forms.DockStyle.Top;
            this.etchedLine1.Edge = Sdl.Utilities.SplitSDLXLIFF.Wizard.EtchEdge.Top;
            this.etchedLine1.Location = new System.Drawing.Point(0, 0);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(384, 8);
            this.etchedLine1.TabIndex = 4;
            // 
            // WizardSheet
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 141);
            this.Controls.Add(this.pagePanel);
            this.Controls.Add(this.buttonPanel);
            this.KeyPreview = true;
            this.Name = "WizardSheet";
            this.Text = "WizardSheet";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.WizardSheet_Closing);
            this.Load += new System.EventHandler(this.WizardSheet_Load);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Panel buttonPanel;
        private Sdl.Utilities.SplitSDLXLIFF.Wizard.EtchedLine etchedLine1;
		private System.Windows.Forms.Panel pagePanel;

		private IList _pages = new ArrayList();
        private Button btnClose;
        private Button btnStart;
		private WizardPage _activePage;

		private void WizardSheet_Load(object sender, System.EventArgs e)
		{
			if (_pages.Count != 0)
			{
				ResizeToFit();
				SetActivePage(0);
			}
			else
				SetWizardButtons(WizardButtons.None);
		}

		private void ResizeToFit()
		{
			Size maxPageSize = new Size(buttonPanel.Width, 0);

			foreach (WizardPage page in _pages)
			{
				if (page.Width > maxPageSize.Width)
					maxPageSize.Width = page.Width;
				if (page.Height > maxPageSize.Height)
					maxPageSize.Height = page.Height;
			}

			foreach (WizardPage page in _pages)
			{
				page.Size = maxPageSize;
			}

			Size extraSize = this.Size;
			extraSize -= pagePanel.Size;

			Size newSize = maxPageSize + extraSize;
			this.Size = newSize;
		}

		public IList Pages
		{
			get { return _pages; }
		}

		private int GetActiveIndex()
		{
			WizardPage activePage = GetActivePage();

			for (int i = 0; i < _pages.Count; ++i)
			{
				if (activePage == _pages[i])
					return i;
			}

			return -1;
		}

		private WizardPage GetActivePage()
		{
			return _activePage;
		}
		
		public void SetActivePage(int pageIndex)
		{
			if (pageIndex < 0 || pageIndex >= _pages.Count)
				throw new ArgumentOutOfRangeException("pageIndex");

			WizardPage page = (WizardPage)_pages[pageIndex];
			SetActivePage(page);
		}

		private WizardPage FindPage(string pageName)
		{
			foreach (WizardPage page in _pages)
			{
				if (page.Name == pageName)
					return page;
			}

			return null;
		}

		private void SetActivePage(string newPageName)
		{
			WizardPage newPage = FindPage(newPageName);

			if (newPage == null)
				throw new Exception(string.Format("Can't find page named {0}", newPageName));

			SetActivePage(newPage);
		}

		private void SetActivePage(WizardPage newPage)
		{
			WizardPage oldActivePage = _activePage;

			// If this page isn't in the Controls collection, add it.
			// This is what causes the Load event, so we defer
			// it as late as possible.
			if (!pagePanel.Controls.Contains(newPage))
				pagePanel.Controls.Add(newPage);
			
			// Show this page.
			newPage.Visible = true;

			_activePage = newPage;
			CancelEventArgs e = new CancelEventArgs();
			newPage.OnSetActive(e);

			if (e.Cancel)
			{
				newPage.Visible = false;
				_activePage = oldActivePage;
			}

			// Hide all of the other pages.
			foreach (WizardPage page in _pages)
			{
				if (page != _activePage)
					page.Visible = false;
			}

            newPage.OnPostSetActive(e);
		}

		internal void SetWizardButtons(WizardButtons buttons)
		{
			// The Back button is simple.
            backButton.Enabled = ((buttons & WizardButtons.Back) != 0);

            // The Start button will be shown in one case only
            btnStart.Enabled = btnStart.Visible = ((buttons & WizardButtons.Start) != 0);

            // The Next button is a bit more complicated. If we've got a Finish button, then it's disabled and hidden.
            if ((buttons == WizardButtons.Finish) || buttons == (WizardButtons.Back | WizardButtons.Finish))
			{
                finishButton.Visible = true;
                finishButton.Enabled = true;

                btnClose.Visible = false;
                btnClose.Enabled = false;

				nextButton.Visible = false;
				nextButton.Enabled = false;

				this.AcceptButton = finishButton;
			}
            else if ((buttons == WizardButtons.Close) || buttons == (WizardButtons.Back | WizardButtons.Close) 
                || buttons == (WizardButtons.Start | WizardButtons.Close))
            {
                btnClose.Visible = true;
                btnClose.Enabled = true;

                finishButton.Visible = false;
                finishButton.Enabled = false;

                nextButton.Visible = false;
                nextButton.Enabled = false;

                this.AcceptButton = btnClose;
            }
            else
            {
                finishButton.Visible = false;
                finishButton.Enabled = false;

                btnClose.Visible = false;
                btnClose.Enabled = false;

                nextButton.Visible = true;
                nextButton.Enabled = ((buttons & WizardButtons.Next) != 0);

                this.AcceptButton = nextButton;
            }
		}

		private WizardPageEventArgs PreChangePage(int delta)
		{
            int nextIndex;
            if (delta == 0)
                nextIndex = 0;
            else
            {
                // Figure out which page is next.
                int activeIndex = GetActiveIndex();
                nextIndex = activeIndex + delta;

                if (nextIndex < 0 || nextIndex >= _pages.Count)
                    nextIndex = activeIndex;
            }

			// Fill in the event args.
			WizardPage newPage = (WizardPage)_pages[nextIndex];

			WizardPageEventArgs e = new WizardPageEventArgs();
			e.NewPage = newPage.Name;
			e.Cancel = false;

			return e;
		}

		private void PostChangePage(WizardPageEventArgs e)
		{
			if (!e.Cancel)
				SetActivePage(e.NewPage);
		}

		private void nextButton_Click(object sender, System.EventArgs e)
		{
            WizardPageEventArgs wpea2 = new WizardPageEventArgs();
            wpea2.NewPage = _activePage.Name;
            _activePage.OnWizardPreNext(wpea2);

            if (_activePage.AllowNext)
            {
                WizardPageEventArgs wpea = PreChangePage(+1);
                _activePage.OnWizardNext(wpea);
                PostChangePage(wpea);
            }
		}

		private void backButton_Click(object sender, System.EventArgs e)
		{
			WizardPageEventArgs wpea = PreChangePage(-1);
			_activePage.OnWizardBack(wpea);
			PostChangePage(wpea);
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
            WizardPageEventArgs wpea2 = new WizardPageEventArgs();
            wpea2.NewPage = _activePage.Name;
            _activePage.OnWizardPreNext(wpea2);

            if (_activePage.AllowNext)
            {
                WizardPageEventArgs wpea = PreChangePage(+1);
                _activePage.OnWizardNext(wpea);
                PostChangePage(wpea);
            }

            //CancelEventArgs cea = new CancelEventArgs();
            //_activePage.OnWizardFinish(cea);
            //if (cea.Cancel)
            //    return;

            //this.DialogResult = DialogResult.OK;
            //this.Close();
		}

		internal void PressButton(WizardButtons buttons)
		{
			if ((buttons & WizardButtons.Finish) == WizardButtons.Finish)
				finishButton.PerformClick();
			else if ((buttons & WizardButtons.Next) == WizardButtons.Next)
				nextButton.PerformClick();
            else if ((buttons & WizardButtons.Back) == WizardButtons.Back)
                backButton.PerformClick();
            else if ((buttons & WizardButtons.Close) == WizardButtons.Close)
                btnClose.PerformClick();
            else if ((buttons & WizardButtons.Start) == WizardButtons.Start)
                btnStart.PerformClick();
        }

        internal void EnableCancelButton(bool enableCancelButton)
        {
            cancelButton.Enabled = enableCancelButton;
        }

        internal void EnableCloseButton(bool enableCloseButton)
        {
            btnClose.Enabled = enableCloseButton;
        }

        internal void EnableStartButton(bool enableStartButton)
        {
            btnStart.Enabled = enableStartButton;
        }

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void WizardSheet_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            //if (!cancelButton.Enabled)
            //    e.Cancel = true;
			//else if (!finishButton.Enabled)
			if (cancelButton.Enabled && !finishButton.Enabled)
				OnQueryCancel(e);
		}

		protected virtual void OnQueryCancel(CancelEventArgs e)
		{
			_activePage.OnQueryCancel(e);
		}

        private void btnClose_Click(object sender, EventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            _activePage.OnWizardFinish(cea);
            if (cea.Cancel)
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            WizardPageEventArgs wpea = PreChangePage(0);
            _activePage.OnWizardBack(wpea);
            _activePage.OnWizardNewStart(wpea);
            PostChangePage(wpea);
        }
	}

	[Flags]
	public enum WizardButtons
	{
		None = 0,
		Back = 1,
		Next = 2,
        Finish = 4,
        Close = 8,
        Start = 16,
    }
}
