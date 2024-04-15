using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.IATETerminologyProvider.View
{
    /// <summary>
    /// Interaction logic for DomainsAndTermTypes.xaml
    /// </summary>
    public partial class DomainsAndTermTypes : UserControl
    {
        public DomainsAndTermTypes()
        {
            InitializeComponent();
        }

        private void DomainsListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (IModel model in DomainsListBox.ItemsSource)
                if (model.IsSelected) DomainsListBox.SelectedItems.Add(model);
        }
        
        private void TermTypesListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (IModel model in TermTypesListBox.ItemsSource)
                if (model.IsSelected) TermTypesListBox.SelectedItems.Add(model);
        }

        private void SelectAllCommand_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is not ListBox listBox) return;
            foreach (var item in listBox.ItemsSource) listBox.SelectedItems.Add(item);
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            var listBox = button.Name == nameof(SelectAllDomainsButton) ? DomainsListBox : TermTypesListBox;

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

        private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) if (e.AddedItems[0] is IModel model) model.IsSelected = true;
            if (e.RemovedItems.Count > 0) if (e.RemovedItems[0] is IModel model) model.IsSelected = false;
        }
    }
}