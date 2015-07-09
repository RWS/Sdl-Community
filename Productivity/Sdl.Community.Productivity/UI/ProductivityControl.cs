using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NLog;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Community.Productivity.Util;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TweetSharp;

namespace Sdl.Community.Productivity.UI
{
    public partial class ProductivityControl : UserControl
    {

        private ProductivityService _productivityService;
        private List<TrackInfoView> _trackingInfoVews;
        private Logger _logger;

 
        public ProductivityControl()
        {
            InitializeComponent();
          
        }

        protected override void OnLoad(EventArgs e)
        {
            _logger = LogManager.GetLogger("log");

            if (!this.DesignMode)
            {
                Initialize();
            }
            base.OnLoad(e);
        }

        private void Initialize()
        {
            _productivityService = new ProductivityService();

            _trackingInfoVews =
                _productivityService.TrackInfoViews;

            btnScore.Text = string.Format("{0}%", _productivityService.ProductivityScore);
            lblScore.Text = _productivityService.Score.ToString(CultureInfo.InvariantCulture);


            var twitterPersistenceService = new TwitterPersistenceService(_logger);
            lblScore.Text = string.Format("Your score is:\r\n{0} points!", _productivityService.Score);


            if (ProductivityUiHelper.IsTwitterAccountConfigured(twitterPersistenceService, _logger))
            {
                var twitterAccountInformation = twitterPersistenceService.Load();

                var twitterService = new TwitterService(Constants.ConsumerKey,
                    Constants.ConsumerSecret);
                twitterService.AuthenticateWith(twitterAccountInformation.AccessToken,
                    twitterAccountInformation.AccessTokenSecret);
               
                var getUpo = new GetUserProfileOptions()
                {
                    IncludeEntities = false,
                    SkipStatus = false
                };
                var twitterAccount = twitterService.GetUserProfile(getUpo);
                if (twitterAccount != null)
                {
                    lblScore.Text = string.Format("{0}, your score is:\r\n{1} points!", twitterAccount.Name,
                        _productivityService.Score);
                    pbTweetAccountImage.Load(twitterAccount.ProfileImageUrl);
                }
            }

            InitializeListView();
        }

        private void InitializeListView()
        {
            segmentTextColumn.GroupKeyGetter = delegate(object rowObject)
            {
                var trackInfoview = (TrackInfoView)rowObject;
                return trackInfoview.ProjectName;
            };

            segmentTextColumn.AspectToStringConverter = delegate(object x)
            {
                var text = x.ToString();
                return string.IsNullOrEmpty(text) ? "No text" : text;
            };

            listView.AboutToCreateGroups += listView_AboutToCreateGroups;

            efficiencyColumn.AspectToStringConverter = score => string.Format("{0}%", score);

            listView.SetObjects(_trackingInfoVews);
            listView.BuildList(true);
          
        }

        void listView_AboutToCreateGroups(object sender, CreateGroupsEventArgs e)
        {
            foreach (var group in e.Groups)
            {
                Int64 insertedCharacters = 0;
                Int64 keystrokesSaved = 0;
                var efficiencies = new List<double>();
                foreach (var item in group.Items)
                {
                    var trackInfoView = (TrackInfoView) item.RowObject;
                    insertedCharacters += trackInfoView.InsertedCharacters;
                    keystrokesSaved += trackInfoView.KeystrokesSaved;
                    efficiencies.Add(trackInfoView.Efficiency);
                }

                var efficiency = efficiencies.Average(x => Math.Round(x, 0));

                group.Header = string.Format("{0} {1} (inserted characters) - {2}% (efficiency) - {3} (keystrokes saved)", group.Header, insertedCharacters, efficiency,
                    keystrokesSaved);
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            var trackInfoView =listView.SelectedObject as TrackInfoView;
            if (trackInfoView == null) return;

            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

            var document = editorController.GetDocuments().FirstOrDefault(x => x.ActiveFile.Id == trackInfoView.FileId);



            if (document == null)
            {
                FileBasedProject project = null;
                foreach (var fileBasedProject in projectsController.GetAllProjects())
                {
                    var projectInfo = fileBasedProject.GetProjectInfo();
                    if (projectInfo.Id == trackInfoView.ProjectId)
                    {
                        project = fileBasedProject;
                    }
                }
                if (project == null) return;
                var projectFile = project.GetFile(trackInfoView.FileId);
                if (projectFile == null) return;
                document = editorController.Open(projectFile, EditingMode.Translation);
            }

            editorController.Activate(document);
        }

        private void btnTweet_Click(object sender, EventArgs e)
        {
            TweetFactory.CreateTweet(_logger);
        }

        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            var sInfo = new ProcessStartInfo(PluginResources.Leaderboard_Link);
            Process.Start(sInfo);
        }

    }
}
