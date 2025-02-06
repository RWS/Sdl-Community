using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel
{
    public class ReportViewFilterViewModel : ViewModelBase
    {
        //private ObservableCollection<string> _matchTypes;

        //private ObservableCollection<string> _statusTypes;

        //public ReportViewFilterViewModel()
        //{
        //    //Initialize();
        //}

        public ObservableCollection<string> MatchTypes { get; set; } =
            ["CM", "PM", "AT", "Exact Match", "Fuzzy Match", "No Match"];

        public ObservableCollection<string> Status { get; set; } =
            [
                "Not Translated", "Draft", "Translated", "Translation Rejected", "Translation Approved",
            "Sign-off Rejected", "Signed Off"
            ];

        //private void Initialize()
        //{
        //    //List<string> statusTypes =

        //    //Status = [];

        //    //foreach (var statusType in statusTypes)
        //    //{
        //    //    Status.Add(statusType);
        //    //}

        //    //List<string> matchTypes = ["CM", "PM", "AT", "Exact Match", "Fuzzy Match", "No Match"];
        //}
    }
}