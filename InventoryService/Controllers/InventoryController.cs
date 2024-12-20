using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InventoryService.Model;
using InventoryService.Service;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : Controller
    {
        private readonly ProductService _productService;
        public InventoryController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetProduct")]
        public async Task<Product> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);
            return product;
        }

        [HttpGet("GetAllProducts")]
        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return products;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            product.Id = ObjectId.GenerateNewId().ToString();
            System.Console.WriteLine($"CreateProduct controller {product.ProductId} : {product.Name} : {product.Stock} : {product.Id}");
            await _productService.CreateProduct(product);
            return await Task.FromResult(Ok());
        }

        [HttpPut("UpdateInventory")]
        public async Task<IActionResult> UpdateInventory([FromBody] Product product)
        {
            await _productService.UpdateProduct(product);
            return await Task.FromResult(Ok());
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProduct(id);
            return await Task.FromResult(Ok());
        }
    }
}