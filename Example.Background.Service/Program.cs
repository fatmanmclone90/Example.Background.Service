using System.Collections.Generic;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.Background.Service
{
    public class Program
    {
        private static List<(string namespsace, string metric)> RuntimeMetrics => new()
        {
            ( SystemRuntimeNamespace, "threadpool-completed-items-count" ),
            ( SystemRuntimeNamespace, "threadpool-queue-length" )   ,
            ( SystemRuntimeNamespace, "threadpool-thread-count" ) ,
            ( SystemNetHttpNamespace, "http11-requests-queue-duration" ),
            ( SystemNetDnsNamespace, "dns-lookups-duration"),
        };
        private const string SystemRuntimeNamespace = "System.Runtime";
        private const string SystemNetHttpNamespace = "System.Net.Http";
        private const string SystemNetDnsNamespace = "System.Net.NameResolution";

        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService(options =>
                    {
                        options.EnableAdaptiveSampling = false;
                    });

                    services.ConfigureTelemetryModule<EventCounterCollectionModule>(
                    (module, o) =>
                    {
                        module.Counters.Clear();
                        foreach (var (namespace, metric) in RuntimeMetrics)
                        {
                            module.Counters.Add(
                                new EventCounterCollectionRequest(
                                namespace,
                                metric));
                        }
                    });
                    services.AddHostedService<Worker>();
                });
    }
}