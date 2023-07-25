using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example.Background.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TelemetryClient tc;

    public Worker(ILogger<Worker> logger, TelemetryClient tc)
    {
        _logger = logger;
        this.tc = tc;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        tc.TrackMetric("here I am", 1);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
