using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Metrics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.Monitoring
{
    public static class TelemetryClientExtensions
    {
        private const string MonitoringMetricNamespace = "Core";

        public static void AddMetric(this TelemetryClient telemetryClient, string metricName, double value, string dimension1Name, string dimension1Value)
        {
            MetricIdentifier mi = new MetricIdentifier(MonitoringMetricNamespace, metricName, dimension1Name);
            Metric m = telemetryClient.GetMetric(mi);
            m.TrackValue(value, dimension1Value);
        }
        public static void AddMetric(this TelemetryClient telemetryClient, string metricName, double value)
        {
            MetricIdentifier mi = new MetricIdentifier(MonitoringMetricNamespace, metricName);
            Metric m = telemetryClient.GetMetric(mi);
            m.TrackValue(value);
        }
    }
}
