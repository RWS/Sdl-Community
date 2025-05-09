using System;

namespace GroupshareExcelAddIn.Models
{
    public class DateRange
    {
        public DateRange(DateTime? startPublishingDate, DateTime? endPublishingDate, DateTime? startDeliveryDate, DateTime? endDeliveryDate)
        {
            StartPublishingDate = startPublishingDate;
            EndPublishingDate = endPublishingDate;
            StartDeliveryDate = startDeliveryDate;
            EndDeliveryDate = endDeliveryDate;
        }

        public DateTime? EndDeliveryDate { get; set; }
        public DateTime? EndPublishingDate { get; set; }
        public DateTime? StartDeliveryDate { get; set; }
        public DateTime? StartPublishingDate { get; set; }
    }
}