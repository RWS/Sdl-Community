using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Sdl.Community.ExcelTerminology.Insights
{
    public class UserSessionInitializer: ITelemetryInitializer
	{
	    public void Initialize(ITelemetry telemetry)
	    {
			telemetry.Context.User.Id = Environment.UserName;
		    telemetry.Context.User.UserAgent = "Studio plugin";
		    telemetry.Context.Session.Id = Guid.NewGuid().ToString();
		    telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
		    telemetry.Context.Device.Language = CultureInfo.CurrentCulture.DisplayName;
		    telemetry.Context.Component.Version = typeof(UserSessionInitializer)
			    .Assembly
			    .GetName()
			    .Version
			    .ToString();

		    telemetry.Context.InstrumentationKey = "e57cc02b-cb9e-4d81-b456-ebaea04091dd";
		}
    }
}
