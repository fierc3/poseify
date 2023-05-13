using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
    options.DefaultSignOutScheme = "oidc";
})
.AddCookie("Cookies", options =>
{
    options.Cookie.Name = "__Host-bff";
    options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:8001/";
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
          .RequireAccessToken(Duende.Bff.TokenType.User);

app.MapRemoteBffApiEndpoint("/api/GetAttachment", "https://localhost:" + remoteApiPort + "/api/Attachment/GetAttachment")
          .RequireAccessToken(Duende.Bff.TokenType.User);
          
app.MapRemoteBffApiEndpoint("/api/PostUpload", "https://localhost:" + remoteApiPort + "/api/Upload/PostUpload")
          .RequireAccessToken();

app.MapRemoteBffApiEndpoint("/api/DeleteEstimation", "https://localhost:" + remoteApiPort + "/api/Estimation/DeleteEstimation")
          .RequireAccessToken();

app.Run();

