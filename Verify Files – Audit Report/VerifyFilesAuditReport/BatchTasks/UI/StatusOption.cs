namespace VerifyFilesAuditReport.BatchTasks.UI;

public class StatusOption(string name) : ObservableObject
{
    private bool _isSelected;

    public string Name { get; } = name;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
