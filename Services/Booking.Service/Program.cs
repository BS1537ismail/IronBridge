using Microsoft.EntityFrameworkCore;
using Booking.Domain.Interfaces;
using Booking.Domain.Managers;
using Booking.Domain.Services;
using Booking.Domain.Models;
using Booking.Repository.Interfaces;
using Booking.Repository.Repositories;
using Booking.Repository.Models;
using IronBridge.Shared.Interfaces;
using IronBridge.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "BookingService_";
});

// Add Cache Service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Configure SSLCommerz Settings
builder.Services.Configure<SSLCommerzSettings>(builder.Configuration.GetSection("SSLCommerz"));

// Add HttpClient for SSLCommerz
builder.Services.AddHttpClient<ISSLCommerzService, SSLCommerzService>();

// Add Repository
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Add Managers
builder.Services.AddScoped<IBookingManager, BookingManager>();
builder.Services.AddScoped<IPaymentManager, PaymentManager>();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
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
