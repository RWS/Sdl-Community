using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Productivity.UI
{
    public partial class ProductivityControl : UserControl
    {
        public ProductivityReportViewPart Controller { get; set; }

        private ProductivityService _productivityService;
        private List<TrackInfoView> _trackingInfoVews;

 
        public ProductivityControl()
        {
            InitializeComponent();
            
        }

        protected override void OnLoad(EventArgs e)
        {
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
                _productivityService.TrackInfoViews.Where(
                    x => !string.IsNullOrEmpty(x.Text)).ToList();

            lblProductivityScore.Text = string.Format("Productivity score: {0}%", _productivityService.ProductivityScore);
            lblScore.Text = _productivityService.Score.ToString(CultureInfo.InvariantCulture);


            InitializeListView();
        }

        private void InitializeListView()
        {
            segmentTextColumn.GroupKeyGetter = delegate(object rowObject)
            {
                var trackInfoview = (TrackInfoView) rowObject;
                return string.Format("{0} - {1}%", trackInfoview.FileName, trackInfoview.FileProductivityScore);
            };

            segmentTextColumn.AspectToStringConverter = delegate(object x)
            {
                var text = x.ToString();
                return string.IsNullOrEmpty(text) ? "No text" : text;
            };

            

            isTranslatedColumn.AspectToStringConverter = delegate(object x)
            {
                var translated = (bool) x;
                return translated ? "Confirmed" : "Not Confirmed";
            };

            segmentProductivityScoreColumn.AspectToStringConverter = score => string.Format("{0}%", score);

            listView.HeaderStyle = ColumnHeaderStyle.None;
            listView.SetObjects(_trackingInfoVews);
            listView.Sort(segmentProductivityScoreColumn);
            listView.BuildList(true);
            if (listView.OLVGroups != null)
            {
                foreach (var group in listView.OLVGroups)
                {
                    group.Collapsed = true;
                }
            }
        }

        private void lblScore_Click(object sender, EventArgs e)
        {
            var sInfo = new ProcessStartInfo(PluginResources.Leaderboard_Link);
            Process.Start(sInfo);
        }

        private void pbScore_Click(object sender, EventArgs e)
        {
            var sInfo = new ProcessStartInfo(PluginResources.Leaderboard_Link);
            Process.Start(sInfo);
        }

        private void lblProductivityScore_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            var trackInfoView =listView.SelectedObject as TrackInfoView;
            if (trackInfoView == null) return;

            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var filesController = SdlTradosStudio.Application.GetController<FilesController>();

            var document = editorController.GetDocuments().FirstOrDefault(x => x.ActiveFile.Id == trackInfoView.FileId);

            if (document == null)
            {
                var projectFile = filesController.CurrentProject.GetFile(trackInfoView.FileId);
                if (projectFile == null) return;
                document = editorController.Open(projectFile, EditingMode.Translation);
            }

            document.SetActiveSegmentPair(document.ActiveFile,
                trackInfoView.SegmentId);
            editorController.Activate(document);
          

            
        }

    }
}
