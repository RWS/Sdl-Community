using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.UI.Helpers
{
    public class TimeHelper
    {
        public static DateTime? SetDateTime(DateTime? dueDate, int selectedHour, int selectedMinute, string selectedMoment)
        {

            if (selectedMoment == "PM")
            {
                selectedHour = (selectedHour % 12) + 12;
            }
            var date = new DateTime(dueDate.GetValueOrDefault().Year, dueDate.GetValueOrDefault().Month, dueDate.GetValueOrDefault().Day, selectedHour, selectedMinute, 0);

            return date;
        }
    }
}
