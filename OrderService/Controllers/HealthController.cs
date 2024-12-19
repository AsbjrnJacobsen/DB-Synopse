using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : Controller
    {
        [HttpGet("GetHealthStatus")]
        public async Task<IActionResult> GetHealthStatus()
        {
            return await Task.FromResult(Ok());
        }
    }
}