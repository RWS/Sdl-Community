using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class ProjectTermsBatchTaskSettingsControl
    {
        private ListView listViewBlackList;

        private void InitializeComponent()
        {
            this.listViewBlackList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listViewBlackList
            // 
            this.listViewBlackList.Location = new System.Drawing.Point(43, 41);
            this.listViewBlackList.Name = "listViewBlackList";
            this.listViewBlackList.Size = new System.Drawing.Size(162, 187);
            this.listViewBlackList.TabIndex = 0;
            this.listViewBlackList.UseCompatibleStateImageBehavior = false;
            // 
            // ProjectTermsBatchTaskSettingsControl
            // 
            this.Controls.Add(this.listViewBlackList);
            this.Name = "ProjectTermsBatchTaskSettingsControl";
            this.Size = new System.Drawing.Size(509, 405);
            this.ResumeLayout(false);

        }
    }
}
