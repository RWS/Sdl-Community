using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class EditGlossaryWindow : INotifyPropertyChanged
    {
        private string _filterQuery;
        private ObservableCollection<GlossaryEntry> _glossaryEntries;
        private string _glossaryName;
        private bool _isEditing;

        public EditGlossaryWindow(List<GlossaryEntry> glossaryEntries, string glossaryName)
        {
            GlossaryName = glossaryName;
            GlossaryEntries = new ObservableCollection<GlossaryEntry>(glossaryEntries);
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand DeleteEntryCommand => new CommandWithParameter(DeleteEntry);

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                SetField(ref _filterQuery, value);
                Filter();
            }
        }

        public ObservableCollection<GlossaryEntry> GlossaryEntries
        {
            get => _glossaryEntries;
            set
            {
                SetField(ref _glossaryEntries, value);

                _glossaryEntries.CollectionChanged += GlossaryEntries_CollectionChanged;
                _glossaryEntries.ForEach(ge => ge.PropertyChanged += (_, _) => GlossaryEntries_CollectionChanged(null, null));
            }
        }

        public string GlossaryName
        {
            get => _glossaryName;
            set => SetField(ref _glossaryName, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetField(ref _isEditing, value);
                Edit_Button.Content = value ? "✓" : "✏";
            }
        }

        public ICommand KeyboardCommand => new CommandWithParameter(ExecuteKeyboardShortcut);

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
            if (!GlossaryEntries.Any(ge => ge.IsEmpty())) GlossaryEntries.Add(new GlossaryEntry());
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            AddRow();
            IsEditing = true;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e) => CloseEditingWindow();

        private void CloseEditingWindow()
        {
            DialogResult = true;
            Close();
        }

        private void DeleteEntry(object glossaryEntry) => GlossaryEntries.Remove((GlossaryEntry)glossaryEntry);

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditing = !IsEditing;
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

            var termsToBeRemoved = GlossaryEntries.Where(glossaryEntry =>
                glossaryEntry.IsInvalid()).ToList();

            var duplicates = GlossaryEntries
                .GroupBy(ge => ge.SourceTerm)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Apply_Button.IsEnabled = !termsToBeRemoved.Any() && !duplicates.Any() && GlossaryEntries.Any();
        }
    }
}