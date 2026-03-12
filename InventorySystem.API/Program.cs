using InventorySystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using InventorySystem.Application;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventorySystem.Application.Interfaces;
using InventorySystem.API.Controllers;
using InventorySystem.Application.Warehouses;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateWarehouse).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(WarehousesController).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IInventoryDbContext>(sp =>
    sp.GetRequiredService<InventoryDbContext>());

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("AllowReact");

app.MapControllers();

// redirect root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/health", () => "Inventory API is running");

app.Run();