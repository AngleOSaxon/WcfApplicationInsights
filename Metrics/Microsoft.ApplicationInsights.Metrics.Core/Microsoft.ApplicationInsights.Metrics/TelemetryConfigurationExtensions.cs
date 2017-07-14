﻿using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.ApplicationInsights.Extensibility;

namespace Microsoft.ApplicationInsights.Metrics
{
    public static class TelemetryConfigurationExtensions
    {
        private static MetricManager s_defaultMetricManager = null;
        private static ConditionalWeakTable<TelemetryConfiguration, MetricManager> s_metricManagers = null;

        public static MetricManager Metrics(this TelemetryConfiguration telemetryPipeline)
        {
            if (telemetryPipeline == null)
            {
                return null;
            }

            // Fast path for the default configuration:
            if (telemetryPipeline == TelemetryConfiguration.Active)
            {
                MetricManager manager = s_defaultMetricManager;
                if (manager == null)
                {
                    MetricManager newManager = new MetricManager(telemetryPipeline);
                    MetricManager prevManager = Interlocked.CompareExchange(ref s_defaultMetricManager, newManager, null);
                    manager = prevManager ?? newManager;
                }

                return manager;
            }

            // Ok, we have a non-default config. Get the table:

            ConditionalWeakTable<TelemetryConfiguration, MetricManager> metricManagers = s_metricManagers;
            if (metricManagers == null)
            {
                ConditionalWeakTable<TelemetryConfiguration, MetricManager> newTable = new ConditionalWeakTable<TelemetryConfiguration, MetricManager>();
                ConditionalWeakTable<TelemetryConfiguration, MetricManager> prevTable = Interlocked.CompareExchange(ref s_metricManagers, newTable, null);
                metricManagers = prevTable ?? newTable;
            }

            // Get the manager from the table:
            {
                MetricManager manager = metricManagers.GetValue(telemetryPipeline, (tp) => new MetricManager(tp));
                return manager;
            }
        }
    }
}
