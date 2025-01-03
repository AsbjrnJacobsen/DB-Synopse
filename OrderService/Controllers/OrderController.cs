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
        public OrderController(IDBService dbService, OrderMessagePublisher publisher)
        {
            _publisher = publisher;
            this._dbService = dbService;
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
            _publisher.PublishOrder(payload);
            
            
            var response = await _dbService.CreateOrder(payload);
            if (response._status != 200) return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] Payload payload)
        {
            var response = await _dbService.UpdateOrder(payload);
            if (response._status != 200) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder([FromBody] Payload payload)
        {
            var response = await _dbService.DeleteOrder(payload);
            if (response._status != 200) return BadRequest(response);
            return Ok(response);
        }
    }
}