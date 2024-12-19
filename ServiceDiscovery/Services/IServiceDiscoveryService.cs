using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDiscovery.Model;

namespace ServiceDiscovery.Services
{
    public interface IServiceDiscoveryService
    {
        Task Register(MicroServiceInstance instance);
        Task Deregister(MicroServiceInstance instance);
        Task<List<MicroServiceInstance>> GetInstances();
        Task CleanUpSequence();
    }
}