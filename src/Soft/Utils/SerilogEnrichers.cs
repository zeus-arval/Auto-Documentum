using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft.Utils
{
    public class LogScopeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Scope", "[root]"));
        }
    }

    public class PaddedLevelEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("PaddedLevel", $"[{logEvent.Level}]".PadRight(13)));
        }
    }

    public class ScopeContextMinifierEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = logEvent.Properties["SourceContext"].ToString();
            var minContext = context?.Trim('"').Split('.').LastOrDefault();

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SourceContextMin", minContext));
        }
    }
}
