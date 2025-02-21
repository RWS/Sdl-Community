using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Sdl.ProjectAutomation.Core;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks
{
	public class TagSettingsPresenter : ITagSettingsPresenter
    {
	    public static readonly Log Log = Log.Instance;
        private readonly ITagsSettingsControl control = null;
        private readonly XNamespace sdl = @"http://sdl.com/FileTypes/SdlXliff/1.0";

        public TagSettingsPresenter(ITagsSettingsControl control)
        {
            this.control = control;
        }

        public void Initialize()
        {
            GenerateFormatTags();

            GeneratePlaceholderTags();
        }

        public void SaveSettings()
        {
            Dictionary<string, bool> fmtDict = GetList(control.FormatTagList);

            control.Settings.FormatTagList = fmtDict;

            Dictionary<string, bool> phDict = GetList(control.PlaceholderTagList);

            control.Settings.PlaceholderTagList = phDict;
        }

        private void AddToList(IEnumerable<KeyValuePair<string, bool>> tagList, CheckedListBox listBox)
        {
            foreach (var item in tagList)
            {
                // Ensure we are not adding a placeholder the plug-in made
                if (control.Settings.Placeholders.All(ph => ph.Content != item.Key))
                {
                    if (listBox.FindStringExact(item.Key) == ListBox.NoMatches)
                    {
                        listBox.Items.Add(item.Key, item.Value);
                    }
                }
            }
        }

        private void GenerateFormatTags()
        {
            AddToList(control.Settings.FormatTagList, control.FormatTagList);

            control.FormatTagList.ItemCheck += TagList_ItemCheck;
        }

        private void GeneratePlaceholderTags()
        {
            // Add from settings first!
            AddToList(control.Settings.PlaceholderTagList, control.PlaceholderTagList);

            // Check project files and add any not in settings
            var projFiles = ProjectFileManager.GetProjectFiles();

            var placeholderTagList = GetPlaceholderTagList(projFiles);

            AddToList(placeholderTagList, control.PlaceholderTagList);

            control.PlaceholderTagList.ItemCheck += TagList_ItemCheck;
        }

        private Dictionary<string, bool> GetList(CheckedListBox listBox)
        {
            var dict = new Dictionary<string, bool>();

            var count = listBox.Items.Count;

            for (int i = 0; i < count; ++i)
            {
                var checkState = listBox.GetItemCheckState(i);
                var item = listBox.Items[i] as string;

                if (checkState == CheckState.Checked)
                {
                    dict.Add(item, true);
                }
                else
                {
                    dict.Add(item, false);
                }
            }

            return dict;
        }

        private IEnumerable<KeyValuePair<string, bool>> GetPlaceholderTagList(IEnumerable<ProjectFile> projFiles)
        {
            var placeholderTagList = new Dictionary<string, bool>();

            // Project could have hundreds of files, so stops reading after 10 file structures are read
            int counter = 0;

            foreach (var file in projFiles)
            {
                foreach (var pair in ReadPlaceholderTagInfo(file))
                {
	                Log.Logger.Info($"Loaded file: {file.LocalFilePath}\r\n\r\n");
                    if (!placeholderTagList.ContainsKey(pair.Key))
                    {
						Log.Logger.Info($"{pair.Key}\r\n");
                        placeholderTagList.Add(pair.Key, pair.Value);
                    }
                }

                counter++;

                if (counter > 10)
                {
                    break;
                }
            }

            return placeholderTagList;
        }

        private IEnumerable<KeyValuePair<string, bool>> ReadPlaceholderTagInfo(ProjectFile file)
        {
            var placeholderTagList = new Dictionary<string, bool>();

            if (file.LocalFileState == LocalFileState.None &&
                File.Exists(file.LocalFilePath) &&
                Path.GetExtension(file.LocalFilePath) == ".sdlxliff")
            {
                var root = XElement.Load(file.LocalFilePath, LoadOptions.None);
                foreach (var tag in root.Descendants(sdl + "tag"))
                {
                    foreach (var ph in tag.Descendants(sdl + "ph"))
                    {
                        var value = ph.Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!placeholderTagList.ContainsKey(value))
                            {
                                placeholderTagList.Add(value, false);
                            }
                        }
                    }
                }
            }

            return placeholderTagList;
        }

        private void TagList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedListBox = (CheckedListBox)sender;

            if (checkedListBox.Name == "fmtCheckedListBox")
            {
                var item = (string)control.FormatTagList.Items[e.Index];
                control.Settings.FormatTagList[item] = e.NewValue != CheckState.Unchecked;

                // Set tag list to itself
                var formatTagList = control.Settings.FormatTagList;
                control.Settings.FormatTagList = formatTagList;
            }
            else
            {
                var item = (string)control.PlaceholderTagList.Items[e.Index];
                control.Settings.PlaceholderTagList[item] = e.NewValue != CheckState.Unchecked;

                // Set tag list to itself
                var placeholderTagList = control.Settings.PlaceholderTagList;
                control.Settings.PlaceholderTagList = placeholderTagList;
            }
        }
    }
}