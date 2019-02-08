using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.CleanUpTasks
{
	public class SegmentLockerPresenter : ISegmentLockerPresenter
    {
        private const string IsCaseSensitive = "IsCaseSensitive";
        private const string IsRegex = "IsRegex";
        private const string SearchText = "SearchText";
        private const string WholeWord = "WholeWord";
        private readonly ISegmentLockerControl control = null;
        private readonly XNamespace sdl = @"http://sdl.com/FileTypes/SdlXliff/1.0";
        private BindingList<SegmentLockItem> bindingList = null;

        public SegmentLockerPresenter(ISegmentLockerControl control)
        {
            this.control = control;
        }

        public void Initialize()
        {
            AttachEventHandlers();

            AddColumn();

            SetBindingList();

            GenerateStructure();
        }

        public void SaveSettings()
        {
            control.Settings.SegmentLockList = bindingList;
            control.Settings.StructureLockList = control.StructureList.Items.Cast<ContextDef>().ToList();
        }

        private void AddColumn()
        {
            control.ContentGrid.AutoGenerateColumns = false;

            var column1 = new DataGridViewTextBoxColumn();
            column1.ReadOnly = false;
            column1.HeaderText = "Search";
            column1.DataPropertyName = SearchText;
            column1.Name = SearchText;

            var column2 = new DataGridViewCheckBoxColumn();
            column2.ReadOnly = false;
            column2.HeaderText = "Regex";
            column2.DataPropertyName = IsRegex;
            column2.Name = IsRegex;

            var column3 = new DataGridViewCheckBoxColumn();
            column3.ReadOnly = false;
            column3.HeaderText = "Case";
            column3.DataPropertyName = IsCaseSensitive;
            column3.Name = IsCaseSensitive;

            var column4 = new DataGridViewCheckBoxColumn();
            column4.ReadOnly = false;
            column4.HeaderText = "Whole";
            column4.DataPropertyName = WholeWord;
            column4.Name = WholeWord;

            control.ContentGrid.Columns.Add(column1);
            control.ContentGrid.Columns.Add(column2);
            control.ContentGrid.Columns.Add(column3);
            control.ContentGrid.Columns.Add(column4);
        }

        private void AddToList(IEnumerable<ContextDef> structureList)
        {
            foreach (var item in structureList)
            {
                if (control.StructureList.FindStringExact(item.Type) == ListBox.NoMatches)
                {
                    control.StructureList.Items.Add(item, item.IsChecked);
                }
            }
        }

        private void AttachEventHandlers()
        {
            control.ContentGrid.DataBindingComplete += ContentGrid_DataBindingComplete;
            control.ContentGrid.CellValidating += ContentGrid_CellValidating;
            control.ContentGrid.CellEndEdit += ContentGrid_CellEndEdit;
            control.ContentGrid.CellMouseUp += ContentGrid_CellMouseUp;
        }

        private void ContentGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            control.ContentGrid.Rows[e.RowIndex].ErrorText = string.Empty;

            if (e.ColumnIndex == control.ContentGrid.Columns[IsRegex].Index)
            {
                // Check if regex is checked and disable whole word if needed
                var regexCell = control.ContentGrid.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell;
                var wholeWordCell = control.ContentGrid.Rows[e.RowIndex].Cells[3] as DataGridViewCheckBoxCell;

                ToggleMutuallyExclusiveCells(regexCell, wholeWordCell);

                var textBoxCell = control.ContentGrid.Rows[e.RowIndex].Cells[0] as DataGridViewTextBoxCell;

                if (textBoxCell != null)
                {
                    ValidateRegex(e.RowIndex, (string)textBoxCell.Value);
                }
            }
            else if (e.ColumnIndex == control.ContentGrid.Columns[WholeWord].Index)
            {
                // Check if whole word is check and disable regex if needed
                var regexCell = control.ContentGrid.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell;
                var wholeWordCell = control.ContentGrid.Rows[e.RowIndex].Cells[3] as DataGridViewCheckBoxCell;

                ToggleMutuallyExclusiveCells(wholeWordCell, regexCell);
            }
        }

        private void ContentGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == control.ContentGrid.Columns[IsRegex].Index)
            {
                control.ContentGrid.EndEdit();
            }
        }

        private void ContentGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == control.ContentGrid.Columns[SearchText].Index)
            {
                ValidateRegex(e.RowIndex, e.FormattedValue.ToString(), e);
            }
        }

        private void ContentGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            control.ContentGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            control.ContentGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            control.ContentGrid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            control.ContentGrid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void GenerateStructure()
        {
            var projFiles = ProjectFileManager.GetProjectFiles();

            // Combine lists together
            var structureList = GetStructureList(projFiles);

            var listItems = new List<ContextDef>(control.Settings.StructureLockList.Count +
                                                 structureList.Count());
            listItems.AddRange(control.Settings.StructureLockList);
            listItems.AddRange(structureList);

            // Sort
            listItems.Sort();

            // Add distinct items
            AddToList(listItems.Distinct());

            // Track checkbox changes
            control.StructureList.ItemCheck += StructureList_ItemCheck;
        }

        private ContextPurpose GetPurpose(XAttribute xAttribute)
        {
            if (xAttribute != null)
            {
                switch (xAttribute.Value)
                {
                    case "Match":
                        return ContextPurpose.Match;

                    case "Information":
                        return ContextPurpose.Information;

                    case "Location":
                        return ContextPurpose.Location;
                }
            }

            return ContextPurpose.Information;
        }

        private IEnumerable<ContextDef> GetStructureList(IEnumerable<ProjectFile> projFiles)
        {
            var structureList = new List<ContextDef>();

            // Project could have hundreds of files, so stops reading after 10 files structures are read
            int counter = 0;

            foreach (var file in projFiles)
            {
                structureList.AddRange(ReadStructInfo(file));
                counter++;

                if (counter > 10)
                {
                    break;
                }
            }

            return structureList;
        }

        private bool IsChecked(DataGridViewCheckBoxCell cell)
        {
            return cell.Value != null && (bool)cell.Value;
        }

        private IEnumerable<ContextDef> ReadStructInfo(ProjectFile file)
        {

            var contextDefs = new List<ContextDef>();

            if (file.LocalFileState == LocalFileState.None && 
                File.Exists(file.LocalFilePath) &&
                Path.GetExtension(file.LocalFilePath) == ".sdlxliff")
            {
                var root = XElement.Load(file.LocalFilePath, LoadOptions.None);
                foreach (var cxtdef in root.Descendants(sdl + "cxt-def"))
                {
                    var type = (string)cxtdef.Attribute("type");

                    if (!string.IsNullOrEmpty(type))
                    {
                        contextDefs.Add(new ContextDef()
                        {
                            Type = type,
                            Purpose = GetPurpose(cxtdef.Attribute("purpose"))
                        });
                    }
                }
            }

            return contextDefs;
        }

        private void SetBindingList()
        {
            bindingList = control.Settings.SegmentLockList;
            control.ContentGrid.DataSource = bindingList;
        }

        private void StructureList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var ctxDef = (ContextDef)control.StructureList.Items[e.Index];
            ctxDef.IsChecked = e.NewValue != CheckState.Unchecked;
        }

        private void ToggleMutuallyExclusiveCells(DataGridViewCheckBoxCell cell1, DataGridViewCheckBoxCell cell2)
        {
            if (IsChecked(cell1))
            {
                cell2.Value = false;
                cell2.ReadOnly = true;
                cell2.Style.BackColor = Color.LightGray;
                cell2.Style.ForeColor = Color.DarkGray;
            }
            else
            {
                cell2.ReadOnly = false;
                cell2.Style.BackColor = control.ContentGrid.DefaultCellStyle.BackColor;
                cell2.Style.ForeColor = control.ContentGrid.DefaultCellStyle.ForeColor;
            }
        }
        private void ValidateRegex(int rowIndex, string value, DataGridViewCellValidatingEventArgs e = null)
        {
            var checkBoxCell = control.ContentGrid.Rows[rowIndex].Cells[1] as DataGridViewCheckBoxCell;
            if (checkBoxCell != null)
            {
                if (checkBoxCell.Value != null && (bool)checkBoxCell.Value)
                {
                    if (!Validator.IsValidRegexPattern(value))
                    {
                        control.ContentGrid.Rows[rowIndex].ErrorText = "Invalid regex expression";

                        if (e != null)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
    }
}