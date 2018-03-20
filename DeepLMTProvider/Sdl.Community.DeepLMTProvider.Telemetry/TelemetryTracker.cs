using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Sdl.Community.DeepLMTProvider.Telemetry
{
    public class TelemetryTracker:ITelemetryTracker
    {
		private readonly TelemetryClient _telemetry;
		private IOperationHolder<RequestTelemetry> internalOperation;
		public TelemetryTracker()
		{
			_telemetry = new TelemetryClient
			{
				InstrumentationKey = "eded01aa-a2d7-467c-8a92-a3522090f670"
			};
			_telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
			_telemetry.Context.Location.Ip = "****";
		}

		public void TrackException(Exception ex,
		  IDictionary<string, string> properties = null,
		  IDictionary<string, double> metrics = null)
		{
			_telemetry.TrackException(ex, properties, metrics);
		}

		public void TrackTrace(string message, SeverityLevel severity)
		{
			_telemetry.TrackTrace(message, severity);
		}

		public void TrackEvent(string eventName,
			IDictionary<string, string> properties = null,
			IDictionary<string, double> metrics = null)
		{
			_telemetry.TrackEvent(eventName, properties, metrics);

		}

		public IOperationHolder<RequestTelemetry> StartTrackRequest(string requestName)
		{
			var operation = _telemetry.StartOperation<RequestTelemetry>(requestName);
			operation.Telemetry.Start();
			internalOperation = operation;
			return operation;
		}

		public void StopTrackRequest(IOperationHolder<RequestTelemetry> operation, string responseCode)
		{
			if (operation == null) return;
			operation.Telemetry.Stop();
			operation.Telemetry.ResponseCode = responseCode;
			_telemetry.StopOperation(operation);
			operation.Dispose();
			internalOperation = null;
		}

		public void StopTrackRequest(string responseCode)
		{
			StopTrackRequest(internalOperation, responseCode);
		}

		public void Flush()
		{
			_telemetry.Flush();
		}
	}
}
