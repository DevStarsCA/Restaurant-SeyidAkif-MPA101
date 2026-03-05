using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.BasketItemDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services;

public class BasketService : IBasketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BasketService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BasketSummaryDto>> GetByTableAsync(Guid tableId)
    {
        var items = await _unitOfWork.BasketItems.GetBasketItemsByTableIdAsync(tableId);
        var summary = new BasketSummaryDto
        {
            TableId = tableId,
            Items = _mapper.Map<List<BasketItemDto>>(items)
        };
        return ApiResponse<BasketSummaryDto>.SuccessResponse(summary);
    }

    public async Task<ApiResponse<BasketItemDto>> AddToBasketAsync(AddToBasketDto dto)
    {
        // Masa yoxla
        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table == null) return ApiResponse<BasketItemDto>.FailResponse("Masa tapılmadı.");

        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        if (product == null) return ApiResponse<BasketItemDto>.FailResponse("Məhsul tapılmadı.");
        if (!product.IsAvailable) return ApiResponse<BasketItemDto>.FailResponse("Məhsul mövcud deyil.");

        // Miqdar limiti: 1-50
        if (dto.Quantity <= 0 || dto.Quantity > 50)
            return ApiResponse<BasketItemDto>.FailResponse("Miqdar 1-50 arasında olmalıdır.");

        var existingItems = await _unitOfWork.BasketItems.GetBasketItemsByTableIdAsync(dto.TableId);
        var existingItem = existingItems.FirstOrDefault(x => x.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            // Ümumi miqdar limiti
            if (existingItem.Quantity + dto.Quantity > 50)
                return ApiResponse<BasketItemDto>.FailResponse("Bir məhsuldan maksimum 50 ədəd ola bilər.");
            existingItem.Quantity += dto.Quantity;
            _unitOfWork.BasketItems.Update(existingItem);
        }
        else
        {
            // Səbətdə maksimum 20 fərqli məhsul
            if (existingItems.Count() >= 20)
                return ApiResponse<BasketItemDto>.FailResponse("Səbətdə maksimum 20 fərqli məhsul ola bilər.");
            existingItem = new BasketItem { TableId = dto.TableId, ProductId = dto.ProductId, Quantity = dto.Quantity };
            await _unitOfWork.BasketItems.AddAsync(existingItem);
        }

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<BasketItemDto>.SuccessResponse(_mapper.Map<BasketItemDto>(existingItem), "Səbətə əlavə edildi.");
    }

    public async Task<ApiResponse<BasketItemDto>> UpdateQuantityAsync(UpdateBasketItemDto dto)
    {
        var item = await _unitOfWork.BasketItems.GetByIdAsync(dto.Id);
        if (item == null) return ApiResponse<BasketItemDto>.FailResponse("Element tapılmadı.");

        if (dto.Quantity <= 0)
            _unitOfWork.BasketItems.Delete(item);
        else
        {
            item.Quantity = dto.Quantity;
            _unitOfWork.BasketItems.Update(item);
        }

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<BasketItemDto>.SuccessResponse(_mapper.Map<BasketItemDto>(item), "Yeniləndi.");
    }

    public async Task<ApiResponse<bool>> RemoveItemAsync(Guid id)
    {
        var item = await _unitOfWork.BasketItems.GetByIdAsync(id);
        if (item == null) return ApiResponse<bool>.FailResponse("Element tapılmadı.");

        _unitOfWork.BasketItems.Delete(item);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Silindi.");
    }

    public async Task<ApiResponse<bool>> ClearBasketAsync(Guid tableId)
    {
        await _unitOfWork.BasketItems.ClearBasketByTableIdAsync(tableId);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Səbət boşaldıldı.");
    }
}