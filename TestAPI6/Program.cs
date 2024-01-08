using Microsoft.Extensions.Caching.Memory;
using TestAPI6;

var builder = WebApplication.CreateBuilder(args);


// Add memory cache dependencies 
builder.Services.AddMemoryCache();
//builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<Forecast, Forecast>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
