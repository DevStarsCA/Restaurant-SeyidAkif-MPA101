using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ITableService, TableService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService_App, PaymentAppService>();
        services.AddScoped<IWaiterService, WaiterService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IReservationService, ReservationService>();

        return services;
    }
}
