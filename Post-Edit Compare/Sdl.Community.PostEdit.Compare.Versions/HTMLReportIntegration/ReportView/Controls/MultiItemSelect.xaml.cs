using Sdl.Community.PostEdit.Versions.Commands;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>
    /// Interaction logic for MultiItemSelect.xaml
    /// </summary>
    public partial class MultiItemSelect : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register(nameof(BorderBackground), typeof(Brush), typeof(MultiItemSelect), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(MultiItemSelect), new PropertyMetadata(default(CornerRadius)));
        public static readonly DependencyProperty DeleteButtonAssistiveTextProperty = DependencyProperty.Register(nameof(DeleteButtonAssistiveText), typeof(string), typeof(MultiItemSelect), new PropertyMetadata("Press DELETE to unselect"));
        public static readonly DependencyProperty DeleteEnabledProperty = DependencyProperty.Register(nameof(DeleteEnabled), typeof(bool), typeof(MultiItemSelect), new PropertyMetadata(true, PropertyChangedCallback));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiItemSelect), new PropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MultiItemSelect), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(ObservableCollection<object>), typeof(MultiItemSelect), new PropertyMetadata(new ObservableCollection<object>(), SelectedItemsChanged));

        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(nameof(SelectedItemTemplate), typeof(DataTemplate), typeof(MultiItemSelect), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ShowDropdownProperty = DependencyProperty.Register(nameof(ShowDropdown), typeof(bool), typeof(MultiItemSelect), new PropertyMetadata(default(bool)));

        public MultiItemSelect()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Brush BorderBackground
        {
            get => (Brush)GetValue(BorderBackgroundProperty);
            set => SetValue(BorderBackgroundProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public string DeleteButtonAssistiveText
        {
            get => (string)GetValue(DeleteButtonAssistiveTextProperty);
            set => SetValue(DeleteButtonAssistiveTextProperty, value);
        }

        public bool DeleteEnabled
        {
            get => (bool)GetValue(DeleteEnabledProperty);
            set => SetValue(DeleteEnabledProperty, value);
        }

        public ICommand DeleteItemCommand => new RelayCommand(DeleteItem);

        public ICommand HideDropdownCommand => new RelayCommand(HideDropdown);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public DataTemplate SelectedItemTemplate
        {
            get => (DataTemplate)GetValue(SelectedItemTemplateProperty);
            set => SetValue(SelectedItemTemplateProperty, value);
        }

        public bool ShowDropdown
        {
            get => (bool)GetValue(ShowDropdownProperty);
            set
            {
                SetValue(ShowDropdownProperty, value);
                if (!value) return;

                Focus(AllItemsListBox);
                var firstItem = (ListBoxItem)AllItemsListBox.ItemContainerGenerator.ContainerFromIndex(0);
                Focus(firstItem);
            }
        }

        public ICommand ShowDropdownCommand => new RelayCommand(ShowDropdownHandler);

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not MultiItemSelect multiItemSelectControl) return;
            multiItemSelectControl.DeleteButtonAssistiveText =
                !multiItemSelectControl.DeleteEnabled ? null : "Press DELETE to unselect";
        }

        //public string DeleteButtonAssistiveText => !DeleteEnabled ? null : "Press DELETE to unselect";

        private static void SelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not MultiItemSelect multiItemSelectControl) return;
            var selectedItems = multiItemSelectControl.SelectedItems;

            var uiSelectedItems = multiItemSelectControl.AllItemsListBox.SelectedItems;
            uiSelectedItems.Clear();

            if (selectedItems is null) return;
            foreach (var selectedItem in selectedItems.ToList()) uiSelectedItems.Add(selectedItem);
        }

        private void AllItemsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems is null || SelectedItems.Count >= AllItemsListBox.SelectedItems.Count) return;

            foreach (var selectedItem in AllItemsListBox.SelectedItems)
            {
                if (SelectedItems.Contains(selectedItem)) continue;
                SelectedItems.Add(selectedItem);
            }
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!DeleteEnabled) return;
            DeleteItem(AllItemsListBox.SelectedItem);
        }

        private async void DeleteItem(object obj)
        {
            AllItemsListBox.SelectedItems.Remove(obj);
            SelectedItems?.Remove(obj);

            //await NotifyUser("Item unselected");

            var item = (ListBoxItem)SelectedItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedItemsControl.SelectedItem);
            Focus(item);
        }

        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDropdown = !ShowDropdown;
        }

        private void Focus(UIElement element)
        {
            if (element is null)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate
                {
                    element.Focus();
                }));
        }

        private void HideDropdown(object obj)
        {
            ShowDropdown = false;
            //var firstItem = (ListBoxItem)SelectedItemsControl.ItemContainerGenerator.ContainerFromItem(SelectedItemsControl.SelectedItem);
            Focus(SelectedItemsControl);
        }

        //private async Task NotifyUser(string message)
        //{
        //    Notifications = message;
        //    NotificationsPopup.IsOpen = true;
        //    Focus(NotificationsTextBlock);

        //    await Task.Delay(TimeSpan.FromSeconds(1));
        //    Focus(SelectedItemsControl);

        //    Notifications = "";
        //    NotificationsPopup.IsOpen = false;
        //}

        private void ShowDropdownHandler(object obj)
        {
            ShowDropdown = true;
        }

        //private void SelectAllItemsButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    AllItemsListBox.SelectAll();
        //}
        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            AllItemsListBox.SelectedItems.Clear();
            SelectedItems.Clear();
        }
    }
}