using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementApp.API.DTOs;
using ProductManagementApp.API.Helpers;
using ProductManagementApp.API.Repositories.Interfaces;

namespace ProductManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository repository, ILogger<ProductController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            _logger.LogInformation("Fetching products with filters: Name={Name}, MinPrice={MinPrice}, MaxPrice={MaxPrice}", name, minPrice, maxPrice);

            try
            {
                var products = await _repository.GetAllAsync(name, minPrice, maxPrice);
                var productDtos = products.Select(p => p.ToDto());

                _logger.LogInformation("Successfully fetched {ProductCount} products", productDtos.Count());

                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching product with ID={ProductId}", id);

            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID={ProductId} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched product with ID={ProductId}", id);
                return Ok(product.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product with ID={ProductId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateProductDto: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new product: {@ProductDto}", dto);

            try
            {
                var product = dto.ToModel();
                await _repository.AddAsync(product); 

                _logger.LogInformation("Product created successfully with ID={ProductId}", product.Id);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            _logger.LogInformation("Updating product with ID={ProductId}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateProductDto: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Product with ID={ProductId} not found", id);
                    return NotFound();
                }

                existing.UpdateModel(dto);
                await _repository.UpdateAsync(existing);

                _logger.LogInformation("Successfully updated product with ID={ProductId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product with ID={ProductId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting product with ID={ProductId}", id);

            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID={ProductId} not found", id);
                    return NotFound();
                }

                await _repository.DeleteAsync(product);

                _logger.LogInformation("Successfully deleted product with ID={ProductId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product with ID={ProductId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
