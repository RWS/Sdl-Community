using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class EditGlossaryWindow : INotifyPropertyChanged
    {
        private ObservableCollection<GlossaryEntry> _glossaryEntries;
        private bool _isEditing;

        public EditGlossaryWindow(List<GlossaryEntry> glossaryEntries)
        {
            GlossaryEntries = new ObservableCollection<GlossaryEntry>(glossaryEntries);
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand DeleteEntryCommand => new CommandWithParameter(DeleteEntry);

        public ObservableCollection<GlossaryEntry> GlossaryEntries
        {
            get => _glossaryEntries;
            set => SetField(ref _glossaryEntries, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetField(ref _isEditing, value);
        }

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

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            var glossaryEntry = new GlossaryEntry();
            GlossaryEntries.Add(glossaryEntry);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void DeleteEntry(object glossaryEntry) => GlossaryEntries.Remove((GlossaryEntry)glossaryEntry);

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);

            IsEditing = !IsEditing;
            button.Content = IsEditing ? "✓" : "📝";
            button.ToolTip = IsEditing ? "Finish editing" : "Edit glossary";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:

                    if (IsEditing) { IsEditing = false; break;}

                    DialogResult = false;
                    e.Handled = true;
                    Close();
                    break;

                case Key.Delete:
                    GlossaryEntries.Remove((GlossaryEntry)Entries_DataGrid.SelectedItem);
                    e.Handled = true;
                    break;
            }
        }
    }
}