using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Telemetry
{
    public interface ITelemetryTracker
    {
        void TrackException(Exception ex, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        void TrackTrace(string message, SeverityLevel severity);
        void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        IOperationHolder<RequestTelemetry> StartTrackRequest(string requestName);
        void StopTrackRequest(IOperationHolder<RequestTelemetry> operation, string responseCode);
        void StopTrackRequest(string responseCode);

        void Flush();
    }
}
