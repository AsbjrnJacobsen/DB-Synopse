namespace LoadBalancer.Model
{
    public class MicroServiceInstance
    {
        public required string ServiceName { get; set; }
        public required string IpAddress { get; set; }
        public required int Port { get; set; }
        public string? HealthCheckUrl { get; set; }
        public DateTime? LastHeartbeat { get; set; }
    }
}