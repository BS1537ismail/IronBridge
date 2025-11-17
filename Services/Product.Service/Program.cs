using Microsoft.EntityFrameworkCore;
using Product.Domain.Interfaces;
using Product.Domain.Managers;
using Product.Repository.Interfaces;
using Product.Repository.Models;
using Product.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Services
builder.Services.AddScoped<IProductManager, ProductManager>();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
