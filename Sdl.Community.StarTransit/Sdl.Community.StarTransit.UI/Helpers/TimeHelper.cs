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
            if (selectedMoment == "PM" && selectedHour != -1)
            {
                selectedHour = (selectedHour % 12) + 12;
            }
            if (selectedHour != -1 && selectedMinute != -1)
            {
                
                var date = new DateTime(dueDate.GetValueOrDefault().Year, dueDate.GetValueOrDefault().Month,
                    dueDate.GetValueOrDefault().Day, selectedHour, selectedMinute, 0);

                return date;
            }
            if( selectedMinute==-1 && selectedHour ==-1)
            {
                var date = new DateTime(dueDate.GetValueOrDefault().Year, dueDate.GetValueOrDefault().Month, dueDate.GetValueOrDefault().Day,0,0,0);
                return date;
            }
            if (selectedMinute == -1)
            {
                var date = new DateTime(dueDate.GetValueOrDefault().Year, dueDate.GetValueOrDefault().Month, dueDate.GetValueOrDefault().Day, selectedHour, 0, 0);
                return date;
            }
            if (selectedHour == -1)
            {
                var date = new DateTime(dueDate.GetValueOrDefault().Year, dueDate.GetValueOrDefault().Month, dueDate.GetValueOrDefault().Day, 0, selectedMinute, 0);
                return date;
            }
            return null;
        }
    }
}
