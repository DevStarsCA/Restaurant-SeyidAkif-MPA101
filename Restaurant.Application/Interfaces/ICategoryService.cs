using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.CategoryDtos;

namespace Restaurant.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<List<CategoryDto>>> GetAllAsync();
    Task<ApiResponse<List<CategoryWithProductsDto>>> GetWithProductsAsync();
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto, IFormFile? image);
    Task<ApiResponse<CategoryDto>> UpdateAsync(UpdateCategoryDto dto, IFormFile? image);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
