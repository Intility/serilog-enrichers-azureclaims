using Serilog.Events;

namespace Serilog.Enrichers.AuthenticationInformation.Tests.Helpers
{
    internal static class LiteralValueExtensions
    {
        public static object LiteralValue(this LogEventPropertyValue @this)
        {
            return ((ScalarValue)@this).Value;
        }
    }
}
