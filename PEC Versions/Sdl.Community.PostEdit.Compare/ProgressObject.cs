namespace PostEdit.Compare
{

    public class ProgressObject
    {
        public string ProgessTitle { get; set; }
        public int TotalProgressValue { get; set; } //2
        public string TotalProgressValueMessage { get; set; }//Uploading ## of ##               
        public int CurrentProgressValue { get; set; } //0
        public string CurrentProgressValueMessage { get; set; }//Transferring ## of ##               
        public string CurrentProcessingMessage { get; set; } //currently processing...
        public object Result { get; set; }
                
    }
}
