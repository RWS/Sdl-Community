using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using BrightIdeasSoftware;
using System.IO;
using Trados.Community.Toolkit.Core;
using Sdl.Versioning;

namespace ChangeScalingBehavior
{
    public partial class HighDPIChange : Form
    {
        private readonly List<string> _processNames = new List<string> { "MultiTerm", "SDLTradosStudio" };
        private List<Sdl.Versioning.StudioVersion> _installedStudioVersions;
        private List<MultiTermVersion> _installedMultiTermVersions;
        private readonly List<ApplicationVersion> _versions =new List<ApplicationVersion>();

        public HighDPIChange()
        {
            InitializeComponent();
        }

        private void HighDPIChange_Load(object sender, EventArgs e)
        {
	        Load -= HighDPIChange_Load;

			var tradosStudio = new StudioVersionService();        
            _installedStudioVersions = tradosStudio.GetInstalledStudioVersions();

	        var multiTerm = new MultiTerm();
            _installedMultiTermVersions = multiTerm.GetInstalledMultiTermVersion();

            olvColumnAction.AspectGetter = Getter;

            objectListView.SetObjects(_installedStudioVersions);
            objectListView.AddObjects(_installedMultiTermVersions);

	        objectListView.ButtonClick += ObjectListView_ButtonClick;
        }

        private static string Getter(object rowObject)
        {            
            var registerKeyMan = new RegistryKeyManager();
	        if (rowObject is Sdl.Versioning.StudioVersion studioVersion)
            {
                if (File.Exists(studioVersion.InstallPath + "SDLTradosStudio.exe.manifest") && registerKeyMan.IsRegistryKey())
                {
                    return "Undo changes";
                }
                return "Apply changes";
            }

	        if (rowObject is MultiTermVersion multiTermVersion && 
	            File.Exists(multiTermVersion.InstallPath + "MultiTerm.exe.manifest") && registerKeyMan.IsRegistryKey())
	        {
		        return "Undo changes";
	        }

	        return "Apply changes";
        }


        private void ObjectListView_ButtonClick(object sender, CellClickEventArgs e)
        {
            CheckProcesses(_processNames);

            try
            {
                var studio = _installedStudioVersions.FirstOrDefault(sVersion => sVersion.PublicVersion == e.Item.Text);
                if(studio != null)
                {
                    var appVersion = new ApplicationVersion
                    {
                        InstallPath = studio.InstallPath,
                        PublicVersion = studio.PublicVersion,
                        ResourceFileName = "SDLTradosStudio.exe.manifest",
                        StudioVersions = _installedStudioVersions,
                        MultiTermVersions = _installedMultiTermVersions
                    };
                    _versions.Add(appVersion);
                }

                var multiTerm = _installedMultiTermVersions.FirstOrDefault(mtVersion => mtVersion.PublicVersion.Equals(e.Item.Text));
                if(multiTerm != null)
                {
                    var appVersion = new ApplicationVersion
                    {
                        InstallPath = multiTerm.InstallPath,
                        PublicVersion = multiTerm.PublicVersion,
                        ResourceFileName = "MultiTerm.exe.manifest",
                        StudioVersions = _installedStudioVersions,
                        MultiTermVersions = _installedMultiTermVersions
                    };
                    _versions.Add(appVersion);
                }

                foreach(var version in _versions.Where(version => version.PublicVersion == e.Item.Text))
                {
                    if (e.SubItem.Text == "Apply changes")
                    {
                        Helpers.ApplyChanges(version.InstallPath, version.ResourceFileName);
                    }
                    else
                    {
                        Helpers.UndoChanges(version, version.InstallPath, version.ResourceFileName);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Admin rights are required!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                InitializeLog.Retlogger().Error(ex, "Error when adding files!");
            }
        }    
            
        private static void CheckProcesses(IEnumerable<string> processNames)
        {
            foreach (var processName in processNames)
            {
                var names = Process.GetProcessesByName(processName);
                if (names.Length > 0)
                {
                    MessageBox.Show(processName + " is open! Please close the application before applying the fix.");
                    Application.Exit();
                }
            }
        }        
    }
}
