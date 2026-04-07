using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Dtos;
using NCMENERGY.Services.ProductService;
using System;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("view-all-products")]
        public async Task<IActionResult> ViewAllProducts()
        {
            var result = await _productService.ViewAllProducts();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("view-product-by-id/{productId}")]
        public async Task<IActionResult> ViewProductById(Guid productId)
        {
            var result = await _productService.ViewProductById(productId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto request)
        {
            var result = await _productService.CreateProduct(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("edit-product-by-id/{productId}")]
        public async Task<IActionResult> EditProduct(Guid productId, [FromForm] CreateProductDto request)
        {
            var result = await _productService.EditProduct(productId, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("delete-product/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var result = await _productService.DeleteProduct(productId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}