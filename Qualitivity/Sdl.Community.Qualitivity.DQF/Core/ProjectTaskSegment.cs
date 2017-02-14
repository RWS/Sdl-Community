namespace Sdl.Community.DQF.Core
{
    public class ProjectTaskSegment
    {
        public string Segmentid { get; set; }
        public string DqfTranslatorKey { get; set; }
        public string DqfProjectKey { get; set; }
        public int Projectid { get; set; }
        public int Taskid { get; set; }
        public string SourceSegment { get; set; }
        public string TargetSegment { get; set; }
        public string NewTargetSegment { get; set; }
        public long Time { get; set; }
        public double TmMatch { get; set; }
        public int Cattool { get; set; }
        public int Mtengine { get; set; }
        public string MtEngineVersion { get; set; }


    }
}
