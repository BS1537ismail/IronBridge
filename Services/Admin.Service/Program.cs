using Admin.Domain.Interfaces;
using Admin.Domain.Managers;
using Admin.Repository;
using Admin.Repository.Interfaces;
using Admin.Repository.Repositories;
using Admin.Service.HttpClients;
using IronBridge.Shared.Interfaces;
using IronBridge.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext - using same database as Product service
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "IronBridge_";
});

// Add Cache Service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Add Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Managers
builder.Services.AddScoped<IProductManager, ProductManager>();

// Add HttpClient for UserAuth Service
builder.Services.AddHttpClient<IUserHttpClient, UserHttpClient>();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
