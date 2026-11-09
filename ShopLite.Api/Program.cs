using Microsoft.EntityFrameworkCore;
using ShopLite.Api.Data;
using ShopLite.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=shoplite.db";

builder.Services.AddDbContext<ShopLiteDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopLiteDbContext>();
    await DbInitializer.InitializeAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

