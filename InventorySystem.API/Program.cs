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

// MediatR (Application assembly)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateWarehouse).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(WarehousesController).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IInventoryDbContext>(sp =>
    sp.GetRequiredService<InventoryDbContext>());

builder.Services.AddSwaggerGen(c =>
{
    // Use full type name to avoid collisions (CreateWarehouse+Command vs UpdateWarehouse+Command)
    c.CustomSchemaIds(type => type.FullName!.Replace("+", "."));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();