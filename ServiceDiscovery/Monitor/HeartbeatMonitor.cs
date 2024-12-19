using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDiscovery.Services;

namespace ServiceDiscovery.Monitor
{
    public class HeartbeatMonitorService : IHostedService, IDisposable
    {
        private readonly IServiceDiscoveryService _serviceDiscoveryService;
        private Timer _timer;

        public HeartbeatMonitorService(IServiceDiscoveryService serviceDiscoveryService)
        {
            _serviceDiscoveryService = serviceDiscoveryService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run cleanup task every 30 seconds
            _timer = new Timer(async state => await _serviceDiscoveryService.CleanUpSequence(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}