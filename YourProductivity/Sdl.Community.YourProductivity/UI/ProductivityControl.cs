using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NLog;
using Sdl.Community.YourProductivity.Model;
using Sdl.Community.YourProductivity.Services;
using Sdl.Community.YourProductivity.Util;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.YourProductivity.UI
{
    public partial class ProductivityControl : UserControl
    {

        private ProductivityService _productivityService;
        private TwitterShareService _twitterShareService;
        private List<TrackInfoView> _trackingInfoVews;
        private bool _initialized;
        private Logger _logger;

 
        public ProductivityControl()
        {
            InitializeComponent();

            btnTweet.MouseEnter += btnTweet_MouseEnter;
            btnTweet.MouseLeave += btnTweet_MouseLeave;
            btnLeaderboard.MouseEnter += btnLeaderboard_MouseEnter;
            btnLeaderboard.MouseLeave += btnLeaderboard_MouseLeave;
            _initialized = false;
        }

        void btnLeaderboard_MouseLeave(object sender, EventArgs e)
        {
            btnLeaderboard.BackColor = Color.FromArgb(72, 121, 197);

        }

        void btnLeaderboard_MouseEnter(object sender, EventArgs e)
        {
            btnLeaderboard.BackColor = Color.FromArgb(112, 151, 212);

        }

        void btnTweet_MouseLeave(object sender, EventArgs e)
        {
            btnTweet.BackColor = Color.FromArgb(72, 121, 197);
        }

        void btnTweet_MouseEnter(object sender, EventArgs e)
        {
            btnTweet.BackColor = Color.FromArgb(112, 151, 212);

        }

        public void Initialize(ProductivityService productivityService, TwitterShareService twitterShareService)
        {
            _productivityService = productivityService;
            _twitterShareService = twitterShareService;

            _initialized = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            _logger = LogManager.GetLogger("log");

            if (!DesignMode && _initialized)
            {
                InternalInitialize();
            }
            base.OnLoad(e);
        }

        private void InternalInitialize()
        {
            _trackingInfoVews =
                _productivityService.TrackInfoViews;

            btnScore.Text = string.Format("{0}%", _productivityService.ProductivityScore);
            lblScore.Text = _productivityService.Score.ToString(CultureInfo.InvariantCulture);
            lblScore.Text = string.Format("Your score is:\r\n{0:n0} points!", _productivityService.Score);

            var twitterAccount = _twitterShareService.GetUserProfile();
            if (twitterAccount != null)
            {
                lblScore.Text = string.Format("{0}, your score is:\r\n{1:n0} points!", twitterAccount.Name,
                    _productivityService.Score);
                pbTweetAccountImage.Load(twitterAccount.ProfileImageUrl);
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

                group.Header = string.Format("{0} {1:n0} (inserted characters) - {2}% (efficiency) - {3:n0} (keystrokes saved)", group.Header, insertedCharacters, efficiency,
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
            FormFactory.CreateTweetForm(_logger);
        }

        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            var sInfo = new ProcessStartInfo(PluginResources.Leaderboard_Link);
            Process.Start(sInfo);
        }

    }
}
