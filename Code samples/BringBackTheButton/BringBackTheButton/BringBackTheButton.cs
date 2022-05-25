using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.BringBackTheButton
{
    public partial class BringBackTheButton : Form
    {
        public BringBackTheButton()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var player = new SoundPlayer(PluginResources.CuckooClock);
            player.Play();
        }

        private void lnkDont_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var player = new SoundPlayer(PluginResources.CuckooClock);
            player.Play();
        }

      
    }
}
