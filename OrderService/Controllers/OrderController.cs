using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Model;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        public OrderController()
        {

        }

        [HttpGet("GetOrder")]
        public async Task<Order> GetOrder(int id)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new Order();
        }

        [HttpGet("GetAllOrders")]
        public async Task<List<Order>> GetAllOrders()
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return new List<Order>();
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] Order order)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            int delay = new Random().Next(1000, 5000); 
            await Task.Delay(delay); 
            return await Task.FromResult(Ok());
        }
    }
}