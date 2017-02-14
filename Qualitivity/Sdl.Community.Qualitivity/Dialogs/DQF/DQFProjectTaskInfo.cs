using System;
using System.Windows.Forms;
using Sdl.Community.DQF.Core;
using Sdl.Community.Structures.DQF;

namespace Sdl.Community.Qualitivity.Dialogs.DQF
{
    public partial class DqfProjectTaskInfo : Form
    {

        public DqfProject DqfProject { get; set; }
        public DqfProjectTask DqfProjectTask { get; set; }
        public DqfProjectTaskInfo()
        {
            InitializeComponent();
        }

       
        private void FormAppendAnlaysisBand_Load(object sender, EventArgs e)
        {

            richTextBox_dqf_project_info.Text = string.Empty;
            richTextBox_dqf_project_info.SelectedText += PluginResources.Project_ID_ + DqfProject.DqfProjectId + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Project_Key_ + DqfProject.DqfProjectKey + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Translator_Key_ + DqfProjectTask.DqfTranslatorKey + "\r\n\r\n";

            richTextBox_dqf_project_info.SelectedText += PluginResources.Project_Name_ + DqfProject.Name + "\r\n";
            if (DqfProject.Created != null)
                richTextBox_dqf_project_info.SelectedText += PluginResources.Project_Created_ + DqfProject.Created.Value + "\r\n\r\n";

            richTextBox_dqf_project_info.SelectedText += PluginResources.Task_ID__ + DqfProjectTask.DqfTaskId + "\r\n";
            if (DqfProjectTask.Uploaded != null)
                richTextBox_dqf_project_info.SelectedText += PluginResources.Task_Uploaded_ + DqfProjectTask.Uploaded.Value + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Document_Name_ + DqfProjectTask.DocumentName + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Source_Language_ + DqfProject.SourceLanguage + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Target_Language_ + DqfProjectTask.TargetLanguage + "\r\n\r\n";

            richTextBox_dqf_project_info.SelectedText += PluginResources.Content_Type_ + Configuration.ContentTypes.Find(a => a.Id == DqfProject.ContentType).DisplayName + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Industry__ + Configuration.Industries.Find(a => a.Id == DqfProject.Industry).DisplayName + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Process__ + Configuration.Processes.Find(a => a.Id == DqfProject.Process).Name + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.Quality_Level_ + Configuration.QualityLevel.Find(a => a.Id == DqfProject.QualityLevel).DisplayName + "\r\n";
            richTextBox_dqf_project_info.SelectedText += PluginResources.CAT_Tool_ + Configuration.CatTools.Find(a => a.Id == DqfProjectTask.CatTool).Name + "\r\n";



        }

        private void button_save_Click(object sender, EventArgs e)
        {
        
            Close();
        }

      
    }
}
