using AutoMapper;
using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Application.DTOs.WaiterDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Domain.Interfaces;

namespace Restaurant.Application.Services;

public class WaiterService : IWaiterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;

    public WaiterService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<List<WaiterDto>>> GetAllAsync()
    {
        var waiters = await _unitOfWork.Waiters.GetActiveWaitersAsync();
        return ApiResponse<List<WaiterDto>>.SuccessResponse(_mapper.Map<List<WaiterDto>>(waiters));
    }

    public async Task<ApiResponse<WaiterPanelDto>> GetWaiterPanelAsync(Guid waiterId)
    {
        var waiter = await _unitOfWork.Waiters.GetWaiterWithTablesAsync(waiterId);
        if (waiter == null) return ApiResponse<WaiterPanelDto>.FailResponse("Ofisiant tapılmadı.");

        var panel = new WaiterPanelDto { WaiterId = waiter.Id, WaiterName = waiter.FullName };

        foreach (var wt in waiter.WaiterTables.Where(x => x.IsActive))
        {
            var orders = await _unitOfWork.Orders.GetOrdersByTableIdAsync(wt.TableId);
            panel.Tables.Add(new WaiterTableInfoDto
            {
                TableId = wt.TableId,
                TableName = wt.Table.Name,
                HasReadyOrder = orders.Any(o => o.Status == OrderStatus.Ready),
                ActiveOrderCount = orders.Count(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Delivered)
            });
        }

        return ApiResponse<WaiterPanelDto>.SuccessResponse(panel);
    }

    public async Task<ApiResponse<WaiterDto>> CreateAsync(CreateWaiterDto dto, IFormFile? image)
    {
        var waiter = new Waiter { FullName = dto.FullName, Phone = dto.Phone, AppUserId = dto.AppUserId };
        if (image != null) waiter.ImageUrl = await _cloudinaryService.UploadImageAsync(image, "waiters");

        await _unitOfWork.Waiters.AddAsync(waiter);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<WaiterDto>.SuccessResponse(_mapper.Map<WaiterDto>(waiter), "Ofisiant yaradıldı.");
    }

    public async Task<ApiResponse<WaiterDto>> UpdateAsync(UpdateWaiterDto dto)
    {
        var waiter = await _unitOfWork.Waiters.GetByIdAsync(dto.Id);
        if (waiter == null) return ApiResponse<WaiterDto>.FailResponse("Tapılmadı.");

        waiter.FullName = dto.FullName;
        waiter.Phone = dto.Phone;
        waiter.IsActive = dto.IsActive;

        _unitOfWork.Waiters.Update(waiter);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<WaiterDto>.SuccessResponse(_mapper.Map<WaiterDto>(waiter), "Yeniləndi.");
    }

    public async Task<ApiResponse<bool>> AssignToTableAsync(AssignWaiterToTableDto dto)
    {
        var waiter = await _unitOfWork.Waiters.GetByIdAsync(dto.WaiterId);
        if (waiter == null) return ApiResponse<bool>.FailResponse("Ofisiant tapılmadı.");

        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table == null) return ApiResponse<bool>.FailResponse("Masa tapılmadı.");

        var waiterWithTables = await _unitOfWork.Waiters.GetWaiterWithTablesAsync(dto.WaiterId);
        if (waiterWithTables != null && waiterWithTables.WaiterTables.Any(wt => wt.TableId == dto.TableId && wt.IsActive))
            return ApiResponse<bool>.FailResponse("Bu masa artıq bu ofisianta təyin olunub.");

        var allWaiters = await _unitOfWork.Waiters.GetActiveWaitersAsync();
        var alreadyAssigned = allWaiters.Any(w => w.WaiterTables.Any(wt => wt.TableId == dto.TableId && wt.IsActive));
        if (alreadyAssigned)
            return ApiResponse<bool>.FailResponse("Bu masa artıq başqa ofisianta təyin olunub.");

        await _unitOfWork.Waiters.AssignTableAsync(dto.WaiterId, dto.TableId);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, $"{waiter.FullName} → {table.Name} təyin edildi.");
    }

    public async Task<ApiResponse<bool>> UnassignFromTableAsync(AssignWaiterToTableDto dto)
    {
        var waiter = await _unitOfWork.Waiters.GetWaiterWithTablesAsync(dto.WaiterId);
        if (waiter == null) return ApiResponse<bool>.FailResponse("Ofisiant tapılmadı.");

        var wt = waiter.WaiterTables.FirstOrDefault(x => x.TableId == dto.TableId && x.IsActive);
        if (wt == null) return ApiResponse<bool>.FailResponse("Bu masa təyin olunmayıb.");

        wt.IsActive = false;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Masa çıxarıldı.");
    }
}
