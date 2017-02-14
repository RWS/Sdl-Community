namespace Sdl.Community.Qualitivity.Progress
{
    public class ProgressObject
    {

        public string ProgessTitle { get; set; }



        public string TotalProgressTitle { get; set; } //Total Progress
        public int TotalProgressMaximum { get; set; } //2
        public int TotalProgressValue { get; set; } //2
        public string TotalProgressValueMessage { get; set; }//Uploading ## of ##        
        public string TotalProgressPercentage { get; set; } //2%


        public string CurrentProcessingMessage { get; set; } //currently processing...
        public string CurrentProgressTitle { get; set; } //Record Progress       
        public int CurrentProgressMaximum { get; set; } //0
        public int CurrentProgressValue { get; set; } //0
        public string CurrentProgressValueMessage { get; set; }//Transferring ## of ##        
        public string CurrentProgressPercentage { get; set; }//0% complete


        public object Result { get; set; }

    }
}
