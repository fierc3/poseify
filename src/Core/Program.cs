using Core.Services.Estimations;
using Core.Services.Queues;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000;
});

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => { p.WithOrigins("http://localhost:3000", "https://localhost:3000"); });
    options.AddDefaultPolicy(p => { p.WithOrigins("http://localhost:7184", "https://localhost:7184"); });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEstimationService, EstimationService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddHostedService<QueueListenerService>();
builder.Services.AddScoped<IEstimationCleanService, EstimationCleanService>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://identity.poseify.ngrok.app/";
        //options.Audience = "https://localhost:44462";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "poseifyApiScope");
    })
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("ApiScope");

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        // This helps for devs to track if the authHeader has been forwarded
        Console.WriteLine($"Authorization Header: {authHeader}");
    }
    await next();
});

app.Run();
