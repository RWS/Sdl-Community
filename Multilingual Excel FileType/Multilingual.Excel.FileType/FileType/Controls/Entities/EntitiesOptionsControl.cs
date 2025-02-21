using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Multilingual.XML.FileType.FileType.Settings.Entities;
using Multilingual.XML.FileType.Models;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
    public partial class EntitiesOptionsControl : UserControl, IFileTypeSettingsAware<UniqueEntitySettings>
    {
        private UniqueEntitySettings _settings;
        private BindingSource _source;

        public EntitiesOptionsControl()
        {
            InitializeComponent();
            SetupGrid();

            EnableButtons();
        }

        private void SetupGrid()
        {
            _source = new BindingSource();
            dataGridViewEntityMappings.DataSource = _source;
        }

        public UniqueEntitySettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                UpdateUI();
            }
        }

        public void UpdateUI()
        {
            checkBoxConvertEntities.Checked = _settings.ConvertEntities;

            checkBoxConvertNumericEntityToPlaceholder.Checked = _settings.ConvertNumericEntityToPlaceholder;
            checkBoxSkipInsideLocked.Checked = _settings.SkipConversionInsideLockedContent;

            PopulateEntitySetListView(_settings.EntitySets.Values);

            PopulateByDefaultEntityMappingListView();
        }

        private void PopulateByDefaultEntityMappingListView()
        {
            var selectedSet = GetSelectedEntitySet();

            if (selectedSet == null)
            {
                _source.Clear();
                return;
            }

            PopulateEntityMappingListView(selectedSet.Mappings);
        }

        public void UpdateSettings()
        {
            _settings.ConvertEntities = checkBoxConvertEntities.Checked;
            _settings.ConvertNumericEntityToPlaceholder = checkBoxConvertNumericEntityToPlaceholder.Checked;
            _settings.SkipConversionInsideLockedContent = checkBoxSkipInsideLocked.Checked;
        }

        private void PopulateEntitySetListView(ICollection<EntitySet> values)
        {
            var insertionIndex = 0;

            listViewEntitySets.Items.Clear();

            foreach (var set in values)
            {
                if (set.IsDefault)
                    listViewEntitySets.Items.Add(new EntitySetListViewItem(set.Name, GetSetsDisplayName(set.Name)));
                else
                    listViewEntitySets.Items.Insert(insertionIndex++, new EntitySetListViewItem(set.Name, GetSetsDisplayName(set.Name)));
            }
        }

        private string GetSetsDisplayName(string name)
        {
            switch (name)
            {
                case "USERDEFINED":
                    return PluginResources.UserDefinedEntitySetName;
                case "ISOAMSA":
                    return PluginResources.ISOAMSA;
                case "ISOAMSB":
                    return PluginResources.ISOAMSB;
                case "ISOAMSC":
                    return PluginResources.ISOAMSC;
                case "ISOAMSO":
                    return PluginResources.ISOAMSO;
                case "ISOAMSR":
                    return PluginResources.ISOAMSR;
                case "ISOBOX":
                    return PluginResources.ISOBOX;
                case "ISOCYR1":
                    return PluginResources.ISOCYR1;
                case "ISOCYR2":
                    return PluginResources.ISOCYR2;
                case "ISODIA":
                    return PluginResources.ISODIA;
                case "ISOGRK1":
                    return PluginResources.ISOGRK1;
                case "ISOGRK2":
                    return PluginResources.ISOGRK2;
                case "ISOGRK3":
                    return PluginResources.ISOGRK3;
                case "ISOGRK4":
                    return PluginResources.ISOGRK4;
                case "ISOLAT1":
                    return PluginResources.ISOLAT1;
                case "ISOLAT2":
                    return PluginResources.ISOLAT2;
                case "ISONUM":
                    return PluginResources.ISONUM;
                case "ISOPUB":
                    return PluginResources.ISOPUB;
                case "ISOTECH":
                    return PluginResources.ISOTECH;
                case "HTMLSPECIAL":
                    return PluginResources.HTMLSPECIAL;
                case "HTMLSYMBOL":
                    return PluginResources.HTMLSYMBOL;
                case "ISOAMSN":
                    return PluginResources.ISOAMSN;
                default:
                    return $"{PluginResources.UnknownEntitySet}: {name}";
            }
        }

        private void PopulateEntityMappingListView(List<EntityMapping> entityMappings)
        {
            _source.DataSource = entityMappings;

            EnableButtons();
        }

        private void checkBoxConvertEntities_CheckedChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void listViewEntitySets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSet = GetSelectedEntitySet();

            if (selectedSet == null)
            {
                if (!string.IsNullOrEmpty(textBoxSearch.Text))
                {
                    DisplaySearchedEntitiesAndTheirsGroups();
                }
                else
                {
                    _source.Clear();
                }
                return;
            }

            DisplaySearchedEntitiesForSelectedSet(selectedSet);
        }

        private void DisplaySearchedEntitiesForSelectedSet(EntitySet selectedSet)
        {
            var mappings = selectedSet.Mappings;

            if (!string.IsNullOrEmpty(textBoxSearch.Text))
            {
                mappings = selectedSet.Mappings.Where(ContainsSearchedText).ToList();
            }

            PopulateEntityMappingListView(mappings);
        }

        private void dataGridViewEntityMappings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            var row = GetSelectedEntityMappingRow();

            if (row.Cells[e.ColumnIndex] is DataGridViewCheckBoxCell)
            {
                var cell = (DataGridViewCheckBoxCell)dataGridViewEntityMappings.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell == null) return;

                cell.Value = !(bool)cell.Value;
            }
        }

        private void dataGridViewEntityMappings_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedSet = GetSelectedEntitySet();
            if (selectedSet == null) return;
            if (selectedSet.IsDefault) return;

            EditEntityMapping();
        }

        private void buttonAddEntityMapping_Click(object sender, EventArgs e)
        {
            var addEditEntityMappingForm = new AddEditEntityMappingForm { Text = PluginResources.AddEntityMappingFormTitle };
            var dlgResult = addEditEntityMappingForm.ShowDialog();

            if (dlgResult == DialogResult.Cancel) return;

            var selectedSet = GetSelectedEntitySet();
            if (selectedSet == null) return;

            if (selectedSet.Mappings.Any(m => m.Name == addEditEntityMappingForm.EntityName))
            {
                MessageBox.Show(PluginResources.DuplicateEntityNameInList, PluginResources.DuplicateEntitiyNameInListTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var mapping = new EntityMapping(addEditEntityMappingForm.EntityName, (char)addEditEntityMappingForm.UnicodeValue, true, true);
            selectedSet.AddEntityMapping(mapping);

            PopulateEntityMappingListView(selectedSet.Mappings);
        }

        private void buttonEditEntityMapping_Click(object sender, EventArgs e)
        {
            EditEntityMapping();
        }

        private void buttonRemoveEntityMapping_Click(object sender, EventArgs e)
        {
            var selectedRow = GetSelectedEntityMappingRow();
            if (selectedRow == null) return;

            var selectedSet = GetSelectedEntitySet();

            if (selectedSet == null) return;

            var entityName = selectedSet.Mappings[selectedRow.Index].Name;

            selectedSet.RemoveEntityMapping(entityName);

            PopulateEntityMappingListView(selectedSet.Mappings);
        }

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            SetValueOnCheckBoxes(true);
        }

        private void buttonCheckNone_Click(object sender, EventArgs e)
        {
            SetValueOnCheckBoxes(false);
        }

        private void SetValueOnCheckBoxes(bool value)
        {
            var selectedSet = GetSelectedEntitySet();
            if (selectedSet == null) return;

            foreach (var mapping in selectedSet.Mappings)
            {
                mapping.Read = value;
                mapping.Write = value;
            }

            PopulateEntityMappingListView(selectedSet.Mappings);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            var selectedIndex = GetSelectedEntitySetIndex();
            if (selectedIndex == null) return;

            var dlgResult = MessageBox.Show(PluginResources.ResetEntitySet, PluginResources.ResetEntitySetTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult == DialogResult.No) return;

            var defaultEntitySettings = new UniqueEntitySettings();
            defaultEntitySettings.PopulateDefaultEntitySets();

            if (!(listViewEntitySets.Items[(int)selectedIndex] is EntitySetListViewItem selectedSetItem)) return;
            var defaultEntitySet = defaultEntitySettings.EntitySets[selectedSetItem.EntitySetId];
            var selectedEntitySet = GetSelectedEntitySet();

            selectedEntitySet.ClearEntityMappings();
            foreach (var item in defaultEntitySet.EntityMappingsByName)
                selectedEntitySet.AddEntityMapping(item.Value);

            PopulateEntityMappingListView(selectedEntitySet.Mappings);
        }

        private int? GetSelectedEntitySetIndex()
        {
            var indices = listViewEntitySets.SelectedIndices;
            if (indices.Count == 0 || indices.Count > 1) return null;

            return indices[0];
        }

        private EntitySet GetSelectedEntitySet()
        {
            var selectedIndex = GetSelectedEntitySetIndex();
            if (selectedIndex == null) return null;

            if (!(listViewEntitySets.Items[(int)selectedIndex] is EntitySetListViewItem lvItemEntitySet)) return null;
            if (!_settings.EntitySets.ContainsKey(lvItemEntitySet.EntitySetId)
              || _settings.EntitySets[lvItemEntitySet.EntitySetId] == null) return null;

            var entitySet = _settings.EntitySets[lvItemEntitySet.EntitySetId];
            return entitySet;
        }

        private DataGridViewRow GetSelectedEntityMappingRow()
        {
            var selectedRows = dataGridViewEntityMappings.SelectedRows;
            if (selectedRows.Count == 0 || selectedRows.Count > 1) return null;

            var selectedIndex = selectedRows[0].Index;

            return dataGridViewEntityMappings.Rows[selectedIndex];
        }

        private void EnableButtons()
        {
            var enableConversions = checkBoxConvertEntities.Checked;
            var selectedSet = GetSelectedEntitySet();

            var enableAdd = selectedSet != null && !selectedSet.IsDefault;
            buttonAddEntityMapping.Enabled = enableConversions;
            buttonAddEntityMapping.Visible = enableAdd;

            var enableEdit = selectedSet != null && !selectedSet.IsDefault && dataGridViewEntityMappings.SelectedRows.Count > 0;
            buttonEditEntityMapping.Enabled = enableConversions;
            buttonRemoveEntityMapping.Enabled = enableConversions;
            buttonEditEntityMapping.Visible = enableEdit;
            buttonRemoveEntityMapping.Visible = enableEdit;

            var enableCheck = enableConversions && dataGridViewEntityMappings.SelectedRows.Count > 0;
            buttonCheckAll.Enabled = enableCheck;
            buttonCheckNone.Enabled = enableCheck;

            tableLayoutPanel2.RowStyles[0].SizeType = enableAdd ? SizeType.AutoSize : SizeType.Absolute;
            tableLayoutPanel2.RowStyles[1].SizeType = enableAdd ? SizeType.AutoSize : SizeType.Absolute;
            tableLayoutPanel2.RowStyles[2].SizeType = enableAdd ? SizeType.AutoSize : SizeType.Absolute;

            buttonReset.Enabled = enableConversions && dataGridViewEntityMappings.SelectedRows.Count > 0;

            listViewEntitySets.Enabled = enableConversions;
            dataGridViewEntityMappings.Enabled = enableConversions;
            textBoxSearch.Enabled = enableConversions;

            ChangeForegroundColorForDataGridView(enableConversions);
        }

        private void EditEntityMapping()
        {
            var selectedRow = GetSelectedEntityMappingRow();
            if (selectedRow == null) return;

            var selectedSet = GetSelectedEntitySet();
            if (selectedSet == null) return;

            var entityName = selectedSet.Mappings[selectedRow.Index].Name;
            var unicodeValue = selectedSet.Mappings[selectedRow.Index].CharAsInt;
            var read = selectedSet.Mappings[selectedRow.Index].Read;
            var write = selectedSet.Mappings[selectedRow.Index].Write;

            var addEditEntityMappingForm = new AddEditEntityMappingForm(entityName, unicodeValue)
            {
                Text = PluginResources.EditEntityMappingFormTitle
            };

            var dlgResult = addEditEntityMappingForm.ShowDialog();

            if (dlgResult == DialogResult.Cancel) return;

            selectedSet.RemoveEntityMapping(entityName);

            var mapping = new EntityMapping(addEditEntityMappingForm.EntityName, (char)addEditEntityMappingForm.UnicodeValue, read, write);
            selectedSet.AddEntityMapping(mapping);

            PopulateEntityMappingListView(selectedSet.Mappings);
        }

        private void ChangeForegroundColorForDataGridView(bool isActive)
        {
            foreach (DataGridViewRow row in dataGridViewEntityMappings.Rows)
            {
                ChangeForegroundColorForRow(isActive, row);
            }
        }

        private static void ChangeForegroundColorForRow(bool isActive, DataGridViewRow selectedRow)
        {
            foreach (DataGridViewCell cell in selectedRow.Cells)
            {
                cell.Style = new DataGridViewCellStyle
                {
                    ForeColor = isActive ? Color.Black : Color.Gray
                };
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxSearch.Text))
            {
                PopulateEntitySetListView(_settings.EntitySets.Values);
                PopulateByDefaultEntityMappingListView();
            }
            else
            {
                DisplaySearchedEntitiesAndTheirsGroups();
            }

            textBoxSearch.Focus();
        }

        private void DisplaySearchedEntitiesAndTheirsGroups()
        {
            var searchedEntityMappings = new List<EntityMapping>();
            var groupsEntity = new List<EntitySet>();

            foreach (var value in _settings.EntitySets.Values)
            {
                var entities = value.Mappings.Where(ContainsSearchedText).ToList();

                if (entities.Any())
                {
                    groupsEntity.Add(value);

                    searchedEntityMappings.AddRange(entities);
                }
            }

            PopulateEntityMappingListView(searchedEntityMappings);

            PopulateEntitySetListView(groupsEntity);
        }

        private bool ContainsSearchedText(EntityMapping mapping)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf
                       (mapping.Name, textBoxSearch.Text, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
