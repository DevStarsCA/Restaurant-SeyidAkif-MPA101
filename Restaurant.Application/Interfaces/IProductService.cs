using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.ProductDtos;

namespace Restaurant.Application.Interfaces;

public interface IProductService
{
    Task<ApiResponse<List<ProductDto>>> GetAllAsync();
    Task<ApiResponse<List<ProductDto>>> GetByCategoryAsync(Guid categoryId);
    Task<ApiResponse<List<ProductDto>>> GetAvailableAsync();
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, IFormFile? image);
    Task<ApiResponse<ProductDto>> UpdateAsync(UpdateProductDto dto, IFormFile? image);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
