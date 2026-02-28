using AutoMapper;
using Restaurant.Application.DTOs.BasketItemDtos;
using Restaurant.Application.DTOs.CategoryDtos;
using Restaurant.Application.DTOs.ChatDtos;
using Restaurant.Application.DTOs.OrderDtos;
using Restaurant.Application.DTOs.PaymentDtos;
using Restaurant.Application.DTOs.ProductDtos;
using Restaurant.Application.DTOs.ReservationDtos;
using Restaurant.Application.DTOs.TableDTOs;
using Restaurant.Application.DTOs.WaiterDtos;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Table, TableDto>();
        CreateMap<CreateTableDto, Domain.Entities.Table>();

        CreateMap<Domain.Entities.Category, CategoryDto>()
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Products.Count));
        CreateMap<Domain.Entities.Category, CategoryWithProductsDto>();
        CreateMap<CreateCategoryDto, Domain.Entities.Category>();

        CreateMap<Domain.Entities.Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
        CreateMap<CreateProductDto, Domain.Entities.Product>();

        CreateMap<Domain.Entities.BasketItem, BasketItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImageUrl, opt => opt.MapFrom(s => s.Product.ImageUrl))
            .ForMember(d => d.ProductPrice, opt => opt.MapFrom(s => s.Product.Price));

        CreateMap<Domain.Entities.Order, OrderDto>()
            .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table.Name))
            .ForMember(d => d.WaiterName, opt => opt.MapFrom(s => s.Waiter != null ? s.Waiter.FullName : null));
        CreateMap<Domain.Entities.Order, KitchenOrderDto>()
            .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table.Name));

        CreateMap<Domain.Entities.OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImageUrl, opt => opt.MapFrom(s => s.Product.ImageUrl));

        CreateMap<Domain.Entities.Payment, PaymentDto>()
            .ForMember(d => d.OrderNumber, opt => opt.MapFrom(s => s.Order.OrderNumber));

        CreateMap<Domain.Entities.Waiter, WaiterDto>();
        CreateMap<WaiterTable, WaiterTableDto>()
            .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table != null ? s.Table.Name : ""));

        CreateMap<WaiterTable, WaiterTableDto>()
    .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table != null ? s.Table.Name : ""));
        CreateMap<Domain.Entities.ChatMessage, ChatMessageDto>()
            .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table.Name));

        CreateMap<Domain.Entities.Reservation, ReservationDto>()
            .ForMember(d => d.TableName, opt => opt.MapFrom(s => s.Table.Name));
        CreateMap<CreateReservationDto, Domain.Entities.Reservation>();
    }
}