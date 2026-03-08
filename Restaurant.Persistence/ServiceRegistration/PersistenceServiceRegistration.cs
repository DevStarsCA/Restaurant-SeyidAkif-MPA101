using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Domain.Interfaces;
using Restaurant.Domain.Interfaces.Repositories;
using Restaurant.Persistence.Context;
using Restaurant.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Persistence.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Disable";
            }
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketItemRepository, BasketItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWaiterRepository, WaiterRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
