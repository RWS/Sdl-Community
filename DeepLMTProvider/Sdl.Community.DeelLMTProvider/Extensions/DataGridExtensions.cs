using System.Windows.Controls;

namespace Sdl.Community.DeepLMTProvider.Extensions;

public static class DataGridExtensions
{
    public static bool IsInEditMode(this DataGrid dataGrid)
    {
        bool isInEdit = false;

        // Ensure the operation runs on the UI thread
        if (!dataGrid.Dispatcher.CheckAccess())
        {
            dataGrid.Dispatcher.Invoke(() =>
            {
                isInEdit = CheckEditMode(dataGrid);
            });
        }
        else
        {
            isInEdit = CheckEditMode(dataGrid);
        }

        return isInEdit;
    }

    private static bool CheckEditMode(DataGrid dataGrid)
    {
        foreach (var item in dataGrid.Items)
        {
            var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
            if (row != null && row.IsEditing)
                return true;
        }
        return false;
    }
}