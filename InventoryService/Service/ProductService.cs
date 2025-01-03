using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventoryService.Service
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(IOptions<MongoDbSettings> settings)
        {
            try
            {
                var client = new MongoClient(settings.Value.ConnectionString);
                var database = client.GetDatabase(settings.Value.DatabaseName);
                _products = database.GetCollection<Product>(settings.Value.CollectionName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                return await _products.Find(new BsonDocument()).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return new List<Product>();
            }
        }

        public async Task<Product> GetProduct(int id)
        {
            try
            {
                return await _products.Find(Builders<Product>.Filter.Eq(p => p.ProductId, id)).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        public async Task CreateProduct(Product product)
        {
            try
            {
                System.Console.WriteLine(
                    $"CreateProduct service {product.ProductId} : {product.Name} : {product.Stock} : {product.Id}");
                await _products.InsertOneAsync(product);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while creating product: " + e.Message);
            }
        }

        public async Task UpdateProduct(Product product)
        {
            try
            {
                await _products.ReplaceOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId),
                    product);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while updating product: " + e.Message);
            }
        }

        public async Task DeleteProduct(int id)
        {
            try
            {
                await _products.DeleteOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, id));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while deleting product: " + e.Message);
            }
        }

        public async Task<bool> HandleOrderCreated(Payload payload)
        {
            try
            {
                var productId = payload.OrderDto.ProductId;
                var quantity = payload.Quantity;
                
                var product = await _products.Find(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId))
                    .FirstOrDefaultAsync();
                if (product != null)
                {
                    Console.WriteLine($"Product: {productId} does exist, it is: {product}");
                    
                    if (product.Stock >= quantity)
                    {
                        product.Stock -= quantity;
                        await _products.ReplaceOneAsync(Builders<Product>.Filter.Eq(p => p.ProductId, (int)productId),
                            product);

                        return true;
                    }
                } return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error handling order created event: " + e.Message);

                // Send problematic message to Dead Letter Queue
                throw;
            }

            
        }
    }
}

