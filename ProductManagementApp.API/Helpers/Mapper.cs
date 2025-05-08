using ProductManagementApp.API.DTOs;
using ProductManagementApp.API.Models;

namespace ProductManagementApp.API.Helpers
{
    public static class Mapper
    {
        public static ProductDto ToDto(this Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };

        public static Product ToModel(this CreateProductDto dto) => new()
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        public static void UpdateModel(this Product product, CreateProductDto dto)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
        }
    }
}
