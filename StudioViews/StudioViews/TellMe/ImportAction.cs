using Sdl.Community.StudioViews.Actions;
using Sdl.Community.StudioViews.Model;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Linq;
using System.Windows;

namespace Sdl.Community.StudioViews.TellMe
{
    public class ImportAction : AbstractTellMeAction
    {
        public ImportAction()
        {
            Name = $"{PluginResources.Plugin_Name} Import";
        }

        public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.StudioViewsImport_Icon;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            var selectedFiles = ApplicationInstance.FilesController.SelectedFiles;

            if (!selectedFiles.Any())
                MessageBox.Show(PluginResources.Message_No_files_selected, PluginResources.Plugin_Name,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                SdlTradosStudio.Application.GetAction<ImportSelectedFilesAction>().Execute(
                    selectedFiles.Select(fp => new SystemFileInfo(fp.LocalFilePath)).ToList(),
                    selectedFiles.First().Language);
        }
    }
}