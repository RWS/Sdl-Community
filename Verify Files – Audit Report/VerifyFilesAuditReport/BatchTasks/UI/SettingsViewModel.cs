using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace VerifyFilesAuditReport.BatchTasks.UI;

public class SettingsViewModel : ObservableObject
{
    private bool _allStatusesChecked;
    private bool _includeIgnoredMessages;
    private bool _includeVerificationDetails;

    // Also holds statuses restored from saved settings that are not present in the
    // current project's files, so they survive a round-trip through this page.
    private readonly List<string> _selectedStatuses = [];

    public ObservableCollection<StatusOption> Statuses { get; } = [];

    public bool IncludeIgnoredMessages
    {
        get => _includeIgnoredMessages;
        set => SetProperty(ref _includeIgnoredMessages, value);
    }

    public bool IncludeVerificationDetails
    {
        get => _includeVerificationDetails;
        set => SetProperty(ref _includeVerificationDetails, value);
    }

    public void LoadFrom(VerifyFilesExtendedSettings settings)
    {
        IncludeIgnoredMessages = settings.IncludeIgnoredMessages;
        IncludeVerificationDetails = settings.IncludeVerificationDetails;

        foreach (var status in settings.ReportStatuses.Where(s => !_selectedStatuses.Contains(s)))
            _selectedStatuses.Add(status);

        foreach (var option in Statuses)
            option.IsSelected = _selectedStatuses.Contains(option.Name);
    }

    public void SaveTo(VerifyFilesExtendedSettings settings)
    {
        settings.ReportStatuses = _selectedStatuses.ToList();
        settings.IncludeIgnoredMessages = IncludeIgnoredMessages;
        settings.IncludeVerificationDetails = IncludeVerificationDetails;
    }

    public void SetAvailableStatuses(IEnumerable<string> statusNames)
    {
        foreach (var name in statusNames.Where(n => Statuses.All(o => o.Name != n)))
        {
            var option = new StatusOption(name) { IsSelected = _selectedStatuses.Contains(name) };
            option.PropertyChanged += OnOptionPropertyChanged;
            Statuses.Add(option);
        }
    }

    public void ToggleAllStatuses()
    {
        _allStatusesChecked = !_allStatusesChecked;

        foreach (var option in Statuses)
            option.IsSelected = _allStatusesChecked;

        if (!_allStatusesChecked)
            _selectedStatuses.Clear();
    }

    private void OnOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(StatusOption.IsSelected) || sender is not StatusOption option)
            return;

        if (option.IsSelected && !_selectedStatuses.Contains(option.Name))
            _selectedStatuses.Add(option.Name);
        else if (!option.IsSelected)
            _selectedStatuses.Remove(option.Name);
    }
}
