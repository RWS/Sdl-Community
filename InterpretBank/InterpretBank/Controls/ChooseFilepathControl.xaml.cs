using Autofac;
using InterpretBank.Helpers;
using InterpretBank.Interface;
using InterpretBank.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace InterpretBank.Controls
{
    /// <summary>
    /// Interaction logic for ChooseFilepathControl.xaml
    /// </summary>
    public partial class ChooseFilepathControl : UserControl
    {
        public static readonly DependencyProperty DatabaseListProperty =
                    DependencyProperty.Register(nameof(DatabaseList), typeof(DatabaseList), typeof(ChooseFilepathControl),
                new PropertyMetadata(default(DatabaseList)));

        public static readonly DependencyProperty FilepathProperty =
                            DependencyProperty.Register(nameof(Filepath), typeof(string), typeof(ChooseFilepathControl),
                new PropertyMetadata(string.Empty));

        private static readonly string DbListFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Trados AppStore\InterpretBank");

        private static readonly string DbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Trados AppStore\InterpretBank\DatabaseList.json");

        public ChooseFilepathControl()
        {
            InitializeComponent();
        }

        public DatabaseList DatabaseList
        {
            get => (DatabaseList)GetValue(DatabaseListProperty);
            set => SetValue(DatabaseListProperty, value);
        }

        public string Filepath
        {
            get => (string)GetValue(FilepathProperty);
            set
            {
                if (!File.Exists(value))
                {
                    var confirmation =
                        UserInteractionService.Confirm("This DB no longer exists. Do you wish to remove it from this list?");
                    if (confirmation)
                    {
                        DatabaseList.List.Remove(value);
                        DatabaseList.List.Remove(value);
                    }
                }

                AddToDatabaseList(value);

                SetValue(FilepathProperty, value);
            }
        }

        private IUserInteractionService UserInteractionService => ApplicationInitializer.ApplicationLifetimeScope.Resolve<IUserInteractionService>();

        private static void CreateDatabaseListFile()
        {
            if (!Directory.Exists(DbListFolderPath)) Directory.CreateDirectory(DbListFolderPath);
            using var file = File.Create(DbListPath);
        }

        private void AddToDatabaseList(string filepath)
        {
            if (!DatabaseList.List.Contains(filepath))
                DatabaseList.List.Add(filepath);

            File.WriteAllText(DbListPath, JsonConvert.SerializeObject(DatabaseList));
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!UserInteractionService.GetFilePath(out var filepath, "Interpret Bank Databases (*.db)|*.db")) return;
            Filepath = filepath;
        }

        private void ChooseFilepathControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(DbListPath)) CreateDatabaseListFile();

            var deserializationActionResult = ErrorHandler.WrapTryCatch(DeserializeListFromFile);

            if (!deserializationActionResult.Success)
            {
                //File.Delete(DbListPath);

                var dbListDeserializationActionResult = ErrorHandler.WrapTryCatch(() =>
                    JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(DbListPath)));

                if (!dbListDeserializationActionResult.Success)
                {
                    File.Delete(DbListPath);
                    CreateDatabaseListFile();
                }

                DatabaseList = new DatabaseList
                {
                    LastUsed = "",
                    List = dbListDeserializationActionResult.Result
                };

                File.WriteAllText(DbListPath, JsonConvert.SerializeObject(DatabaseList));
            }

            if (!string.IsNullOrWhiteSpace(DatabaseList.LastUsed))
            {
                FilepathCombobox.SelectedIndex = DatabaseList.List.IndexOf(DatabaseList.LastUsed);
            }
        }

        private void DeserializeListFromFile()
        {
            DatabaseList = JsonConvert.DeserializeObject<DatabaseList>(File.ReadAllText(DbListPath)) ??
                new DatabaseList();
        }

        private void DocumentationButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://appstore.rws.com/Plugin/243?tab=documentation");
        }

        private void FilepathCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatabaseList.LastUsed = Filepath;
            File.WriteAllText(DbListPath, JsonConvert.SerializeObject(DatabaseList));
        }
    }
}