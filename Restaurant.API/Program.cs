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

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

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

        foreach (var role in Enum.GetNames<Restaurant.Domain.Enums.RoleEnum>())
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Restaurant.Domain.Entities.AppRole(role));
        }

        var adminSeed = builder.Configuration.GetSection("AdminSeed").Get<Restaurant.Domain.Options.AdminSeedOptions>();

        if (adminSeed != null && await userManager.FindByNameAsync(adminSeed.UserName) == null)
        {
            var admin = new Restaurant.Domain.Entities.AppUser
            {
                UserName = adminSeed.UserName,
                Email = adminSeed.Email,
                FullName = adminSeed.FullName,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, adminSeed.Password);
            await userManager.AddToRoleAsync(admin, Restaurant.Domain.Enums.RoleEnum.Admin.ToString());
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