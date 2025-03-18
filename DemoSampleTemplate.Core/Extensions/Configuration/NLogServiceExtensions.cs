using Microsoft.ApplicationInsights.NLogTarget;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoSampleTemplate.Core.Extensions.Configuration
{
    public static class NLogServiceExtensions
    {
        /// <summary>
        /// Add Nlog configuration && transfer nlog into file and into appinsights
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="logFileName"></param>
        /// <exception cref="ArgumentException">logFileName cannot be null or WhiteSpace</exception>
        /// <returns></returns>
        public static IServiceCollection AddNLogConfiguration(this IServiceCollection services, IConfiguration configuration, string logFileName, string logFolderPath)
        {
            if (string.IsNullOrWhiteSpace(logFileName))
            {
                throw new ArgumentException("logFileName cannot be null or WhiteSpace. Please provide a valid filename", nameof(logFileName));
            }

            var applicationInsightsTelemetry = new ApplicationInsightsTelemetryOptions();
            configuration.GetSection(ApplicationInsightsTelemetryOptions.ApplicationInsightsTelemetry).Bind(applicationInsightsTelemetry);

            var config = new LoggingConfiguration();

            ApplicationInsightsTarget appInsightsTarget = new ApplicationInsightsTarget();
            appInsightsTarget.InstrumentationKey = applicationInsightsTelemetry.AIKey;

            NLog.Targets.Target appTarget = new NLog.Targets.FileTarget("appTarget")
            {
                FileName = logFolderPath + (logFolderPath.EndsWith("/") ? "" : "/") + logFileName + ".log",
                ArchiveFileName = logFolderPath + (logFolderPath.EndsWith("/") ? "" : "/") + "archives/" + logFileName + ".{#}.zip",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveDateFormat = "yyyyMMdd",
                CreateDirs = true,
                EnableArchiveFileCompression = true,
                MaxArchiveFiles = 30,
                ConcurrentWrites = true,
                KeepFileOpen = false,
                Encoding = Encoding.Default
            };

            LoggingRule ruleAppInsights = new LoggingRule("*", NLog.LogLevel.Trace, appInsightsTarget);

            LoggingRule ruleDebug = new LoggingRule("*", NLog.LogLevel.Trace, appTarget);

            config.LoggingRules.Add(ruleAppInsights);
            config.LoggingRules.Add(ruleDebug);

            LogManager.Configuration = config;

            return services;
        }
    }

    public class ApplicationInsightsTelemetryOptions
    {
        public const string ApplicationInsightsTelemetry = "ApplicationInsightsTelemetry";
        public string AIKey { get; set; }
        public string AIProxyUrl { get; set; }
        public string AIRoleName { get; set; }
        public bool AIUseProxy { get; set; }
        public string RunningEnvironment { get; set; }
    }
}
