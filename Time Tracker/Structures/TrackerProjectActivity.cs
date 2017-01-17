using System;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class TrackerProjectActivity: ICloneable
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // New; Confirmed

        public string ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }

        public string ActivityTypeClientId { get; set; }
       
        public string TrackerProjectId { get; set; }
        public string TrackerProjectName { get; set; }
        public string TrackerProjectStatus { get; set; } //In progress, Completed

        public string ClientId { get; set; }
        public string ClientName { get; set; }

        public bool Billable { get; set; }

        public bool Invoiced { get; set; }
        public DateTime InvoicedDate { get; set; }
                
       

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public decimal Quantity { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal HourlyRateAdjustment { get; set; }

        public decimal Total { get; set; }

        public string Currency { get; set; }


        public TrackerProjectActivity()
        {
            Id = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Status = "New"; //New; Confirmed

            ActivityTypeId = string.Empty;
            ActivityTypeName = string.Empty;

            ActivityTypeClientId = string.Empty;

            TrackerProjectId = string.Empty;
            TrackerProjectName = string.Empty;
            TrackerProjectStatus = string.Empty;

            ClientId = string.Empty;
            ClientName = string.Empty;

            Billable = true;

            Invoiced = false;
            InvoicedDate = Common.DateNull;

            


            DateStart = Common.DateNull;
            DateEnd = Common.DateNull;

            Quantity = 0;
            HourlyRate = 0;
            HourlyRateAdjustment = 0;

            Total = 0;

            Currency = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
