using System;
using System.Globalization;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ExcelTerminology.Insights
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