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

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var products = await _repository.GetAllAsync(name, minPrice, maxPrice);
            var productDtos = products.Select(p => p.ToDto());
            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = dto.ToModel();
            await _repository.AddAsync(product);
            var saved = await _repository.SaveChangesAsync();

            if (!saved)
                return BadRequest("Failed to create product");

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.UpdateModel(dto);
            await _repository.UpdateAsync(existing);
            var saved = await _repository.SaveChangesAsync();

            if (!saved)
                return BadRequest("Failed to update product");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await _repository.DeleteAsync(product);
            var saved = await _repository.SaveChangesAsync();

            if (!saved)
                return BadRequest("Failed to delete product");

            return NoContent();
        }
    }
}
