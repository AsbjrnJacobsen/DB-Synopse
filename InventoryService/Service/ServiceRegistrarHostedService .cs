using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryService.Model;

namespace InventoryService.Service
{
    public class ServiceRegistrarHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;

        public ServiceRegistrarHostedService(IHostApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() => Task.Run(() => RegisterServiceAsync(cancellationToken)));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Optional: Deregister service when the application stops.
            Task.Run(() => DeregisterServiceAsync(cancellationToken));
            return Task.CompletedTask;
        }

        private async Task RegisterServiceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(5000, cancellationToken); // Simulate delay if needed
                var ipFinder = new ReplicaIpFinder();
                string ownIp = ipFinder.GetOwnIpAddress();
                string targetApiUrl = "http://servicediscovery:8085/ServiceDiscovery/register";

                var payload = new MicroServiceInstance { IpAddress = ownIp, Port = 8083, ServiceName = "InventoryService" };
                string jsonPayload = JsonSerializer.Serialize(payload);

                using var httpClient = new HttpClient();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(targetApiUrl, content, cancellationToken);

                response.EnsureSuccessStatusCode();
                Console.WriteLine("Service successfully registered.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering the service: {ex.Message}");
            }
        }

        private async Task DeregisterServiceAsync(CancellationToken cancellationToken)
        {
            try
            {
                var ipFinder = new ReplicaIpFinder();
                string ownIp = ipFinder.GetOwnIpAddress();
                string targetApiUrl = "http://servicediscovery:8085/ServiceDiscovery/deregister";

                var payload = new MicroServiceInstance { IpAddress = ownIp, Port = 8083, ServiceName = "InventoryService" };
                string jsonPayload = JsonSerializer.Serialize(payload);

                using var httpClient = new HttpClient();

                // Create a DELETE request with a body
                var request = new HttpRequestMessage(HttpMethod.Delete, targetApiUrl)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                Console.WriteLine("Service successfully deregistered.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deregistering the service: {ex.Message}");
            }
        }
    }
}