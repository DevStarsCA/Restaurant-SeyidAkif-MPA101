using AutoMapper;
using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.ProductDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces;

namespace Restaurant.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<List<ProductDto>>> GetAllAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return ApiResponse<List<ProductDto>>.SuccessResponse(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<List<ProductDto>>> GetByCategoryAsync(Guid categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return ApiResponse<List<ProductDto>>.SuccessResponse(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<List<ProductDto>>> GetAvailableAsync()
    {
        var products = await _unitOfWork.Products.GetAvailableProductsAsync();
        return ApiResponse<List<ProductDto>>.SuccessResponse(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto, IFormFile? image)
    {
        var existing = await _unitOfWork.Products.GetAsync(p => p.Name == dto.Name);
        if (existing.Any()) return ApiResponse<ProductDto>.FailResponse("Bu adda məhsul artıq mövcuddur.");

        var product = _mapper.Map<Product>(dto);
        if (image != null) product.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "products");

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<ProductDto>.SuccessResponse(_mapper.Map<ProductDto>(product), "Məhsul yaradıldı.");
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(UpdateProductDto dto, IFormFile? image)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
        if (product == null) return ApiResponse<ProductDto>.FailResponse("Tapılmadı.");
        var existing = await _unitOfWork.Products.GetAsync(p => p.Name == dto.Name && p.Id != dto.Id);
        if (existing.Any()) return ApiResponse<ProductDto>.FailResponse("Bu adda məhsul artıq mövcuddur.");
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.IsAvailable = dto.IsAvailable;
        product.PreparationTimeMinutes = dto.PreparationTimeMinutes;
        product.CategoryId = dto.CategoryId;
        if (image != null) product.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "products");

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<ProductDto>.SuccessResponse(_mapper.Map<ProductDto>(product), "Yeniləndi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return ApiResponse<bool>.FailResponse("Tapılmadı.");

        _unitOfWork.Products.SoftDelete(product);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Silindi.");
    }
}
