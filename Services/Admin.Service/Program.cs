using Admin.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add HttpClients for UserAuth and Product Services
builder.Services.AddHttpClient<IUserHttpClient, UserHttpClient>();
builder.Services.AddHttpClient<IProductHttpClient, ProductHttpClient>();

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
