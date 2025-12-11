using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Determine which Ocelot configuration to use based on environment
var environment = builder.Environment.EnvironmentName;
var ocelotConfig = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";

// Add Ocelot configuration
builder.Configuration.AddJsonFile(ocelotConfig, optional: false, reloadOnChange: true);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Ocelot
builder.Services.AddOcelot();

// Add Swagger for Ocelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");

// Use Swagger
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
    //opt.ReConfigureUpstreamSwaggerJson = (context, swaggerJson) =>
    //{
    //    // Override the servers in swagger JSON to point to the gateway
    //    swaggerJson.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
    //    {
    //        new Microsoft.OpenApi.Models.OpenApiServer { Url = "http://localhost:5000" }
    //    };
    //    return swaggerJson;
    //};
});

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
