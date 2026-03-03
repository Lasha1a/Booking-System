using BookingSystem;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");

builder.Services.AddMainApiDI(builder.Configuration); //added this line to add the dependencies for the main api project

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); //added scalar Ui
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
