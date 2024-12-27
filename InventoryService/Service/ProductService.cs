using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using EasyNetQ;

namespace InventoryService.Service
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IBus _bus;

        public ProductService(IOptions<MongoDbSettings> settings, IBus bus)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<Product>(settings.Value.CollectionName);
            _bus = bus;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _products.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Product> GetProduct(int id)
        {
            return await _products.Find(Builders<Product>.Filter.Eq(p => p.ProductId, id)).FirstOrDefaultAsync();
        }

        public async Task CreateProduct(Product product)
        {
            product.Id = ObjectId.GenerateNewId().ToString();
            await _products.InsertOneAsync(product);
        }

        public async Task UpdateProduct(Product product)
        {
            await _products.ReplaceOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId), product);
        }

        public async Task DeleteProduct(int id)
        {
            await _products.DeleteOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, id));
        }

        public async Task HandleOrderCreated(dynamic eventData)
        {
            try
            {
                var productId = eventData.ProductId;
                var quantity = eventData.Quantity;

                var product = await _products.Find(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId)).FirstOrDefaultAsync();
                if (product == null)
                {
                    Console.WriteLine($"Product with ID {productId} not found.");
                    return;
                }

                if (product.Stock >= quantity)
                {
                    await ReserveInventory(product, quantity, eventData.OrderId, productId);
                }
                else
                {
                    await PublishInventoryNotAvailableEvent(eventData.OrderId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error handling order created event: " + e.Message);
                await PublishDeadLetterMessage(eventData, e.Message);
            }
        }

        private async Task ReserveInventory(Product product, int quantity, int orderId, int productId)
        {
            product.Stock -= quantity;
            await _products.ReplaceOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId), product);

            var inventoryReservedEvent = new
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity
            };
            await _bus.PubSub.PublishAsync(inventoryReservedEvent);
        }

        private async Task PublishInventoryNotAvailableEvent(int orderId)
        {
            var inventoryNotAvailableEvent = new
            {
                OrderId = orderId
            };
            await _bus.PubSub.PublishAsync(inventoryNotAvailableEvent);
        }

        private async Task PublishDeadLetterMessage(dynamic eventData, string errorMessage)
        {
            var deadLetterMessage = new
            {
                EventData = eventData,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            };
            try
            {
                await _bus.PubSub.PublishAsync(deadLetterMessage, "DeadLetterQueue");
            }
            catch (Exception dlqException)
            {
                Console.WriteLine("Error sending to Dead Letter Queue: " + dlqException.Message);
            }
        }
    }
}
