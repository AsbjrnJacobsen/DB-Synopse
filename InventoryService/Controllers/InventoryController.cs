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
            await _productService.CreateProduct(product);
            return Ok();
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] Product productUpdate)
        {
            await _productService.UpdateProduct(productUpdate);
            return Ok();
        }


        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProduct(productId);
            return Ok();
        }
    }
}