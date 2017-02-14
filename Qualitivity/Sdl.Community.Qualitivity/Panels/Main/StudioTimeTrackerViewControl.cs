using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity.Panels.Main
{
    public partial class QualitivityViewControl : UserControl
    {


        private List<Project> ProjectsTemp { get; set; }
        private List<CompanyProfile> CompanyProfilesTemp { get; set; }
        public QualitivityViewControl()
        {
            InitializeComponent();
            ProjectsTemp = new List<Project>();
            CompanyProfilesTemp = new List<CompanyProfile>();
       


            olvColumn_client_name.ImageGetter = delegate {
                return "client";
            };
            olvColumn_source.AspectGetter = delegate(object x)
            {
                var source=string.Empty;
                if (((Activity)x).Activities.Count>0)
                {
                    source = ((Activity)x).Activities[0].TranslatableDocument.SourceLanguage;
                }
                return source;
            };
            olvColumn_source.ImageGetter = delegate(object x)
            {
                var source = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    source = ((Activity)x).Activities[0].TranslatableDocument.SourceLanguage + ".gif";
                }
                if (!imageList1.Images.ContainsKey(source))
                {
                    return "empty.png";
                }
                return source;
            };

            olvColumn_target.AspectGetter = delegate(object x)
            {
                var target = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    target = ((Activity)x).Activities[0].TranslatableDocument.TargetLanguage;
                }
                return target;
            };
            olvColumn_target.ImageGetter = delegate(object x)
            {
                var target = string.Empty;
                if (((Activity)x).Activities.Count > 0)
                {
                    target = ((Activity)x).Activities[0].TranslatableDocument.TargetLanguage + ".gif";
                }
                if (!imageList1.Images.ContainsKey(target))
                {
                    return "empty.png";
                }
                return target;
            };

            olvColumn_client_name.AspectGetter = delegate(object x)
            {
                var companyProfileName = "[no client]";

                CompanyProfile activityCompanyProfile = null;
                if (CompanyProfilesTemp.Exists(a => a.Id == ((Activity)x).CompanyProfileId))
                    activityCompanyProfile = CompanyProfilesTemp.Find(a => a.Id == ((Activity)x).CompanyProfileId);
                else
                {
                    activityCompanyProfile = Helper.GetClientFromId(((Activity)x).CompanyProfileId);
                    if (activityCompanyProfile!=null)
                        CompanyProfilesTemp.Add(activityCompanyProfile);
                }
                if (activityCompanyProfile != null)
                    companyProfileName = activityCompanyProfile.Name;


                return companyProfileName;              
            };


            olvColumn_project.ImageGetter = delegate(object x)
            {
                Project activityProject;
                if (ProjectsTemp.Exists(a => a.Id == ((Activity)x).ProjectId))
                    activityProject = ProjectsTemp.Find(a => a.Id == ((Activity)x).ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(((Activity)x).ProjectId);
                    ProjectsTemp.Add(activityProject);
                }


                return activityProject.ProjectStatus == "In progress" ? "flag_blue" : "flag_green";
            };


            olvColumn_project.AspectGetter = delegate(object x)
            {
                Project activityProject;
                if (ProjectsTemp.Exists(a => a.Id == ((Activity)x).ProjectId))
                    activityProject = ProjectsTemp.Find(a => a.Id == ((Activity)x).ProjectId);
                else
                {
                    activityProject = Helper.GetProjectFromId(((Activity)x).ProjectId);
                    ProjectsTemp.Add(activityProject);
                }

                return activityProject.Name;

                
            };


            olvColumn_activity_status.ImageGetter = delegate(object x)
            {
                return ((Activity)x).ActivityStatus == Activity.Status.New ? "question_blue" : "tick";
            };


            olvColumn_billable.ImageGetter = delegate(object x)
            {
                if (((Activity)x).Billable)
                {
                    return "vyes";
                }
                return "vno";
            };

            olvColumn_documents.AspectGetter = x => ((Activity) x).Activities.Count.ToString();
            olvColumn_activity_total.AspectGetter = delegate(object x)
            {
                var totalHr = Math.Round(((Activity)x).DocumentActivityRates.HourlyRateTotal, 2);
                totalHr = ((Activity)x).HourlyRateChecked ? totalHr : 0;

                var totalPem = Math.Round(((Activity)x).DocumentActivityRates.LanguageRateTotal, 2);
                totalPem = ((Activity)x).LanguageRateChecked ? totalPem : 0;


                var totalCustom = Math.Round(((Activity)x).DocumentActivityRates.CustomRateTotal, 2);
                totalCustom = ((Activity)x).CustomRateChecked ? totalCustom : 0;

                var total = Math.Round(totalHr + totalPem + totalCustom, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                return total + " " + currency;
                
            };
            olvColumn_hr_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.HourlyRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).HourlyRateChecked ? total : 0;
                return total + " " + currency;
                
            };
            olvColumn_pem_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.LanguageRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).LanguageRateChecked ? total : 0;
                return total + " " + currency;
                
            };
            olvColumn_custom_total.AspectGetter = delegate(object x)
            {
                var total = Math.Round(((Activity)x).DocumentActivityRates.CustomRateTotal, 2);
                var currency = ((Activity)x).DocumentActivityRates.HourlyRateCurrency;
                total = ((Activity)x).CustomRateChecked ? total : 0;
                return total + " " + currency;

            };


            olvColumn_activity_name.ImageGetter = delegate {
                return "calendar";
            };


        }

        private static QualitivityViewController _controller { get; set; }
        public static QualitivityViewController Controller
        {
            get
            {                     
                return _controller;
            }
            set
            {
                _controller = value;
            }
        }

        private void CollapseAllGroups()
        {
            if (objectListView1.OLVGroups != null)
            {
                foreach (OLVGroup t in objectListView1.OLVGroups)
                {
                    t.Collapsed = true;
                }
            }
            
        }

        private void ExpandAllGroups()
        {
            if (objectListView1.OLVGroups != null)
            {
                foreach (var t in objectListView1.OLVGroups)
                {
                    t.Collapsed = false;
                }
            }
           
        }

        private void linkLabel_expand_all_groups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ExpandAllGroups();
            objectListView1.Select();
        }

        private void linkLabel_collapse_all_groups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CollapseAllGroups();
            objectListView1.Select();
        }

        private void linkLabel_select_all_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (OLVListItem itmx in objectListView1.Items)
            {
                itmx.Selected = true;
            }
            objectListView1.Select();
        }

        private void linkLabel_unselect_all_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            foreach (OLVListItem itmx in objectListView1.Items)
            {
                itmx.Selected = false;
            }
            objectListView1.Select();
        }

        public void linkLabel_turn_off_groups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var defaultActivityViewGroupsIsOn = Convert.ToBoolean(Tracked.Settings.GetGeneralProperty("defaultActivityViewGroupsIsOn").Value);

            objectListView1.ShowGroups = !defaultActivityViewGroupsIsOn;            
            Tracked.Settings.GetGeneralProperty("defaultActivityViewGroupsIsOn").Value = objectListView1.ShowGroups.ToString().ToLower();
            linkLabel_turn_off_groups.Text = objectListView1.ShowGroups ? "Turn off groups" : "Turn on groups";
        }

    }
}
