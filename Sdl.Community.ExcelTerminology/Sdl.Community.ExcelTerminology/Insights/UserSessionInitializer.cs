using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Sdl.Community.ExcelTerminology.Insights
{
    public class UserSessionInitializer: IContextInitializer
    {
        public void Initialize(TelemetryContext context)
        {
            context.User.Id = Environment.UserName;
            context.Session.Id = Guid.NewGuid().ToString();
            context.Device.OperatingSystem = Environment.OSVersion.ToString();
            context.Device.Language = CultureInfo.CurrentCulture.DisplayName;
        }
    }
}
