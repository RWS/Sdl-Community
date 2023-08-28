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
        private ObservableCollection<GlossaryItem> _glossaries;
        private bool _isEditing;

        public BrowseGlossariesWindow(List<string> supportedLanguages, IBrowseDialog openFileDialog)
        {
            Glossaries = new ObservableCollection<GlossaryItem>();
            Glossaries.CollectionChanged += Glossaries_CollectionChanged;

            SupportedLanguages = supportedLanguages;
            OpenFileDialog = openFileDialog;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<GlossaryItem> Glossaries
        {
            get => _glossaries;
            set => SetField(ref _glossaries, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetField(ref _isEditing, value);
                EditButton.Content = value ? "Done" : "Edit";
            }
        }

        public bool IsImportEnabled => Glossaries.All(g => g.SourceLanguage != null && g.TargetLanguage != null);

        public ICommand KeyboardShortcutCommand => new CommandWithParameter(ExecuteKeyboardShortcut);
        public List<string> SupportedLanguages { get; }
        private IBrowseDialog OpenFileDialog { get; }

        public void AddGlossaries(string[] importDialogFileNames)
        {
            importDialogFileNames.ForEach(AddNewGlossary);
        }

        public void Browse()
        {
            if (OpenFileDialog.ShowDialog()) return;
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
            //TODO Delete Source and Target (testing purposes)
            if (Glossaries.All(g => g.Path != fn)) Glossaries.Add(new GlossaryItem(fn) { SourceLanguage = "EN", TargetLanguage = "DE" });
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

                case "ImportGlossaries":
                    ImportGlossaries();
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