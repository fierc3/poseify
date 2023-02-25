using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the Entity Framework Core DbContext like you normally would.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // use in memory database for testing
    options.UseInMemoryDatabase("TestMemory");
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( p => { p.WithOrigins("http://localhost:3000", "https://localhost:3000"); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
