﻿using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enrichers.AzureAuthInfo.Tests.Helpers
{
    public class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _write;

        public DelegatingSink(Action<LogEvent> write)
        {
            _write = write ?? throw new ArgumentNullException(nameof(write));
        }

        public void Emit(LogEvent logEvent)
        {
            _write(logEvent);
        }

        public static LogEvent GetLogEvent(Action<ILogger> writeAction)
        {
            LogEvent result = null;
            var l = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(new DelegatingSink(le => result = le))
                .CreateLogger();

            writeAction(l);
            return result;
        }
    }
}
