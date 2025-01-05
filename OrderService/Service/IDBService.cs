using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model;
using OrderService.Request_Responce;

namespace OrderService.Service
{
    public interface IDBService
    {
        Task<GeneralResponse> GetOrder(int id);
        Task<GeneralResponse> GetAllOrders();
        Task<GeneralResponse> CreateOrder(Payload payload);
        Task<GeneralResponse> UpdateOrder(Payload payload);
        Task<GeneralResponse> DeleteOrder(Payload payload);
        Task<GeneralResponse> DeleteOrderPermanently(Payload payload);
    }
}