using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Sdl.Community.DeepLMTProvider.Telemetry
{
	public interface ITelemetryTracker
	{
		void TrackException(Exception ex,
	 IDictionary<string, string> properties = null,
	 IDictionary<string, double> metrics = null);
		void TrackTrace(string message, SeverityLevel severity);
		void TrackEvent(string eventName,
			IDictionary<string, string> properties = null,
			IDictionary<string, double> metrics = null);
		IOperationHolder<RequestTelemetry> StartTrackRequest(string requestName);
		void StopTrackRequest(IOperationHolder<RequestTelemetry> operation, string responseCode);
		void StopTrackRequest(string responseCode);

		void Flush();
	}
}
