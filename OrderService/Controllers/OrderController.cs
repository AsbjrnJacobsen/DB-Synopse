using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Messaging;
using OrderService.Model;
using OrderService.Request_Responce;
using OrderService.Service;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IDBService _dbService;
        private readonly OrderMessagePublisher _publisher;
        public OrderController(OrderMessagePublisher publisher, IDBService dbService)
        {
            this._dbService = dbService;
            _publisher = publisher;
        }

        [HttpGet("GetOrder")]
        public async Task<GeneralResponse> GetOrder(int id)
        {
            var response = await _dbService.GetOrder(id);
            return response;
        }

        [HttpGet("GetAllOrders")]
        public async Task<GeneralResponse> GetAllOrders()
        {
            var response = await _dbService.GetAllOrders();
            return response;
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