using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.IATETerminologyProvider.View
{
    /// <summary>
    /// Interaction logic for FineGrainedFilter.xaml
    /// </summary>
    public partial class FineGrainedFilter : UserControl
    {
        public FineGrainedFilter()
        {
            InitializeComponent();
        }

        private void CollectionListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (IModel model in CollectionsListBox.ItemsSource)
                if (model.IsSelected) CollectionsListBox.SelectedItems.Add(model);
        }

        private void InstitutionsListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (IModel model in InstitutionsListBox.ItemsSource)
                if (model.IsSelected) InstitutionsListBox.SelectedItems.Add(model);
        }

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) if (e.AddedItems[0] is IModel model) model.IsSelected = true;
            if (e.RemovedItems.Count > 0) if (e.RemovedItems[0] is IModel model) model.IsSelected = false;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            var listBox = button.Name == nameof(SelectAllCollectionsButton) ? CollectionsListBox : InstitutionsListBox;

            var someChecked = false;
            var someUnchecked = false;
            foreach (IModel selectedItem in listBox.ItemsSource)
            {
                if (selectedItem.IsSelected) someChecked = true;
                if (!selectedItem.IsSelected) someUnchecked = true;
            }

            var checkedStatus = someChecked && someUnchecked ? CheckState.SomeChecked
                : someChecked ? CheckState.AllChecked : CheckState.AllNotChecked;

            switch (checkedStatus)
            {
                case CheckState.AllNotChecked:
                    foreach (IModel domain in listBox.ItemsSource)
                        listBox.SelectedItems.Add(domain);
                    break;

                case CheckState.SomeChecked:
                    foreach (IModel domain in listBox.ItemsSource)
                        if (!listBox.SelectedItems.Contains(domain))
                            listBox.SelectedItems.Add(domain);
                    break;

                case CheckState.AllChecked:
                    foreach (IModel domain in listBox.ItemsSource)
                        listBox.SelectedItems.Remove(domain);
                    break;
            }
        }

        private void SelectAllCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is not ListBox listBox) return;
            foreach (var item in listBox.ItemsSource) listBox.SelectedItems.Add(item);
        }
    }
}