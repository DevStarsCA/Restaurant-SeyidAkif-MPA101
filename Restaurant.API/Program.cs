using Microsoft.EntityFrameworkCore;
using Restaurant.Application.ServiceRegistration;
using Restaurant.Infrastructure.Hubs;
using Restaurant.Infrastructure.ServiceRegistration;
using Restaurant.Persistence.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Layer Services
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT token daxil edin: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseMiddleware<Restaurant.API.Middlewares.ExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<ChatHub>("/hubs/chat");
// ============ SEED DATA ============
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Restaurant.Persistence.Context.AppDbContext>();
    var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Restaurant.Domain.Entities.AppUser>>();
    var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Restaurant.Domain.Entities.AppRole>>();

    try
    {
        Console.WriteLine("DB MIGRASIYA BASLADI...");
        await context.Database.MigrateAsync();
        Console.WriteLine("DB MIGRASIYA BITDI!");

        // 1. Roller
        string[] roles = { "Admin", "Waiter", "Kitchen", "Cashier" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Restaurant.Domain.Entities.AppRole(role));
        }

        // 2. Istifadeciler (sifre: Admin123!)
        var users = new[]
        {
            new { Id = "a0000001-0001-0001-0001-000000000001", UserName = "admin",   Email = "admin@restaurant.az",   FullName = "Admin User",    Role = "Admin" },
            new { Id = "a0000002-0002-0002-0002-000000000002", UserName = "waiter1", Email = "waiter1@restaurant.az", FullName = "Eli Hesenov",   Role = "Waiter" },
            new { Id = "a0000003-0003-0003-0003-000000000003", UserName = "waiter2", Email = "waiter2@restaurant.az", FullName = "Vusal Memmedov", Role = "Waiter" },
            new { Id = "a0000004-0004-0004-0004-000000000004", UserName = "kitchen", Email = "kitchen@restaurant.az", FullName = "Kitchen User",  Role = "Kitchen" },
            new { Id = "a0000005-0005-0005-0005-000000000005", UserName = "cashier", Email = "cashier@restaurant.az", FullName = "Cashier User",  Role = "Cashier" },
        };
        foreach (var u in users)
        {
            if (await userManager.FindByNameAsync(u.UserName) == null)
            {
                var user = new Restaurant.Domain.Entities.AppUser { Id = u.Id, UserName = u.UserName, Email = u.Email, FullName = u.FullName, EmailConfirmed = true };
                await userManager.CreateAsync(user, "Admin123!");
                await userManager.AddToRoleAsync(user, u.Role);
            }
        }

        // 3. Kateqoriyalar
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000001-0001-0001-0001-000000000001"), Name = "Salatlar", Description = "Teze salatlar", ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400", DisplayOrder = 1 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000002-0002-0002-0002-000000000002"), Name = "Sorbalar", Description = "Isti sorbalar", ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400", DisplayOrder = 2 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000003-0003-0003-0003-000000000003"), Name = "Kebablar", Description = "Enenevi Azerbaijan kebablari", ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=400", DisplayOrder = 3 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000004-0004-0004-0004-000000000004"), Name = "Pizzalar", Description = "Italyan pizzalari", ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400", DisplayOrder = 4 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000005-0005-0005-0005-000000000005"), Name = "Burgerler", Description = "Sulu burgerler", ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400", DisplayOrder = 5 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000006-0006-0006-0006-000000000006"), Name = "Desertler", Description = "Sirin desertler", ImageUrl = "https://images.unsplash.com/photo-1551024506-0bccd828d307?w=400", DisplayOrder = 6 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000007-0007-0007-0007-000000000007"), Name = "Ickiler", Description = "Soyuq ve isti ickiler", ImageUrl = "https://images.unsplash.com/photo-1544145945-f90425340c7e?w=400", DisplayOrder = 7 },
                new Restaurant.Domain.Entities.Category { Id = Guid.Parse("c0000008-0008-0008-0008-000000000008"), Name = "Qarnirler", Description = "Elave qarnirler", ImageUrl = "https://images.unsplash.com/photo-1534938665831-4f0cd099f0ff?w=400", DisplayOrder = 8 }
            );
            await context.SaveChangesAsync();
        }

        // 4. Mehsullar
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Sezar Salati", Description = "Toyuq, parmezan, kruton, sezar sousu", Price = 8.50m, ImageUrl = "https://images.unsplash.com/photo-1546793665-c74683f339c1?w=400", IsAvailable = true, PreparationTimeMinutes = 10, CategoryId = Guid.Parse("c0000001-0001-0001-0001-000000000001") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Yunan Salati", Description = "Pomidor, xiyar, zeytun, pendir", Price = 7.00m, ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=400", IsAvailable = true, PreparationTimeMinutes = 8, CategoryId = Guid.Parse("c0000001-0001-0001-0001-000000000001") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Coban Salati", Description = "Pomidor, xiyar, biber, sogan", Price = 5.50m, ImageUrl = "https://images.unsplash.com/photo-1607532941433-304659e8198a?w=400", IsAvailable = true, PreparationTimeMinutes = 5, CategoryId = Guid.Parse("c0000001-0001-0001-0001-000000000001") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Mercimek Sorbasi", Description = "Qirmizi mercimek, sogan, pomidor", Price = 5.00m, ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400", IsAvailable = true, PreparationTimeMinutes = 15, CategoryId = Guid.Parse("c0000002-0002-0002-0002-000000000002") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Toyuq Sorbasi", Description = "Toyuq, eriste, terevez", Price = 6.00m, ImageUrl = "https://images.unsplash.com/photo-1588566565463-180a5b2090d2?w=400", IsAvailable = true, PreparationTimeMinutes = 20, CategoryId = Guid.Parse("c0000002-0002-0002-0002-000000000002") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Dovga", Description = "Yogurt, duyu, noxud, goyerti", Price = 4.50m, ImageUrl = "https://images.unsplash.com/photo-1583937443191-e61c8aee1a43?w=400", IsAvailable = true, PreparationTimeMinutes = 25, CategoryId = Guid.Parse("c0000002-0002-0002-0002-000000000002") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Lule Kebab", Description = "Dana eti, sogan, istiot", Price = 12.00m, ImageUrl = "https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?w=400", IsAvailable = true, PreparationTimeMinutes = 20, CategoryId = Guid.Parse("c0000003-0003-0003-0003-000000000003") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Tike Kebab", Description = "Dana tike eti, pomidor, biber", Price = 14.00m, ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?w=400", IsAvailable = true, PreparationTimeMinutes = 25, CategoryId = Guid.Parse("c0000003-0003-0003-0003-000000000003") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Toyuq Kebab", Description = "Toyuq sinesi, terevez", Price = 10.00m, ImageUrl = "https://images.unsplash.com/photo-1603360946369-dc9bb6258143?w=400", IsAvailable = true, PreparationTimeMinutes = 18, CategoryId = Guid.Parse("c0000003-0003-0003-0003-000000000003") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Qabirga", Description = "Dana qabirga, xususi sous", Price = 18.00m, ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?w=400", IsAvailable = true, PreparationTimeMinutes = 35, CategoryId = Guid.Parse("c0000003-0003-0003-0003-000000000003") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Marqarita Pizza", Description = "Mozzarella, pomidor sousu, reyhan", Price = 11.00m, ImageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400", IsAvailable = true, PreparationTimeMinutes = 20, CategoryId = Guid.Parse("c0000004-0004-0004-0004-000000000004") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Pepperoni Pizza", Description = "Pepperoni, mozzarella, pomidor sousu", Price = 13.00m, ImageUrl = "https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400", IsAvailable = true, PreparationTimeMinutes = 22, CategoryId = Guid.Parse("c0000004-0004-0004-0004-000000000004") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Qarisiq Pizza", Description = "Toyuq, gobelek, biber, zeytun", Price = 14.50m, ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400", IsAvailable = true, PreparationTimeMinutes = 25, CategoryId = Guid.Parse("c0000004-0004-0004-0004-000000000004") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Klassik Burger", Description = "Dana kotlet, pomidor, xiyar, sous", Price = 9.50m, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400", IsAvailable = true, PreparationTimeMinutes = 15, CategoryId = Guid.Parse("c0000005-0005-0005-0005-000000000005") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Cheese Burger", Description = "Dana kotlet, cheddar, sogan helqesi", Price = 11.00m, ImageUrl = "https://images.unsplash.com/photo-1553979459-d2229ba7433b?w=400", IsAvailable = true, PreparationTimeMinutes = 15, CategoryId = Guid.Parse("c0000005-0005-0005-0005-000000000005") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Double Burger", Description = "2x dana kotlet, pendir, bekon", Price = 15.00m, ImageUrl = "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=400", IsAvailable = true, PreparationTimeMinutes = 18, CategoryId = Guid.Parse("c0000005-0005-0005-0005-000000000005") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Tiramisu", Description = "Italyan desert, mascarpone, qehve", Price = 7.50m, ImageUrl = "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=400", IsAvailable = true, PreparationTimeMinutes = 5, CategoryId = Guid.Parse("c0000006-0006-0006-0006-000000000006") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Baklava", Description = "Enenevi Azerbaijan baklavasi", Price = 6.00m, ImageUrl = "https://images.unsplash.com/photo-1598110750624-207050c4f28c?w=400", IsAvailable = true, PreparationTimeMinutes = 5, CategoryId = Guid.Parse("c0000006-0006-0006-0006-000000000006") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Sokoladli Keks", Description = "Erimis sokolad, vanil dondurma", Price = 8.00m, ImageUrl = "https://images.unsplash.com/photo-1551024506-0bccd828d307?w=400", IsAvailable = true, PreparationTimeMinutes = 10, CategoryId = Guid.Parse("c0000006-0006-0006-0006-000000000006") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Coca-Cola", Description = "330ml", Price = 2.50m, ImageUrl = "https://images.unsplash.com/photo-1554866585-cd94860890b7?w=400", IsAvailable = true, PreparationTimeMinutes = 1, CategoryId = Guid.Parse("c0000007-0007-0007-0007-000000000007") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Limonata", Description = "Teze sixilmis limon, nane", Price = 3.50m, ImageUrl = "https://images.unsplash.com/photo-1621263764928-df1444c5e859?w=400", IsAvailable = true, PreparationTimeMinutes = 5, CategoryId = Guid.Parse("c0000007-0007-0007-0007-000000000007") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Cay", Description = "Azerbaijan cayi, armudu stekanda", Price = 2.00m, ImageUrl = "https://images.unsplash.com/photo-1571934811356-5cc061b6201f?w=400", IsAvailable = true, PreparationTimeMinutes = 5, CategoryId = Guid.Parse("c0000007-0007-0007-0007-000000000007") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Turk Qehvesi", Description = "Enenevi usulla hazirlanmis", Price = 3.00m, ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefda?w=400", IsAvailable = true, PreparationTimeMinutes = 7, CategoryId = Guid.Parse("c0000007-0007-0007-0007-000000000007") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Ayran", Description = "Ev ayrani", Price = 2.00m, ImageUrl = "https://images.unsplash.com/photo-1583937443191-e61c8aee1a43?w=400", IsAvailable = true, PreparationTimeMinutes = 2, CategoryId = Guid.Parse("c0000007-0007-0007-0007-000000000007") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Frit Kartof", Description = "Qizardilmis kartof", Price = 4.00m, ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=400", IsAvailable = true, PreparationTimeMinutes = 10, CategoryId = Guid.Parse("c0000008-0008-0008-0008-000000000008") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Duyu Plov", Description = "Safran duyu", Price = 5.00m, ImageUrl = "https://images.unsplash.com/photo-1596560548464-f010549b84d7?w=400", IsAvailable = true, PreparationTimeMinutes = 15, CategoryId = Guid.Parse("c0000008-0008-0008-0008-000000000008") },
                new Restaurant.Domain.Entities.Product { Id = Guid.NewGuid(), Name = "Kozlenmis Terevez", Description = "Biber, badimcan, pomidor", Price = 5.50m, ImageUrl = "https://images.unsplash.com/photo-1534938665831-4f0cd099f0ff?w=400", IsAvailable = true, PreparationTimeMinutes = 12, CategoryId = Guid.Parse("c0000008-0008-0008-0008-000000000008") }
            );
            await context.SaveChangesAsync();
        }

        // 5. Masalar
        if (!context.Tables.Any())
        {
            context.Tables.AddRange(
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b01-0001-0001-0001-000000000001"), Name = "Masa 1", QRCode = "QR-TABLE-001", Capacity = 2, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b02-0002-0002-0002-000000000002"), Name = "Masa 2", QRCode = "QR-TABLE-002", Capacity = 2, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b03-0003-0003-0003-000000000003"), Name = "Masa 3", QRCode = "QR-TABLE-003", Capacity = 4, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b04-0004-0004-0004-000000000004"), Name = "Masa 4", QRCode = "QR-TABLE-004", Capacity = 4, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b05-0005-0005-0005-000000000005"), Name = "Masa 5", QRCode = "QR-TABLE-005", Capacity = 4, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b06-0006-0006-0006-000000000006"), Name = "Masa 6", QRCode = "QR-TABLE-006", Capacity = 6, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b07-0007-0007-0007-000000000007"), Name = "Masa 7", QRCode = "QR-TABLE-007", Capacity = 6, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b08-0008-0008-0008-000000000008"), Name = "Masa 8", QRCode = "QR-TABLE-008", Capacity = 8, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b09-0009-0009-0009-000000000009"), Name = "Masa 9", QRCode = "QR-TABLE-009", Capacity = 8, Status = Restaurant.Domain.Enums.TableStatus.Available },
                new Restaurant.Domain.Entities.Table { Id = Guid.Parse("a0000b10-0010-0010-0010-000000000010"), Name = "Masa 10", QRCode = "QR-TABLE-010", Capacity = 10, Status = Restaurant.Domain.Enums.TableStatus.Available }
            );
            await context.SaveChangesAsync();
        }

        // 6. Ofisiantlar
        if (!context.Waiters.Any())
        {
            context.Waiters.AddRange(
                new Restaurant.Domain.Entities.Waiter { Id = Guid.Parse("d0000e01-0001-0001-0001-000000000001"), FullName = "Eli Hesenov", Phone = "0551234567", IsActive = true, AppUserId = "a0000002-0002-0002-0002-000000000002" },
                new Restaurant.Domain.Entities.Waiter { Id = Guid.Parse("d0000e02-0002-0002-0002-000000000002"), FullName = "Vusal Memmedov", Phone = "0551234568", IsActive = true, AppUserId = "a0000003-0003-0003-0003-000000000003" }
            );
            await context.SaveChangesAsync();
        }

        // 7. Ofisiant-Masa teyinatlari
        if (!context.WaiterTables.Any())
        {
            var wt = new List<Restaurant.Domain.Entities.WaiterTable>();
            var w1 = Guid.Parse("d0000e01-0001-0001-0001-000000000001");
            var w2 = Guid.Parse("d0000e02-0002-0002-0002-000000000002");
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w1, TableId = Guid.Parse("a0000b01-0001-0001-0001-000000000001"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w1, TableId = Guid.Parse("a0000b02-0002-0002-0002-000000000002"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w1, TableId = Guid.Parse("a0000b03-0003-0003-0003-000000000003"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w1, TableId = Guid.Parse("a0000b04-0004-0004-0004-000000000004"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w1, TableId = Guid.Parse("a0000b05-0005-0005-0005-000000000005"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w2, TableId = Guid.Parse("a0000b06-0006-0006-0006-000000000006"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w2, TableId = Guid.Parse("a0000b07-0007-0007-0007-000000000007"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w2, TableId = Guid.Parse("a0000b08-0008-0008-0008-000000000008"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w2, TableId = Guid.Parse("a0000b09-0009-0009-0009-000000000009"), IsActive = true });
            wt.Add(new Restaurant.Domain.Entities.WaiterTable { Id = Guid.NewGuid(), WaiterId = w2, TableId = Guid.Parse("a0000b10-0010-0010-0010-000000000010"), IsActive = true });
            context.WaiterTables.AddRange(wt);
            await context.SaveChangesAsync();
        }

        Console.WriteLine("SEED DATA UGURLA DAXIL EDILDI!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("SEED XETASI: " + ex);
        throw;
    }
}

app.Run();