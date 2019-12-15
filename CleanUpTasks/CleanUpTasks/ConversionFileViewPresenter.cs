using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using Microsoft.VisualBasic;
using SDLCommunityCleanUpTasks.Dialogs;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks
{
	public class ConversionFileViewPresenter : IConversionFileViewPresenter
    {
        private readonly IFileDialog dialog = null;
        private readonly BatchTaskMode taskMode;
        private readonly IConversionFileView view = null;
        private BindingListView<ConversionItem> bindingListView = null;
        private bool isInputFormDisabled = false;
        private ConversionFileViewMode viewMode;

        public ConversionFileViewPresenter(IConversionFileView view, IFileDialog dialog, ConversionFileViewMode viewMode, BatchTaskMode taskMode)
        {
            this.view = view;
            this.dialog = dialog;
            this.viewMode = viewMode;
            this.taskMode = taskMode;
        }

        public void CheckSaveButton()
        {
            if (IsGridEmpty())
            {
                view.SaveButton.Enabled = false;
                view.SaveAsButton.Enabled = false;
                view.Description.Enabled = false;
                view.Search.Enabled = false;
                view.Replace.Enabled = false;
                view.CaseSensitive.Enabled = false;
                view.Regex.Enabled = false;
                view.WholeWord.Enabled = false;
                view.StrConv.Enabled = false;
                view.ToLower.Enabled = false;
                view.ToUpper.Enabled = false;
                view.Placeholder.Enabled = false;
                view.TagPair.Enabled = false;
                view.EmbeddedTags.Enabled = false;
                isInputFormDisabled = true;
            }
            else if (isInputFormDisabled)
            {
                if (viewMode == ConversionFileViewMode.Existing)
                {
                    view.SaveButton.Enabled = true;
                }

                view.SaveAsButton.Enabled = true;
                view.Description.Enabled = true;
                view.Search.Enabled = true;
                view.Replace.Enabled = true;
                view.WholeWord.Enabled = true;
                view.CaseSensitive.Enabled = true;
                view.Regex.Enabled = true;
                view.StrConv.Enabled = true;
                view.EmbeddedTags.Enabled = true;
                // Disabled by default
                // view.ToLower.Enabled = false;
                // view.ToUpper.Enabled = false;
                view.Placeholder.Enabled = true;
                view.TagPair.Enabled = true;
                isInputFormDisabled = false;
            }
        }

        public void Initialize()
        {
            AttachEventHandlers();

            TypeDescriptor.AddProvider(new ConversionItemDescriptionProvider(), typeof(ConversionItem));

            AddColumnsToGrid();

            SetUpBindings();

            SetUpStrConvComboBox();

            CheckTaskMode();

            CheckSaveButton();
        }

        public void SaveFile(string lastUsedDirectory, bool isSaveAs)
        {
            if (isSaveAs)
            {
                var savePath = dialog.SaveFile(lastUsedDirectory);

                if (!string.IsNullOrEmpty(savePath))
                {
                    PersistData(savePath);

                    view.SavedFilePath = savePath;
                    view.DialogResult = DialogResult.OK;
                    viewMode = ConversionFileViewMode.Existing;
                    view.SaveButton.Enabled = true;
                }
            }
            else
            {
                PersistData(view.SavedFilePath);
            }
            view.Form.Close();
        }

        private void AddColumnsToGrid()
        {
            view.Grid.AutoGenerateColumns = false;

            var column1 = new DataGridViewTextBoxColumn();
            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column1.ReadOnly = true;
            column1.HeaderText = "Description";
            column1.DataPropertyName = "Description";
            column1.Name = "Description";
            column1.SortMode = DataGridViewColumnSortMode.NotSortable;

            var column2 = new DataGridViewTextBoxColumn();
            column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column2.ReadOnly = true;
            column2.HeaderText = "Search";
            column2.DataPropertyName = "SearchText";
            column2.Name = "Search";
            column2.SortMode = DataGridViewColumnSortMode.NotSortable;

            var column3 = new DataGridViewTextBoxColumn();
            column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column3.ReadOnly = true;
            column3.HeaderText = "Replace";
            column3.DataPropertyName = "ReplacementText";
            column3.Name = "Replacement";
            column3.SortMode = DataGridViewColumnSortMode.NotSortable;

            view.Grid.Columns.Add(column1);
            view.Grid.Columns.Add(column2);
            view.Grid.Columns.Add(column3);
        }

        private void AttachEventHandlers()
        {
            view.Regex.Click += Regex_Click;
            view.WholeWord.Click += WholeWord_Click;
            view.StrConv.Click += StrConv_Click;
            view.TagPair.Click += TagPair_Click;
            view.Search.Validating += Search_Validating;
            view.Replace.Validating += Replace_Validating;
            view.ToLower.Click += ToLowerOrUpper_Click;
            view.ToUpper.Click += ToLowerOrUpper_Click;
            view.Placeholder.Click += Placeholder_Click;
            view.EmbeddedTags.Click += EmbeddedTags_Click;
            view.Filter.TextChanged += Filter_TextChanged;
            view.ClearFilter.Click += ClearFilter_Click;

            view.Grid.CellEnter += Grid_CellEnter;
            view.Form.Shown += Form_Shown;
        }

        private void CheckMutuallyExclusiveValues(string first, string second)
        {
            var checkBoxItems = view.VbStrConv.CheckBoxItems;

            if (checkBoxItems[first].Checked)
            {
                view.VbStrConv.CheckBoxItems[second].Enabled = false;
            }
            else
            {
                view.VbStrConv.CheckBoxItems[second].Enabled = true;
            }
        }

        private void CheckTaskMode()
        {
            if (taskMode == BatchTaskMode.Target)
            {
                view.Placeholder.Enabled = false;
                view.TagPair.Enabled = false;
                view.EmbeddedTags.Enabled = false;
                view.Placeholder.Visible = false;
                view.TagPair.Visible = false;
                view.EmbeddedTags.Visible = false;
            }
        }

        private void ClearErrorProvider(TextBox txtbox)
        {
            view.ErrorProvider.Clear();
            view.ErrorProvider.SetError(txtbox, string.Empty);
        }

        private void ClearFilter_Click(object sender, EventArgs e)
        {
            bindingListView.RemoveFilter();
        }

        private void EmbeddedTags_Click(object sender, EventArgs e)
        {
            if (view.EmbeddedTags.Checked)
            {
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair, view.StrConv);
                view.Replace.Enabled = false;
            }
            else
            {
                ToggleCheckBoxState(true, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair, view.StrConv);
                view.Replace.Enabled = true;
            }
        }

        private void Filter_TextChanged(object sender, EventArgs e)
        {
            var checkedButton = view.ColumnFilter.Controls.OfType<RadioButton>()
                .FirstOrDefault(r => r.Checked);

            if (checkedButton != null)
            {
                var text = view.Filter.Text;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    switch (checkedButton.Name)
                    {
                        case "replaceRadioButton":
                            bindingListView.ApplyFilter(item => item.Replacement.Text.Contains(text));
                            break;

                        case "searchRadioButton":
                            bindingListView.ApplyFilter(item => item.Search.Text.Contains(text));
                            break;

                        case "titleRadioButton":
                            bindingListView.ApplyFilter(item => item.Description.Contains(text));
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    bindingListView.RemoveFilter();
                }
            }
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            if (viewMode == ConversionFileViewMode.Existing)
            {
                SetUIStatus();
            }

            SetCheckBoxComboBoxBinding();
        }

        private ConversionItem GetCurrentItem()
        {
            var convItem = (ObjectView<ConversionItem>)view.BindingSource.Current;

            return convItem?.Object;
        }

        private IBindingListView GetDataSource()
        {
            if (viewMode == ConversionFileViewMode.New)
            {
                bindingListView = new BindingListView<ConversionItem>(new List<ConversionItem>());
                return bindingListView;
            }
            else if (viewMode == ConversionFileViewMode.Existing)
            {
                var list = XmlUtilities.Deserialize(view.SavedFilePath);
                bindingListView = new BindingListView<ConversionItem>(list.Items);
                return bindingListView;
            }
            else
            {
                throw new InvalidOperationException($"{viewMode} is not a valid view viewMode");
            }
        }

        private void Grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            SetUIStatus();
            SetCheckBoxComboBoxBinding();
        }

        private bool IsGridEmpty()
        {
            var bindingSource = view.BindingSource;
            return bindingSource.Count == 0;
        }

        private void PersistData(string savePath)
        {
            var bindingSource = view.BindingSource;

            var list = new ConversionItemList();
            list.Items = bindingSource.List.Cast<ConversionItem>().ToList();

            XmlUtilities.Serialize(list, savePath);
        }

        private void Placeholder_Click(object sender, EventArgs e)
        {
            if (view.Placeholder.Checked)
            {
                ValidateReplaceTextBox();
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.StrConv);
            }
            else
            {
                ToggleCheckBoxState(true, view.ToLower, view.ToUpper, view.StrConv);
            }
        }

        private void Regex_Click(object sender, EventArgs e)
        {
            if (view.Regex.Checked)
            {
                ValidateRegexSearchTextBox();

                // ToLower and ToUpper only useful for regex
                ToggleCheckBoxState(true, view.ToLower, view.ToUpper);

                ToggleCheckBoxState(false, view.WholeWord);
            }
            else
            {
                ClearErrorProvider(view.Search);

                ToggleCheckBoxState(false, view.ToLower, view.ToUpper);
                ToggleCheckBoxState(true, view.WholeWord);

                // If strConv is checked, we disallow unchecking regex
                if (view.StrConv.Checked)
                {
                    ToggleCheckBoxState(true, view.Regex);
                }
            }
        }

        private void Replace_Validating(object sender, CancelEventArgs e)
        {
            if (view.Placeholder.Checked)
            {
                ValidateReplaceTextBox(e);
            }
        }

        private void Search_Validating(object sender, CancelEventArgs e)
        {
            if (view.Regex.Checked)
            {
                ValidateRegexSearchTextBox(e);
            }
            else if (view.TagPair.Checked)
            {
                ValidateTagPairSearchTextBox(e);
            }
        }

        private void SetCheckBoxComboBoxBinding()
        {
            var curItem = GetCurrentItem();

            if (curItem != null)
            {
                foreach (VbStrConv value in Enum.GetValues(typeof(VbStrConv)))
                {
                    var name = Enum.GetName(typeof(VbStrConv), value);

                    if (curItem.Search.VbStrConv.Contains(value))
                    {
                        view.VbStrConv.CheckBoxItems[name].Checked = true;
                    }
                    else
                    {
                        view.VbStrConv.CheckBoxItems[name].Checked = false;
                    }
                }
            }
        }

        private void SetUIStatus()
        {
            // Renable all check boxes except ToUpper and ToLower first
            ToggleCheckBoxState(true, view.Regex, view.WholeWord, view.StrConv, view.TagPair, view.Placeholder);
            // Renable Replace text box
            view.Replace.Enabled = true;

            if (view.Regex.Checked)
            {
                ToggleCheckBoxState(true, view.ToLower, view.ToUpper);
                ToggleCheckBoxState(false, view.WholeWord);
            }

            if (view.StrConv.Checked)
            {
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair);
                view.Regex.Checked = true;
                view.Replace.Enabled = false;
                view.VbStrConv.Visible = true;
            }

            if (view.Placeholder.Checked)
            {
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.StrConv);
            }

            if (view.ToUpper.Checked || view.ToLower.Checked)
            {
                view.Regex.Checked = true;
                view.Replace.Enabled = false;
                ToggleCheckBoxState(false, view.Placeholder, view.TagPair);
            }

            if (view.WholeWord.Checked)
            {
                ToggleCheckBoxState(false, view.Regex);
            }

            if (view.TagPair.Checked)
            {
                ToggleCheckBoxState(false, view.StrConv);
            }

            if (view.EmbeddedTags.Checked)
            {
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair, view.StrConv);
                view.Replace.Enabled = false;
            }
        }

        private void SetUpBindings()
        {
            view.BindingSource.DataSource = GetDataSource();
            view.Grid.DataSource = view.BindingSource;
            view.Description.DataBindings.Add("Text", view.BindingSource, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
            view.Search.DataBindings.Add("Text", view.BindingSource, "Search.Text", false, DataSourceUpdateMode.OnValidation);
            view.Replace.DataBindings.Add("Text", view.BindingSource, "Replacement.Text", false, DataSourceUpdateMode.OnValidation);
            view.CaseSensitive.DataBindings.Add("Checked", view.BindingSource, "Search.CaseSensitive", false, DataSourceUpdateMode.OnValidation);
            view.Regex.DataBindings.Add("Checked", view.BindingSource, "Search.UseRegex", false, DataSourceUpdateMode.OnValidation);
            view.WholeWord.DataBindings.Add("Checked", view.BindingSource, "Search.WholeWord", false, DataSourceUpdateMode.OnValidation);
            view.TagPair.DataBindings.Add("Checked", view.BindingSource, "Search.TagPair", false, DataSourceUpdateMode.OnValidation);
            view.StrConv.DataBindings.Add("Checked", view.BindingSource, "Search.StrConv", false, DataSourceUpdateMode.OnValidation);
            view.ToLower.DataBindings.Add("Checked", view.BindingSource, "Replacement.ToLower", false, DataSourceUpdateMode.OnValidation);
            view.ToUpper.DataBindings.Add("Checked", view.BindingSource, "Replacement.ToUpper", false, DataSourceUpdateMode.OnValidation);
            view.Placeholder.DataBindings.Add("Checked", view.BindingSource, "Replacement.Placeholder", false, DataSourceUpdateMode.OnValidation);
            view.EmbeddedTags.DataBindings.Add("Checked", view.BindingSource, "Search.EmbeddedTags", false, DataSourceUpdateMode.OnValidation);
        }

        private void SetUpStrConvComboBox()
        {
            foreach (var name in Enum.GetNames(typeof(VbStrConv)))
            {
                view.VbStrConv.Items.Add(name);
            }

            view.VbStrConv.CheckBoxCheckedChanged += VbStrConv_CheckBoxCheckedChanged;
        }

        private void StrConv_Click(object sender, EventArgs e)
        {
            if (view.StrConv.Checked)
            {
                ToggleCheckBoxState(false, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair);
                view.Regex.Checked = true;
                view.Replace.Enabled = false;
                view.VbStrConv.Visible = true;
            }
            else
            {
                ToggleCheckBoxState(true, view.ToLower, view.ToUpper, view.Placeholder, view.TagPair);
                view.Replace.Enabled = true;
                view.VbStrConv.Visible = false;
            }
        }

        private void TagPair_Click(object sender, EventArgs e)
        {
            if (view.TagPair.Checked)
            {
                ToggleCheckBoxState(false, view.StrConv);
            }
            else
            {
                ToggleCheckBoxState(true, view.StrConv);
            }
        }

        private void ToggleCheckBoxState(bool activate, params CheckBox[] list)
        {
            if (activate)
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    list[i].Enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    list[i].Checked = false;
                    list[i].Enabled = false;
                }
            }
        }

        private void ToLowerOrUpper_Click(object sender, EventArgs e)
        {
            if (view.ToUpper.Checked || view.ToLower.Checked)
            {
                view.Regex.Checked = true;
                view.Replace.Enabled = false;
                ToggleCheckBoxState(false, view.Placeholder);
            }
            else
            {
                view.Replace.Enabled = true;
                ToggleCheckBoxState(true, view.Placeholder);
            }
        }

        private VbStrConv UpdateVbStrConv()
        {
            int conv = 0;

            foreach (var item in view.VbStrConv.CheckBoxItems)
            {
                if (item.Checked)
                {
                    VbStrConv c;
                    if (Enum.TryParse(item.ToString(), out c))
                    {
                        conv += (int)c;
                    }
                }
            }

            return (VbStrConv)conv;
        }

        private void ValidateRegexSearchTextBox(CancelEventArgs e = null)
        {
            if (!Validator.IsValidRegexPattern(view.Search.Text))
            {
                view.ErrorProvider.SetError(view.Search, "Invalid regular expression");

                if (e != null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                ClearErrorProvider(view.Search);
            }
        }

        private void ValidateReplaceTextBox(CancelEventArgs e = null)
        {
            if (!Validator.IsValidXml(view.Replace.Text))
            {
                view.ErrorProvider.SetError(view.Replace, "Invalid placeholder expression");

                if (e != null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                ClearErrorProvider(view.Replace);
            }
        }

        private void ValidateTagPairSearchTextBox(CancelEventArgs e)
        {
            if (!Validator.IsValidXml(view.Search.Text))
            {
                view.ErrorProvider.SetError(view.Search, "Invalid tag pair expression");

                if (e != null)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                ClearErrorProvider(view.Search);
            }
        }

        private void VbStrConv_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var checkBoxItems = view.VbStrConv.CheckBoxItems;

            if (checkBoxItems["Uppercase"].Checked || checkBoxItems["Lowercase"].Checked)
            {
                view.VbStrConv.CheckBoxItems["LinguisticCasing"].Enabled = true;
            }
            else
            {
                view.VbStrConv.CheckBoxItems["LinguisticCasing"].Enabled = false;
            }

            CheckMutuallyExclusiveValues("Wide", "Narrow");
            CheckMutuallyExclusiveValues("Narrow", "Wide");

            ConversionItem curItem = GetCurrentItem();

            if (curItem != null)
            {
                var checkedItems = new List<VbStrConv>();

                foreach (var cb in view.VbStrConv.CheckBoxItems)
                {
                    if (cb.Checked)
                    {
                        var name = cb.ComboBoxItem as string;
                        VbStrConv v;
                        if (Enum.TryParse(name, out v))
                        {
                            checkedItems.Add(v);
                        }
                    }
                }

                curItem.Search.VbStrConv = checkedItems;
            }
        }

        private void WholeWord_Click(object sender, EventArgs e)
        {
            if (view.WholeWord.Checked)
            {
                ToggleCheckBoxState(false, view.Regex);
            }
            else
            {
                ToggleCheckBoxState(true, view.Regex);
            }
        }
    }
}