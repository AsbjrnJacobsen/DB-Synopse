using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceDiscovery.Model;

namespace ServiceDiscovery.Services
{
    public class ServiceDiscoveryService : IServiceDiscoveryService
    {
        private List<MicroServiceInstance> _instances = new List<MicroServiceInstance>();
        private readonly TimeSpan _heartbeatTimeout = TimeSpan.FromSeconds(30);
        private HttpClient _httpClient = new HttpClient();

        public async Task Register(MicroServiceInstance instance)
        {

            Console.WriteLine("Service {0} at {1} registered", instance.Port, instance.IpAddress);
            instance.LastHeartbeat = DateTime.Now;
            instance.HealthCheckUrl = $"http://{instance.IpAddress}:{instance.Port}/Health/GetHealthStatus";
            _instances.Add(instance);

            //TODO : Implement Service Discovery send updated list to LoadBalancer
            await NotifyGateway();
        }

        public async Task Deregister(MicroServiceInstance instance)
        {
            Console.WriteLine("Service {0} at {1} deregistered", instance.Port, instance.IpAddress);
            _instances.RemoveAll(x => x.IpAddress == instance.IpAddress && x.Port == instance.Port);

            //TODO : Implement Service Discovery send updated list to LoadBalancer
            await NotifyGateway();
        }

        private async Task GetHeartbeat()
        {
            // Update LastHeartbeat for the specified service instance
            foreach (var instance in _instances)
            {
                try
                {
                    var response = await _httpClient.GetAsync(instance.HealthCheckUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Received heartbeat for {0} at {1}", instance.Port, instance.IpAddress);
                        instance.LastHeartbeat = DateTime.Now;
                    }
                    else
                    {
                        System.Console.WriteLine($"Failed to receive heartbeat for {instance.Port} at {instance.IpAddress}");
                    }
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
        }

        private async Task CleanStaleInstances()
        {
            Console.WriteLine("Cleaning stale instances...");
            Console.WriteLine($"Instance Count {_instances.Count}");
            
            var now = DateTime.Now;
            var staleInstances = _instances
                .Where(x => now - x.LastHeartbeat > _heartbeatTimeout)
                .ToList();

            if (staleInstances.Any())
            {
                foreach (var instance in staleInstances)
                {
                    Console.WriteLine("Removed service: {0}, IP: {1}, LastHeartbeat: {2}",
                        instance.Port, instance.IpAddress, instance.LastHeartbeat);
                }

                _instances.RemoveAll(x => staleInstances.Contains(x));

                await NotifyGateway();
            }
            else
            {
                Console.WriteLine("No service removed. All instances are healthy.");
            }
        }

        public async Task<List<MicroServiceInstance>> GetInstances()
        {
            return _instances;
        }

        private async Task NotifyGateway()
        {
            var gatewayUrl = "http://loadbalancer:8086/GWLB/updateInstance";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var json = JsonSerializer.Serialize(_instances);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(gatewayUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Gateway notified successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to notify gateway: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error notifying gateway: {ex.Message}");
                }
            }
        }

        public async Task CleanUpSequence()
        {
            await GetHeartbeat();
            await CleanStaleInstances();
        }
    }
}