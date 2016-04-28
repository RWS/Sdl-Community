using System;
using System.Globalization;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Sdl.Community.InSource.Insights
{
    public class UserSessionInitializer : IContextInitializer
    {
        public void Initialize(TelemetryContext context)
        {
            context.User.Id = Environment.UserName;
            context.User.UserAgent = "Studio plugin";
            context.Session.Id = Guid.NewGuid().ToString();
            context.Device.OperatingSystem = Environment.OSVersion.ToString();
            context.Device.Language = CultureInfo.CurrentCulture.DisplayName;
            context.Component.Version = typeof(UserSessionInitializer)
                .Assembly
                .GetName()
                .Version
                .ToString();

            context.InstrumentationKey = "fdbba35b-c4ab-4049-a952-5caba827b86a";


        }
    }
}
