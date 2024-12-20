using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InventoryService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        public InventoryController()
        {
        }

        [HttpGet("GetInventory")]
        public async Task<Product> GetInventory(int id)
        {
            int delay = new Random().Next(1000, 5000);
            await Task.Delay(delay);
            return new Product();
        }

        [HttpGet("GetAllInventory")]
        public async Task<List<Product>> GetAllInventory()
        {
            int delay = new Random().Next(1000, 5000);
            await Task.Delay(delay);
            return new List<Product>();
        }

        [HttpPost("CreateInventory")]
        public async Task<IActionResult> CreateInventory([FromBody] Product inventory)
        {
            int delay = new Random().Next(1000, 5000);
            await Task.Delay(delay);
            return await Task.FromResult(Ok());
        }

        [HttpPut("UpdateInventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] Product inventory)
        {
            int delay = new Random().Next(1000, 5000);
            await Task.Delay(delay);
            return await Task.FromResult(Ok());
        }

        [HttpDelete("DeleteInventory")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            int delay = new Random().Next(1000, 5000);
            await Task.Delay(delay);
            return await Task.FromResult(Ok());
        }
    }
}