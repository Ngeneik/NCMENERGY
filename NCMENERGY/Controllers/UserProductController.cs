using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Dtos;
using NCMENERGY.Services; // adjust if your service namespace is different
using NCMENERGY.Services.UserProductService;
using System;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProductController : ControllerBase
    {
        private readonly IUserProductService _userProductService;

        public UserProductController(IUserProductService userProductService)
        {
            _userProductService = userProductService;
        }

        [HttpGet("index-products")]
        public async Task<IActionResult> GetIndexProducts()
        {
            var response = await _userProductService.GetIndexProducts();
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("build-system")]
        public async Task<IActionResult> BuildSystem()
        {
            var response = await _userProductService.BuildSystem();
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _userProductService.GetAllProducts();
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetIndexProductById(Guid productId)
        {
            var response = await _userProductService.GetIndexProductById(productId);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpGet("related/index/{relation}")]
        public async Task<IActionResult> GetRelatedProductsIndex(string relation)
        {
            var response = await _userProductService.GetRelatedProductsIndex(relation);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpGet("related/{relation}")]
        public async Task<IActionResult> GetRelatedProducts(string relation)
        {
            var response = await _userProductService.GetRelatedProducts(relation);
            return StatusCode(response.Success ? 200 : 404, response);
        }
        [HttpGet("get-search/{query}")]
        public async Task<IActionResult> SearchQuery(string query)
        {
            var response = await _userProductService.SearchQuery(query);
            return StatusCode(response.Success ? 200 : 404, response);
        }

        [HttpPost("get-cart-items")]
        public async Task<IActionResult> GetCartItems([FromBody] GetCartDto request)
        {
            var response = await _userProductService.GetCartItems(request);
            return StatusCode(response.Success ? 200 : 400, response);
        }
    }
}