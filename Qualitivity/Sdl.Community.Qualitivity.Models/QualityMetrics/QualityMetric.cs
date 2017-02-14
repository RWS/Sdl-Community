using System;

namespace Sdl.Community.Structures.QualityMetrics
{
   
    [Serializable]
    public class QualityMetric : ICloneable
    {
      
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Modifed { get; set; }
        public Severity MetricSeverity { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public QualityMetric()
        {
            Id = -1;
            GroupId = -1;
            Name = string.Empty;
            Description = string.Empty;
            Modifed = DateTime.Now;
            MetricSeverity = new Severity();            
        }


        public object Clone()
        {
            var qm = new QualityMetric();          
            qm.Id = Id;
            qm.GroupId = GroupId;
            qm.Name = Name;
            qm.Description = Description;
            qm.Modifed = Modifed;
            qm.MetricSeverity = (Severity)MetricSeverity.Clone();

            return qm;
        }
    }
}
