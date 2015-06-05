using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.Productivity.Services;

namespace Sdl.Community.Productivity.UI
{
    public partial class TweetForm : Form
    {
        private readonly ShareService _shareService;

        public TweetForm()
        {
            InitializeComponent();
            
        }
        public TweetForm(ShareService shareService):this()
        {
            _shareService = shareService;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            txtTweetMessage.Text = _shareService.GetTwitterMessage();
        }

        private void shareTweet_Click(object sender, EventArgs e)
        {
            _shareService.Share();
            this.Close();
        }
    }
}
