using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using NSubstitute;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;


namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TestUtilities
    {
        public TestUtilities()
        {
            // http://stackoverflow.com/a/283917/906773
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            SaveFolder = Path.Combine(Path.GetDirectoryName(path), "SaveFolder");
        }

        public string SaveFolder { get; }

        public IEnumerator<IAbstractMarkupData> AbstractMarkupDataContainer(IAbstractMarkupData data)
        {
            yield return data;
        }

        public IConversionFileView CreateConversionFileView(bool saveButtonEnabled = true,
                                                            bool saveAsButtonEnabled = true,
                                                            bool descriptionEnabled = true,
                                                            bool searchEnabled = true,
                                                            bool replaceEnabled = true,
                                                            bool caseSensitiveEnabled = true,
                                                            bool regexEnabled = true,
                                                            bool wholeWordEnabled = true,
                                                            bool tagPairEnabled = true,
                                                            bool embeddedTags = true,
                                                            bool strConvEnabled = true,
                                                            bool toLowerEnabled = true,
                                                            bool toUpperEnabled = true,
                                                            bool placeHolderEnabled = true,
                                                            bool subSegmentEnabled = true,
                                                            ConversionItem convItem = null)
        {
            var view = Substitute.For<IConversionFileView>();

            view.SaveButton.Returns(new Button() { Enabled = saveButtonEnabled });
            view.SaveAsButton.Returns(new Button() { Enabled = saveAsButtonEnabled });
            view.Description.Returns(new TextBox() { Enabled = descriptionEnabled });
            view.Search.Returns(new TextBox() { Enabled = searchEnabled });
            view.Replace.Returns(new TextBox() { Enabled = replaceEnabled });
            view.CaseSensitive.Returns(new CheckBox() { Enabled = caseSensitiveEnabled });
            view.Regex.Returns(new CheckBox() { Enabled = regexEnabled });
            view.WholeWord.Returns(new CheckBox() { Enabled = wholeWordEnabled });
            view.TagPair.Returns(new CheckBox() { Enabled = tagPairEnabled });
            view.EmbeddedTags.Returns(new CheckBox() { Enabled = embeddedTags });
            view.StrConv.Returns(new CheckBox() { Enabled = strConvEnabled });
            view.ToLower.Returns(new CheckBox() { Enabled = toLowerEnabled });
            view.ToUpper.Returns(new CheckBox() { Enabled = toUpperEnabled });
            view.Placeholder.Returns(new CheckBox() { Enabled = placeHolderEnabled });
            var bindingSource = new BindingSource();
            if (convItem != null)
            {
                bindingSource.Add(convItem);
            }

            view.BindingSource.Returns(bindingSource);

            return view;
        }

        public List<ConversionItemList> CreateConversionItemLists(string searchText, string replacementText = "",
                                                                          bool caseSensitive = false,
                                                                  List<VbStrConv> vbstrConv = null,
                                                                  bool useRegex = false,
                                                                  bool wholeWord = false,
                                                                  bool strConv = false,
                                                                  bool toLower = false,
                                                                  bool toUpper = false,
                                                                  bool placeHolder = false,
                                                                  bool tagPair = false,
                                                                  bool embeddedTags = false)
        {
            var search = new SearchText()
            {
                Text = searchText,
                CaseSensitive = caseSensitive,
                UseRegex = useRegex,
                WholeWord = wholeWord,
                StrConv = strConv,
                VbStrConv = vbstrConv,
                TagPair = tagPair,
                EmbeddedTags = embeddedTags
            };

            var replacement = new ReplacementText()
            {
                Text = replacementText,
                ToLower = toLower,
                ToUpper = toUpper,
                Placeholder = placeHolder,
            };

            var list = new ConversionItemList();
            list.Items = new List<ConversionItem>()
            {
                new ConversionItem()
                {
                    Search = search,
                    Replacement = replacement
                }
            };

            var lists = new List<ConversionItemList>()
            {
                list
            };

            return lists;
        }

        public IConversionsSettingsControl CreateConversionsSettingsControl()
        {
            var control = Substitute.For<IConversionsSettingsControl>();

            control.FileList.Returns(new CheckedListBox());
            control.Edit.Returns(new Button());
            control.Remove.Returns(new Button());
            control.Up.Returns(new Button());
            control.Down.Returns(new Button());
            control.Settings = Substitute.For<ICleanUpConversionSettings>();
            control.Settings.ConversionFiles = new Dictionary<string, bool>();

            return control;
        }

        public IVerifyingFormattingVisitor CreateFormattingVisitor()
        {
            var fmtVisitor = Substitute.For<IVerifyingFormattingVisitor>();
            return fmtVisitor;
        }

        public IParagraphUnit CreateParagraphUnit(string text)
        {
            var paragraphUnit = Substitute.For<IParagraphUnit>();
            paragraphUnit.Properties = Substitute.For<IParagraphUnitProperties>();
            paragraphUnit.Properties.Contexts = Substitute.For<IContextProperties>();
            var contextInfo = Substitute.For<IContextInfo>();
            contextInfo.ContextType = text;
            contextInfo.Purpose = ContextPurpose.Information;
            paragraphUnit.Properties.Contexts.Contexts.Returns(new List<IContextInfo>() { contextInfo });

            return paragraphUnit;
        }

        public string CreatePath(string fileName)
        {
            return Path.Combine(SaveFolder, fileName);
        }

        public ISegment CreateSegment(bool isLocked = false)
        {
            var segment = Substitute.For<ISegment>();
            segment.Properties = Substitute.For<ISegmentPairProperties>();
            segment.Properties.IsLocked = isLocked;

            return segment;
        }

        public ICleanUpSourceSettings CreateSettings(bool useSegmentLocker = false,
                                                     bool useContentLocker = false,
                                                     bool useStructureLocker = false,
                                                     SegmentLockItem segmentLockItem = null,
                                                     ContextDef contextDef = null,
                                                     bool backGroundColor = false,
                                                     bool bold = false,
                                                     bool fontNameSize = false,
                                                     bool italic = false,
                                                     bool strikeThrough = false,
                                                     bool textColor = false,
                                                     bool textDirection = false,
                                                     bool textPosition = false,
                                                     bool underLine = false)
        {
            var settings = Substitute.For<ICleanUpSourceSettings>();
            settings.UseSegmentLocker = useSegmentLocker;
            settings.UseContentLocker = useContentLocker;
            settings.UseStructureLocker = useStructureLocker;
            settings.SegmentLockList = new BindingList<SegmentLockItem>(new List<SegmentLockItem>()
            {
                segmentLockItem
            });
            settings.StructureLockList = new List<ContextDef>()
            {
                contextDef
            };

            return settings;
        }

        public ITagPair CreateTag(string start, string end, IText text)
        {
            var tagPair = Substitute.For<ITagPair>();
            tagPair.StartTagProperties = Substitute.For<IStartTagProperties>();
            tagPair.StartTagProperties.TagContent = start;
            tagPair.AllSubItems.Returns(new List<IAbstractMarkupData>() { text });
            tagPair.EndTagProperties = Substitute.For<IEndTagProperties>();
            tagPair.EndTagProperties.TagContent = end;

            tagPair.StartTagProperties.Formatting = Substitute.For<IFormattingGroup>();

            return tagPair;
        }

        public ITagPair CreateTag(IFormattingItem item = null)
        {
            var tagPair = Substitute.For<ITagPair>();
            tagPair.StartTagProperties = Substitute.For<IStartTagProperties>();
            var formatting = Substitute.For<IFormattingGroup>();
            formatting.Values.Returns(new List<IFormattingItem>() { item });
            tagPair.StartTagProperties.Formatting = formatting;

            return tagPair;
        }

        public ITagsSettingsControl CreateTagSettingsControl(List<Placeholder> placeHolders = null,
                                                             Dictionary<string, bool> placeHolderTagList = null,
                                                             CheckedListBox formatList = null,
                                                             CheckedListBox placeHolderList = null,
                                                             Dictionary<string, bool> formatTagList = null)
        {
            var control = Substitute.For<ITagsSettingsControl>();
            control.Settings = Substitute.For<ICleanUpSourceSettings>();
            control.Settings.Placeholders = placeHolders ?? new List<Placeholder>();
            control.Settings.PlaceholderTagList = placeHolderTagList ?? new Dictionary<string, bool>();

            control.FormatTagList.Returns(formatList ?? new CheckedListBox());
            control.PlaceholderTagList.Returns(placeHolderList ?? new CheckedListBox());
            control.Settings.FormatTagList = formatTagList ?? new Dictionary<string, bool>();

            return control;
        }

        public IText CreateText(string text = null, ISegment segment = null)
        {
            var txt = Substitute.For<IText>();
            txt.Properties = Substitute.For<ITextProperties>();
            txt.Properties.Text = text;
            txt.Parent = segment;

            return txt;
        }

        public List<VbStrConv> CreateVbStrConvList(params VbStrConv[] list)
        {
            return list.ToList();
        }
    }
}