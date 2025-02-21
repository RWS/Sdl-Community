using Multilingual.XML.FileType.FileType.Controls.Entities;
using Multilingual.XML.FileType.FileType.Settings.Entities;
using Multilingual.XML.FileType.Models;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Pages
{
    [FileTypeSettingsPage(
        Id = Constants.EntitiesId,
        Name = "MultilingualXMLFileType_EntitiesPage_Name",
        Description = "MultilingualXMLFileType_EntitiesPage_Description",
        HelpTopic = "")]
    public class EntitiesOptionsPage : AbstractFileTypeSettingsPage<EntitiesOptionsControl, UniqueEntitySettings>
    {
        public override object GetControl()
        {
            if (Control == null)
            {
                EntitiesOptionsControl control = base.GetControl() as EntitiesOptionsControl;
                //tivi
                //if (control != null)
                //{
                //    control.SetSettingsConfiguration(SettingsBundle, FileTypeConfigurationId);
                //}

                return control;
            }

            return Control;
        }
        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            Control.UpdateUI();
        }

        public override void Save()
        {
            Control.UpdateSettings();
            base.Save();
        }

        public override void Refresh()
        {
            base.Refresh();
            Control.UpdateUI();
        }
    }
}
