using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class EditGlossaryWindow : INotifyPropertyChanged

    {
        public EditGlossaryWindow(List<GlossaryEntry> glossaryEntries,
            string glossaryName)
        {
            GlossaryName = glossaryName;
            GlossaryEntries = new ObservableCollection<GlossaryEntry>(glossaryEntries);

            InitializeComponent();
        }

        public event Action ImportEntriesRequested;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand DeleteEntryCommand => new CommandWithParameter(DeleteEntry);

        public string FilterQuery
        {
            get;
            set
            {
                SetField(ref field, value);
                Filter();
            }
        }

        public ObservableCollection<GlossaryEntry> GlossaryEntries
        {
            get;
            set
            {
                SetField(ref field, value);

                field.CollectionChanged += GlossaryEntries_CollectionChanged;
                field.ForEach(ge => ge.PropertyChanged += (_, _) => GlossaryEntries_CollectionChanged(null, null));
                GlossaryEntries_CollectionChanged(null, null);
            }
        }

        public string GlossaryName
        {
            get;
            set => SetField(ref field, value);
        }

        public bool IsEditing
        {
            get;
            set
            {
                SetField(ref field, value);

                ApplyEditModeUiChanges(value);

                if (value) return;

                ValidateEntries();
                foreach (var glossaryEntry in GlossaryEntries)
                {
                    if (!glossaryEntry.IsEmpty()) continue;
                    GlossaryEntries.Remove(glossaryEntry);
                    break;
                }
            }
        }

        public ICommand KeyboardCommand => new CommandWithParameter(ExecuteKeyboardShortcut);
        public ICommand ValidateCommand => new ParameterlessCommand(ValidateEntries);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void AddRow()
        {
            if (GlossaryEntries.Any(ge => ge.IsEmpty())) return;

            var glossaryEntry = new GlossaryEntry();
            GlossaryEntries.Add(glossaryEntry);
            Entries_DataGrid.ScrollIntoView(glossaryEntry);
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
            AddRow();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e) => CloseEditingWindow();

        private void ApplyEditModeUiChanges(bool value)
        {
            if (!value) return;

            if (GlossaryEntries.Count != 1) return;
            var item = GlossaryEntries[0];
            if (item.IsDummyTerm()) item.CleanTerm();
        }

        private void CloseEditingWindow()
        {
            DialogResult = true;
            Close();
        }

        private void DeleteEntry(object glossaryEntry)
        {
            GlossaryEntries.Remove((GlossaryEntry)glossaryEntry);
            ValidateEntries();
        }

        private void Entries_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsEditing = true;
        }

        private void ExecuteKeyboardShortcut(object parameter)
        {
            var command = parameter.ToString();

            switch (command)
            {
                case "Edit":
                    IsEditing = !IsEditing;
                    break;

                case "Escape":
                    if (IsEditing) { IsEditing = false; break; }

                    if (ClearButtonTextBox.Filter_TextBox.IsFocused) { Keyboard.Focus(Apply_Button); break; }

                    DialogResult = false;
                    Close();
                    break;

                case "Delete":
                    var selectedIndex = Entries_DataGrid.SelectedIndex;
                    GlossaryEntries.Remove((GlossaryEntry)Entries_DataGrid.SelectedItem);

                    if (Entries_DataGrid.Items.Count - 1 >= selectedIndex) Entries_DataGrid.SelectedIndex = selectedIndex;
                    else if (Entries_DataGrid.Items.Count > 0) Entries_DataGrid.SelectedIndex = selectedIndex - 1;

                    break;

                case "Up":
                    if (Entries_DataGrid.SelectedIndex > 0) Entries_DataGrid.SelectedIndex--;
                    break;

                case "Down":
                    if (Entries_DataGrid.SelectedIndex < Entries_DataGrid.Items.Count - 1)
                        Entries_DataGrid.SelectedIndex++;
                    break;

                case "Enter":
                    if (!IsEditing) CloseEditingWindow();
                    else IsEditing = false;
                    break;

                case "Add":
                    AddRow();
                    IsEditing = true;
                    break;

                case "ImportEntries":
                    ImportButton_Click(null, null);
                    break;
            }
        }

        private void Filter()
        {
            var collectionView = CollectionViewSource.GetDefaultView(GlossaryEntries);

            if (string.IsNullOrWhiteSpace(FilterQuery))
            {
                collectionView.Filter = null;
                return;
            }

            collectionView.Filter = null;
            collectionView.Filter = entry =>
                ((GlossaryEntry)entry).SourceTerm.Contains(FilterQuery) ||
                ((GlossaryEntry)entry).TargetTerm.Contains(FilterQuery);
        }

        private void GlossaryEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.Action == NotifyCollectionChangedAction.Add)
                ((GlossaryEntry)e.NewItems[0]).PropertyChanged +=
                    (_, _) => GlossaryEntries_CollectionChanged(null, null);

            var termsToBeRemoved = GlossaryEntries.Where(glossaryEntry => glossaryEntry.IsInvalid()).ToList();

            var duplicates = GlossaryEntries.GetDuplicates();

            if (Apply_Button is null) return;

            Apply_Button.ToolTip = null;
            Apply_Button.IsEnabled = !termsToBeRemoved.Any() && !duplicates.Any() && GlossaryEntries.Any();

            if (!Apply_Button.IsEnabled) Apply_Button.ToolTip = "Invalid state. Cannot apply to glossary because DeepL would delete it on save.";
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportEntriesRequested?.Invoke();
            ValidateEntries();
        }

        private void TryRefreshDataGrid()
        {
            Task.Run(() =>
            {
                var count = 0;
                while (count < 10)
                {
                    count++;
                    if (Entries_DataGrid.IsInEditMode()) continue;
                    Entries_DataGrid.Dispatcher.Invoke(() => Entries_DataGrid.Items.Refresh());
                    return;
                }
            });
        }

        private void ValidateEntries()
        {
            var duplicates = GlossaryEntries.GetDuplicates();

            foreach (var entry in GlossaryEntries)
            {
                entry.Validate();
                if (duplicates.Contains(entry))
                {
                    entry.AddValidationError("Duplicate", "Duplicate entry found.");
                }
            }

            TryRefreshDataGrid();
        }
    }
}