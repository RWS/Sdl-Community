using Autofac;
using InterpretBank.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterpretBank.SettingsService.UI.Controls
{
    /// <summary>
    /// Interaction logic for ChooseFilepathControl.xaml
    /// </summary>
    public partial class ChooseFilepathControl : UserControl
    {
        public static readonly DependencyProperty DatabaseListProperty =
                    DependencyProperty.Register(nameof(DatabaseList), typeof(List<string>), typeof(ChooseFilepathControl),
                new PropertyMetadata(default(List<string>)));

        public static readonly DependencyProperty FilepathProperty =
                            DependencyProperty.Register(nameof(Filepath), typeof(string), typeof(ChooseFilepathControl),
                new PropertyMetadata(string.Empty));

        private static readonly string DbListPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"Trados AppStore\InterpretBank\DatabaseList.json");

        public ChooseFilepathControl()
        {
            InitializeComponent();

            if (!File.Exists(DbListPath))
            {
                using var file = File.Create(DbListPath);
            }

            DatabaseList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(DbListPath)) ?? new List<string>();
        }

        public List<string> DatabaseList
        {
            get => (List<string>)GetValue(DatabaseListProperty);
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
                        DatabaseList.Remove(value);
                        DatabaseList.Remove(value);
                    }
                }

                AddToDatabaseList(value);
                SetValue(FilepathProperty, value);
            }
        }

        private IUserInteractionService UserInteractionService => ApplicationInitializer.ApplicationLifetimeScope.Resolve<IUserInteractionService>();

        private void AddToDatabaseList(string filepath)
        {
            if (!DatabaseList.Contains(filepath))
                DatabaseList.Add(filepath);
            File.WriteAllText(DbListPath, JsonConvert.SerializeObject(DatabaseList));
        }

        //private void AutoCompleteList_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var autoCompleteOption = (string)AutoCompleteList.SelectedItem;

        //    if (!File.Exists(autoCompleteOption))
        //    {
        //        var confirmation =
        //            UserInteractionService.Confirm("This DB no longer exists. Do you wish to remove it from this list?");
        //        if (confirmation)
        //        {
        //            DatabaseList.Remove(autoCompleteOption);
        //            DatabaseList.Remove(autoCompleteOption);
        //        }
        //    }
        //    else
        //    {
        //        Filepath = autoCompleteOption;
        //    }
        //    AutoCompleteList.IsDropDownOpen = false;
        //}

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!UserInteractionService.GetFilePath(out var filepath, "Interpret Bank Databases (*.db)|*.db")) return;
            Filepath = filepath;
        }

        //private void ClearFilepathButton(object sender, RoutedEventArgs e)
        //{
        //    FilepathTextBox.Clear();
        //    Filepath = null;
        //}

        //private void FilepathTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    OpenMenu();
        //}

        //private void FilepathTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //    {
        //        e.Handled = true;

        //        if (AutoCompleteList.IsDropDownOpen)
        //            AutoCompleteList.IsDropDownOpen = false;
        //        else if (!string.IsNullOrWhiteSpace(Filepath))
        //            FilepathTextBox.Clear();
        //        else
        //        {
        //            e.Handled = false;
        //        }
        //    }

        //    if (e.Key == Key.Enter)
        //    {
        //        if (AutoCompleteList.IsDropDownOpen)
        //        {
        //            Filepath = DatabaseList[AutoCompleteList.SelectedIndex];
        //            AutoCompleteList.IsDropDownOpen = false;
        //        }
        //        else OpenMenu();

        //        e.Handled = true;
        //    }

        //    if (e.Key == Key.Down)
        //    {
        //        AutoCompleteList.IsDropDownOpen = true;
        //        if (AutoCompleteList.SelectedIndex < AutoCompleteList.Items.Count - 1)
        //        {
        //            AutoCompleteList.SelectedIndex++;
        //            //AutoCompleteList.ScrollIntoView(AutoCompleteList.SelectedItem);
        //        }
        //        e.Handled = true;
        //    }
        //    else if (e.Key == Key.Up)
        //    {
        //        if (AutoCompleteList.SelectedIndex > 0)
        //        {
        //            AutoCompleteList.SelectedIndex--;
        //            //AutoCompleteList.ScrollIntoView(AutoCompleteList.SelectedItem);
        //        }
        //        e.Handled = true;
        //    }
        //}

        //private void OpenMenu()
        //{
        //    var isDropDownOpen = AutoCompleteList.IsDropDownOpen;
        //    AutoCompleteList.IsDropDownOpen = !isDropDownOpen;

        //    //AutoCompleteList.Visibility = AutoCompleteList.Visibility == Visibility.Visible
        //    //    ? Visibility.Collapsed
        //    //    : Visibility.Visible;
        //}
    }
}