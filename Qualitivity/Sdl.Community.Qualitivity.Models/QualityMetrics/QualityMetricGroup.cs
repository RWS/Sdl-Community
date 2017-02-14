using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.QualityMetrics
{
   
    [Serializable]
    public class QualityMetricGroup : ICloneable
    {
      
        public int Id { get; set; }      
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxSeverityValue { get; set; }
        public int MaxSeverityInValue { get; set; }
        public string MaxSeverityInType { get; set; }
        public bool IsDefault { get; set; }

        public List<QualityMetric> Metrics { get; set; }
        public List<Severity> Severities { get; set; }


      

        public QualityMetricGroup()
        {
            Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            MaxSeverityValue = 50;
            MaxSeverityInValue = 1000;
            MaxSeverityInType = "words";
            IsDefault = false;
            Metrics = new List<QualityMetric>();
            Severities = new List<Severity>();
        }


        public object Clone()
        {
            var qmg = new QualityMetricGroup
            {
                Id = Id,
                Name = Name,
                IsDefault = IsDefault,
                Description = Description,
                MaxSeverityInValue = MaxSeverityInValue,
                MaxSeverityInType = MaxSeverityInType,
                MaxSeverityValue = MaxSeverityValue,
                Metrics = new List<QualityMetric>()
            };


            foreach (var qm in Metrics)
                qmg.Metrics.Add((QualityMetric)qm.Clone());

            qmg.Severities = new List<Severity>();
            foreach (var sv in Severities)
                qmg.Severities.Add((Severity)sv.Clone());


            return qmg;


        }
    }
}
