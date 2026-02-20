using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.UI
{
    public partial class BrowseGlossariesWindow : INotifyPropertyChanged
    {
        public BrowseGlossariesWindow(List<string> supportedLanguages, IBrowseDialog openFileDialog, IGlossarySniffer glossarySniffer)
        {
            Glossaries.CollectionChanged += Glossaries_CollectionChanged;

            SupportedLanguages = supportedLanguages;
            OpenFileDialog = openFileDialog;
            GlossarySniffer = glossarySniffer;

            InitializeComponent();
        }

        public BrowseGlossariesWindow(List<string> supportedLanguages, IGlossarySniffer glossarySniffer)
        {
            SupportedLanguages = supportedLanguages;
            GlossarySniffer = glossarySniffer;

            InitializeComponent();

            Browse_Button.Visibility = Visibility.Collapsed;
            ImportGlossaries_Button.Content = "Add new glossary";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<GlossaryItem> Glossaries
        {
            get;
            set => SetField(ref field, value);
        } = new();

        public bool IsEditing
        {
            get;
            set
            {
                SetField(ref field, value);
                EditButton.Content = value ? "Done" : "Edit";
            }
        }

        public bool IsImportEnabled => Glossaries.All(g => g.SourceLanguage != null && g.TargetLanguage != null);

        public ICommand KeyboardShortcutCommand => new CommandWithParameter(ExecuteKeyboardShortcut);
        public List<string> SupportedLanguages { get; }
        private IGlossarySniffer GlossarySniffer { get; }
        private IBrowseDialog OpenFileDialog { get; }

        public void AddGlossaries(string[] importDialogFileNames)
        {
            importDialogFileNames.ForEach(AddNewGlossary);
        }

        public void Browse()
        {
            if (!OpenFileDialog.ShowDialog()) return;
            AddGlossaries(OpenFileDialog.FileNames);
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

        private void AddNewGlossary(string fn)
        {
            if (Glossaries.Any(g => g.Path == fn)) return;

            var newGlossaryItem = new GlossaryItem(fn);
            var fnLower = fn.ToLowerInvariant();
            if (fnLower.Contains(".csv"))
            {
                var metadata = GlossarySniffer.GetGlossaryFileMetadata(fn, SupportedLanguages);

                newGlossaryItem.Delimiter = metadata.Delimiter.ToString();
                newGlossaryItem.SourceLanguage = metadata.Source;
                newGlossaryItem.TargetLanguage = metadata.Target;
            }

            Glossaries.Add(newGlossaryItem /*{ SourceLanguage = "EN", TargetLanguage = "DE" }*/);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Browse();
        }

        private void CancelImportGlossaries()
        {
            DialogResult = false;
            Close();
        }

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsEditing = true;
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsEditing = !IsEditing;
        }

        private void ExecuteKeyboardShortcut(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Edit":
                    IsEditing = true;
                    break;

                case "Escape":
                    if (IsEditing) IsEditing = false;
                    else CancelImportGlossaries();
                    break;

                case "Enter":
                    if (IsEditing) IsEditing = false;
                    else if (ImportGlossaries_Button.IsEnabled) ImportGlossaries();
                    break;
            }
        }

        private void Glossaries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var eNewItem = (GlossaryItem)e.NewItems[0];
                eNewItem.PropertyChanged +=
                    (_, _) => OnPropertyChanged(nameof(IsImportEnabled));
            }

            OnPropertyChanged(nameof(IsImportEnabled));
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportGlossaries();
        }

        private void ImportGlossaries()
        {
            DialogResult = true;
            Close();
        }
    }
}