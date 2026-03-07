using AutoMapper;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.TableDTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services;

public class TableService : ITableService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IQRCodeService _qrCodeService;

    public TableService(IUnitOfWork unitOfWork, IMapper mapper, IQRCodeService qrCodeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _qrCodeService = qrCodeService;
    }

    public async Task<ApiResponse<List<TableDto>>> GetAllAsync()
    {
        var tables = await _unitOfWork.Tables.GetAllAsync();
        return ApiResponse<List<TableDto>>.SuccessResponse(_mapper.Map<List<TableDto>>(tables));
    }

    public async Task<ApiResponse<TableDto>> GetByIdAsync(Guid id)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id);
        if (table == null) return ApiResponse<TableDto>.FailResponse("Masa tapılmadı.");
        return ApiResponse<TableDto>.SuccessResponse(_mapper.Map<TableDto>(table));
    }

    public async Task<ApiResponse<TableDto>> GetByQRCodeAsync(string qrCode)
    {
        var table = await _unitOfWork.Tables.GetByQRCodeAsync(qrCode);
        if (table == null) return ApiResponse<TableDto>.FailResponse("Masa tapılmadı.");
        return ApiResponse<TableDto>.SuccessResponse(_mapper.Map<TableDto>(table));
    }

    public async Task<ApiResponse<List<TableDto>>> GetByStatusAsync(TableStatus status)
    {
        var tables = await _unitOfWork.Tables.GetTablesByStatusAsync(status);
        return ApiResponse<List<TableDto>>.SuccessResponse(_mapper.Map<List<TableDto>>(tables));
    }

    public async Task<ApiResponse<TableDto>> CreateAsync(CreateTableDto dto)
    {
        var existing = await _unitOfWork.Tables.GetAsync(t => t.Name == dto.Name);
        if (existing.Any()) return ApiResponse<TableDto>.FailResponse("Bu adda masa artıq mövcuddur.");

        var table = _mapper.Map<Domain.Entities.Table>(dto);
        table.QRCode = _qrCodeService.GenerateQRCode(table.Id.ToString());

        await _unitOfWork.Tables.AddAsync(table);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<TableDto>.SuccessResponse(_mapper.Map<TableDto>(table), "Masa yaradıldı.");
    }

    public async Task<ApiResponse<TableDto>> UpdateAsync(UpdateTableDto dto)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(dto.Id);
        if (table == null) return ApiResponse<TableDto>.FailResponse("Masa tapılmadı.");
        var existing = await _unitOfWork.Tables.GetAsync(t => t.Name == dto.Name && t.Id != dto.Id);
        if (existing.Any()) return ApiResponse<TableDto>.FailResponse("Bu adda masa artıq mövcuddur.");
        table.Name = dto.Name;
        table.Capacity = dto.Capacity;
        table.Status = dto.Status;

        _unitOfWork.Tables.Update(table);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<TableDto>.SuccessResponse(_mapper.Map<TableDto>(table), "Masa yeniləndi.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id);
        if (table == null) return ApiResponse<bool>.FailResponse("Masa tapılmadı.");

        _unitOfWork.Tables.SoftDelete(table);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Masa silindi.");
    }

    public async Task<ApiResponse<string>> GetQRCodeImageAsync(Guid tableId)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(tableId);
        if (table == null) return ApiResponse<string>.FailResponse("Masa tapılmadı.");

        var qrBase64 = _qrCodeService.GenerateQRCode($"menu.html?tableId={tableId}");
        return ApiResponse<string>.SuccessResponse(qrBase64);
    }
}