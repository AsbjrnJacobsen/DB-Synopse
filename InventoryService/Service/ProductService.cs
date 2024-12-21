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
        private readonly string _orderQueue = "order_queue";
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
            System.Console.WriteLine($"CreateProduct service {product.ProductId} : {product.Name} : {product.Stock} : {product.Id}");
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
            var productId = eventData.ProductId;
            var quantity = eventData.Quantity;

            var product = await _products.Find(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId)).FirstOrDefaultAsync();
            if (product.Stock >= quantity)
            {
                product.Stock -= quantity;
                await _products.ReplaceOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId), product);

                // Publish event to RabbitMQ via EasyNetQ
                var inventoryReservedEvent = new
                {
                    OrderId = eventData.OrderId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _bus.PubSub.PublishAsync(inventoryReservedEvent);
            }
            else
            {
                // Publish event to RabbitMQ via EasyNetQ
                var inventoryNotAvailableEvent = new
                {
                    OrderId = eventData.OrderId
                };
                await _bus.PubSub.PublishAsync(inventoryNotAvailableEvent);
            }
        }
    }
}
