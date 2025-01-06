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
        private readonly OrderMessageManager _manager;
        public OrderController(IDBService dbService, OrderMessageManager manager)
        {
            _manager = manager;
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
        public async Task<IActionResult> CreateOrder([FromBody] Payload payload)
        {
            try
            {   
                var createOrder = await _dbService.CreateOrder(payload);
                if (createOrder._status != 200) return BadRequest(createOrder);

                var response = await _manager.PublishOrderAsync(payload, TimeSpan.FromSeconds(10));

                if (response == "Order Confirmed")
                {
                    Console.WriteLine("Order successfully confirmed.");
                    Console.WriteLine($"Order created: {createOrder} status: {createOrder._status} objectList count: {createOrder} payload: {payload.OrderDto.OrderId}");
                    return Ok("Order Confirmed");
                }
                
                await _dbService.DeleteOrderPermanently((Payload)createOrder._objectType!);
                
                return BadRequest("Order Denied: " + response);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Timeout waiting for order confirmation.");
                return StatusCode(504, "Timeout waiting for order confirmation");
            }
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