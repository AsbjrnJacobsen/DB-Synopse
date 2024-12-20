using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceDiscovery.Model;
using ServiceDiscovery.Services;

namespace ServiceDiscovery.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceDiscoveryController : ControllerBase
    {
        private readonly IServiceDiscoveryService _serviceDiscoveryService;
        public ServiceDiscoveryController(IServiceDiscoveryService serviceDiscoveryService)
        {
            _serviceDiscoveryService = serviceDiscoveryService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MicroServiceInstance product)
        {
            await _serviceDiscoveryService.Register(product);
            return Ok("Service registered successfully");
        }

        [HttpDelete("deregister")]
        public async Task<IActionResult> Deregister([FromBody] MicroServiceInstance product)
        {
            await _serviceDiscoveryService.Deregister(product);
            return Ok("Service deregistered successfully");
        }
    }
}