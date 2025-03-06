using Microsoft.EntityFrameworkCore;
using ViVuStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register DBContext
builder.Services.AddDbContext<ViVuStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ViVuStoreDbConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();
