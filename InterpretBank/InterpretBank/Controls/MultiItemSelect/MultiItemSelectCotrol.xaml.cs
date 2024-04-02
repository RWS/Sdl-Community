using InterpretBank.Commands;
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

namespace InterpretBank.Controls.MultiItemSelect
{
    /// <summary>
    /// Interaction logic for MultiItemSelect.xaml
    /// </summary>
    public partial class MultiItemSelectControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register(nameof(BorderBackground), typeof(Brush), typeof(MultiItemSelectControl), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(MultiItemSelectControl), new PropertyMetadata(default(CornerRadius)));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiItemSelectControl), new PropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MultiItemSelectControl), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(string), typeof(MultiItemSelectControl), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(ObservableCollection<object>), typeof(MultiItemSelectControl), new PropertyMetadata(new ObservableCollection<object>(), PropertyChangedCallback));

        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(nameof(SelectedItemTemplate), typeof(DataTemplate), typeof(MultiItemSelectControl), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ShowDropdownProperty = DependencyProperty.Register(nameof(ShowDropdown), typeof(bool), typeof(MultiItemSelectControl), new PropertyMetadata(default(bool)));

        public MultiItemSelectControl()
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

        public string Notifications
        {
            get => (string)GetValue(NotificationsProperty);
            set => SetValue(NotificationsProperty, value);
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
            if (d is not MultiItemSelectControl multiItemSelectControl) return;
            multiItemSelectControl.AllItemsListBox.SelectedItems.Clear();
            foreach (var selectedItem in multiItemSelectControl.SelectedItems.ToList())
            {
                multiItemSelectControl.AllItemsListBox.SelectedItems.Add(selectedItem);
            }
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteItem(AllItemsListBox.SelectedItem);
        }

        private async void DeleteItem(object obj)
        {
            AllItemsListBox.SelectedItems.Remove(obj);
            await NotifyUser("Item unselected");
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

        private void ItemList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItems.Clear();
            foreach (var selectedItem in ((ListBox)sender).SelectedItems)
            {
                SelectedItems.Add(selectedItem);
            }
        }

        private async Task NotifyUser(string message)
        {
            Notifications = message;
            NotificationsPopup.IsOpen = true;
            Focus(NotificationsTextBlock);

            await Task.Delay(TimeSpan.FromSeconds(1));
            Focus(SelectedItemsControl);

            Notifications = "";
            NotificationsPopup.IsOpen = false;
        }

        private void ShowDropdownHandler(object obj)
        {
            ShowDropdown = true;
        }
    }
}