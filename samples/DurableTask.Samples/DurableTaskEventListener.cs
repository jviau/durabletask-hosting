using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

namespace DurableTask.Samples
{
    /// <summary>
    /// A trace listener for shimming to <see cref="ILogger"/>.
    /// </summary>
    public class DurableTaskEventListener : EventListener
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="DurableTaskEventListener"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public DurableTaskEventListener(ILogger<DurableTaskEventListener> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == "DurableTask-Core")
            {
                EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
            }
        }

        /// <inheritdoc />
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            LogLevel level = ToLogLevel(eventData.EventId);

            if (_logger.IsEnabled(level))
            {
                _logger.Log(level, FormatEvent(eventData));
            }
        }

        private static string FormatEvent(EventWrittenEventArgs eventData)
        {
            var sb = new StringBuilder();

            foreach ((string name, object payload) in eventData.PayloadNames.Zip(
                eventData.Payload, (n, p) => (n, p)))
            {
                sb.AppendFormat("{0}: {1}\n", name, payload, CultureInfo.InvariantCulture);
            }

            return sb.ToString();
        }

        private static LogLevel ToLogLevel(int eventId)
        {
            // the event id's durable task sends
            switch (eventId)
            {
                case 1: return LogLevel.Trace;
                case 2: return LogLevel.Debug;
                case 3: return LogLevel.Information;
                case 4: return LogLevel.Warning;
                case 5: return LogLevel.Error;
                case 6: return LogLevel.Critical;
                default: return LogLevel.None;
            }
        }
    }
}
