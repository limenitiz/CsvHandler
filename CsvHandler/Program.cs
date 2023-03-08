using CsvHandler.Entity;
using CsvHandler.Repository;
using CsvHandler.Service;
using CsvHandler.Validator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add db context into dependency container
builder.Services.AddDbContext<CsvDbContext>(
    opt => opt.UseSqlServer(builder
        .Configuration
        .GetConnectionString("MSSQLConnection"))
);

// Add dependencies into dependency container
builder.Services.AddScoped<CsvService>();
builder.Services.AddScoped<CsvServiceHelper>();
builder.Services.AddScoped<ValuesValidator>();
builder.Services.AddScoped<ResultsValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage()
        .UseSwagger()
        .UseSwaggerUI();
}

app.MapControllers();

ConfigDb(app);

app.Run();


// create db if not exists
void ConfigDb(IApplicationBuilder applicationBuilder)
{
    using var serviceScope = applicationBuilder.ApplicationServices
        .GetService<IServiceScopeFactory>()
        ?.CreateScope();

    var context = serviceScope
        ?.ServiceProvider
        .GetRequiredService<CsvDbContext>();
    context?.Database.EnsureCreated();
}
