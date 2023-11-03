using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.$

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000;
});

builder.Services.AddControllers();
builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddRazorPages()
       .AddRazorPagesOptions(options =>
       {
           options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
       });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
    options.DefaultSignOutScheme = "oidc";
}).AddCookie("Cookies", options =>
{
    options.Cookie.Name = "__Host-bff";
    options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://identity.poseify.ngrok.app/";
    options.ClientId = "PoseifyBff";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.ResponseMode = "query";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("poseifyApiScope");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.MapInboundClaims = true;
    options.SaveTokens = true;

    options.TokenValidationParameters = new()
    {
        NameClaimType = ClaimTypes.NameIdentifier,
    };
}).AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://identity.poseify.ngrok.app/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier,
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();

app.MapBffManagementEndpoints();


int remoteApiPort = 7236;

app.MapRemoteBffApiEndpoint("/api/GetUserEstimations", "https://localhost:" + remoteApiPort + "/api/Estimation/GetUserEstimations")
          .RequireAccessToken();

app.MapRemoteBffApiEndpoint("/api/GetAttachment", "https://localhost:" + remoteApiPort + "/api/Attachment/GetAttachment")
          .RequireAccessToken(Duende.Bff.TokenType.User);
          
app.MapRemoteBffApiEndpoint("/api/PostUpload", "https://localhost:" + remoteApiPort + "/api/Upload/PostUpload")
          .RequireAccessToken();

app.MapRemoteBffApiEndpoint("/api/DeleteEstimation", "https://localhost:" + remoteApiPort + "/api/Estimation/DeleteEstimation")
          .RequireAccessToken();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        // This helps to debug if the client has sent an authheader (AccessToken needed for mobile)
        Console.WriteLine($"Authorization Header: {authHeader}");
    }
    await next();
});

app.Run();

