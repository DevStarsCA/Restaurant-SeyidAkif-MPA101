using AutoMapper;
using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.CategoryDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces;

namespace Restaurant.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return ApiResponse<List<CategoryDto>>.SuccessResponse(_mapper.Map<List<CategoryDto>>(categories));
    }

    public async Task<ApiResponse<List<CategoryWithProductsDto>>> GetWithProductsAsync()
    {
        var categories = await _unitOfWork.Categories.GetCategoriesWithProductsAsync();
        return ApiResponse<List<CategoryWithProductsDto>>.SuccessResponse(_mapper.Map<List<CategoryWithProductsDto>>(categories));
    }

    public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto, IFormFile? image)
    {
        var category = _mapper.Map<Category>(dto);
        if (image != null) category.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "categories");

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<CategoryDto>.SuccessResponse(_mapper.Map<CategoryDto>(category), "Kateqoriya yaradıldı.");
    }

    public async Task<ApiResponse<CategoryDto>> UpdateAsync(UpdateCategoryDto dto, IFormFile? image)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
        if (category == null) return ApiResponse<CategoryDto>.FailResponse("Tapılmadı.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.DisplayOrder = dto.DisplayOrder;
        if (image != null) category.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "categories");

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<CategoryDto>.SuccessResponse(_mapper.Map<CategoryDto>(category), "Yeniləndi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return ApiResponse<bool>.FailResponse("Tapılmadı.");

        _unitOfWork.Categories.SoftDelete(category);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Silindi.");
    }
}
