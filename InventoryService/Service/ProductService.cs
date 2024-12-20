using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryService.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventoryService.Service
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _products = database.GetCollection<Product>(collectionName);
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
    }
}