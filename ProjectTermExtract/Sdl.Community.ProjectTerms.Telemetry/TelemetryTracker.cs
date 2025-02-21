using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;

namespace Sdl.Community.ProjectTerms.Telemetry
{
    public class TelemetryTracker : ITelemetryTracker
    {
        private readonly TelemetryClient telemetry;
        private IOperationHolder<RequestTelemetry> internalOperation;
        public TelemetryTracker()
        {
            telemetry = new TelemetryClient
            {
                InstrumentationKey = "e1702407-04b8-4a4f-9922-f2d7317a56fa"
            };
            telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
        }

        public void TrackException(Exception ex, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetry.TrackException(ex, properties, metrics);
        }

        public void TrackTrace(string message, SeverityLevel severity)
        {
            telemetry.TrackTrace(message, severity);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetry.TrackEvent(eventName, properties, metrics);

        }

        public IOperationHolder<RequestTelemetry> StartTrackRequest(string requestName)
        {
            var operation = telemetry.StartOperation<RequestTelemetry>(requestName);
            operation.Telemetry.Start();
            internalOperation = operation;
            return operation;
        }

        public void StopTrackRequest(IOperationHolder<RequestTelemetry> operation, string responseCode)
        {
            if (operation == null) return;
            operation.Telemetry.Stop();
            operation.Telemetry.ResponseCode = responseCode;
            telemetry.StopOperation(operation);
            operation.Dispose();
            internalOperation = null;
        }

        public void StopTrackRequest(string responseCode)
        {
            StopTrackRequest(internalOperation, responseCode);
        }

        public void Flush()
        {
            telemetry.Flush();
        }
    }
}
