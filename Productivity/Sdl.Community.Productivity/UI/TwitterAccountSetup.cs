using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
using TweetSharp;

namespace Sdl.Community.Productivity.UI
{
    public partial class TwitterAccountSetup : Form
    {
        private OAuthRequestToken _requestToken;
        private TwitterService _twitterService;
        private readonly TwitterPersistenceService _twitterPersistenceService;
        private string _url;
        public TwitterAccountSetup(TwitterPersistenceService twitterPersistenceService)
        {
            InitializeComponent();
            _twitterPersistenceService = twitterPersistenceService;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _twitterService = new TwitterService(Constants.ConsumerKey,
                Constants.ConsumerSecret);
            _requestToken = _twitterService.GetRequestToken();
            //_applicationCredentials = CredentialsCreator.GenerateApplicationCredentials);
            _url = _twitterService.GetAuthorizationUri(_requestToken).ToString();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.None) return;
            Validate();
            string error;
            if (txtPin.Text.Length == 0)
            {
                error = "Please enter the pin code from twitter page";
                errPin.SetError(txtPin, error);
                e.Cancel = true;
                return;
            }
            var pin = txtPin.Text;

            var accessToken = _twitterService.GetAccessToken(_requestToken, pin);
            if (accessToken == null)
            {
                error = "The pin entered is invalid";
                e.Cancel = true;
                errPin.SetError(txtPin, error);
                return;
            }

            _twitterPersistenceService.Save(new TwitterAccountInfo
            {
                AccessToken = accessToken.Token,
                AccessTokenSecret = accessToken.TokenSecret
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
            var sInfo = new ProcessStartInfo(_url);
            Process.Start(sInfo);
        }

        private void txtPin_Validating(object sender, CancelEventArgs e)
        {
            string error;
            if (txtPin.Text.Length == 0)
            {
                error = "Please enter the pin code from twitter page";
                e.Cancel = true;
                errPin.SetError((Control)sender, error);
                return;
            }
            int parsedValue;
            if (int.TryParse(txtPin.Text, out parsedValue)) return;
            error = "The pin can only contain numbers";
            e.Cancel = true;
            errPin.SetError((Control)sender, error);
        }

    }
}
