using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Model;
using OrderService.Request_Responce;

namespace OrderService.Service
{
    public class DBService : IDBService
    {
        
        private readonly OrderDbContext _orderDbContext;

        public DBService(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<GeneralResponse> GetOrder(int id)
        {
            try {
                var order = await _orderDbContext.ordersTable.FirstOrDefaultAsync();

                if (order == null)
                {
                    return new GeneralResponse(405, "Order not found");
                }    
                
                return new GeneralResponse(200, "Order found", order);
                
            } catch (Exception e) {
                Console.WriteLine(e);
                return new GeneralResponse(400, "Order not found: " + e.Message);
            }
        }

        public async Task<GeneralResponse> GetAllOrders()
        {
            try {
                var order = await _orderDbContext.ordersTable.ToListAsync();

                if (order == null || order.Count == 0)
                {
                    return new GeneralResponse(405, "Order not found");
                }    
                
                return new GeneralResponse(200, "Order found", order);
                
            } catch (Exception e) {
                Console.WriteLine(e);
                return new GeneralResponse(400, "Order not found: " + e.Message);
            }
        }

        public async Task<GeneralResponse> CreateOrder(Payload payload)
        {
            try {

                if (payload.OrderDto.ProductId.HasValue)
                {
                    //TODO

                    await _orderDbContext.ordersTable.AddAsync(new Order() 
                    {
                        ProductId = payload.OrderDto.ProductId.Value,
                        VisableFlag = true
                    });

                    await _orderDbContext.SaveChangesAsync();
                    
                    return new GeneralResponse(200, "Order Created");
                }

                return new GeneralResponse(400, "Order not Created");
                
            } catch (Exception e) {
                Console.WriteLine(e);
                return new GeneralResponse(400, "Order not Created: " + e.Message);
            }
        }

        public async Task<GeneralResponse> UpdateOrder(Payload payload)
        {
            try
            {
                if (payload.OrderDto.OrderId.HasValue)
                {
                    // Fetch the existing order from the database
                    var existingOrder = await _orderDbContext.ordersTable
                        .FirstOrDefaultAsync(o => o.OrderId == payload.OrderDto.OrderId.Value);

                    if (existingOrder == null)
                    {
                        return new GeneralResponse(404, "Order not found");
                    }

                    // Update fields only if they have values in the payload
                    if (payload.OrderDto.ProductId.HasValue)
                    {
                        existingOrder.ProductId = payload.OrderDto.ProductId.Value;
                    }

                    if (payload.OrderDto.VisableFlag.HasValue)
                    {
                        existingOrder.VisableFlag = payload.OrderDto.VisableFlag.Value;
                    }

                    // Update the database
                    _orderDbContext.ordersTable.Update(existingOrder);
                    await _orderDbContext.SaveChangesAsync();

                    return new GeneralResponse(200, "Order updated");
                }

                return new GeneralResponse(400, "OrderId is required");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new GeneralResponse(400, "Order not updated: " + e.Message);
            }
}

        public async Task<GeneralResponse> DeleteOrder(Payload payload)
        {
            try {
                if (payload.OrderDto.OrderId.HasValue)
                {
                    var order = await _orderDbContext.ordersTable
                    .Where(o => o.OrderId == payload.OrderDto.OrderId.Value)
                    .FirstOrDefaultAsync();

                    if (order == null)
                    {
                        return new GeneralResponse(405, "Order not found");
                    }    
                    
                    order.VisableFlag = false;
                    _orderDbContext.ordersTable.Update(order);
                    await _orderDbContext.SaveChangesAsync();

                    return new GeneralResponse(200, "Order deleted");
                }
                
                return new GeneralResponse(400, "OrderId is required");
            } catch (Exception e) {
                Console.WriteLine(e);
                return new GeneralResponse(400, "Order not deleted: " + e.Message);
            }
        }
    }
}